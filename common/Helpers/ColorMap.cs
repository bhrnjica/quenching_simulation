using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEMCommon.Results
{
    public class ColorMap
    {
        public ColorMap()
        {
            Colors = new Color[nSegs + 1];
        }

        public ColorMap(double Max, double Min)
        {
            Colors = new Color[nSegs + 1];
            this.Max = Max;
            this.Min = Min;
            setColors();
        }
        // A simple implementation of colormap.
        // only red-blue map is implemented.

        public double Max { get; set; }
        public double Min { get; set; }

        private int nSegs = 100;
        private Color[] Colors;

        public Color getColor(double Value)
        {
            // value exists somewhere between min and max..
            // we find color between min color and max color corresponding to value and return
            double rangeVal = Max - Min;
            if (rangeVal < 0.00001)
                return Colors[0];
            double dv = Value - Min;
            double ratio = dv / rangeVal;

            if(double.IsNaN(ratio))
                return Colors[0];

            int colIndex = Convert.ToInt32(ratio * nSegs);
            if (colIndex < 0)
                colIndex = 0;
            if (colIndex > nSegs)
                return Colors[Colors.Length-1];
            return Colors[colIndex];
        }

        private void setColors()
        {

            //
            var mainColors = new Color[] {Color.Blue, Color.Cyan, Color.LightGreen,Color.Yellow, Color.Red, };
            int nColors = mainColors.Length;
            // this is from top to bottom.

            double percentStep = 100 / (double)(nColors - 1);
            var Indices = new int[nColors]; // we will have these many main indices
            int i, j;
            
            for (i = 0; i < Indices.Length; i++)
                Indices[i] = Convert.ToInt32(i * percentStep / 100 * nSegs);

            Indices[0] = 0;
            Indices[Indices.Length - 1] = nSegs;
            Colors = new Color[nSegs + 1];
            Color[] c;
            var loopTo1 = Indices.Length - 2;
            for (i = 0; i <= loopTo1; i++)
            {
                c = getSteppedColors(mainColors[i + 1], mainColors[i], Indices[i + 1] - Indices[i]);
                var loopTo2 = c.Length - 1;
                for (j = 0; j <= loopTo2; j++)
                    Colors[Indices[i] + j] = c[j];
            }
        }

        private void setColors1()
        {

            //
            var mainColors = new Color[] { Color.Blue, Color.Cyan, Color.LightGreen, Color.Yellow, Color.Red, };
            int nColors = mainColors.Length;
            // this is from top to bottom.

            double percentStep = 100 / (double)(nColors - 1);
            var Indices = new int[nColors]; // we will have these many main indices
            int i, j;

            var loopTo = nColors - 1;
            for (i = 0; i <= loopTo; i++)
                Indices[i] = Convert.ToInt32(i * percentStep / 100 * nSegs);

            Indices[0] = 0;
            Indices[Indices.Length - 1] = nSegs;
            Colors = new Color[nSegs + 1];
            Color[] c;
            var loopTo1 = Indices.Length - 2;
            for (i = 0; i <= loopTo1; i++)
            {
                c = getSteppedColors(mainColors[i + 1], mainColors[i], Indices[i + 1] - Indices[i]);
                var loopTo2 = c.Length - 1;
                for (j = 0; j <= loopTo2; j++)
                    Colors[Indices[i] + j] = c[j];
            }
        }
        private Color[] getSteppedColors(Color cTop, Color cBottom, int nPoints)
        {
            var btm = cBottom;
            var top = cTop;
            var cols = new Color[nPoints + 1];
            int i;
            int dr, dg, db;
            dr = Convert.ToInt32(top.R) - Convert.ToInt32(btm.R);
            dg = Convert.ToInt32(top.G) - btm.G;
            db = Convert.ToInt32(top.B) - btm.B;

            // these differences have to be spanned in the nPoints-1 steps
            double stepR, stepG, stepB;
            stepR = dr / (double)nPoints;
            stepG = dg / (double)nPoints;
            stepB = db / (double)nPoints;
            int r, g, b;
            var loopTo = nPoints;
            for (i = 0; i <= loopTo; i++)
            {
                r = Convert.ToInt32(btm.R + i * stepR);
                g = Convert.ToInt32(btm.G + i * stepG);
                b = Convert.ToInt32(btm.B + i * stepB);
                cols[i] = Color.FromArgb(r, g, b);
            }

            return cols;
        }

        private void setRBColors()
        {
            var top = Color.Blue;
            var btm = Color.Red;
            int i;
            int dr, dg, db;
            dr = Convert.ToInt32(top.R) - Convert.ToInt32(btm.R);
            dg = Convert.ToInt32(top.G) - btm.G;
            db = Convert.ToInt32(top.B) - btm.B;

            // these differences have to be spanned in the nSeg steps
            double stepR, stepG, stepB;
            stepR = dr / (double)nSegs;
            stepG = dg / (double)nSegs;
            stepB = db / (double)nSegs;
            int r, g, b;
            var loopTo = nSegs;
            for (i = 0; i <= loopTo; i++)
            {
                r = Convert.ToInt32(btm.R + i * stepR);
                g = Convert.ToInt32(btm.G + i * stepG);
                b = Convert.ToInt32(btm.B + i * stepB);
                Colors[i] = Color.FromArgb(r, g, b);
            }
        }
    }
}
