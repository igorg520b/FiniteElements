namespace GeoLoader
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslMeshDetails = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslMatrixDimensions = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslBenchmark = new System.Windows.Forms.ToolStripStatusLabel();
            this.glControl1 = new OpenTK.GLControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tscbPrecision = new System.Windows.Forms.ToolStripComboBox();
            this.tscbDevice = new System.Windows.Forms.ToolStripComboBox();
            this.tscbSurface1 = new System.Windows.Forms.ToolStripComboBox();
            this.tscbSurface2 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tslStatus = new System.Windows.Forms.ToolStripLabel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslMeshDetails,
            this.tsslMatrixDimensions,
            this.tsslBenchmark});
            this.statusStrip1.Location = new System.Drawing.Point(0, 840);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(4, 0, 24, 0);
            this.statusStrip1.Size = new System.Drawing.Size(2348, 56);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslMeshDetails
            // 
            this.tsslMeshDetails.Name = "tsslMeshDetails";
            this.tsslMeshDetails.Size = new System.Drawing.Size(374, 51);
            this.tsslMeshDetails.Text = "toolStripStatusLabel1";
            // 
            // tsslMatrixDimensions
            // 
            this.tsslMatrixDimensions.Name = "tsslMatrixDimensions";
            this.tsslMatrixDimensions.Size = new System.Drawing.Size(37, 51);
            this.tsslMatrixDimensions.Text = "-";
            // 
            // tsslBenchmark
            // 
            this.tsslBenchmark.Name = "tsslBenchmark";
            this.tsslBenchmark.Size = new System.Drawing.Size(37, 51);
            this.tsslBenchmark.Text = "-";
            // 
            // glControl1
            // 
            this.glControl1.BackColor = System.Drawing.Color.Black;
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl1.Location = new System.Drawing.Point(0, 58);
            this.glControl1.Margin = new System.Windows.Forms.Padding(20, 18, 20, 18);
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(2348, 782);
            this.glControl1.TabIndex = 0;
            this.glControl1.VSync = false;
            this.glControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.glControl1_Paint);
            this.glControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseUp);
            this.glControl1.Resize += new System.EventHandler(this.glControl1_Resize);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3,
            this.toolStripButton4,
            this.toolStripSeparator1,
            this.tscbPrecision,
            this.tscbDevice,
            this.tscbSurface1,
            this.tscbSurface2,
            this.toolStripSeparator3,
            this.toolStripLabel2,
            this.toolStripSeparator2,
            this.tslStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(2348, 58);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(46, 55);
            this.toolStripButton1.Tag = "1";
            this.toolStripButton1.Text = "1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(46, 55);
            this.toolStripButton2.Tag = "2";
            this.toolStripButton2.Text = "2";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
            this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton3.Name = "toolStripButton3";
            this.toolStripButton3.Size = new System.Drawing.Size(46, 55);
            this.toolStripButton3.Tag = "3";
            this.toolStripButton3.Text = "3";
            this.toolStripButton3.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton4.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton4.Image")));
            this.toolStripButton4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton4.Name = "toolStripButton4";
            this.toolStripButton4.Size = new System.Drawing.Size(46, 55);
            this.toolStripButton4.Tag = "4";
            this.toolStripButton4.Text = "4";
            this.toolStripButton4.Click += new System.EventHandler(this.toolStripButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 58);
            // 
            // tscbPrecision
            // 
            this.tscbPrecision.CausesValidation = false;
            this.tscbPrecision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbPrecision.Items.AddRange(new object[] {
            "float",
            "double"});
            this.tscbPrecision.Name = "tscbPrecision";
            this.tscbPrecision.Size = new System.Drawing.Size(196, 58);
            // 
            // tscbDevice
            // 
            this.tscbDevice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbDevice.Items.AddRange(new object[] {
            "CPU",
            "GPU"});
            this.tscbDevice.Name = "tscbDevice";
            this.tscbDevice.Size = new System.Drawing.Size(146, 58);
            // 
            // tscbSurface1
            // 
            this.tscbSurface1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbSurface1.Name = "tscbSurface1";
            this.tscbSurface1.Size = new System.Drawing.Size(146, 58);
            this.tscbSurface1.SelectedIndexChanged += new System.EventHandler(this.tscbSurface1_SelectedIndexChanged);
            // 
            // tscbSurface2
            // 
            this.tscbSurface2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscbSurface2.Name = "tscbSurface2";
            this.tscbSurface2.Size = new System.Drawing.Size(146, 58);
            this.tscbSurface2.SelectedIndexChanged += new System.EventHandler(this.tscbSurface1_SelectedIndexChanged);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 58);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(392, 55);
            this.toolStripLabel2.Text = "RMB to deform object";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 58);
            // 
            // tslStatus
            // 
            this.tslStatus.ForeColor = System.Drawing.Color.OliveDrab;
            this.tslStatus.Name = "tslStatus";
            this.tslStatus.Size = new System.Drawing.Size(114, 55);
            this.tslStatus.Text = "ready";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(2348, 896);
            this.Controls.Add(this.glControl1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Text = "Linear Solver Benchmark";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslMeshDetails;
        private OpenTK.GLControl glControl1;
        private System.Windows.Forms.ToolStripStatusLabel tsslBenchmark;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripButton toolStripButton3;
        private System.Windows.Forms.ToolStripButton toolStripButton4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel tslStatus;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox tscbPrecision;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripComboBox tscbDevice;
        private System.Windows.Forms.ToolStripComboBox tscbSurface1;
        private System.Windows.Forms.ToolStripComboBox tscbSurface2;
        private System.Windows.Forms.ToolStripStatusLabel tsslMatrixDimensions;
    }
}

