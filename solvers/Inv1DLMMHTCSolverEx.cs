using Daany.MathStuff;
using FEMCommon.Interfaces;
using FEMHeat.Lib.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FEM.Quenching.InverseSolvers
{
    /// <summary>
    /// Class implementation of Levenberg-Marquardt Method for Inverse HT problem for function y=f(x)
    /// </summary>
    public class Inv1DLMMHTCSolverEx: LMMBase
    {

        DHT _directSolver;
        List<(float time, double tm)> _Y;

        public Inv1DLMMHTCSolverEx(DHT directSOlver, List<(float time, double tm)> Y)
        {
            _directSolver = directSOlver;
            _Y = Y;
        }

        public double[] Optimize(BoundaryConditions1D bc, int Steps, bool verbose = false)
        {
            //add initial step
            int timeStep = 1;

            //solve direct problem for timeStep
            while (timeStep <= Steps)
            {
                var temps = _Y.Where(x => x.time == bc.time[timeStep]).FirstOrDefault();
                var y = temps.tm;
                solveInverseHTC(bc, timeStep, y);
               
                timeStep++;
            }

            if(verbose)
                Console.WriteLine($"1D optimization been completed.");

            return bc.htc.ToArray();
        }

        private void solveInverseHTC(BoundaryConditions1D bc, int timeStep, double Y)
        {

            double nu = 0.001;
            //calculate previous temperature field
            bc.TimeSteps = timeStep - 1;
            var tFieldInitial = _directSolver.Solve(bc);
            var prevField = tFieldInitial.Last().Value;
            var tempsInitial = _directSolver.Calculate(prevField, bc.Tloc[0]);
            var prevTemp = tempsInitial;


            //set initial value of htc to previous value
            double dP = bc.htc[timeStep-1];
            chageHtc(bc, dP, timeStep,0);

            //reset timeStep
            bc.TimeSteps = timeStep;

            //solve DHTP
            var bv1 = bc.GetBondaryConditions(_directSolver.Fes, _directSolver.Nodes, prevField, timeStep);
            var param1 = bc.CalculateMDataParam(_directSolver.Fes, _directSolver.Nodes, prevField, timeStep);
            var dt1 = bc.time[timeStep] - bc.time[timeStep - 1];

            //calculate temperature field
            var tField0 = _directSolver.Solve(prevField,bv1, dt1);
            var currTemp = _directSolver.Calculate(tField0, bc.Tloc[0]).First();


            //Calculate objective function
            var aev = currTemp - Y;  
            var SE0 = aev * aev;
            
            while (true)
            {

                //calculate sensitivity and Omega matrices
                var J = calculateSensitivity(tField0, currTemp, bc, timeStep);
                var O = J;            

                //Solve system of equation
                var lm = J + O * nu;
                var dm = J * aev;

                //calculate current increment for the heat transfer coefficient
                var currdP = dm/lm;

                //stop if dP is less than error2
                if (Math.Abs(currdP) < error1d2)
                    break;

                //change htc for dP
                chageHtc(bc, currdP, timeStep,0);


                //DHTP (FEM)
                var bv = bc.GetBondaryConditions(_directSolver.Fes, _directSolver.Nodes, prevField, timeStep);
                var param = bc.CalculateMDataParam(_directSolver.Fes, _directSolver.Nodes, prevField, timeStep);
                var dt = bc.time[timeStep] - bc.time[timeStep - 1];

                var tField = _directSolver.Solve(prevField,bv, dt);
                currTemp = _directSolver.Calculate(tField, bc.Tloc[0]).First();

                //calculate objective function
                aev = currTemp - Y;
                var SP = aev * aev;

                //update dumping parameter          
                if (SP >= SE0)
                    nu = 10 * nu;
                else
                    nu = 0.1 * nu;

                //check stopping criteria
                if (SP < error1d1)
                    return;

                SE0 = SP;

               //reset nu value if it is assigned to zero, or infinity
               if (nu == 0 || double.IsNaN(nu) || double.IsInfinity(nu))
                 nu = 0.001;
            }

            return;
        }


        protected override void chageHtc(IBoundaryConditions ibc, double dP, int timeStep, int htcIndex)
        {
            BoundaryConditions1D bc = ibc as BoundaryConditions1D;
            var dh = 0.0;
            dh = bc.htc[timeStep] + dP;
            if (dh >= 0 && dh <= maxHtc)
                bc.htc[timeStep] = dh;     
        }

        private double calculateSensitivity(double[] tField0, double prevTemp, BoundaryConditions1D bc, int timeStep)
        {
            var bc1 = bc.Clone() as BoundaryConditions1D;
            var bc2 = bc.Clone() as BoundaryConditions1D;

            double eP1 = 0;
            eP1 = bc1.htc[timeStep] == 0 ? 100.0 * epsilon : bc1.htc[timeStep] * epsilon;
            bc1.htc[timeStep] -= eP1;

            //solve for P1 change
            var bv1 = bc1.GetBondaryConditions(_directSolver.Fes, _directSolver.Nodes, tField0, timeStep);
            var param1 = bc1.CalculateMDataParam(_directSolver.Fes, _directSolver.Nodes, tField0, timeStep);

            double eP2 = 0;
            eP2 = bc2.htc[timeStep] == 0 ? 100.0 * epsilon : bc1.htc[timeStep] * epsilon;
            bc2.htc[timeStep] += eP2;

            //solve for P1 change
            var bv2 = bc2.GetBondaryConditions(_directSolver.Fes, _directSolver.Nodes, tField0, timeStep);
            var param2 = bc2.CalculateMDataParam(_directSolver.Fes, _directSolver.Nodes, tField0, timeStep);

            var dt1 = bc1.time[timeStep] - bc1.time[timeStep - 1];
            var tfH1 = _directSolver.Solve(tField0,bv1, dt1);
            var tP1 = _directSolver.Calculate(tfH1, bc1.Tloc[0]).First();

            var dt2 = bc2.time[timeStep] - bc2.time[timeStep - 1];
            var tfH2 = _directSolver.Solve(tField0, bv2, dt2);
            var tP2 = _directSolver.Calculate(tfH2, bc2.Tloc[0]).First();

            return (tP2- tP1)/(2.0 * epsilon * eP1);

        }
    }
}