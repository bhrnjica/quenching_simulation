
namespace HTWnd
{
    partial class VisualResult
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisualResult));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.tbR = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBoxCyl = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbH = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolFileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolLoadModel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolShowNodes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolShowElementIds = new System.Windows.Forms.ToolStripMenuItem();
            this.toolShowResults = new System.Windows.Forms.ToolStripMenuItem();
            this.toolShowMesh = new System.Windows.Forms.ToolStripMenuItem();
            this.toolShowBC = new System.Windows.Forms.ToolStripMenuItem();
            this.toolResults = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuExportTem = new System.Windows.Forms.ToolStripMenuItem();
            this.button5 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.btnSolve = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lnodeCount = new System.Windows.Forms.Label();
            this.lelementsCount = new System.Windows.Forms.Label();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtStartTime = new System.Windows.Forms.TextBox();
            this.txtTimeStep = new System.Windows.Forms.TextBox();
            this.txtStepCount = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBoxCyl.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox7.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(652, 44);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(609, 964);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.Resize += new System.EventHandler(this.pictureBox1_Resize);
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar1.Location = new System.Drawing.Point(1278, 38);
            this.vScrollBar1.Maximum = 15000;
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 998);
            this.vScrollBar1.TabIndex = 1;
            this.vScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vScrollBar1_Scroll);
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar1.Location = new System.Drawing.Point(0, 1019);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(1278, 17);
            this.hScrollBar1.TabIndex = 2;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // tbR
            // 
            this.tbR.Location = new System.Drawing.Point(120, 56);
            this.tbR.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbR.Name = "tbR";
            this.tbR.ReadOnly = true;
            this.tbR.Size = new System.Drawing.Size(95, 35);
            this.tbR.TabIndex = 6;
            this.tbR.Text = "1.25";
            this.tbR.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox5_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 30);
            this.label2.TabIndex = 7;
            this.label2.Text = "Radius:";
            // 
            // groupBoxCyl
            // 
            this.groupBoxCyl.Controls.Add(this.label5);
            this.groupBoxCyl.Controls.Add(this.label4);
            this.groupBoxCyl.Controls.Add(this.tbH);
            this.groupBoxCyl.Controls.Add(this.label3);
            this.groupBoxCyl.Controls.Add(this.tbR);
            this.groupBoxCyl.Controls.Add(this.label2);
            this.groupBoxCyl.Location = new System.Drawing.Point(319, 46);
            this.groupBoxCyl.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBoxCyl.Name = "groupBoxCyl";
            this.groupBoxCyl.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBoxCyl.Size = new System.Drawing.Size(308, 178);
            this.groupBoxCyl.TabIndex = 8;
            this.groupBoxCyl.TabStop = false;
            this.groupBoxCyl.Text = "Cylinder dimension";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(228, 120);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 30);
            this.label5.TabIndex = 11;
            this.label5.Text = "cm.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(228, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 30);
            this.label4.TabIndex = 10;
            this.label4.Text = "cm.";
            // 
            // tbH
            // 
            this.tbH.Location = new System.Drawing.Point(120, 114);
            this.tbH.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tbH.Name = "tbH";
            this.tbH.ReadOnly = true;
            this.tbH.Size = new System.Drawing.Size(95, 35);
            this.tbH.TabIndex = 9;
            this.tbH.Text = "10";
            this.tbH.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox5_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 112);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 30);
            this.label3.TabIndex = 8;
            this.label3.Text = "Height:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolFileMenu,
            this.toolOptions,
            this.toolResults});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1295, 38);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "File";
            // 
            // toolFileMenu
            // 
            this.toolFileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolLoadModel});
            this.toolFileMenu.Name = "toolFileMenu";
            this.toolFileMenu.Size = new System.Drawing.Size(62, 34);
            this.toolFileMenu.Text = "&File";
            // 
            // toolLoadModel
            // 
            this.toolLoadModel.Name = "toolLoadModel";
            this.toolLoadModel.Size = new System.Drawing.Size(256, 40);
            this.toolLoadModel.Text = "Load Model...";
            this.toolLoadModel.Click += new System.EventHandler(this.toolLoadModel_Click);
            // 
            // toolOptions
            // 
            this.toolOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolShowNodes,
            this.toolShowElementIds,
            this.toolShowResults,
            this.toolShowMesh,
            this.toolShowBC});
            this.toolOptions.Name = "toolOptions";
            this.toolOptions.Size = new System.Drawing.Size(104, 34);
            this.toolOptions.Text = "Options";
            // 
            // toolShowNodes
            // 
            this.toolShowNodes.CheckOnClick = true;
            this.toolShowNodes.Name = "toolShowNodes";
            this.toolShowNodes.Size = new System.Drawing.Size(295, 40);
            this.toolShowNodes.Text = "Show Nodes";
            this.toolShowNodes.CheckedChanged += new System.EventHandler(this.toolShowNodes_Click);
            // 
            // toolShowElementIds
            // 
            this.toolShowElementIds.CheckOnClick = true;
            this.toolShowElementIds.Name = "toolShowElementIds";
            this.toolShowElementIds.Size = new System.Drawing.Size(295, 40);
            this.toolShowElementIds.Text = "Show Element Ids";
            this.toolShowElementIds.Click += new System.EventHandler(this.toolShowElementIds_Click);
            // 
            // toolShowResults
            // 
            this.toolShowResults.CheckOnClick = true;
            this.toolShowResults.Name = "toolShowResults";
            this.toolShowResults.Size = new System.Drawing.Size(295, 40);
            this.toolShowResults.Text = "Show Results";
            this.toolShowResults.CheckedChanged += new System.EventHandler(this.toolShowResults_CheckedChanged);
            // 
            // toolShowMesh
            // 
            this.toolShowMesh.CheckOnClick = true;
            this.toolShowMesh.Name = "toolShowMesh";
            this.toolShowMesh.Size = new System.Drawing.Size(295, 40);
            this.toolShowMesh.Text = "Show Mesh";
            // 
            // toolShowBC
            // 
            this.toolShowBC.CheckOnClick = true;
            this.toolShowBC.Name = "toolShowBC";
            this.toolShowBC.Size = new System.Drawing.Size(295, 40);
            this.toolShowBC.Text = "Show BC";
            this.toolShowBC.CheckedChanged += new System.EventHandler(this.toolShowBC_Click);
            // 
            // toolResults
            // 
            this.toolResults.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuExportTem});
            this.toolResults.Name = "toolResults";
            this.toolResults.Size = new System.Drawing.Size(96, 34);
            this.toolResults.Text = "Results";
            // 
            // toolStripMenuExportTem
            // 
            this.toolStripMenuExportTem.Name = "toolStripMenuExportTem";
            this.toolStripMenuExportTem.Size = new System.Drawing.Size(321, 40);
            this.toolStripMenuExportTem.Text = "Export Temperatures";
            this.toolStripMenuExportTem.Click += new System.EventHandler(this.toolStripMenuExportTem_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(8, 118);
            this.button5.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(139, 44);
            this.button5.TabIndex = 27;
            this.button5.Text = "Run";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.button5);
            this.groupBox4.Controls.Add(this.label24);
            this.groupBox4.Controls.Add(this.btnSolve);
            this.groupBox4.Controls.Add(this.textBox2);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Location = new System.Drawing.Point(319, 431);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.groupBox4.Size = new System.Drawing.Size(308, 233);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Results in time";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(8, 166);
            this.button3.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(139, 44);
            this.button3.TabIndex = 28;
            this.button3.Text = "Stop";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(185, 50);
            this.label24.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(48, 30);
            this.label24.TabIndex = 8;
            this.label24.Text = "sec.";
            // 
            // btnSolve
            // 
            this.btnSolve.Location = new System.Drawing.Point(169, 118);
            this.btnSolve.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.btnSolve.Name = "btnSolve";
            this.btnSolve.Size = new System.Drawing.Size(139, 44);
            this.btnSolve.TabIndex = 27;
            this.btnSolve.Text = "Show";
            this.btnSolve.UseVisualStyleBackColor = true;
            this.btnSolve.Click += new System.EventHandler(this.btnSolve_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(77, 44);
            this.textBox2.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(95, 35);
            this.textBox2.TabIndex = 6;
            this.textBox2.Text = "1";
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox5_KeyPress);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(13, 44);
            this.label14.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(58, 30);
            this.label14.TabIndex = 7;
            this.label14.Text = "Step:";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.pictureBox2);
            this.groupBox5.Location = new System.Drawing.Point(14, 46);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.groupBox5.Size = new System.Drawing.Size(275, 782);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Boundary Condition";
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(5, 36);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(264, 742);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            // 
            // lnodeCount
            // 
            this.lnodeCount.AutoSize = true;
            this.lnodeCount.Location = new System.Drawing.Point(13, 89);
            this.lnodeCount.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lnodeCount.Name = "lnodeCount";
            this.lnodeCount.Size = new System.Drawing.Size(98, 30);
            this.lnodeCount.TabIndex = 28;
            this.lnodeCount.Text = "Nodes=0";
            // 
            // lelementsCount
            // 
            this.lelementsCount.AutoSize = true;
            this.lelementsCount.Location = new System.Drawing.Point(13, 128);
            this.lelementsCount.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lelementsCount.Name = "lelementsCount";
            this.lelementsCount.Size = new System.Drawing.Size(184, 30);
            this.lelementsCount.TabIndex = 29;
            this.lelementsCount.Text = "Finite elements =0";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label1);
            this.groupBox7.Controls.Add(this.lelementsCount);
            this.groupBox7.Controls.Add(this.lnodeCount);
            this.groupBox7.Location = new System.Drawing.Point(319, 233);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox7.Size = new System.Drawing.Size(308, 188);
            this.groupBox7.TabIndex = 30;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "2D Mesh";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 47);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 30);
            this.label1.TabIndex = 31;
            this.label1.Text = "Element type = Triangle";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(40, 48);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(111, 30);
            this.label8.TabIndex = 8;
            this.label8.Text = "Start Time:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(32, 108);
            this.label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(108, 30);
            this.label9.TabIndex = 9;
            this.label9.Text = "Time step:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(32, 152);
            this.label10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(117, 30);
            this.label10.TabIndex = 10;
            this.label10.Text = "Step count:";
            // 
            // txtStartTime
            // 
            this.txtStartTime.Location = new System.Drawing.Point(167, 48);
            this.txtStartTime.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtStartTime.Name = "txtStartTime";
            this.txtStartTime.Size = new System.Drawing.Size(95, 35);
            this.txtStartTime.TabIndex = 11;
            this.txtStartTime.Text = "0";
            // 
            // txtTimeStep
            // 
            this.txtTimeStep.Location = new System.Drawing.Point(167, 102);
            this.txtTimeStep.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtTimeStep.Name = "txtTimeStep";
            this.txtTimeStep.Size = new System.Drawing.Size(95, 35);
            this.txtTimeStep.TabIndex = 12;
            this.txtTimeStep.Text = "1";
            // 
            // txtStepCount
            // 
            this.txtStepCount.Location = new System.Drawing.Point(167, 152);
            this.txtStepCount.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.txtStepCount.Name = "txtStepCount";
            this.txtStepCount.Size = new System.Drawing.Size(95, 35);
            this.txtStepCount.TabIndex = 13;
            this.txtStepCount.Text = "5";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(272, 56);
            this.label12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(27, 30);
            this.label12.TabIndex = 10;
            this.label12.Text = "s.";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 218);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(139, 44);
            this.button2.TabIndex = 17;
            this.button2.Text = "Solve";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(272, 104);
            this.label11.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(27, 30);
            this.label11.TabIndex = 11;
            this.label11.Text = "s.";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.txtStepCount);
            this.groupBox3.Controls.Add(this.txtTimeStep);
            this.groupBox3.Controls.Add(this.txtStartTime);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(319, 689);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.groupBox3.Size = new System.Drawing.Size(311, 289);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Time Settings";
            this.groupBox3.Visible = false;
            // 
            // VisualResult
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 30F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1295, 1036);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBoxCyl);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.vScrollBar1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "VisualResult";
            this.Text = "Axisymetric Transient Heat Transfer Result Viewer";
            this.Load += new System.EventHandler(this.Model_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBoxCyl.ResumeLayout(false);
            this.groupBoxCyl.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.TextBox tbR;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbH;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolFileMenu;
        private System.Windows.Forms.ToolStripMenuItem toolOptions;
        private System.Windows.Forms.ToolStripMenuItem toolResults;
        private System.Windows.Forms.ToolStripMenuItem toolLoadModel;
        private System.Windows.Forms.ToolStripMenuItem toolShowNodes;
        private System.Windows.Forms.ToolStripMenuItem toolShowResults;
        private System.Windows.Forms.ToolStripMenuItem toolShowMesh;
        private System.Windows.Forms.ToolStripMenuItem toolShowBC;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox bTa;
        private System.Windows.Forms.Button btnSolve;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.ToolStripMenuItem toolShowElementIds;
        private System.Windows.Forms.Label lnodeCount;
        private System.Windows.Forms.Label lelementsCount;
        protected System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuExportTem;
        private System.Windows.Forms.GroupBox groupBoxCyl;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtStartTime;
        private System.Windows.Forms.TextBox txtTimeStep;
        private System.Windows.Forms.TextBox txtStepCount;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

