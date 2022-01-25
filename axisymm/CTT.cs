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
using FEMCommon.Helpers;
using FEMHeat.Lib.BC;
using FEMCommon.Types;

namespace FEMHeat.Lib.AxiSymmetrics
{
    /// <summary>
    /// Class implementation for the axi-symmetric finite element in 2d heat transfer problem.
    /// The shape of the element is triangle, and the interpolation function interpolates 
    /// the temperatures inside the triangle as constant temperature.
    /// </summary>
    public class AxiCTT : IFiniteElement
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
        /// <summary>
        /// Area of the triangle
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// centroid point of the triangle
        /// </summary>
        public (double rc, double zc) tc { get; set; }

        /// <summary>
        /// Solution value of the integral int(rdA)
        /// </summary>
        public double RR { get; set; }

        /// <summary>
        /// a coefficients of the matrix
        /// </summary>
        public (double a1, double a2, double a3) a { get; set; }

        /// <summary>
        /// b coefficients of the matrix
        /// </summary>
        public (double b1, double b2, double b3) b { get; set; }

        /// <summary>
        /// c coefficients of the matrix
        /// </summary>
        public (double c1, double c2, double c3) c { get; set; }

        /// <summary>
        /// Lengths of the triangle sides
        /// </summary>
        public (double l12, double l23, double l31) l { get; set; }

        /// <summary>
        /// Capacitance matrix
        /// </summary>
        double[,] C { get; set; }

        public double[] NodeTemperatures { get; set; }

        //Points for temperature distribution within the triangle
        public PointD[] IN { get; set; }
        public double[] TField { get; set; }

        /// <summary>
        /// Main constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nodes"></param>
        public AxiCTT(int id, params INode[] nodes)
        {
            Id = id;
            // Nodes = nodes;
            N = new int[3] { nodes[0].Id, nodes[1].Id, nodes[2].Id };
            T = ElementType.triangle;
            //initialize element
            intialize(nodes);
        }


        /// <summary>
        /// Initialization of all finite element properties
        /// </summary>
        public void intialize(params INode[] n)
        {
            //a coeff
            a = (n[1].P.X * n[2].P.Y - n[2].P.X * n[1].P.Y,
                 n[2].P.X * n[0].P.Y - n[0].P.X * n[2].P.Y,
                 n[0].P.X * n[1].P.Y - n[1].P.X * n[0].P.Y);
             
            //b coeffs
            b = (n[1].P.Y - n[2].P.Y,
                 n[2].P.Y - n[0].P.Y,
                 n[0].P.Y - n[1].P.Y);

            //c coeff
            c = (n[2].P.X - n[1].P.X,
                 n[0].P.X - n[2].P.X,
                 n[1].P.X - n[0].P.X);

            //Area calculation
            var bValue = n[0].P.X * (n[1].P.Y - n[2].P.Y) +
                         n[1].P.X * (n[2].P.Y - n[0].P.Y) +
                         n[2].P.X * (n[0].P.Y - n[1].P.Y);
            A = bValue / 2.0;
            Debug.Assert(A > 0);

            //calculation triangle's sides
            l = (Math.Sqrt((n[0].P.X - n[1].P.X) * (n[0].P.X - n[1].P.X) +
                                   (n[0].P.Y - n[1].P.Y) * (n[0].P.Y - n[1].P.Y)),
                Math.Sqrt((n[1].P.X - n[2].P.X) * (n[1].P.X - n[2].P.X) +
                                   (n[1].P.Y - n[2].P.Y) * (n[1].P.Y - n[2].P.Y)),
                Math.Sqrt((n[0].P.X - n[2].P.X) * (n[0].P.X - n[2].P.X) +
                                  (n[0].P.Y - n[2].P.Y) * (n[0].P.Y - n[2].P.Y)));
            l = (l.l12, l.l23, l.l31);

            //centroid calculation
            tc = (Math.Round((n[0].P.X + n[1].P.X + n[2].P.X) / 3.0,4),
                 Math.Round((n[0].P.Y + n[1].P.Y + n[2].P.Y) / 3.0,4));

            //RR Integral value 
            var retVal = (n[0].P.X * (2 * n[0].P.X + n[1].P.X + n[2].P.X)) +
                         (n[1].P.X * (n[0].P.X + 2 * n[1].P.X + n[2].P.X)) +
                         (n[2].P.X * (n[0].P.X + n[1].P.X + 2 * n[2].P.X));

            RR =  retVal / 12.0f;

            //Capacitance matrix
            var cc = new double[3, 3]
               {
                    {n[0].P.X * 6.0 + n[1].P.X * 2.0 + n[2].P.X * 2.0,
                     n[0].P.X * 2.0 + n[1].P.X * 2.0 + n[2].P.X * 1.0,
                     n[0].P.X * 2.0 + n[1].P.X * 1.0 + n[2].P.X * 2.0},

                    {n[0].P.X * 2.0 + n[1].P.X * 2.0 + n[2].P.X * 1.0,
                     n[0].P.X * 2.0 + n[1].P.X * 6.0 + n[2].P.X * 2.0,
                     n[0].P.X * 1.0 + n[1].P.X * 2.0 + n[2].P.X * 2.0},

                    {n[0].P.X * 2.0 + n[1].P.X * 1.0 + n[2].P.X * 2.0,
                     n[0].P.X * 1.0 + n[1].P.X * 2.0 + n[2].P.X * 2.0,
                     n[0].P.X * 2.0 + n[1].P.X * 2.0 + n[2].P.X * 6.0},

               };
            C = cc.Multiply(Math.PI * this.A / 30.0);

            //split triangle into 10 small triangles in order to color temperature field within the triangle
            //First calculate 3 points on each triangle side.
            if (IN == null)
                IN = Enumerable.Range(0,10).Select(x=> new PointD(0,0)).ToArray();
            //side 1-2 will have Pt121, pt122, pt123
            IN[0].X = n[0].P.X;
            IN[0].Y = n[0].P.Y ;

            IN[1].X = n[1].P.X ;
            IN[1].Y = n[1].P.Y ;

            IN[2].X = n[2].P.X ;
            IN[2].Y = n[2].P.Y ;

            IN[3].X = n[0].P.X + 0.33f * (n[1].P.X - n[0].P.X);
            IN[3].Y = n[0].P.Y + 0.33f * (n[1].P.Y - n[0].P.Y);

            IN[4].X = n[0].P.X + 0.66f * (n[1].P.X - n[0].P.X);
            IN[4].Y = n[0].P.Y + 0.66f * (n[1].P.Y - n[0].P.Y);

            IN[5].X = n[1].P.X + 0.33f * (n[2].P.X - n[1].P.X);
            IN[5].Y = n[1].P.Y + 0.33f * (n[2].P.Y - n[1].P.Y);

            IN[6].X = n[1].P.X + 0.66f * (n[2].P.X - n[1].P.X);
            IN[6].Y = n[1].P.Y + 0.66f * (n[2].P.Y - n[1].P.Y);

            IN[7].X = n[2].P.X + 0.33f * (n[0].P.X - n[2].P.X);
            IN[7].Y = n[2].P.Y + 0.33f * (n[0].P.Y - n[2].P.Y);

            IN[8].X = n[2].P.X + 0.66f * (n[0].P.X - n[2].P.X);
            IN[8].Y = n[2].P.Y + 0.66f * (n[0].P.Y - n[2].P.Y);

            IN[9].X = Math.Round(IN[5].X + 0.5f * (IN[8].X - IN[5].X), 4);
            IN[9].Y = Math.Round(IN[5].Y + 0.5f * (IN[8].Y - IN[5].Y), 4);

        }

        public int GetDof()
        {
            return 3;
        }

        public double Estimate(double[] globalNodeValues, PointD pt) 
        {
            var s1 = EvaluateShapeFunction(1, pt) * globalNodeValues[N[0]];
            var s2 = EvaluateShapeFunction(2, pt) * globalNodeValues[N[1]];
            var s3 = EvaluateShapeFunction(3, pt) * globalNodeValues[N[2]];
            return s1 + s2 + s3;
        }


        private double EvaluateShapeFunction(int index, PointD pt)
        {
            if (index == 1)
                return (this.a.a1 + this.b.b1 * pt.X + this.c.c1 * pt.Y) / (2 * this.A);
            else if (index == 2)
                return (this.a.a2 + this.b.b2 * pt.X + this.c.c2 * pt.Y) / (2 * this.A);
            else if (index == 3)
                return (this.a.a3 + this.b.b3 * pt.X + this.c.c3 * pt.Y) / (2 * this.A);
            else
                throw new Exception("Index of Shape function is not valid.");
        }

        //Calculation of the temperature in several inner triangle point in order to 
        //public void CalculateTemperatureField(double[] nodeTemp)
        //{
        //    if (TField == null)
        //        TField = new double[9];

        //    //calculate 10 temperature points within the triangle

        //    //1. triangle 1-4-9
        //    var rc = (IN[0].X + IN[3].X + IN[8].X) / 3.0;
        //    var zc = (IN[0].Y + IN[3].Y + IN[8].Y) / 3.0;
        //    var t = CalculateTemperature(nodeTemp,rc, zc);
        //    TField[0] = t;

        //    //2.  triangle 4-5-10
        //    rc = (IN[3].X + IN[4].X + IN[9].X) / 3.0;
        //    zc = (IN[3].Y + IN[4].Y + IN[9].Y) / 3.0;
        //    t = CalculateTemperature(nodeTemp, rc, zc);
        //    TField[1] = t;

        //    //3.  triangle 5-2-6
        //    rc = (IN[4].X + IN[1].X + IN[5].X) / 3.0;
        //    zc = (IN[4].Y + IN[1].Y + IN[5].Y) / 3.0;
        //    t = CalculateTemperature(nodeTemp, rc, zc);
        //    TField[2] = t;

        //    //4.  triangle 4-10-9
        //    rc = (IN[3].X + IN[9].X + IN[8].X) / 3.0;
        //    zc = (IN[3].Y + IN[9].Y + IN[8].Y) / 3.0;
        //    t = CalculateTemperature(nodeTemp, rc, zc);
        //    TField[3] = t;

        //    //5.  triangle 5-6-10
        //    rc = (IN[4].X + IN[5].X + IN[9].X) / 3.0;
        //    zc = (IN[4].Y + IN[5].Y + IN[9].Y) / 3.0;
        //    t = CalculateTemperature(nodeTemp, rc, zc);
        //    TField[4] = t;

        //    //6.  triangle 9-10-8
        //    rc = (IN[8].X + IN[9].X + IN[7].X) / 3.0;
        //    zc = (IN[8].Y + IN[9].Y + IN[7].Y) / 3.0;
        //    t = CalculateTemperature(nodeTemp, rc, zc);
        //    TField[5] = t;

        //    //7.  triangle 10-6-7
        //    rc = (IN[9].X + IN[5].X + IN[6].X) / 3.0;
        //    zc = (IN[9].Y + IN[5].Y + IN[6].Y) / 3.0;
        //    t = CalculateTemperature(nodeTemp, rc, zc);
        //    TField[6] = t;

        //    //8.  triangle 10-7-8
        //    rc = (IN[9].X + IN[6].X + IN[7].X) / 3.0;
        //    zc = (IN[9].Y + IN[6].Y + IN[7].Y) / 3.0;
        //    t = CalculateTemperature(nodeTemp, rc, zc);
        //    TField[7] = t;

        //    //9.  triangle 8-7-3
        //    rc = (IN[9].X + IN[6].X + IN[3].X) / 3.0;
        //    zc = (IN[9].Y + IN[6].Y + IN[3].Y) / 3.0;
        //    t = CalculateTemperature(nodeTemp, rc, zc);
        //    TField[8] = t;


        //}


        /// <summary>
        /// Calculate Capacitive matrix for axi-symmetric triangle finite element for heat transfer
        /// </summary>
        /// <param name="rho"></param>
        /// <param name="cp"></param>
        /// <returns></returns>
        public double[,] CalculateCMatrix(INode[] nodes, params double[] p)
        {
            //Order of the parameters
            //kx,ky,pho,cp, q1, q2, q3, h1, h2, h3,ta1, ta2, ta3, Q
            var rho = p[2];
            var cp = p[3];

            //var c = new double[3, 3]
            //   {
            //        {nodes[NIds[0]-1].P.X * 6.0 + nodes[NIds[1]-1].P.X * 2.0 + nodes[NIds[2]-1].P.X * 2.0,
            //         nodes[NIds[0]-1].P.X * 2.0 + nodes[NIds[1]-1].P.X * 2.0 + nodes[NIds[2]-1].P.X * 1.0,
            //         nodes[NIds[0]-1].P.X * 2.0 + nodes[NIds[1]-1].P.X * 1.0 + nodes[NIds[2]-1].P.X * 2.0},

            //        {nodes[NIds[0]-1].P.X * 2.0 + nodes[NIds[1]-1].P.X * 2.0 + nodes[NIds[2]-1].P.X * 1.0,
            //         nodes[NIds[0]-1].P.X * 2.0 + nodes[NIds[1]-1].P.X * 6.0 + nodes[NIds[2]-1].P.X * 2.0,
            //         nodes[NIds[0]-1].P.X * 1.0 + nodes[NIds[1]-1].P.X * 2.0 + nodes[NIds[2]-1].P.X * 2.0},

            //        {nodes[NIds[0]-1].P.X * 2.0 + nodes[NIds[1]-1].P.X * 1.0 + nodes[NIds[2]-1].P.X * 2.0,
            //         nodes[NIds[0]-1].P.X * 1.0 + nodes[NIds[1]-1].P.X * 2.0 + nodes[NIds[2]-1].P.X * 2.0,
            //         nodes[NIds[0]-1].P.X * 2.0 + nodes[NIds[1]-1].P.X * 2.0 + nodes[NIds[2]-1].P.X * 6.0},

            //   };
            //var f = Math.PI * this.A * rho * cp /30.0;

            return C.Multiply(rho * cp);
        }
       

        /// <summary>
        /// Calculates stiffness matrix for axi-symmetric triangle finite element for heat transfer
        /// </summary>
        /// <param name="kr"></param>
        /// <param name="kz"></param>
        /// <param name="h12"></param>
        /// <param name="h23"></param>
        /// <param name="h31"></param>
        /// <returns></returns>
        public double[,] CalculateKMatrix(INode[] nodes, params double[] p)
        {
            //Order of the parameters
            //kx,ky,pho,cp, q1, q2, q3, h1, h2, h3,ta1, ta2, ta3, Q
            double kx = p[0];
            double ky = p[1];
            double h12 = p[7];
            double h23 = p[8];
            double h31 = p[9];
            //
            //Conduction part of Stiffness matrix
            //there is a different calculation of the term 
            double kre = 0;
            kre = (Math.PI * this.tc.rc * kx) / (2 * this.A); //Lewis "Fundamentals od the Finite Element Method for Heat and Fluid Flow"

            //Conduction in r direction
            var kkre = new double[3, 3]
                {
                    {b.b1 * b.b1, b.b1 * b.b2, b.b1 * b.b3},
                    {b.b2 * b.b1, b.b2 * b.b2, b.b2 * b.b3},
                    {b.b3 * b.b1, b.b3 * b.b2, b.b3 * b.b3},
                };

            //Conduction in z direction
            var kze = (Math.PI * RR * ky) / (2 * A);
            var kkze = new double[3, 3]
                 {
                    {c.c1 * c.c1, c.c1 * c.c2, c.c1 * c.c3},
                    {c.c2 * c.c1, c.c2 * c.c2, c.c2 * c.c3},
                    {c.c3 * c.c1, c.c3 * c.c2, c.c3 * c.c3},
                };
            //
            double[,] k1;
            if (kx == ky)
                k1 = kkze.Add(kkre).Multiply(kre);
            else
            {
                kkre = kkre.Multiply(kre);
                kkze = kkze.Multiply(kze);
                k1 = kkre.Add(kkze);
            }
           

            //Convection side 1
            double[,] kkh12 = null;
            if (h12 != 0)
            {
                var kh12 = (Math.PI * h12 * l.l12) / (6.0);
                kkh12 = new double[3, 3]
                                {
                    {3 * nodes[N[0]].P.X + nodes[N[1]].P.X,nodes[N[0]].P.X + nodes[N[1]].P.X, 0 },
                    {nodes[N[0]].P.X + nodes[N[1]].P.X,nodes[N[0]].P.X + 3 * nodes[N[1]].P.X, 0 },
                    {0, 0 , 0},
                                };

                kkh12 = kkh12.Multiply(kh12);
            }

            //Convection on side 2
            double[,] kkh23 = null;
            if (h23 != 0)
            {
                var kh23 = (Math.PI * h23 * l.l23) / (6.0);

                kkh23 = new double[3, 3]
                   {
                        {0, 0 , 0},
                        {0, 3 * nodes[N[1]].P.X + nodes[N[2]].P.X,nodes[N[1]].P.X + nodes[N[2]].P.X },
                        {0, nodes[N[1]].P.X + nodes[N[2]].P.X,nodes[N[1]].P.X + 3 * nodes[N[2]].P.X },
                   };

                kkh23 = kkh23.Multiply(kh23);
            }

            //Convection on side 3
            double[,] kkh31 = null;
            if (h31 != 0)
            {
                var kh31 = (Math.PI * h31 * l.l31) / (6.0);
                kkh31 = new double[3, 3]
                   {
                        {3 * nodes[N[0]].P.X + nodes[N[2]].P.X,0, nodes[N[0]].P.X + nodes[N[2]].P.X},
                        {0, 0 , 0},
                        {nodes[N[0]].P.X + nodes[N[2]].P.X,0, nodes[N[0]].P.X + 3 * nodes[N[2]].P.X},
                   };

                kkh31 = kkh31.Multiply(kh31);
            }

            //prepare result
            double[,] result = new double[3,3];
            result = result.Add(k1);//Conduction

            //add convection part
            if (h12 != 0)
                result = result.Add(kkh12);

            if (h23 != 0)
                result = result.Add(kkh23);

            if (h31 != 0)
                result = result.Add(kkh31);

            return result;
        }

      
        /// <summary>
        /// Calculates load vector for axi-symmetric triangle finite element for heat transfer
        /// </summary>
        /// <param name="Q"></param>
        /// <param name="q12"></param>
        /// <param name="q23"></param>
        /// <param name="q31"></param>
        /// <param name="h12"></param>
        /// <param name="h23"></param>
        /// <param name="h31"></param>
        /// <param name="Ta12"></param>
        /// <param name="Ta23"></param>
        /// <param name="Ta31"></param>
        /// <returns></returns>
        public double[] CalculateFVector(INode[] nodes, params double[] p)
        {
            //Order of the parameters
            //kx,ky,pho,cp, q1, q2, q3, h1, h2, h3,ta1, ta2, ta3, Q
            double kx = p[0];
            double ky = p[1];
            double pho = p[2];
            double cp = p[3];
            double q12 = p[4];
            double q23 = p[5];
            double q31 = p[6];
            double h12 = p[7];
            double h23 = p[8];
            double h31 = p[9];
            double Ta12 = p[10];
            double Ta23 = p[11];
            double Ta31 = p[12];
            double Q = p[13];

            //Calculate Heat source
            double[] f1 = null;
            if (Q != 0)
            {
                var hs = 0d;
                hs = Math.PI * Q * A / 6.0f;  //Lewis "Fundamentals od the Finite Element Method for Heat and Fluid Flow"

                var khs = new double[3, 3] { { 2, 1, 1 }, { 1, 2, 1 }, { 1, 1, 2 } };
                var nds = new double[3] {nodes[N[0]].P.X, nodes[N[1]].P.X, nodes[N[2]].P.X };
                f1 = khs.Dot(nds).Multiply(hs);
            }

            //calculate conduction part
            double[] fq = null;
            if (q12 != 0)
            {
                var kq12 = (Math.PI * q12 * this.l.l12) / (3.0);
                var vq12 = new double[3] {2 * nodes[N[0]].P.X + nodes[N[1]].P.X,
                                      nodes[N[0]].P.X + 2 * nodes[N[1]].P.X,
                                      0};
                fq = vq12.Multiply(kq12);

            }
            if (q23 != 0)
            {
                var kq23 = (Math.PI * q23 * this.l.l23) / (3.0);
                var vq23 = new double[3] {0,
                                      2 * nodes[N[1]].P.X + nodes[N[2]].P.X,
                                      nodes[N[1]].P.X + 2 * nodes[N[2]].P.X};
                if (fq == null)
                    fq = vq23.Multiply(kq23);
                else
                    fq = fq.Add(vq23.Multiply(kq23));

            }
            if (q31 != 0)
            {
                var kq31 = (Math.PI * q31 * l.l31) / (3.0);
                var vq31 = new double[3] {2 * nodes[N[0]].P.X + nodes[N[2]].P.X,
                                      0,
                                      nodes[N[0]].P.X + 2 * nodes[N[2]].P.X};

                if (fq == null)
                    fq = vq31.Multiply(kq31);
                else
                    fq = fq.Add(vq31.Multiply(kq31));

            }

            //convection
            //calculate conduction part
            double[] fh = null;
            if (h12 != 0)
            {
                var kh12 = (Math.PI * h12 * Ta12 * l.l12) / (3.0);
                var vh12 = new double[3] {2 * nodes[N[0]].P.X + nodes[N[1]].P.X,
                                      nodes[N[0]].P.X + 2 * nodes[N[1]].P.X,
                                      0};
                fh = vh12.Multiply(kh12);

            }
            if (h23 != 0)
            {
                var kh23 = (Math.PI * h23 * Ta23 * l.l23) / (3.0);
                var vh23 = new double[3] {0,
                                      2 * nodes[N[1]].P.X + nodes[N[2]].P.X,
                                      nodes[N[1]].P.X + 2 * nodes[N[2]].P.X};
                if (fh == null)
                    fh = vh23.Multiply(kh23);
                else
                    fh = fh.Add(vh23.Multiply(kh23));

            }
            if (h31 != 0)
            {
                var kh31 = (Math.PI * h31 * Ta31 * l.l31) / (3.0);
                var vh31 = new double[3] {2 * nodes[N[0]].P.X + nodes[N[2]].P.X,
                                      0,
                                      nodes[N[0]].P.X + 2 * nodes[N[2]].P.X};

                if (fh == null)
                    fh = vh31.Multiply(kh31);
                else
                    fh = fh.Add(vh31.Multiply(kh31));

            }

            //prepare result
            double[] result = new double[3];
            if (f1 != null)
                result = result.Add(f1);

            if (fq != null)
                result = result.Add(fq.Multiply(1));

            if (fh != null)
                result = result.Add(fh);

            return result;
        }

        /// <summary>
        /// Resolve boundary condition for the current element
        /// </summary>
        /// <param name="bValues"></param>
        /// <returns></returns>
        public double[] ResolveParameters(IMData mData, IList<IBValue> bValues, double[] nodeValues)
        {
            //double kx = 0;//conduction coeff in x direction
            //double ky = 0;//conduction coeff in y direction
            //double rho = 0;//density
            //double cp = 0;//specific heat coefficient
            double q1 = 0;//flux on side 1
            double q2 = 0;//flux on side 2
            double q3 = 0;//flux on side 3
            double h1 = 0;//convection on side 1
            double h2 = 0;//convection on side 2
            double h3 = 0;//convection on side 3
            double ta1 = 0;//ambient temperature on side 1
            double ta2 = 0;//ambient temperature on side 2
            double ta3 = 0;//ambient temperature on side 3
            double Q = 0;//Heat source

            //get material specific properties
            double? tValue = nodeValues==null ? null : (nodeValues[this.N[0]] + nodeValues[this.N[1]] + nodeValues[this.N[2]]) / 3.0;
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
                            else if (htc.Key == 3)
                            {
                                h3 = htc.Value.htc;
                                ta3 = htc.Value.ta;
                            }
                            else
                                throw new Exception("Side value greater than 3 is not supported for triangle finite element.");
                        }
                    }

                    //Heat flux
                    if (bv.HFlux != null && bv.HFlux.Count > 0)
                    {
                        foreach (var hf in bv.HFlux)
                        {
                            if (hf.Key == 1)
                                q1 = hf.Value;
                            else if (hf.Key == 2)
                                q2 = hf.Value;
                            else if (hf.Key == 3)
                                q3 = hf.Value;
                            else
                                throw new Exception("Side value greater than 3 is not supported for triangle finite element.");

                        }
                    }
                }
            }

            return new double[14] { md[0], md[1], md[2], md[3], q1, q2, q3, h1, h2, h3, ta1, ta2, ta3, Q };
        }

        public bool InElement(INode[] n, PointD pt)
        {
            var n1 = n[N[0]] as Node;
            var n2 = n[N[1]] as Node;
            var n3 = n[N[2]] as Node;

            //
            return pt.InTriangle(n1.P, n2.P, n3.P);
        }


        public override string ToString()
        {
            return $"{Id}:(n1={N[0]};n2={N[1]};n3={N[2]} )";
        }

       
    }

}
