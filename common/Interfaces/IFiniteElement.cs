using FEMCommon.Entities;
using FEMCommon.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace FEMCommon.Interfaces
{
   
    public interface IFiniteElement
    {
        int[] N { get; set;}
        int Id { get; set; }
        ElementType T { get;  }
        void intialize(params INode[] n);
        int GetDof();
        bool InElement(INode[] n,PointD pt);
        double Estimate(double[] globalNodeValues, PointD pt);
        double[,] CalculateKMatrix(INode[] nodes, params double[] p);
        double[] CalculateFVector(INode[] nodes, params double[] p);
        double[,] CalculateCMatrix(INode[] nodes, params double[] p);
        double[] ResolveParameters(IMData mData, IList<IBValue> bValues, double[] ndodeValue);
    }
}
