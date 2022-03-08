using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using FEMCommon.Interfaces;
using FEMCommon.Results;
using FEMCommon.Entities;
using FEM.Quenching;

namespace HTWnd
{
    public class NumericModel
    {
        // Graphics
        private bool ShowMesh = true;
        private bool ShowNodes = false;
        private bool ShowElementIds = false;
        private bool ShowResult = true;
        private bool ShowBCond = false;
        private bool ShowThermoCuples = false;

        private double minValue = double.MaxValue;
        private double maxValue = double.MinValue;

        public int TimeStep { get; set; }
        public float[] Time;
        //

        private Color feColor = Color.AliceBlue;
        private ColorMap resultColorMap;

        IFiniteElement[] Elements;
        INode[] Nodes;

        internal IDictionary<float, double[]> Temperatures;
        double _R, _H;
        double _htc1,_htc2,_htc3, _ta, _t0;
        PointD[] _tc;

        public NumericModel(IFiniteElement[] el, INode[] nodes)
        {
            Elements = el;
            Nodes = nodes;
            TimeStep = -1;
            _R = nodes.Max(x => x.P.X);
            _H = nodes.Max(x => x.P.Y);
            _tc = Experiment.createTemp4Sensors(_R, _H);
        }


        public void Draw(Graphics gr, int wndWidth, int wndHeight, double zoom, int shiftX, int shiftY)
        {
            if (Elements == null)
                return;

            gr.Clear(Color.White);

            if (ShowResult && Temperatures != null && Temperatures.Count > 0 && (Temperatures.ContainsKey(TimeStep) || TimeStep==-1))
            {
                try
                {
                    var nodeTemperatures = TimeStep == -1 ? Temperatures.Last().Value : Temperatures[TimeStep];
                    //maxValue = nodeTemperatures.Max();
                    //minValue = nodeTemperatures.Min();

                    //resultColorMap = new ColorMap(Max: maxValue, Min: minValue);

                    //draw results 
                    drawResults(gr, zoom, shiftX, shiftY, resultColorMap, nodeTemperatures);

                    //Draw color map
                    drawColorMap(gr, wndWidth, resultColorMap, nodeTemperatures);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Invalid time step");                    // throw;
                }
            }
            else
            {
                //draw finite element mesh
                if (ShowMesh)
                    drawMesh(gr, zoom, shiftX, shiftY);
            }

            //draw nodes if they are enabled
            if (ShowNodes)
                drawNodes(gr, zoom, shiftX, shiftY);

            //draw element ID if they are enabled
            if (ShowElementIds)
                drawElementIds(gr, zoom, shiftX, shiftY);

            //draw element ID if they are enabled
            //if (ShowBCond && _bValues != null)
            //    drawBC(gr, zoom, shiftX, shiftY);

            if (ShowThermoCuples)
                drawThemoCuples(gr, zoom, shiftX, shiftY);
        }

        private void drawThemoCuples(Graphics gr, double zoom, int shiftX, int shiftY)
        {
            if (_R <= 0 || _H <= 0)
                return;
            int nx, ny;
            string nName;
            // Draw the node numbers
            using (var fnt = new Font("Arial", 12))
            using (var brsh = new SolidBrush(Color.Red))
            {
                //TM1
                nx = Convert.ToInt32((_R - 0.0015f) * zoom + shiftX);
                ny = Convert.ToInt32(-0.0015f * zoom + shiftY);
                nName = ("TM1").ToString();
                var r1 = new Rectangle(nx - 3, ny - 3, 6, 6);
                gr.FillEllipse(brsh, r1);
                gr.DrawString(nName, fnt, brsh, new PointF(nx + 5, ny + 5));

                //TM2
                nx = Convert.ToInt32((_R - 0.0015f) * zoom + shiftX);
                ny = Convert.ToInt32(-(_H / 2f) * zoom + shiftY);
                nName = ("TM2").ToString();
                var r2 = new Rectangle(nx - 3, ny - 3, 6, 6);
                gr.FillEllipse(brsh, r2);
                gr.DrawString(nName, fnt, brsh, new PointF(nx + 5, ny + 5));

                nx = Convert.ToInt32(0 * zoom + shiftX);
                ny = Convert.ToInt32(-(_H / 2f) * zoom + shiftY);
                nName = ("TM3").ToString();
                //
                var r3 = new Rectangle(nx - 3, ny - 3, 6, 6);
                gr.FillEllipse(brsh, r3);
                gr.DrawString(nName, fnt, brsh, new PointF(nx - 35, ny + 5));

                nx = Convert.ToInt32((_R - 0.0015f) * zoom + shiftX);
                ny = Convert.ToInt32(-(_H - 0.0015f) * zoom + shiftY);
                nName = ("TM4").ToString();
                //
                var r4 = new Rectangle(nx - 3, ny - 3, 6, 6);
                gr.FillEllipse(brsh, r4);
                gr.DrawString(nName, fnt, brsh, new PointF(nx + 5, ny + 5));
            }
        }

        //private void drawBC(Graphics gr, double zoom, int shiftX, int shiftY)
        //{
        //    var ptsf = new PointF[2];

        //    // Draw the elements .
        //    for (int i = 0; i <  _bValues.Count; i++)
        //    {
        //        var bv =   _bValues[i];
        //        var fe = Elements.Where(x => x.Id == bv.ElId).FirstOrDefault();
        //        if (fe == null)
        //            throw new Exception("Finite element not found!");

        //        if (bv.HTC != null && bv.HTC.Count > 0)
        //        {
        //            foreach (var bc in bv.HTC)
        //            {
        //                if (bc.Key == 1)
        //                {
        //                    ptsf[0] = new PointF(Convert.ToSingle(Nodes[fe.N[0]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[0]].P.Y * zoom + shiftY));
        //                    ptsf[1] = new PointF(Convert.ToSingle(Nodes[fe.N[1]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[1]].P.Y * zoom + shiftY));
        //                }
        //                else if (bc.Key == 2)
        //                {
        //                    ptsf[0] = new PointF(Convert.ToSingle(Nodes[fe.N[1]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[1]].P.Y * zoom + shiftY));
        //                    ptsf[1] = new PointF(Convert.ToSingle(Nodes[fe.N[2]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[2]].P.Y * zoom + shiftY));
        //                }
        //                else if (bc.Key == 3)
        //                {
        //                    ptsf[0] = new PointF(Convert.ToSingle(Nodes[fe.N[2]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[2]].P.Y * zoom + shiftY));
        //                    ptsf[1] = new PointF(Convert.ToSingle(Nodes[fe.N[0]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[0]].P.Y * zoom + shiftY));
        //                }
        //                else
        //                    throw new Exception("Side is not supported!");


        //                //Draw boundary condition
        //                using (var p = new Pen(Color.Cyan, 2))
        //                {
        //                    gr.DrawPolygon(p, ptsf);
        //                }
        //            }

        //        }


        //    }
        //}

        private void drawResults(Graphics gr, double zoom, int shiftX, int shiftY, ColorMap colorMap, double[] nTemps)
        {
            var ptsf = new PointF[3];
            // Draw the elements .
            for (int i = 0; i < Elements.Length; i++)
            {
                var fe = Elements[i];

                //coordinates of the finite element
                ptsf[0] = new PointF(Convert.ToSingle(Nodes[fe.N[0]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[0]].P.Y * zoom + shiftY));
                ptsf[1] = new PointF(Convert.ToSingle(Nodes[fe.N[1]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[1]].P.Y * zoom + shiftY));
                ptsf[2] = new PointF(Convert.ToSingle(Nodes[fe.N[2]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[2]].P.Y * zoom + shiftY));


                //Calculate temp field
                var xc = (Nodes[fe.N[0]].P.X + Nodes[fe.N[1]].P.X + Nodes[fe.N[2]].P.X) / 3.0;
                var yc = (Nodes[fe.N[0]].P.Y + Nodes[fe.N[1]].P.Y + Nodes[fe.N[2]].P.Y) / 3.0;
                var avgt = fe.Estimate(nTemps, new PointD(xc,yc));
                var eColor = resultColorMap.getColor(avgt);
                using (var br = new SolidBrush(eColor))
                    gr.FillPolygon(br, ptsf);

                //drawTemperatureFields(gr, colorMap,fe, zoom, shiftX, shiftY);

                //draw finite element
                using (var p = new Pen(Color.Black))
                {
                    gr.DrawPolygon(p, ptsf);
                }
            }
        }

        private void drawTemperatureFields(Graphics gr, ColorMap colorMap, IFiniteElement fe, double zoom, int shiftX, int shiftY)
        {
            //if (fe.TField == null)
            //    return;

            //Color eColor;

            ////1. triangle 1-4-9
            //eColor = colorMap.getColor(fe.TField[0]);
            //using (var sb = new SolidBrush(eColor))
            //{
            //    var ptsf = new PointF[3];
            //    ptsf[0] = new PointF(Convert.ToSingle(fe.IN[0].X * zoom + shiftX), Convert.ToSingle(-fe.IN[0].Y * zoom + shiftY));
            //    ptsf[1] = new PointF(Convert.ToSingle(fe.IN[3].X * zoom + shiftX), Convert.ToSingle(-fe.IN[3].Y * zoom + shiftY));
            //    ptsf[2] = new PointF(Convert.ToSingle(fe.IN[8].X * zoom + shiftX), Convert.ToSingle(-fe.IN[8].Y * zoom + shiftY));
            //    gr.FillPolygon(sb, ptsf);
            //    ptsf = null;
            //}

            ////2.  triangle 4-5-10
            //eColor = colorMap.getColor(fe.TField[1]);
            //using (var sb = new SolidBrush(eColor))
            //{
            //    var ptsf = new PointF[3];
            //    ptsf[0] = new PointF(Convert.ToSingle(fe.IN[3].X * zoom + shiftX), Convert.ToSingle(-fe.IN[3].Y * zoom + shiftY));
            //    ptsf[1] = new PointF(Convert.ToSingle(fe.IN[4].X * zoom + shiftX), Convert.ToSingle(-fe.IN[4].Y * zoom + shiftY));
            //    ptsf[2] = new PointF(Convert.ToSingle(fe.IN[9].X * zoom + shiftX), Convert.ToSingle(-fe.IN[9].Y * zoom + shiftY));
            //    gr.FillPolygon(sb, ptsf);
            //    ptsf = null;
            //}

            ////3.  triangle 5-2-6
            //eColor = colorMap.getColor(fe.TField[2]);
            //using (var sb = new SolidBrush(eColor))
            //{
            //    var ptsf = new PointF[3];
            //    ptsf[0] = new PointF(Convert.ToSingle(fe.IN[4].X * zoom + shiftX), Convert.ToSingle(-fe.IN[4].Y * zoom + shiftY));
            //    ptsf[1] = new PointF(Convert.ToSingle(fe.IN[1].X * zoom + shiftX), Convert.ToSingle(-fe.IN[1].Y * zoom + shiftY));
            //    ptsf[2] = new PointF(Convert.ToSingle(fe.IN[5].X * zoom + shiftX), Convert.ToSingle(-fe.IN[5].Y * zoom + shiftY));
            //    gr.FillPolygon(sb, ptsf);
            //    ptsf = null;
            //}

            ////4.  triangle 4-10-9
            //eColor = colorMap.getColor(fe.TField[3]);
            //using (var sb = new SolidBrush(eColor))
            //{
            //    var ptsf = new PointF[3];
            //    ptsf[0] = new PointF(Convert.ToSingle(fe.IN[3].X * zoom + shiftX), Convert.ToSingle(-fe.IN[3].Y * zoom + shiftY));
            //    ptsf[1] = new PointF(Convert.ToSingle(fe.IN[9].X * zoom + shiftX), Convert.ToSingle(-fe.IN[9].Y * zoom + shiftY));
            //    ptsf[2] = new PointF(Convert.ToSingle(fe.IN[8].X * zoom + shiftX), Convert.ToSingle(-fe.IN[8].Y * zoom + shiftY));
            //    gr.FillPolygon(sb, ptsf);
            //    ptsf = null;
            //}

            ////5.  triangle 5-6-10
            //eColor = colorMap.getColor(fe.TField[4]);
            //using (var sb = new SolidBrush(eColor))
            //{
            //    var ptsf = new PointF[3];
            //    ptsf[0] = new PointF(Convert.ToSingle(fe.IN[4].X * zoom + shiftX), Convert.ToSingle(-fe.IN[4].Y * zoom + shiftY));
            //    ptsf[1] = new PointF(Convert.ToSingle(fe.IN[5].X * zoom + shiftX), Convert.ToSingle(-fe.IN[5].Y * zoom + shiftY));
            //    ptsf[2] = new PointF(Convert.ToSingle(fe.IN[9].X * zoom + shiftX), Convert.ToSingle(-fe.IN[9].Y * zoom + shiftY));
            //    gr.FillPolygon(sb, ptsf);
            //    ptsf = null;
            //}

            ////6.  triangle 9-10-8
            //eColor = colorMap.getColor(fe.TField[5]);
            //using (var sb = new SolidBrush(eColor))
            //{
            //    var ptsf = new PointF[3];
            //    ptsf[0] = new PointF(Convert.ToSingle(fe.IN[8].X * zoom + shiftX), Convert.ToSingle(-fe.IN[8].Y * zoom + shiftY));
            //    ptsf[1] = new PointF(Convert.ToSingle(fe.IN[9].X * zoom + shiftX), Convert.ToSingle(-fe.IN[9].Y * zoom + shiftY));
            //    ptsf[2] = new PointF(Convert.ToSingle(fe.IN[7].X * zoom + shiftX), Convert.ToSingle(-fe.IN[7].Y * zoom + shiftY));
            //    gr.FillPolygon(sb, ptsf);
            //    ptsf = null;
            //}

            ////7.  triangle 10-6-7
            //eColor = colorMap.getColor(fe.TField[6]);
            //using (var sb = new SolidBrush(eColor))
            //{
            //    var ptsf = new PointF[3];
            //    ptsf[0] = new PointF(Convert.ToSingle(fe.IN[9].X * zoom + shiftX), Convert.ToSingle(-fe.IN[9].Y * zoom + shiftY));
            //    ptsf[1] = new PointF(Convert.ToSingle(fe.IN[5].X * zoom + shiftX), Convert.ToSingle(-fe.IN[5].Y * zoom + shiftY));
            //    ptsf[2] = new PointF(Convert.ToSingle(fe.IN[6].X * zoom + shiftX), Convert.ToSingle(-fe.IN[6].Y * zoom + shiftY));
            //    gr.FillPolygon(sb, ptsf);
            //    ptsf = null;
            //}

            ////8.  triangle 10-7-8
            //eColor = colorMap.getColor(fe.TField[7]);
            //using (var sb = new SolidBrush(eColor))
            //{
            //    var ptsf = new PointF[3];
            //    ptsf[0] = new PointF(Convert.ToSingle(fe.IN[9].X * zoom + shiftX), Convert.ToSingle(-fe.IN[9].Y * zoom + shiftY));
            //    ptsf[1] = new PointF(Convert.ToSingle(fe.IN[6].X * zoom + shiftX), Convert.ToSingle(-fe.IN[6].Y * zoom + shiftY));
            //    ptsf[2] = new PointF(Convert.ToSingle(fe.IN[7].X * zoom + shiftX), Convert.ToSingle(-fe.IN[7].Y * zoom + shiftY));
            //    gr.FillPolygon(sb, ptsf);
            //    ptsf = null;
            //}

            ////9.  triangle 8-7-3
            //eColor = colorMap.getColor(fe.TField[8]);
            //using (var sb = new SolidBrush(eColor))
            //{
            //    var ptsf = new PointF[3];
            //    ptsf[0] = new PointF(Convert.ToSingle(fe.IN[7].X * zoom + shiftX), Convert.ToSingle(-fe.IN[7].Y * zoom + shiftY));
            //    ptsf[1] = new PointF(Convert.ToSingle(fe.IN[6].X * zoom + shiftX), Convert.ToSingle(-fe.IN[6].Y * zoom + shiftY));
            //    ptsf[2] = new PointF(Convert.ToSingle(fe.IN[2].X * zoom + shiftX), Convert.ToSingle(-fe.IN[2].Y * zoom + shiftY));
            //    gr.FillPolygon(sb, ptsf);
            //    ptsf = null;
            //}
        }

       

      

        private void drawMesh(Graphics gr, double zoom, int shiftX, int shiftY)
        {
            var ptsf = new PointF[3];

            // Draw the elements .
            for (int i = 0; i < Elements.Length; i++)
            {
                var fe = Elements[i];

                ptsf[0] = new PointF(Convert.ToSingle(Nodes[fe.N[0]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[0]].P.Y * zoom + shiftY));
                ptsf[1] = new PointF(Convert.ToSingle(Nodes[fe.N[1]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[1]].P.Y * zoom + shiftY));
                ptsf[2] = new PointF(Convert.ToSingle(Nodes[fe.N[2]].P.X * zoom + shiftX), Convert.ToSingle(-Nodes[fe.N[2]].P.Y * zoom + shiftY));

                // Draw the element
                using (var sb = new SolidBrush(feColor))
                {
                    gr.FillPolygon(sb, ptsf);

                }
                using (var p = new Pen(Color.Black))
                {
                    gr.DrawPolygon(p, ptsf);
                }
            }
        }


        private void drawNodes(Graphics gr, double zoom, int shiftX, int shiftY)
        {
            int nx, ny;
            string nName;
            // Draw the node numbers
            using (var p = new Pen(Color.Black))
            using (var fnt = new Font("Arial", 8))
            using (var brsh = new SolidBrush(Color.Red))
            {
                //
                for (int i = 0; i < Nodes.Length; i++)
                {
                    nx = Convert.ToInt32(Nodes[i].P.X * zoom + shiftX);
                    ny = Convert.ToInt32(-Nodes[i].P.Y * zoom + shiftY);
                    nName = (Nodes[i].Id).ToString();
                    //
                    var r = new Rectangle(nx - 3, ny - 3, 6, 6);
                    gr.DrawEllipse(p, r);
                    gr.DrawString(nName, fnt, brsh, new PointF(nx + 5, ny + 5));
                }
            }
        }


        private void drawElementIds(Graphics gr, double zoom, int shiftX, int shiftY)
        {
            // Draw the elements .
            float nx, ny;

            using (var p = new Pen(Color.Black))
            using (var fnt = new Font("Arial", 8))
            using (var brsh = new SolidBrush(Color.Red))
            {
                for (int i = 0; i < Elements.Length; i++)
                {
                    var fe = Elements[i];
                    var nds = new Node[fe.N.Length];
                    for (int j = 0; j < fe.N.Length; j++)
                        nds[j] = Nodes[fe.N[j]] as Node;

                    nx = Convert.ToSingle(nds.Average(x => x.P.X) * zoom + shiftX);
                    ny = Convert.ToSingle(-nds.Average(x => x.P.Y) * zoom + shiftY);

                    // Draw the element id
                    gr.DrawString(fe.Id.ToString(), fnt, brsh, new PointF(nx + 2, ny + 2));
                }
            }
        }


        public (double w0, double h0, double w, double h) GetSize()
        {
            if (Elements != null && Elements.Length > 0)
            {
                var w = Math.Round(Nodes.Max(x => x.P.X),4);
                var h = Math.Round(Nodes.Max(x => x.P.Y),4);
                var w0 = Math.Round(Nodes.Min(x => x.P.X),4);
                var h0 = Math.Round(Nodes.Min(x => x.P.Y),4);
                return (w0, h0, w, h);
            }
            return (0, 0, 0, 0);
        }


        private void drawColorMap(Graphics gr, int imageWidth, ColorMap resultColorMap, double[] tField)
        {
            //create color map
            var cmap = new ColorMap(250, 0);
            Color c;
            int x1, x2, yy;
            yy = 50;
            x1 = imageWidth - 100;
            x2 = x1 + 50;

            var step = Math.Round(Math.Abs(maxValue - minValue) / 250.0, 2);
            var value = maxValue;

            for (int i = 250; i >= 0; i -= 1)
            {
                c = resultColorMap.getColor(value);
                yy += 1;
                value -= step;
                using (var p = new Pen(c))
                {
                    gr.DrawLine(p, new System.Drawing.Point(x1, yy), new System.Drawing.Point(x2, yy));
                }
            }

            //write some values beside color
            int strWidth;
            float MaxWidth, MinWidth, AvgWidth;
            string Max, Min, Avg;
            using (var barFont = new Font("Arial", 9))
            using (var barBrush = new SolidBrush(Color.Black))
            {
                Max = resultColorMap.Max.ToString("F");
                Min = resultColorMap.Min.ToString("F");

                Avg = ((resultColorMap.Max + resultColorMap.Min) / 2).ToString("F");

                MaxWidth = gr.MeasureString(Max, barFont).Width;
                MinWidth = gr.MeasureString(Min, barFont).Width;
                AvgWidth = gr.MeasureString(Avg, barFont).Width;

                strWidth = Convert.ToInt32(Math.Max(Math.Max(MaxWidth, MinWidth), AvgWidth)) + 10;

                gr.DrawLine(Pens.Black, x1, 50, x1 - 5, 50);
                gr.DrawString(Max, barFont, barBrush, new PointF(x1 - strWidth, 30));
                gr.DrawLine(Pens.Black, x1, 175, x1 - 5, 175);
                gr.DrawString(Avg, barFont, barBrush, new PointF(x1 - strWidth, 155));
                gr.DrawLine(Pens.Black, x1, 300, x1 - 5, 300);
                gr.DrawString(Min, barFont, barBrush, new PointF(x1 - strWidth, 320));

                //draw calculated temperatures
                //int offset = 320+ 30;
                //gr.DrawString($"time={TimeStep} s.", barFont, barBrush, new PointF(x1 - strWidth, offset));
                //offset += 30;
                //var tt1 = Math.Round(Util.CalculateTemperature(Elements, Nodes,tField, _tc[0].X, _tc[0].Y ),2);
                //gr.DrawString($"T1={tt1}", barFont, barBrush, new PointF(x1 - strWidth, offset));
                //offset += 25;
                //var tt2 = Math.Round(Util.CalculateTemperature(Elements, Nodes, tField, _tc[1].X, _tc[1].Y),2);
                //gr.DrawString($"T2={tt2}", barFont, barBrush, new PointF(x1 - strWidth, offset));
                //offset += 25;
                //var tt3 = Math.Round(Util.CalculateTemperature(Elements, Nodes, tField, _tc[2].X, _tc[2].Y),2);
                //gr.DrawString($"T3={tt3}", barFont, barBrush, new PointF(x1 - strWidth, offset));
                //offset += 25;
                //var tt4 = Math.Round(Util.CalculateTemperature(Elements, Nodes, tField, _tc[3].X, _tc[3].Y),2);
                //gr.DrawString($"T4={tt4}", barFont, barBrush, new PointF(x1 - strWidth, offset));

            }

        }

        

        ///// <summary>
        ///// 
        ///// </summary>
        //internal void Solve()
        //{
        //    resetValues();

        //    var solver = new DHTCSolver(Elements, Nodes, _mProp);
        //    Temperatures = solver.SteadySolve(_bValues);

        //    //
        //    SetMinMaxValues();

        //    ShowResult = true;
        //}

        public void SetMinMaxValues(int dt = -1)
        {
            if (Temperatures == null || Temperatures.Count == 0)
            {
                resetValues();
                return;
            }
            //
            if (Temperatures.ContainsKey(dt))
            {
                maxValue = Temperatures[dt].Max(x => x);
                minValue = Temperatures[dt].Min(x => x);
            }
            else
            {
                maxValue = Temperatures.Values.Max(x => x.Max());
                minValue = Temperatures.Values.Min(x => x.Min());
            }

            //create colormap
            resultColorMap = new ColorMap(Max: maxValue, Min: minValue);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void UnSteadySolve(double timeStart, double timeStep, int stepCount, double initialTemp)
        {
            throw new NotImplementedException();
            //resetValues();

            //_t0 = initialTemp;

            //var solver = new HTCSolver(Elements, Nodes, mProperties);
            //solver.SetupBC(boundaryConditions,timeStart, timeStep, stepCount, initialTemp);
            //Temperatures = solver.SolveInTimeExplicit();//.UnSteadySolve();

            ////
            //SetMinMaxValues();
            //ShowResult = true;
        }

       
        private void resetValues()
        {
            minValue = double.MaxValue;
            maxValue = double.MinValue;
        }

        internal void ShowBC(bool value)
        {
            ShowBCond = value;
        }
        internal bool GetShowBC()
        {
            return ShowBCond;
        }

        internal void ShowTC(bool value)
        {
           ShowThermoCuples = value;
        }
        internal bool GetShowTC()
        {
            return ShowThermoCuples;
        }

        internal void ShowElementsNodes(bool value)
        {
            ShowNodes = value;
        }
        internal bool GetShowElementsNodes()
        {
            return ShowNodes;
        }

        internal void ShowFiniteElementIds(bool value)
        {
            ShowElementIds = value;
        }
        internal bool GetFiniteElementIds()
        {
            return ShowElementIds;
        }
        internal void ShowAnalysisResult(bool value)
        {
            ShowResult = value;

        }
        internal bool GetShowAnalysisResult()
        {
            return ShowResult;
        }

    }
}
