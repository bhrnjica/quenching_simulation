using System;
using System.Collections.Generic;
using System.Text;

namespace FEMCommon.Entities
{
    /// <summary>
    /// General object to represent coordinates of the point in space
    /// </summary>
    public record PointD
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public PointD(double x, double y, double z = 0) => (X, Y, Z) = (x,y,z);
        
    }
}
