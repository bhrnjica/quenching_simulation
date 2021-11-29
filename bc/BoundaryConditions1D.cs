using FEMCommon.Entities;
using FEMCommon.Interfaces;
using FEMHeat.Lib.BC;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static FEM.Quenching.Experiment;

namespace FEM.Quenching
{

    /// <summary>
    /// Boundary condition according to Experimental settings and the problem
    /// </summary>
    public class BoundaryConditions1D : IBoundaryConditions
    {
        //
        public IMData _mprop = default!;//Material property
        public double R; 
        public double H;//cylinder dimension

        //Parameters of boundary conditions
        public PointD[] Tloc = default!;//coordinate of thermocouples


        public double Tin;//Initial temperature for starting time.
        public float[] time { get; set; }//time 
        public double[] htc;
       
        public double ta;//ambient temperature through time
        public int TimeSteps { get; set; }

        public BoundaryConditions1D()
        {

        }
        public BoundaryConditions1D (BoundaryConditions bc2D,ThermoCouples tc)
        {
            H = bc2D.H;


            if (tc == ThermoCouples.Bottom)
                htc = bc2D.htc1.Clone() as double[];
            else if (tc == ThermoCouples.Middle)
                htc = bc2D.htc2.Clone() as double[];
            else
                htc = bc2D.htc3.Clone() as double[];

            R = bc2D.R;
            ta = bc2D.ta;
            time = bc2D.time.Clone() as float[];
            TimeSteps = bc2D.TimeSteps;
            Tin = bc2D.Tin;
            Tloc = bc2D.Tloc;
            _mprop = bc2D._mprop;
        }

       
        public IBoundaryConditions Clone()
        {
            var bc = new BoundaryConditions1D();
            bc.H = H;
            
            bc.htc = htc.Clone() as double[];
           
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

            //at last finite element put boundary condition
            var bv = new HTBValue();
            bv.Eid = fe.Last().Id;

            bv.HTC.Add(2, (htc[timeStep], ta));
            bcs.Add(bv);
            return bcs;
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
