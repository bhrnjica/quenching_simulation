using FEMCommon.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace FEMCommon.Helpers
{
    public static class Helpers
    {
        static double sign(PointD p1, PointD p2, PointD p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        public static bool InTriangle(this PointD pt, PointD v1, PointD v2, PointD v3)
        {
            double d1, d2, d3;
            bool has_neg, has_pos;

            d1 = sign(pt, v1, v2);
            d2 = sign(pt, v2, v3);
            d3 = sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }
    }
}
