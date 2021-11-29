using FEMCommon.Entities;
using System;


namespace FEM.Quenching
{
    public enum ThermoCouples
    {
        Top,//corresponded to htc 3
        Middle,//corresponded to htc2
        Bottom,//corresponded to htc1
        OriginMiddle, //corresponded Tm3 thermocouples
    }
    public class Experiment
    {
        public static PointD[] createTempSensor(double R, double H, ThermoCouples tc)
        {
            //setup dimension of cylinder and location of thermocouples
            var pts = new PointD[1];
            if (tc == ThermoCouples.Bottom)
            {
                pts[0] = new PointD(Math.Round(R - 0.0015, 4), 0.0045);
                
            }
            else if (tc == ThermoCouples.Middle)
            {
                pts[0] = new PointD(Math.Round(R - 0.0015, 4), Math.Round(H / 2, 4));
            }
            else if (tc == ThermoCouples.OriginMiddle)
            {
                pts[0] = new PointD(0, Math.Round(H / 2, 4));
            }
            else
            {
                pts[0] = new PointD(Math.Round(R - 0.0015, 4), Math.Round(H - 0.0045, 4));
            }


            return pts;
        }
        public static PointD[] createTemp3Sensors(double R, double H)
        {
            //setup dimension of cylinder and location of thermocouples
            var pt1 = new PointD(Math.Round(R - 0.0015, 4), 0.0045);
            var pt2 = new PointD(Math.Round(R - 0.0015, 4), Math.Round(H / 2, 4));
            var pt3 = new PointD(Math.Round(R - 0.0015, 4), Math.Round(H - 0.0045, 4));
            var pts = new PointD[] { pt1, pt2, pt3 };
            return pts;
        }
        public static PointD[] createTemp4Sensors(double R, double H)
        {
            //setup dimension of cylinder and location of thermocouples
            var pt1 = new PointD(Math.Round(R - 0.0015, 4), 0.0045);
            var pt2 = new PointD(Math.Round(R - 0.0015, 4), Math.Round(H / 2, 4));
            var pt3 = new PointD(0, Math.Round(H / 2.0, 4));
            var pt4 = new PointD(Math.Round(R - 0.0015, 4), Math.Round(H - 0.0045, 4));
            var pts = new PointD[] { pt1, pt2, pt3, pt4 };
            return pts;
        }
    }
}
