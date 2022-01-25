using FEMCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMHeat.Lib.BC
{
    /// <summary>
    /// Boundary Value for Heat Transfer to apply boundary condition
    /// </summary>
    public class HTBValue: IBValue
    {
       
        /// <summary>
        /// Finite element Id
        /// </summary>
        public int Eid { get; set; }

        /// <summary>
        /// Boundary Value of Heat flux attached to specific side of the finite element
        /// </summary>
        public IDictionary<int,double> HFlux { get; set; }//[while/(m^2)]

        /// <summary>
        /// Boundary Value of Convection Coefficient with related ambient temperature.
        /// Key value indicates the side of the finite element
        /// </summary>
        public IDictionary<int,(double htc, double ta)> HTC { get; set; }//[W/(m^2)K]

        //Define heat source in the finite element
        public double Q { get; set; }//[J/(m^3)]

        public HTBValue()
        {
            HFlux = new Dictionary<int, double>();
            HTC = new Dictionary<int, (double htc, double ta)>();

        }

        public override string ToString()
        {
            return string.Format("{0},({1},{2})", Eid,HTC.Keys.FirstOrDefault(), HTC.Values.FirstOrDefault());     
        }
    }
}
