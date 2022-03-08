using FEMHeatLib.NModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTWnd
{
    public partial class VisualResult : Form
    {
        // Graphics
        private double zoom = 1.0;
        private Graphics gr;
        private Bitmap bmp;
        private int MarginX = 30;
        private int MarginY = 30;
        private bool Diagnostic = false;
        private NumericModel Model;
        double H, R;
        

        public VisualResult()
        {
            InitializeComponent();
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

            drawModel();
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            drawModel();
        }

       

        private void drawModel()
        {
            if (Model == null)
                return;

            var size = getModelSize();
           
            int shiftY = pictureBox1.Height - MarginY - (int)(vScrollBar1.Value-size.h0*zoom);
            int shiftX = MarginX - hScrollBar1.Value-(int)(size.w0*zoom);

            if(Diagnostic)
            {
                Debug.WriteLine($"Model Size ({size.w},{size.h})");
                Debug.WriteLine($"Shift (x,y) ({shiftX},{shiftY})");
                Debug.WriteLine($"Image size (x,y) ({bmp.Width},{bmp.Height})");
                Debug.WriteLine($"Zoom Factor ({zoom})");
            }

            gr.DrawRectangle(new Pen(Color.Black), MarginX, MarginY, pictureBox1.Width, pictureBox1.Height);
            Model.SetMinMaxValues();
            Model.Draw(gr, bmp.Width, bmp.Height, zoom, shiftX, shiftY);

            pictureBox1.Refresh();
        }

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            if (bmp is object)
                bmp.Dispose();

            if (gr is object)
                gr.Dispose();

            if (pictureBox1.Width <= 0 || pictureBox1.Height <= 0)
                return;

            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height, pictureBox1.CreateGraphics());
            gr = Graphics.FromImage(bmp);

            var size = getModelSize();
            setscrollBars(size.w, size.h);

            drawModel();
        }

        private void setscrollBars(double w, double h)
        {
            vScrollBar1.Maximum = (int)(h * zoom);
            vScrollBar1.Minimum = 0;
            vScrollBar1.Value = 0;

            hScrollBar1.Maximum = 0;
            hScrollBar1.Minimum = (int)(-1* w * zoom); 
            hScrollBar1.Value = 0;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (bmp is object)
            {
                e.Graphics.DrawImage(bmp, 0, 0);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // zoom in
                zoom = zoom * 1.5;
            }
            else if (e.Button == MouseButtons.Right)
            {
                zoom = zoom / 1.5d;
            }
            //setscrollBars();
            drawModel();
        }

        private void ZoomToFit()
        {
            var w = pictureBox1.Width;
            var h = pictureBox1.Height;

            var wm = getModelSize();

            var z = w / (wm.w);
            var zz = h / (wm.h);

            zoom = Math.Min(z,zz);
            setscrollBars(wm.w, wm.h);
            drawModel();
        }

        private (double w0, double h0,double w,double h) getModelSize()
        {
            if (Model == null)
                return (0,0,0, 0);
            else
                return Model.GetSize();          
        }


        private void Model_Load(object sender, EventArgs e)
        {
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height, pictureBox1.CreateGraphics());
            gr = Graphics.FromImage(bmp);
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '.')
              e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }


        private void checkOptions(NumericModel model)
        {
            
            toolShowResults.Checked =  model.GetShowAnalysisResult();
            toolShowElementIds.Checked =  model.GetShowElementsNodes();
            toolShowBC.Checked = model.GetShowBC();
            model.GetShowTC();
            toolShowElementIds.Checked = model.GetFiniteElementIds();
           
        }

        private void toolShowNodes_Click(object sender, EventArgs e)
        {
            Model.ShowElementsNodes(toolShowNodes.Checked);
            drawModel();
        }

        private void toolShowElementIds_Click(object sender, EventArgs e)
        {
            Model.ShowFiniteElementIds(toolShowElementIds.Checked);
            drawModel();
        }


        private void toolShowResults_CheckedChanged(object sender, EventArgs e)
        {
           
            Model.ShowAnalysisResult(toolShowResults.Checked);
            drawModel();
        }


        private void toolShowBC_Click(object sender, EventArgs e)
        {
            if(Model!=null)
                Model.ShowBC(toolShowBC.Checked);

            drawModel();
        }


       
        /// <summary>
        /// load mesh from file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolLoadModel_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.InitialDirectory = Path.GetDirectoryName("../../");
            f.Filter = "Txt file|*.txt";
            f.Title = "Load an numeric model/modelresults";
            var result = f.ShowDialog();
            if(result == DialogResult.OK)
            {
                var model = FEMHeatLib.NModel.Model.LoadFromFile(f.FileName);

                float[] time = null;
                
                R = model.nds.Max(x=>x.P.X);
                H = model.nds.Max(x => x.P.Y);
                zoom = 210f / R;

                tbR.Text = (R*100).ToString(CultureInfo.InvariantCulture);
                tbH.Text = (H*100).ToString(CultureInfo.InvariantCulture);

                //txttime
                if (model.results.Count > 0)
                {
                    txtTimeStep.Text = (model.results.ElementAt(1).Key - model.results.ElementAt(0).Key).ToString("F2");
                    txtStepCount.Text = model.results.Count.ToString();
                    time = model.results.Keys.ToArray();
                }



                lnodeCount.Text = $"Nodes = {model.nds.Length}";
                lelementsCount.Text = $"Finite elements  = {model.fe.Length}";

                //
                Model = new NumericModel(model.fe, model.nds);
               
                if (model.results.Count > 0)
                {
                    Model.Time = model.results.Keys.ToArray();
                    Model.Temperatures = model.results;
                    Model.ShowAnalysisResult(true);
                    //Model.SetMinMaxValues();
                }

                checkOptions(Model);

                drawModel();
            }
           
        }
      



        

        /// <summary>
        /// Shows the result for specific time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSolve_Click(object sender, EventArgs e)
        {
            int timeStep = int.Parse(textBox2.Text);
            Model.TimeStep = timeStep;
            Model.SetMinMaxValues(timeStep);
            drawModel();
        }

        //Simulate
        private async void button5_Click(object sender, EventArgs e)
        {
            tokenSource = new CancellationTokenSource();
            textBox2.Text = "0";
            double timeStart = double.Parse(txtStartTime.Text);
            double timeStep = double.Parse(txtTimeStep.Text);
            int stepCount = int.Parse(txtStepCount.Text);

            if (Model.Time != null)
                stepCount = Model.Time.Length - 1;

            Model.TimeStep = -1;
            Model.SetMinMaxValues();
            int i = int.Parse(textBox2.Text);
            for (; i<= stepCount; i++ )
            {            
                Model.TimeStep = Model.Time != null?(int)Model.Time[i]:i;
                textBox2.Text = (Model.TimeStep).ToString();
                drawModel();
                if (!tokenSource.Token.IsCancellationRequested)
                    await Task.Delay(500);
                else
                    return;

            }
            textBox2.Text = "0";
        }
       
        CancellationTokenSource tokenSource;
        /// <summary>
        /// Stops the simulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            tokenSource.Cancel();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuExportTem_Click(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// Save whole model to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripSaveModel_Click(object sender, EventArgs e)
        {
            SaveFileDialog f = new SaveFileDialog();
            f.Filter = "Txt file|*.txt";
            f.Title = "Save an numeric model";
            var retVal = f.ShowDialog();
            if(retVal == DialogResult.OK)
            {
                string fileName = f.FileName;
                //Model.SaveToFile(fileName);
            }

        }

    }
}
