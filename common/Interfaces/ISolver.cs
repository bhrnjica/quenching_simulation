using FEMCommon.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FEMCommon.Interfaces
{
    public interface ISolver
    {
        IFiniteElement[] Fes { get; }
        INode[] Nodes { get; }
        IMData MData { get; }
        double[,] CalculateGlobalCMatrix(double[] nodeValues);
        double[,] CalculateGlobalKMatrix(IList<IBValue> bcs, double[] nodeValues);
        double[] CalculateGlobalFVector(IList<IBValue> bcs, double[] nodeValues);

        double[] Calculate(double[] nodeValues, PointD[] pts);
        Dictionary<float, double[]> Calculate(Dictionary<float, double[]> nodeValuesInTime, PointD[] pts);
        double[] SolveSteady(IList<IBValue> bcs);
        Dictionary<float, double[]> Solve(IBoundaryConditions bc);
        public double[] Solve(double[] prevNodeValues, IList<IBValue> bc, float dt);

    }
}
