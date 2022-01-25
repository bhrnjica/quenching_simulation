using System;
using System.Collections.Generic;
using System.Text;

namespace FEMCommon.Interfaces
{
    public interface IBoundaryConditions 
    {
        float[] time { get; set; }
        int TimeSteps { get; set; }
        IList<IBValue> GetBondaryConditions(IFiniteElement[] fe, INode[] nds, double[] prevNodeValues, int timeStep);
        double[] GetInitialValues(INode[] nds);
        double? CalculateMDataParam(IFiniteElement[] fe, INode[] nds, double[] prevNodeValues, int timeStep);

        IBoundaryConditions Clone();
    }
}
