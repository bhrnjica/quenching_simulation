using Daany.MathStuff.Interpolation;
using FEMCommon.Entities;
using FEMCommon.Interfaces;
using FEMHeat.Lib.BC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM.Quenching
{
  
    /// <summary>
    /// Boundary condition according to Experimental settings and the problem
    /// </summary>
    public class BoundaryConditions : IBoundaryConditions
    {
        //
        public IMData _mprop;//Material property
        public double R; 
        public double H;//cylinder dimension

        //Parameters of boundary conditions
        public PointD[] Tloc;//coordinate of thermocouples


        public double Tin;//Initial temperature for starting time.
        public float[] time { get; set; }//time 
        public double[] h;//Height where thermocouples are located
        public double[] htc1 = default!;
        public double[] htc2 = default!;//htc for right cylinder surface 
        public double[] htc3 = default!;//htc for top cylinder surface 
        public double ta;//ambient temperature through time
        public int TimeSteps { get; set; }

        public static BoundaryConditions LoadFromFile(string filePath)
        {
            var bc = new BoundaryConditions();

            var str = File.ReadAllLines(filePath);
            var lines = str.Where(x => x.Length > 1 && x[0] != '!').ToList();

            //Cylinder dimension
            var vals = lines[0].Split('\t', StringSplitOptions.RemoveEmptyEntries);
            (bc.R, bc.H) = (double.Parse(vals[0], CultureInfo.InvariantCulture), double.Parse(vals[1], CultureInfo.InvariantCulture));
            
            //initial temperature
            vals = lines[1].Split('\t', StringSplitOptions.RemoveEmptyEntries);
            bc.Tin = double.Parse(vals[0], CultureInfo.InvariantCulture);

            //number of time steps
            bc.TimeSteps = int.Parse(lines[2], CultureInfo.InvariantCulture);
            //time
            bc.time = lines[3].Split('\t', StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();

            //z coordinates of thermo couples
            bc.h = lines[4].Split('\t', StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();

            //ambient temperature through time
            bc.ta = lines[5].Split('\t', StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray().First();

            //htcon bottom, right side and top sides of the cylinder
            bc.htc1 = lines[6].Split('\t', StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();
            bc.htc2 = lines[7].Split('\t', StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();
            bc.htc3 = lines[8].Split('\t', StringSplitOptions.RemoveEmptyEntries).Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToArray();

            return bc;
        }

        public static void SaveToFile(BoundaryConditions bc, string filePath)
        {

            if (bc == null)
                return;

            var str = new StringBuilder();
            str.AppendLine("!cylinder dimension RxH [m]");
            str.AppendLine($"{Math.Round(bc.R,4)}\t{Math.Round(bc.H, 4)}");
            str.AppendLine(" ");
            str.AppendLine("!initial temperature T0 [C]");
            str.AppendLine($"{Math.Round(bc.Tin, 0)}");

            str.AppendLine(" ");
            str.AppendLine("!number of timeSteps");
            str.AppendLine($"{bc.TimeSteps}");

            str.AppendLine(" ");
            str.AppendLine("! times [s]");
            str.AppendLine($"{string.Join('\t', bc.time)}");


            str.AppendLine(" ");
            str.AppendLine("!z coordinates of thermocouples");
            str.AppendLine($"{string.Join('\t', bc.Tloc.Select(x=>x.Y))}");

  
            str.AppendLine(" ");
            str.AppendLine("!ta [C] - ambient temperature");
            str.AppendLine($"{string.Join('\t', bc.ta)}");

            str.AppendLine(" ");
            str.AppendLine("!htc1, htc2, htc3 [W/(m^2K)]");
            str.AppendLine($"{string.Join('\t', bc.htc1.Select(x => Math.Round(x)))}");
            str.AppendLine($"{string.Join('\t', bc.htc2.Select(x => Math.Round(x)))}");
            str.AppendLine($"{string.Join('\t', bc.htc3.Select(x=> Math.Round(x)))}");


            File.WriteAllLines(filePath, str.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
        }

        public IBoundaryConditions Clone()
        {
            var bc = new BoundaryConditions();
            bc.H = H;
            bc.h = h.Clone() as double[];
            bc.htc1 = htc1.Clone() as double[];
            bc.htc2 = htc2.Clone() as double[];
            bc.htc3 = htc3.Clone() as double[];
            bc.R= R;
            bc.ta = ta;
            bc.time = time.Clone() as float[];
            bc.TimeSteps = TimeSteps;
            bc.Tin= Tin;
            bc.Tloc = Tloc;
            bc._mprop = _mprop;



            return bc;

        }

    

        /// <summary>
        /// Set boundary condition for the AxiSymmetrcic problem. On three sides of the cylinder htcs are applied with ambient temperature
        /// </summary>
        public IList<IBValue> GetBondaryConditions(IFiniteElement[] fe, INode[] nds, double[] prevNodeValues, int timeStep)
        {
            //create bc list
            var bcs = new List<IBValue>();

            //for each finite element check each side of a triangle 
            for (int i = 0; i < fe.Length; i++)
            {
                var e = fe[i];
                //for all triangle side(1-2; 2-3; 3-1)
                for (int j = 0; j < e.N.Length; j++)
                {
                    var side = j + 1;
                    var n1 = nds[e.N[j]];//first node
                    var n2 = j < e.N.Length - 1 ? nds[e.N[j + 1]] : nds[e.N[0]];//second node. 

                    //check first side of a triangle
                    var htc = getHTC(new PointD(n1.P.X, n1.P.Y), new PointD(n2.P.X, n2.P.Y), timeStep);
                    //var htc1 = getHTC1(new PointD(n1.P.X, n1.P.Y), new PointD(n2.P.X, n2.P.Y), timeStep);
                    //Debug.Assert(Math.Round(htc,2)==Math.Round(htc1,2));
                    //var ta = bcv.ta;
                    //side 1
                    //
                    if (Math.Abs(n1.P.Y - n2.P.Y) < 0.000001)
                    {
                        //set alpha 1
                        if (n1.P.Y == 0)
                        {
                            var bv = new HTBValue();
                            bv.Eid = e.Id;

                            bv.HTC.Add(side, (htc, ta));
                            bcs.Add(bv);
                        }
                        //set alpha 3
                        if (Math.Abs(n1.P.Y - H) < 0.000001)
                        {
                            var bv = new HTBValue();
                            bv.Eid = e.Id;
                            bv.HTC.Add(side, (htc, ta));
                            bcs.Add(bv);
                        }
                    }
                    if (Math.Abs(n1.P.X - n2.P.X) < 0.000001 && Math.Abs(n1.P.X - R) < 0.000001)
                    {
                        //set alpha 2
                        var bv = new HTBValue();
                        bv.Eid = e.Id;
                        bv.HTC.Add(side, (htc, ta));
                        bcs.Add(bv);
                    }
                }
            }

            return bcs;
        }


        /// <summary>
        /// Interpolation of the htc for each node of the finite element. We use Spline interpolation for estimation
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="timeStep"></param>
        /// <returns></returns>
        public double getHTC(PointD pt1, PointD pt2, int timeStep)
        {
            var r = pt1.X;
            var z = pt1.Y;

            if ((pt1.Y == pt2.Y && (z == 0 || z == H)) || (pt1.X == pt2.X && r == R))
            {
                var dataX = new double[3] { h[0], h[1], h[2] };
                var dataY = new double[3] { htc1[timeStep], htc2[timeStep], htc3[timeStep] };

                var linInt = new Linear(dataX, dataY);
                return linInt.interp(z);
            }
            else
                return 0;


            
        }

        /// <summary>
        /// Return initial temperature on each node
        /// </summary>
        /// <param name="nds"></param>
        /// <returns></returns>
        public double[] GetInitialValues(INode[] nds)
        {
            return nds.Select(x=>Tin).ToArray();
        }

        public double? CalculateMDataParam(IFiniteElement[] fe, INode[] nds, double[] prevNodeValues, int timeStep)
        {
            //return null;//this will consider constant values of kx, ky, rho and cp
            return prevNodeValues.Average();//this will take average temperature in the domain to calculate material properties
        }

    }
}
