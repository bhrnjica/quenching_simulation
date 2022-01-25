using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daany.MathStuff;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using FEMCommon.Interfaces;
using FEMCommon.Entities;
using FEMCommon.Helpers;
using FEMHeat.Lib.BC;
using static Daany.LinA.LinA;
using Daany.LinA;

namespace FEMHeat.Lib.Solvers
{
    /// <summary>
    /// Class implementation for Direct Heat Transfer Solver for AxiSymmetric Cylinder (R, H)
    /// </summary>
    public class DHT : ISolver
    {
        public IFiniteElement[] Fes => _fe;
        public INode[] Nodes => _nds;
        public IMData MData => _mdata;

        IFiniteElement[] _fe; //List of Finite elements
        INode[] _nds;//List of Nodes
        IMData _mdata;//Material property
  
        public DHT(IFiniteElement[] fe, INode[] nds, IMData md)
        {
            _fe = fe;
            _nds = nds;//nodes should be ordered by ids
            _mdata = md;//material specific data
        }

        /// <summary>
        /// Method calculates temperatures at given points for provided temperature field 
        /// </summary>
        /// <param name="valuesInNodes">Node values</param>
        /// <param name="pts">points </param>
        /// <returns></returns>
        public double[] Calculate(double[] nodeValues, params PointD[] pts)
        {
            var temps = new double[pts.Length];
            for (int i = 0; i < pts.Length; i++)
            {
                foreach (var e in _fe)
                {
                   if(e.InElement(_nds, pts[i]))
                    {
                        temps[i] = e.Estimate(nodeValues, pts[i]);
                        break;
                    }
                }
            }

            return temps;

        }

        /// <summary>
        /// Method calculates temperature at given point for given node values in time.
        /// </summary>
        /// <param name="nodeValuesInTime"></param>
        /// <param name="pts"></param>
        /// <returns>Calculated values for each time step</returns>
        public Dictionary<float, double[]> Calculate(Dictionary<float, double[]> nodeValuesInTime, params PointD[] pts)
        {
            var retVal = new Dictionary<float, double[]>();
            var times = nodeValuesInTime.Keys.ToList();

            for (int i = 0; i < times.Count; i++)
            {
                var t = times[i];
                var nodeValues = nodeValuesInTime[t];
                var values = Calculate(nodeValues,pts);
                retVal.Add(t, values);
            }

            return retVal;

        }

        /// <summary>
        /// Steady State Heat Transfer Solver for AxiSymmetric Geometry
        /// </summary>
        public double[] SolveSteady(IList<IBValue> bcs)
        {
            var nds = _nds.OrderBy(x => x.Id).ToList();
           
            //calculate global stiffness and flux matrices
            var K = CalculateGlobalKMatrix(bcs, new double[nds.Count]);
            var f = CalculateGlobalFVector(bcs, new double[nds.Count]);

            //Solve linear equation
            var tt = LinA.Solve(K,f);
            //
            return tt;
        }

        public double[,] CalculateGlobalCMatrix(double[] nodeValues)
        {
            //define global stiffness matrix
            var nodeCount = _nds.Length * _nds[0].GetDof();
            var C = new double[nodeCount, nodeCount];

            foreach (var e in _fe)
            {
                //
                var p = e.ResolveParameters(_mdata,null, nodeValues);
                //calculate c matrix 
                var c = e.CalculateCMatrix(_nds,p);

                assigntoGlobalMatrix(C, e, c);
            }

            return C;
        }

        public double[,] CalculateGlobalKMatrix(IList<IBValue> bcs, double[] nodeValues)
        {
            //define global stiffness matrix
            var nodeCount = _nds.Length * _nds[0].GetDof();
            var K = new double[nodeCount, nodeCount];

            foreach (var e in _fe)
            {
                double[] p = e.ResolveParameters(_mdata, bcs, nodeValues);
                //calculate stiffness of the current element
                var k = e.CalculateKMatrix(_nds, p);
                assigntoGlobalMatrix(K, e, k);
            }

            return K;
        }

        public double[] CalculateGlobalFVector(IList<IBValue> bcs, double[] nodeValues)
        {
            var nodeCount = _nds.Length * _nds[0].GetDof();
            double[] F = new double[nodeCount];
            foreach (var e in _fe)
            {
                var p = e.ResolveParameters(_mdata, bcs, nodeValues);

                //calculate stiffness 
                var f = e.CalculateFVector(_nds, p);

                //once the stiffness matrix is calculated assign each element to corresponded node
                for (int i = 0; i < e.N.Length; i++)
                {
                    int fid = e.N[i];
                    F[fid] += f[i];
                }
            }
            return F;
        }

        private static void assigntoGlobalMatrix(double[,] K, IFiniteElement e, double[,] k)
        {
            //once the stiffness matrix is calculated assembly it into the global matrix
            for (int i = 0; i < e.GetDof(); i++)
            {
                var k1Ind = e.N[i];
                for (int j = 0; j < e.GetDof(); j++)
                {
                    var k2Ind = e.N[j];
                    K[k1Ind, k2Ind] += k[i, j];
                }

            }
        }

        /// <summary>
        /// Implicit direct Heat Transfer solver, with nonlinear boundary condition, and thermal properties. 
        /// All quantities are calculated for each time step based on the average temperature at the current time
        /// </summary>
        /// <param name="bcv"></param>
        /// <returns></returns>
        public Dictionary<float, double[]> Solve(IBoundaryConditions bc)
        {
            //set initial temperature
            var temps = new Dictionary<float, double[]>();
            double[] t0 = bc.GetInitialValues(_nds);

            //add initial time to collection of the solution
            temps.Add(bc.time[0], t0);
            //
            for (int i=1; i<= bc.TimeSteps; i++)
            {
               var bv = bc.GetBondaryConditions(_fe,_nds, t0, i);
               //calculate parameter in order to estimate material properties in function of current node values
               var param = bc.CalculateMDataParam(_fe, _nds, t0, i);
                var dt = bc.time[i] - bc.time[i-1];
               var t1 =  Solve(t0, bv, dt);
               temps.Add(bc.time[i],t1);
               t0 = t1;
            }

            return temps;
        }


        public double[] Solve(double[] prevNodeValues, IList<IBValue> bc, float dt)
        {
            //if the timeStep=0 (initial time) return initial node values
            if(prevNodeValues == null )
              return null;
            

            var C = CalculateGlobalCMatrix(prevNodeValues);
            var K = CalculateGlobalKMatrix(bc, prevNodeValues);
            var f1 = CalculateGlobalFVector(bc, prevNodeValues);

            //calculation process
            var CC = C.Multiply(1.0 / dt);
            var M = CC.Add(K);

            var finRes01 = LinA.MMult(CC, prevNodeValues, f1);
            var t1 = LinA.Solve(M, finRes01);

            return t1;
        }
  
    }
}
