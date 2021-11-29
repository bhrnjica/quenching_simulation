using Daany.MathStuff;
using FEM.Quenching;
using FEMHeat.Lib.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FEMCommon.Helpers;
using FEMCommon.Interfaces;

namespace FEM.Quenching.InverseSolvers
{
    /// <summary>
    /// Class implementation of the Levenberg-Marquardt Method for Inverse HT problem with multi-objective function f1(x1,x2,x3),f2(x1,x2,x3),f3(x1,x2,x3) 
    /// </summary>
    public class Inv2DLMMHTSolverEx:LMMBase
    {
        
        DHT _directSolver;
        List<(float time, double tm1, double tm2, double tm3)> _Y;

        public Inv2DLMMHTSolverEx(DHT directSOlver, List<(float time, double tm1, double tm2, double tm3)> Y)
        {
            _directSolver = directSOlver;
            _Y = Y;
        }

        public BoundaryConditions Optimize(BoundaryConditions bc, int Steps, bool verbose=false)
        {
            //setup initial conditions
            double[] prevField = _directSolver.Nodes.Select(x => bc.Tin).ToArray(); 
            int timeStep = 1;

            if(verbose)
            {
                Console.WriteLine($"2D optimization has been started.");
                Console.WriteLine($" ");
            }
           
            //iterate trough the time
            while (timeStep <= Steps)
            {
                double[] y = null;
                var temps = _Y.Where(x => x.time == bc.time[timeStep]).FirstOrDefault();
                y = new double[3] { temps.tm1, temps.tm2, temps.tm3 };

                prevField = solveInverseHTC(prevField, bc, timeStep, y);

                Console.WriteLine($"Step {timeStep} of {Steps} completed.");
                //
                timeStep++;
            }
            Console.WriteLine($"2D optimization has completed.");
            return bc.Clone() as BoundaryConditions;
        }

        /// <summary>
        /// Solve inverse Heat Transfer Problem at specified time
        /// </summary>
        /// <param name="prevField">Previous node values in time t-1</param>
        /// <param name="bc">Boundary Conditions </param>
        /// <param name="timeStep">the current time step t</param>
        /// <param name="Y">Measured temperatures</param>
        /// <returns>Current temperature field.</returns>
        private double[] solveInverseHTC(double[] prevField, BoundaryConditions bc, int timeStep, double[] Y)
        {
            double nu = 0.001;
            //reset time step
            bc.TimeSteps = timeStep;

            //set boundary condition for the current time step and solve direct problem
            var bv = bc.GetBondaryConditions(_directSolver.Fes,_directSolver.Nodes, prevField,timeStep);
            var param = bc.CalculateMDataParam(_directSolver.Fes, _directSolver.Nodes, prevField, timeStep);
            var dt = bc.time[timeStep] - bc.time[timeStep - 1];
            var tField = _directSolver.Solve(prevField, bv, dt);
            var currTemp = _directSolver.Calculate(tField, bc.Tloc);


            //calculation of objective function
            var aev = calculateAE(Y, currTemp);
            var SP = calculateSE(Y, currTemp);
            var OF0 = SP.Sum();

            //check stopping criteria
            if (OF0 < error1)
                return tField;

            //LMM iterartion
            while (true)
            {
                //Find the most dominant parameter
                var htcIndex = MaxArg(SP);
                
                //calculate sensitivity and Omega matrices
                var J = calculateSensitivity(prevField, bc, timeStep, htcIndex);
                var Jt = J.Transpose();
                var O = J.Dot(Jt)[0];

                //Solve system of equation
                var lm = J.Dot(Jt).Add(O * nu)[0];
                var dm = J.Dot(aev.Transpose())[0];

                //check result before proceed
                if (Math.Abs(dm) < smallError || dm == double.NaN || double.IsInfinity(dm) || lm == 0)
                    break;

                //calculate current increment for the heat transfer coefficient
                var currdP = dm/lm;

                //check increment
                if (Math.Abs(currdP) < error2)
                    break;

                //check the parameter accorind to previosu calculation
                chageHtc(bc, currdP, timeStep, htcIndex);


                //DHTP (AxiSymm FEM)
                var bv1 = bc.GetBondaryConditions(_directSolver.Fes, _directSolver.Nodes, prevField, timeStep);
                tField = _directSolver.Solve(prevField, bv1, dt);
                currTemp = _directSolver.Calculate(tField, bc.Tloc);

                //Calculate objective function
                aev = calculateAE(Y, currTemp);
                SP = calculateSE(Y, currTemp);
                var OF = SP.Sum();

                //update dumping parameter
                if (OF >= OF0)
                    nu = 5 * nu;
                else
                    nu = 0.5 * nu;

                //check stopping criteria
                if (OF < error1)
                    break;

                OF0 = OF;
            }

            return tField;
        }

        /// <summary>
        /// Returns index of the maximum value
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        private int MaxArg(double[] sp)
        {
            return sp.ToList().IndexOf(sp.Max());
        }



        /// <summary>
        /// Calculation of the sensitivity matrix (Jacobian Matrix)
        /// </summary>
        /// <param name="tField">Current temperature field</param>
        /// <param name="bc">Boundary conditions</param>
        /// <param name="timeStep">Current time step</param>
        /// <param name="htcIndex">Index of the temperature with maximum error</param>
        /// <returns></returns>
        private double[] calculateSensitivity(double[] tField, BoundaryConditions bc, int timeStep, int htcIndex)
        {

           // var epsilon = 0.05;
            var dt = bc.time[timeStep] - bc.time[timeStep - 1];

            //define P2 change
            var bc1 = bc.Clone() as BoundaryConditions;
            if (htcIndex == 0)
            {
                if (bc1.htc1[timeStep] == 0)
                    bc1.htc1[timeStep] = 10 * epsilon;
                else
                {
                    var eP21 = bc1.htc1[timeStep] * epsilon;
                    bc1.htc1[timeStep] -= eP21;
                }
            }
            else if (htcIndex == 1)
            {
                if (bc1.htc2[timeStep] == 0)
                    bc1.htc2[timeStep] = 10 * epsilon;
                else
                {
                    var eP21 = bc1.htc2[timeStep] * epsilon;
                    bc1.htc2[timeStep] -= eP21;
                }
            }
            else if (htcIndex == 2)
            {
                if (bc1.htc3[timeStep] == 0)
                    bc1.htc3[timeStep] = 10 * epsilon;
                else
                {
                    var eP21 = bc1.htc3[timeStep] * epsilon;
                    bc1.htc3[timeStep] -= eP21;
                }
            }
            else
                throw new Exception("Unknown htc parameter.");


            //solve for P2 change
            var bv1 = bc1.GetBondaryConditions(_directSolver.Fes, _directSolver.Nodes, tField, timeStep);
            var param = bc1.CalculateMDataParam(_directSolver.Fes, _directSolver.Nodes, tField, timeStep);
            var tfH1 = _directSolver.Solve(tField, bv1, dt);
            var tP1 = _directSolver.Calculate(tfH1, bc1.Tloc);


            //define P2 change
            var bc2 = bc.Clone() as BoundaryConditions;
            if (htcIndex == 0)
            {
                if (bc2.htc1[timeStep] == 0)
                    bc2.htc1[timeStep] = 10 * epsilon;
                else
                {
                    var eP21 = bc2.htc1[timeStep] * epsilon;
                    bc2.htc1[timeStep] += eP21;
                }
            }
            else if (htcIndex == 1)
            {
                if (bc2.htc2[timeStep] == 0)
                    bc2.htc2[timeStep] = 10 * epsilon;
                else
                {
                    var eP21 = bc2.htc2[timeStep] * epsilon;
                    bc2.htc2[timeStep] += eP21;
                }
            }
            else if(htcIndex ==2)
            {
                if (bc2.htc3[timeStep] == 0)
                    bc2.htc3[timeStep] = 10 * epsilon;
                else
                {
                    var eP21 = bc2.htc3[timeStep] * epsilon;
                    bc2.htc3[timeStep] += eP21;
                }
            }
            else
                throw new Exception("Unknown htc parameter.");

            //solve for P2 change
            var bv2 = bc2.GetBondaryConditions(_directSolver.Fes, _directSolver.Nodes, tField, timeStep);
            var tfH2 = _directSolver.Solve(tField, bv2, dt);
            var tP2 = _directSolver.Calculate(tfH2, bc2.Tloc);

            var dPj = 0.0;
            if (htcIndex == 0)
                dPj = bc.htc1[timeStep] == 0 ? 10 : bc.htc1[timeStep];
            else if (htcIndex == 1)
                dPj = bc.htc2[timeStep] == 0 ? 10 : bc.htc2[timeStep];
            else if (htcIndex == 2)
                dPj = bc.htc3[timeStep] == 0 ? 10 : bc.htc3[timeStep];
            else
                throw new Exception("Unknown htc parameter.");

            var J = tP2.Substruct(tP1).Divide(2.0 * epsilon * dPj);

            return J;

        }

        /// <summary>
        /// calculate the square error
        /// </summary>
        /// <param name="aev"></param>
        /// <returns></returns>
        private double[] calculateSE(double[] measuredTemps, double[] calculatedTemps)
        {
            var ae = measuredTemps.Zip(calculatedTemps, (first, second) => (first - second)*(first - second)).ToArray();
            return ae;
        }

        /// <summary>
        /// Calculate absolute error
        /// </summary>
        /// <param name="measuredTemps"></param>
        /// <param name="calculatedTemps"></param>
        /// <returns></returns>
        private double[] calculateAE(double[] measuredTemps, double[] calculatedTemps)
        {
            var ae = measuredTemps.Zip(calculatedTemps, (first, second) => first - second).ToArray();
            return ae;
        }

        /// <summary>
        /// method changes the current value of htc with repect to htcIndex
        /// </summary>
        /// <param name="bc"></param>
        /// <param name="dP"></param>
        /// <param name="timeStep"></param>
        /// <param name="htcIndex"></param>
        protected override void chageHtc(IBoundaryConditions ibc, double dP, int timeStep, int htcIndex)
        {
            var bc = ibc as BoundaryConditions;
            if (htcIndex == 0)
            {
                var h1 = bc.htc1[timeStep] + dP;

                if (h1 >= 0 && h1 <= maxHtc)
                    bc.htc1[timeStep] = h1;
            }
            else if (htcIndex == 1)
            {
                var h2 = bc.htc2[timeStep] + dP;

                if (h2 >= 0 && h2 <= maxHtc)
                    bc.htc2[timeStep] = h2;
            }
            else if (htcIndex == 2)
            {
                var h3 = bc.htc3[timeStep] + dP;

                if (h3 >= 0 && h3 <= maxHtc)
                    bc.htc3[timeStep] = h3;
            }
            else
                throw new Exception("Unknown parameter.");
           
        }

    }
}
