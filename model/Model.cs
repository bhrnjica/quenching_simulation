using FEMCommon.Entities;
using FEMCommon.Interfaces;
using FEMCommon.Types;
using FEMHeat.Lib.AxiSymmetrics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace FEMHeatLib.NModel
{
    public class Model
    {
        public static (IFiniteElement[] fe, INode[] nds, IDictionary<string, double> mdata, IDictionary<float, double[]> results) LoadFromFile(string filePath)
        {
            var mdata = new Dictionary<string, double>();
            var str = File.ReadAllLines(filePath);
            var lines = str.Where(x => x.Length > 0 && x[0] != '!').ToList();

            //first line should be material data
            int lineCount = 0;
            if(lines[0].StartsWith("MData:"))
            {
                var data = lines[0].Substring("MData".Length).Split(new char[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < data.Length; i += 2)
                    mdata.Add(data[i], double.Parse(data[i+1]));
                lineCount++;
            }
            //second line is Boundary condition if defined
            if (lines[0].StartsWith("BC:"))
            {
                var data = lines[0].Substring("MData".Length).Split(new char[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < data.Length; i += 2)
                    mdata.Add(data[i], double.Parse(data[i + 1]));
                lineCount++;
            }

            //next line "nodes"

            var prop = lines[lineCount].Split(',', StringSplitOptions.RemoveEmptyEntries);
            //node count
            int nCount = int.Parse(prop[0]);
            //next line node type
            lineCount++;
            prop = lines[lineCount].Split(',', StringSplitOptions.RemoveEmptyEntries);
            int type = int.Parse(prop[0]);

            var nodes = new List<INode>();
            for (int i = 0; i < nCount; i++)
            {
                lineCount++;
                prop = lines[lineCount].Split(',', StringSplitOptions.RemoveEmptyEntries);
                
                var node = new Node();
                node.Id = i;
                node.NT = (NodeType)type;
                //parse coordinate
                double x1 = double.Parse(prop[0], CultureInfo.InvariantCulture);
                double x2 = double.Parse(prop[1], CultureInfo.InvariantCulture);
                double x3 = 0;
                //check for third coordinate
                if(prop.Length==3)
                    x3= double.Parse(prop[3], CultureInfo.InvariantCulture);
                //create point
                var p = new PointD(x1,x2,x3);
                node.P = p;
                //add node
                nodes.Add(node);
            }

            //next line "finite element count"
            lineCount++;
            prop = lines[lineCount].Split(',', StringSplitOptions.RemoveEmptyEntries);
            int feCount = int.Parse(prop[0]);

            //next line "finite element type"
            lineCount++;
            prop = lines[lineCount].Split(',', StringSplitOptions.RemoveEmptyEntries);
            int feType = int.Parse(prop[0]);

            var fes = new List<IFiniteElement>();
            for (int i = 0; i < feCount; i++)
            {
                lineCount++;
                prop = lines[lineCount].Split(',', StringSplitOptions.RemoveEmptyEntries);
                var nodeIds = prop.Select(x=> int.Parse(x)).ToArray();
                var nds = nodeIds.Select(x=> nodes[x]).ToArray();
                IFiniteElement fe = null;
                if (feType == 1)//line element
                    fe = new AxiLIN(i, nds);
                else if(feType == 3)//triangle
                    fe = new AxiCTT(i, nds);
                else
                    throw new NotSupportedException("Finite element type is not supported.");
                fes.Add(fe);
            }

            //next line "temperature field"
            lineCount++;
            var results = new Dictionary<float, double[]>();
            if (lines.Count > lineCount)
            {
                prop = lines[lineCount].Split(',', StringSplitOptions.RemoveEmptyEntries);
                int teCount = int.Parse(prop[0]);

                for (int i = 0; i < teCount; i++)
                {
                    lineCount++;
                    prop = lines[lineCount].Split(',', StringSplitOptions.RemoveEmptyEntries);
                    float timeInd = (float)(decimal)Math.Round(double.Parse(prop[0], CultureInfo.InvariantCulture), 1);
                    var temps = new double[nodes.Count];
                    for (int t = 1; t <= nodes.Count; t++)
                    {
                        temps[t - 1] = double.Parse(prop[t], CultureInfo.InvariantCulture);
                    }
                    //
                    results.Add(timeInd, temps);

                }

            }

            //
            return (fes.ToArray(), nodes.ToArray(), mdata, results);
        }

        /// <summary>
        /// Save numeric model with result, so it can be loaded by the ResultViewer for result visualization
        /// </summary>
        /// <param name="fe"></param>
        /// <param name="nds"></param>
        /// <param name="mdata"></param>
        /// <param name="results"></param>
        public static void SaveToFile(IFiniteElement[] fe, INode[] nds, IDictionary<string, double> mdata, IDictionary<float, double[]> results, string fileName)
        {
            var str = new StringBuilder();
            str.AppendLine("!Numerical model file. The file contain nodes, finite elements and material propertis. It can also contains results for visualization.");
            if(mdata!=null)
            {
                str.AppendLine("!Material Properties are stored in for of [\'name\': value]. Value should be in SI system and numeric.");
                var strValue = string.Join(",", mdata.Select((k,v)=>$"{k.Key}:{k.Value.ToString(CultureInfo.InvariantCulture)}"));
                str.AppendLine($"MData:{strValue}");
            }

            str.AppendLine($"!Node count");
            str.AppendLine($"{nds.Length}");
            str.AppendLine($"!Node type");
            str.AppendLine($"{(int)nds.First().NT}");
            str.AppendLine($"!Node coordinates:x,y");
            foreach(var n in nds)
            {
                str.AppendLine($"{n.P.X.ToString(CultureInfo.InvariantCulture)},{n.P.Y.ToString(CultureInfo.InvariantCulture)}");

            }

            //Finite elements
            str.AppendLine($"!Finite element count");
            str.AppendLine($"{fe.Length}");
            str.AppendLine($"!Finite Element type");
            str.AppendLine($"{(int)fe.First().T}");
            str.AppendLine($"!Finite element nodes: n1,n2,...");
            foreach (var e in fe)
            {
                var strVal = string.Join(",", e.N);
                str.AppendLine(strVal);
            }

            //NUmerical results
            str.AppendLine($"!NUmerical results");
            str.AppendLine($"{results.Count}");
           
            str.AppendLine($"!Numerical results: n1,n2,...");
            foreach (var r in results)
            {
                str.AppendLine($"{Math.Round(r.Key, 1)},{string.Join(',', r.Value.Select(x => x.ToString(CultureInfo.InvariantCulture)))}");
            }


            File.WriteAllLines(fileName, str.ToString().Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

        }


    }
}
