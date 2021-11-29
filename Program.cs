using System;
using System.Collections.Generic;
using System.Linq;
using XPlot.Plotly;
using FEMCommon.Interfaces;
using FEMHeat.Lib.Solvers;
using FEMHeatLib.NModel;
using FEM.Quenching.InverseSolvers;
using Daany;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
namespace FEM.Quenching
{
    class Program
    {
        static string root = "";
        static void Main(string[] args)
        {
            var cylinders = new string[] { "25x100" };//, "50x150", "75x225" };
            var quenchants = new string[] { "H2O" } ;// }, "Aquatensid5%", "Isorapid" };
            var result = false;
            var folder = "quenching_simulation";
            var path= Directory.GetCurrentDirectory();
            var index = path.LastIndexOf(folder)+folder.Length;
            root = Path.Combine(path.Substring(0, index), "data");

             //  Invoke this sample with an arbitrary set of command line arguments.
            string[] arguments = Environment.GetCommandLineArgs();
            if(arguments.Length > 1 && arguments.Length !=3 && arguments.Length !=4)
            {
                Console.Write("Command line args are not valid. Example: (--all --all) or (--25x100 H2O)");
                return;
            }
            else if (arguments.Length <= 1)
            {
                cylinders = new string[] { "25x100" , "50x150", "75x225" };
                quenchants = new string[] { "H2O", "Aquatensid5%", "Isorapid" };

            }
            else 
            {
                if(arguments[1] == "--all")
                    cylinders = new string[] { "25x100" , "50x150", "75x225" };
                else if (arguments[1] == "--25x100" ||arguments[1] == "--50x150" || arguments[1] == "--75x225")
                     cylinders = new string[] {arguments[1].Substring(2) };
                else
                {
                    Console.Write("Cylinder Command line arg is not valid. Example: (--all or --25x100 or --50x150 or 75x225)");
                    return;
                }

                if(arguments[2] == "--all")
                    quenchants = new string[] { "H2O", "Aquatensid5%", "Isorapid" };
                else if (arguments[2] == "--H2O" || arguments[2] == "--aquatensid5%" || arguments[2] == "--isorapid")
                     quenchants = new string[] {arguments[2].Substring(2) };
                else
                {
                    Console.Write("Quenchant Command line arg is not valid. Example: (--all or --H2O or --aquatensid5% or --isorapid)");
                    return;
                }

                if(arguments.Length==4)
                {
                    result = true;
                }
                   
            }

            
            var i = 1;

            //clear error summary table
            clearErrorTable();
            var sw= Stopwatch.StartNew();
            foreach (var cyl in cylinders)
            {
                foreach(var quen in quenchants)
                {
                   
                    if(result)
                    {
                         Console.WriteLine($"Results for Cylinder:{cyl} immersed in {quen} will be shown.");
                        showResults(cyl, quen);

                    }
                    else
                    {
                        Console.WriteLine($"Numerical simulation for Cylinder:{cyl} immersed in {quen} has been started.");
                        runSimulation(cyl, quen,i);
                        Console.WriteLine($"Numerical simulation for Cylinder:{cyl} immersed in {quen} has completed.");

                    }

                    Console.WriteLine($" ");
                    i++;
                }
            }

            if(!result)
                Console.WriteLine($"Numerical simulation took {sw.Elapsed.TotalMinutes} minutes.");
        }

        private static void clearErrorTable()
        {
            var esumm = DataFrame.FromCsv(Path.Combine(root,"error_summary.txt"));
            File.Delete(Path.Combine(root,"error_summary.txt"));
            var error = DataFrame.CreateEmpty(esumm.Columns);
            DataFrame.ToCsv(Path.Combine(root,"error_summary.txt"), error);
        }

        private static void runSimulation(string cylinderDim, string quenchant, int i)
        {
            //lod boundary conditions
            var bc2D = BoundaryConditions.LoadFromFile(Path.Combine(root,cylinderDim, quenchant,"bc.txt"));

            //Load experimental measures of temperatures till defined timestep.
            var Y = Helper.LoadTemperatures(Path.Combine(root,cylinderDim, quenchant,"t_data.txt")).Where(x => x.time <= bc2D.time.Max()).ToList();

            //check ambient temperature
            var minTemp = Y.Select(x => Math.Min(Math.Min(Math.Min(x.tm1, x.tm2), x.tm3), x.tm4)).Min();
            if (minTemp < bc2D.ta)
                throw new Exception("Temperature of the quenchant cannot be lower than ambient temperature.");

            //Define material properties.
            IMData mdata = new MHeat(20.5, 7850, 460);//initial values of the properties
            bc2D._mprop = mdata;

            //Load 1D model
            var model1D = Model.LoadFromFile(Path.Combine(root,cylinderDim, "1d_model.txt"));
            var directSolver = new DHT(model1D.fe, model1D.nds, mdata);

            //perform three independent optimizations

            //optimization at position 1
            var bc1 = new BoundaryConditions1D(bc2D, ThermoCouples.Bottom);
            bc1.Tloc = Experiment.createTempSensor(bc2D.R, bc2D.H, ThermoCouples.Bottom);
            var yy1 = Y.Select(x => (x.time, x.tm1)).ToList();

            //set initial values for multi-objective optimization 
            var htc1 = optimize1dProblem(model1D.fe, model1D.nds, directSolver, bc1, yy1, ThermoCouples.Bottom);

            //optimization at location 2
            var bc2 = new BoundaryConditions1D(bc2D, ThermoCouples.Middle);
            bc2.Tloc = Experiment.createTempSensor(bc2D.R, bc2D.H, ThermoCouples.Middle);
            var yy2 = Y.Select(x => (x.time, x.tm2)).ToList();

            //set initial values for multi-objective optimization 
            var htc2 = optimize1dProblem(model1D.fe, model1D.nds, directSolver, bc2, yy2, ThermoCouples.Middle);

            //optimization at location 3
            var bc3 = new BoundaryConditions1D(bc2D, ThermoCouples.Top);
            bc3.Tloc = Experiment.createTempSensor(bc2D.R, bc2D.H, ThermoCouples.Top);
            var yy3 = Y.Select(x => (x.time, x.tm4)).ToList();

            //set initial values for multi-objective optimization 
            var htc3 = optimize1dProblem(model1D.fe, model1D.nds, directSolver, bc3, yy3, ThermoCouples.Top);


            //define necessary measured points for multi-objective optimization
            bc2D.Tloc = Experiment.createTemp3Sensors(bc2D.R, bc2D.H);
            var yy2D = Y.Select(x => (x.time, x.tm1, x.tm2, x.tm4)).ToList();

            //prepare for multi-objective optimization
            var model2D = Model.LoadFromFile(Path.Combine(root,cylinderDim,"2d_model.txt"));
            var direct2DSolver = new DHT(model2D.fe, model2D.nds, mdata);
            bc2D.htc1 = htc1;
            bc2D.htc2 = htc2;
            bc2D.htc3 = htc3;
            var optBc = bc2D.Clone() as BoundaryConditions;
            int Steps = optBc.TimeSteps;
            optBc.TimeSteps = 0;

            //Multi-objective optimization
            var invSolver = new Inv2DLMMHTSolverEx(direct2DSolver, yy2D);
            var oBC = invSolver.Optimize(optBc, Steps);

            //post processing results
            resultPostprocessing(cylinderDim, quenchant, Y, direct2DSolver, oBC,i);
        }

        private static void resultPostprocessing(string cylinderDim, string quenchant, List<(float time, double tm1, double tm2, double tm3, double tm4)> Y, DHT direct2DSolver, BoundaryConditions oBC, int i)
        {
            //calculate DHT and show results
            var optBC = oBC.Clone() as BoundaryConditions;
            optBC.Tloc = Experiment.createTemp4Sensors(optBC.R, optBC.H);
            var retVal = direct2DSolver.Solve(optBC);
            var temps = direct2DSolver.Calculate(retVal, optBC.Tloc);
            var ttime = optBC.time.Take(optBC.TimeSteps + 1);
            var dn1 = temps.Select(x => x.Value[0]).ToArray();
            var dm1 = Y.Where(x => ttime.Contains(x.time)).Select(x => x.tm1).ToArray();
            var dn2 = temps.Select(x => x.Value[1]).ToArray();
            var dm2 = Y.Where(x => ttime.Contains(x.time)).Select(x => x.tm2).ToArray();
            var dn3 = temps.Select(x => x.Value[2]).ToArray();
            var dm3 = Y.Where(x => ttime.Contains(x.time)).Select(x => x.tm3).ToArray();
            var dn4 = temps.Select(x => x.Value[3]).ToArray();
            var dm4 = Y.Where(x => ttime.Contains(x.time)).Select(x => x.tm4).ToArray();

            var dic = new Dictionary<string, List<object>>();
            dic.Add("t", ttime.Select(x => (object)x).ToList());
            dic.Add("TM1", dm1.Select(x => (object)x).ToList());
            dic.Add("TN1", dn1.Select(x => (object)x).ToList());
            dic.Add("TM2", dm2.Select(x => (object)x).ToList());
            dic.Add("TN2", dn2.Select(x => (object)x).ToList());
            dic.Add("TM3", dm3.Select(x => (object)x).ToList());
            dic.Add("TN3", dn3.Select(x => (object)x).ToList());
            dic.Add("TM4", dm4.Select(x => (object)x).ToList());
            dic.Add("TN4", dn4.Select(x => (object)x).ToList());

            //Save optimized values and results
            System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(root,cylinderDim, quenchant,"results"));
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            var temResults = new Daany.DataFrame(dic);
            var cols = new string[] { "R1", "R2", "R3", "R4", "RE1", "RE2", "RE3", "RE4" };

            temResults.AddCalculatedColumns(cols, (IDictionary<string, object> r, int i) =>
            {
                var tm1 = Convert.ToDouble(r["TM1"]);
                var tn1 = Convert.ToDouble(r["TN1"]);
                var tm2 = Convert.ToDouble(r["TM2"]);
                var tn2 = Convert.ToDouble(r["TN2"]);
                var tm3 = Convert.ToDouble(r["TM3"]);
                var tn3 = Convert.ToDouble(r["TN3"]);
                var tm4 = Convert.ToDouble(r["TM4"]);
                var tn4 = Convert.ToDouble(r["TN4"]);
                var r1 = Math.Abs(tm1 - tn1);
                var r2 = Math.Abs(tm2 - tn2);
                var r3 = Math.Abs(tm3 - tn3);
                var r4 = Math.Abs(tm4 - tn4);
                var re1 = r1 / tm1;
                var re2 = r2 / tm2;
                var re3 = r3 / tm3;
                var re4 = r4 / tm4;
                return new object[8] { r1, r2, r3, r4, re1, re2, re3, re4 };
            });

            //load error summary table and insert the error values for the current simulation
            var aetm1 = temResults["R1"].Select(x => Convert.ToDouble(x));
            var aetm2 = temResults["R2"].Select(x => Convert.ToDouble(x));
            var aetm3 = temResults["R3"].Select(x => Convert.ToDouble(x));
            var aetm4 = temResults["R4"].Select(x => Convert.ToDouble(x));
            var retm1 = temResults["RE1"].Select(x => Convert.ToDouble(x));
            var retm2 = temResults["RE2"].Select(x => Convert.ToDouble(x));
            var retm3 = temResults["RE3"].Select(x => Convert.ToDouble(x));
            var retm4 = temResults["RE4"].Select(x => Convert.ToDouble(x));

            var esumm = DataFrame.FromCsv(Path.Combine(root,"error_summary.txt"));
            File.Delete(Path.Combine(root,"error_summary.txt"));
            esumm.AddRow(new List<object>() {$"NS{i}", "Tm1", aetm1.Max(), aetm1.Average(), aetm1.Min(), retm1.Max(), retm1.Average(), retm1.Min()});
            esumm.AddRow(new List<object>() { $"NS{i}", "Tm2", aetm2.Max(), aetm2.Average(), aetm2.Min(), retm2.Max(), retm2.Average(), retm2.Min() });
            esumm.AddRow(new List<object>() { $"NS{i}", "Tm3", aetm3.Max(), aetm3.Average(), aetm3.Min(), retm3.Max(), retm3.Average(), retm3.Min() });
            esumm.AddRow(new List<object>() { $"NS{i}", "Tm4", aetm4.Max(), aetm4.Average(), aetm4.Min(), retm4.Max(), retm4.Average(), retm4.Min() });
            DataFrame.ToCsv(Path.Combine(root,"error_summary.txt"),esumm);

            DataFrame.ToCsv(Path.Combine(root,cylinderDim, quenchant, "results","t_calculated.txt"), temResults);
            BoundaryConditions.SaveToFile(optBC, Path.Combine(root, cylinderDim, quenchant, "results","htc_optimized.txt"));
            Model.SaveToFile(direct2DSolver.Fes, direct2DSolver.Nodes, null, retVal,Path.Combine(root,cylinderDim, quenchant, "results",$"2d_model_results_{DateTime.Now.Ticks}.txt"));
        }

        private static double[] optimize1dProblem(IFiniteElement[] elems, INode[] nodes, DHT directSol, BoundaryConditions1D bc, List<(float time, double tm)> Y, ThermoCouples tc)
        {
            //Get the measured temperature
            var Ym = Y.Select(x => x.tm);
            var pt = bc.Tloc.First();
            //setup time steps
            int Steps = bc.TimeSteps;
            bc.TimeSteps = 0;//we start from initial time 

            var invSolver = new Inv1DLMMHTCSolverEx(directSol, Y);
            var htc = invSolver.Optimize(bc, Steps);
            return htc.ToArray();
        }

        private static void showResults(string cylinderDim, string quenchant)
        {
            //Load experimental measures  of temperatures 
            var Y = Helper.LoadTemperatures(Path.Combine(root,cylinderDim,quenchant, "t_data.txt"));
            var bc = BoundaryConditions.LoadFromFile(Path.Combine(root,cylinderDim,quenchant,"results","htc_optimized.txt"));
            var Ye = DataFrame.FromCsv(Path.Combine(root,cylinderDim,quenchant,"results","t_calculated.txt"));
            var ttime = Ye["t"].Select(x => Convert.ToInt32(x));
            var tm1 = Ye["TM1"].Select(x => Convert.ToDouble(x));
            var tn1 = Ye["TN1"].Select(x => Convert.ToDouble(x));

            var tm2 = Ye["TM2"].Select(x => Convert.ToDouble(x));
            var tn2 = Ye["TN2"].Select(x => Convert.ToDouble(x));

            var tm3 = Ye["TM3"].Select(x => Convert.ToDouble(x));
            var tn3 = Ye["TN3"].Select(x => Convert.ToDouble(x));

            var tm4 = Ye["TM4"].Select(x => Convert.ToDouble(x));
            var tn4 = Ye["TN4"].Select(x => Convert.ToDouble(x));

            var l1 = new Line() { shape = "spline", color="black", width=0.3 };
            var l2 = new Line() { dash="dash", shape = "line", color = "black", width = 0.3 };
            var font = new Textfont() {family= "Times New Roman", size=13, color= "black", };
            var fontTitle = new Font() { family = "Times New Roman", color = "black", size = 16 };
            var font1 = new Font() { family = "Times New Roman", color = "black", size = 13 };

            var layout1 = new Layout.Layout()
            {
                titlefont = fontTitle,
                font = font1,
                xaxis = new Xaxis() { titlefont = font1, tickfont = font1,  showgrid=true },
                yaxis = new Yaxis() { titlefont = font1, tickfont = font1,  showgrid = true },
                width = 500,
                height = 350, legend= new Legend() { font=font1}

            };
            var layout2 = new Layout.Layout()
            {
                titlefont = fontTitle,
                font = font1,
                xaxis = new Xaxis() { titlefont = font1, tickfont = font1, showgrid = true },
                yaxis = new Yaxis() { titlefont = font1, tickfont = font1, showgrid = true },
                width = 500,
                height = 350,
                legend = new Legend() { font = font1 }

            };
            var layout3 = new Layout.Layout()
            {
                titlefont = fontTitle,
                font = font1,
                xaxis = new Xaxis() { titlefont = font1, tickfont = font1, showgrid = true },
                yaxis = new Yaxis() { titlefont = font1, tickfont = font1, showgrid = true },
                width=500, height=350,
                legend = new Legend() { font = font1 }
            };

            var circle01 = new Marker() { color = "white", symbol="circle", size = 7, line = new Line() { color = "black", width = 0.3 } };
            var circle = new Marker() { color = "black", symbol = "circle", size = 7, line = new Line() { color = "black", width = 0.3 } };
            var square = new Marker() { color = "black", symbol = "square", size = 7, line = new Line() { color = "black", width = 0.3 } };
            var diamond = new Marker() { color = "black", symbol = "diamond", size = 7, line = new Line() { color = "black", width = 0.3 } };
            var triangle = new Marker() { color = "black", symbol = "cross", size = 7, line = new Line() { color = "black", width = 0.3 } };

            var stm1 = new Scatter() {textfont=font, name = "TM 1", x = ttime, y = tm1, mode = "markers", marker=circle };
            var stn1 = new Scatter() {textfont=font, name = "TC 1", x = ttime, y = tn1, mode = "line", fillcolor = "black", line = l1 };
            var stm2 = new Scatter() {textfont=font, name = "TM 2", x = ttime, y = tm2, mode = "markers", marker = square };
            var stn2 = new Scatter() {textfont=font, name = "TC 2", x = ttime, y = tn2, mode = "line", fillcolor = "black", line = l1 };
            var stm3 = new Scatter() {textfont=font, name = "TM 3", x = ttime, y = tm3, mode = "markers", marker = diamond };
            var stn3 = new Scatter() {textfont=font, name = "TC 3", x = ttime, y = tn3, mode = "line", fillcolor = "black", line = l1 };
            var stm4 = new Scatter() {textfont=font, name = "TM 4", x = ttime, y = tm4, mode = "markers", marker = triangle };
            var stn4 = new Scatter() {textfont=font, name = "TC 4", x = ttime, y = tn4, mode = "line", fillcolor = "black", line = l1 };

            var chart1 = XPlot.Plotly.Chart.Plot<Scatter>(new Scatter[] { stm1, stn1, stm2, stn2, stm3, stn3, stm4, stn4 },layout1);
            chart1.WithTitle($"Numerical results for cylinder '{cylinderDim}' mm immersed in '{quenchant}'.");
            chart1.WithXTitle($"time, [sec]");
            chart1.WithYTitle($"temperature, [°C]");

            var line = new Scatter() {textfont=font, name = "", x = new double[] {0,850 }, y = new double[] { 0, 850 }, mode = "lines", line=l2 };
            var res1 = new Scatter() {textfont=font, name = "T1", x = tm1, y = tn1, mode = "markers", marker = circle01 };
            var res2 = new Scatter() {textfont=font, name = "T2",x = tm2, y = tn2, mode = "markers", marker = circle01 };
            var res3 = new Scatter() {textfont=font, name = "T3", x = tm3, y = tn3, mode = "markers", marker = circle01 };
            var res4 = new Scatter() {textfont = font, name = "T4", x = tm4, y = tn4, mode = "markers", marker = circle01 };

            var chart2 = XPlot.Plotly.Chart.Plot<Scatter>(new Scatter[] {line, res1,res2,res3,res4 },layout2);
            chart2.WithTitle($"Residual plot for cylinder '{cylinderDim}' mm immersed in '{quenchant}'.");
            chart2.WithXTitle($"measured temperature, [°C]");
            chart2.WithYTitle($"calculated temperature, [°C]");
            chart2.WithLegend(false);

            var htc1 = new Scatter() {textfont=font, name = "α1", x = ttime, y = bc.htc1, mode = "lines+markers", marker = circle, line = l1 };
            var htc2 = new Scatter() {textfont=font, name = "α2", x = ttime, y = bc.htc2, mode = "lines+markers", marker = diamond, line = l1 };
            var htc3 = new Scatter() {textfont = font, name = "α3", x = ttime, y = bc.htc3, mode = "lines+markers", marker = square, line = l1 };

            var chart3 = XPlot.Plotly.Chart.Plot<Scatter>(new Scatter[] { htc1, htc2, htc3 }, layout3);
            chart3.WithTitle($"Optimized HTCs for cylinder '{cylinderDim}' mm immersed in '{quenchant}'.");
            chart3.WithXTitle($"time, [sec]");
            chart3.WithYTitle($"HTC,α [W/m^2 K]");

            var lst = new List<XPlot.Plotly.PlotlyChart>() { chart1, chart2, chart3};
            Chart.ShowAll(lst);



        }

    }
}
