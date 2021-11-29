using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FEM.Quenching
{
    public class Helper
    {
        public static List<(float time, double tm1, double tm2, double tm3, double tm4)> LoadTemperatures(string filePath)
        {
            var str = File.ReadAllLines(filePath);
            var lines = str.Where(x => x.Length > 0 && x[0] != '!').ToList();
            var retVal = new List<(float time, double tm1, double tm2, double tm3, double tm4)>();
            foreach (var l in lines)
            {
                var ll = l.Split('\t', StringSplitOptions.RemoveEmptyEntries);
                if (ll.Length != 5)
                    throw new Exception("File is not consistent.");
                var t = float.Parse(ll[0], CultureInfo.InvariantCulture);
                var t1 = double.Parse(ll[1], CultureInfo.InvariantCulture);
                var t2 = double.Parse(ll[2], CultureInfo.InvariantCulture);
                var t3 = double.Parse(ll[3], CultureInfo.InvariantCulture);
                var t4 = double.Parse(ll[4], CultureInfo.InvariantCulture);

                retVal.Add((t, t1, t2, t3, t4));

            }

            return retVal;
        }

        

    }
}
