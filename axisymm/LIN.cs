using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daany.MathStuff;
using FEMCommon.Entities;
using FEMCommon.Interfaces;
using FEMCommon.Types;
using FEMHeat.Lib;
using FEMHeat.Lib.BC;

namespace FEMHeat.Lib.AxiSymmetrics
{
    /// <summary>
    /// Class implementation for the 1D finite element of heat transfer problem in polar coordinates.
    /// </summary>
    public class AxiLIN : IFiniteElement
    {
        /// <summary>
        /// Identification for the finite element
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Node Ids for the current finite element.
        /// </summary>
        public int [] N { get; set; }

        public ElementType T { get; private set; }

        public (double x1, double x2) a { get; set; }

        /// <summary>
        /// Length of a line
        /// </summary>
        public double L { get; set; }


        /// <summary>
        /// centroid point of the triangle
        /// </summary>
        public double rc { get; set; }

        /// <summary>
        /// Capacitance matrix
        /// </summary>
        double[,] C { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="n"></param>
        public AxiLIN(int id, params INode[] n)
        {
            Id = id;
            // Nodes = nodes;
            N = new int[2] { n[0].Id, n[1].Id};
            T = ElementType.line;
  
            //initialize element
            intialize(n);
        }

        /// <summary>
        /// Initialization of all finite element geometry properties
        /// </summary>
        public void intialize(params INode[] n)
        {
            //length calculation
            a = (n[0].P.X, n[1].P.X);
            L = a.x2 - a.x1;
            rc = (a.x2 + a.x1) / 2.0;

            //capacitance matrix
            C = new double[2, 2] { { 2.0, 1.0 }, { 1.0, 2.0 } };
            C = C.Multiply(Math.PI * L * a.x2 / 6.0);
        }

        /// <summary>
        /// Return the Degree of freedom
        /// </summary>
        /// <returns></returns>
        public int GetDof()
        {
            return 2;
        }

        /// <summary>
        /// Estimate the temperature for the point within the finite element
        /// </summary>
        /// <param name="globalNodeValues"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public double Estimate(double[] globalNodeValues, PointD pt)
        {
            var s1 = EvaluateShapeFunction(1, pt) * globalNodeValues[N[0]];
            var s2 = EvaluateShapeFunction(2, pt) * globalNodeValues[N[1]];
            
            return s1 + s2;
        }

        /// <summary>
        /// Evaluate shape functions for a given point
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public double EvaluateShapeFunction(int index, PointD pt)
        {
            if (index == 1)
                return (a.x2 - pt.X)/L;

            else if (index == 2)
                return (pt.X - a.x1)/L;

            else
                throw new Exception("Index of Shape function is not valid.");
        }

        /// <summary>
        /// For material property ana boundary values return the characteristic parameters for the finite element. 
        /// It is used in the calculation of the characteristics matrices 
        /// </summary>
        /// <param name="mData"></param>
        /// <param name="bValues"></param>
        /// <param name="temperatureValue"></param>
        /// <returns></returns>
        public double[] ResolveParameters(IMData mData, IList<IBValue> bValues, double[] nodeValues)
        {
            //Order of the parameters
            //kx, ky, pho, cp, h1, h2, ta1, ta2, Q
            //double kx = 0;//conduction coeff in x direction
            //double ky = 0;//conduction coeff in y direction
            //double rho = 0;//density
            //double cp = 0;//specific heat coefficient
            double h1 = 0;//convection on side(point) 1
            double h2 = 0;//convection on side(point) 2
            double ta1 = 0;//ambient temperature on side(point) 1
            double ta2 = 0;//ambient temperature on side(point) 2
            double Q = 0;//Heat source

            //get material specific properties
            double? tValue = nodeValues == null ? null : (nodeValues[this.N[0]] + nodeValues[this.N[1]])/ 2.0;
            var md = mData.GetData(tValue);

            //Before fitness matrix calculation apply Boundary Values if exist
            if (bValues != null && bValues.Any(x => x.Eid == Id))
            {
                foreach (var ibv in bValues.Where(x => x.Eid == Id))
                {
                    var bv = ibv as HTBValue;

                    if (bv.HTC != null && bv.HTC.Count > 0)
                    {
                        foreach (var htc in bv.HTC)
                        {
                            if (htc.Key == 1)
                            {
                                h1 = htc.Value.htc;
                                ta1 = htc.Value.ta;
                            }
                            else if (htc.Key == 2)
                            {
                                h2 = htc.Value.htc;
                                ta2 = htc.Value.ta;
                            }
                            else
                                throw new Exception("Side value greater than 3 is not supported for triangle finite element.");
                        }
                    }

                    //heat source 
                    Q = bv.Q;
                }
            }

            return new double[9] {md[0],md[1], md[2], md[3], h1, h2, ta1, ta2, Q, };
        }

        /// <summary>
        /// Calculates the local k matrix
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public double[,] CalculateKMatrix(INode[] nodes, double[] p)
        {
            //Order of the parameters
            //kx, ky, pho, cp, h1, h2, ta1, ta2, Q
            //resolve parameters needed to calculate k matrix

            var kr = p[0];
            var h1 = p[4];
            var h2 = p[5];

            //Conduction part of Stiffness matrix
            var kre = 2 * Math.PI * (this.rc * kr) / (L);
            var km = new double[,] { { 1.0, -1.0 }, { -1.0, 1.0 } };
            km = km.Multiply(kre);
            if (h1 != 0)
            {
                var ff = 2 * Math.PI * nodes[N[0]].P.X * h1;
                var f = new double[2, 2] { { 1, 0 }, { 0, 0 } };
                f = f.Multiply(ff);
                km = km.Add(f);
            }
            if (h2 != 0)
            {
                var ff = 2 * Math.PI * nodes[N[1]].P.X * h2;
                var f = new double[2, 2] { { 0, 0 }, { 0, 1 } };
                f = f.Multiply(ff);
                km = km.Add(f);
            }

            return km;
        }

        /// <summary>
        /// Calculate the f vector
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public double[] CalculateFVector(INode[] nodes, double[] p)
        {
            //Order of the parameters
            //kx, ky, pho, cp, h1, h2, ta1, ta2, Q
            //resolve parameters needed to calculate f vector
            var h1 = p[4];
            var h2 = p[5];
            var t1 = p[6];
            var t2 = p[7];
            var Q = p[8];

            //Calculate Heat source
            double[] f1 = new double[2];
            if (Q != 0)
            {
                var hs = 0d;
                hs = 2 * Math.PI * Q * L / 6.0f;

                var khs = new double[2] 
                    { 
                        2.0 * nodes[N[0]].P.X + nodes[N[1]].P.X, 
                        nodes[N[0]].P.X + 2.0 * nodes[N[1]].P.X 
                    };

                f1 = khs.Multiply(hs);
            }

            //calculate convection part
            var fh = new double[]
            {
                    ( 2* Math.PI * h1 * t1 * nodes[N[0]].P.X),
                    ( 2* Math.PI * h2 * t2 * nodes[N[1]].P.X)
            };

            return f1.Add(fh);
        }

        /// <summary>
        /// Calculates C matrix
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        public double[,] CalculateCMatrix(INode[] nodes, double[] p)
        {
            //Order of the parameters
            //kx, ky, pho, cp, h1, h2, ta1, ta2, Q

            var rho = p[2];
            var cp = p[3];
            return C.Multiply(rho * cp);
        }

        /// <summary>
        /// Returns true if the point belongs to this finite element
        /// </summary>
        /// <param name="n"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public bool InElement(INode[] n, PointD pt)
        {
            var n1 = n[N[0]].P;
            var n2 = n[N[1]].P;
           return pt.X >= n1.X && pt.X <= n2.X;
        }

        public override string ToString()
        {
            return $"{Id}:(n1={N[0]};n2={N[1]};)";
        }  
    }
}
