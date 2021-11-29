using FEMCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEM.Quenching.InverseSolvers
{
    public abstract class LMMBase
    {
        protected double maxHtc = 35000.0;

        protected double epsilon = 0.05;


        protected double smallError = 0.000000001;
        protected double error1 = 5;
        protected double error2 = 0.5;

        

        protected double error1d1 = 1;
        protected double error1d2 = 1;

        protected abstract void chageHtc(IBoundaryConditions ibc, double dP, int timeStep, int htcIndex);
    }
}
