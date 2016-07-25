using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using ManagedCuda;

namespace GeoLoader
{
    public partial class Form1 : Form
    {
        Model model;
        float theta, phi;           // view angle
        double stretch, twist; // boundary condition on surface #2 of the object (surface #1 is not displaced)
        AutoResetEvent mre;         // runs third-party solver on a separate thread
        int surfTag1, surfTag2;     // anchored surface #1, #2 and computational device
        bool useDoublePrecision, useGPU;
        bool allowSelectSurface = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReadModel(1);
            tscbPrecision.SelectedIndex = 0;
            glControl1.MouseWheel += glControl1_MouseWheel;
            SetUpLight();
            mre = new AutoResetEvent(false);
            backgroundWorker1.RunWorkerAsync();

            if (CudaContext.GetDeviceCount() > 0) tscbDevice.SelectedIndex = 1;
            else { tscbDevice.SelectedIndex = 0; tscbDevice.Enabled = false; }

            if(!allowSelectSurface) {
                tscbSurface1.Enabled = false; tscbSurface1.Visible = false;
                tscbSurface2.Enabled = false; tscbSurface2.Visible = false;
            }
        }

        void ReadModel(int n)
        {
            model = new Model(File.OpenRead($"{n}.geo"));
            tsslMeshDetails.Text = $"nds:{model.nodes.Count:#,##0}; elems:{model.elems.Count:#,##0}";
            if (allowSelectSurface)
            {
                tscbSurface1.Items.Clear();
                tscbSurface2.Items.Clear();
                object[] tags = model.surfaceTags.ToArray().Cast<object>().ToArray();
                tscbSurface1.Items.AddRange(tags);
                tscbSurface2.Items.AddRange(tags);
                tscbSurface1.SelectedIndex = 1;
                tscbSurface2.SelectedIndex = 2;
            } else
            {
                if (n == 1) { surfTag1 = -161; surfTag2 = -185; }
                else if (n == 2) { surfTag1 = -2481; surfTag2 = -2526; }
                else if (n == 3) { surfTag1 = -151; surfTag2 = -143; }
                else if (n == 4) { surfTag1 = -151; surfTag2 = -143; }
            }
            model.MarkAnchoredNodes(surfTag1, surfTag2);
            model.InitializeCSR();
            tsslMatrixDimensions.Text = $"N: {model.csr.N:#,##0}; nnz: {model.csr.nnz:#,##0}";
            stretch = twist = theta = phi = 0;
            glControl1.Invalidate();
        }

        #region glContol
        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            // draw background
            float k = 0.7f;
            float k2 = 0.5f;
            GL.Disable(EnableCap.Lighting);
            GL.Enable(EnableCap.DepthTest);

            GL.ClearColor(k, k, k, 1f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //            GL.DepthFunc(DepthFunction.Never);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Begin(PrimitiveType.Quads);
            double s = 1.0;
            GL.Color3(k2, k2, k2);
            GL.Vertex3(-s, -s, 0);
            GL.Color3(k, k, k);
            GL.Vertex3(-s, s, 0);
            GL.Vertex3(s, s, 0);
            GL.Color3(k2, k2, k2);
            GL.Vertex3(s, -s, 0);
            GL.End();
            GL.PopMatrix();

            GL.Clear(ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Rotate(-phi, new Vector3(1, 0, 0));
            //GL.Rotate(-60, new Vector3(1, 0, 0));
            GL.Rotate(-theta, new Vector3(0, 0, 1));

            if (model != null) Render();
            glControl1.SwapBuffers();
        }

        void Render()
        {
            GL.Enable(EnableCap.Lighting);
            GL.Begin(PrimitiveType.Triangles);
            for(int i=0;i<model.drawNodes.Count;i+=3)
            {
                Node n0 = model.drawNodes[i];
                Node n1 = model.drawNodes[i+1];
                Node n2 = model.drawNodes[i+2];
                Vector3d v0 = new Vector3d(n0.cx, n0.cy, n0.cz);
                Vector3d v1 = new Vector3d(n1.cx, n1.cy, n1.cz);
                Vector3d v2 = new Vector3d(n2.cx, n2.cy, n2.cz);
                GL.Normal3(Vector3d.Cross(v1 - v0, v2 - v0));

                int tag = model.drawTags[i / 3];
                if (tag == surfTag1) GL.Color3(Color.Green);
                else if (tag == surfTag2) GL.Color3(Color.Red);
                else GL.Color3(Color.White);

                GL.Vertex3(v2);
                GL.Vertex3(v0);
                GL.Vertex3(v1);
            }
            GL.End();

            GL.DepthFunc(DepthFunction.Lequal);
            GL.Disable(EnableCap.Lighting);
            GL.Begin(PrimitiveType.Lines);
            GL.Color3(0.2f,0.21f,0.19f);
            for (int i = 0; i < model.drawNodes.Count; i += 3)
            {
                Node n0 = model.drawNodes[i];
                Node n1 = model.drawNodes[i + 1];
                Node n2 = model.drawNodes[i + 2];
                Vector3d v0 = new Vector3d(n0.cx, n0.cy, n0.cz);
                Vector3d v1 = new Vector3d(n1.cx, n1.cy, n1.cz);
                Vector3d v2 = new Vector3d(n2.cx, n2.cy, n2.cz);
                GL.Vertex3(v0); GL.Vertex3(v1);
                GL.Vertex3(v0); GL.Vertex3(v2);
                GL.Vertex3(v2); GL.Vertex3(v1);
            }
            GL.End();
        }

        void SetUpLight()
        {
            GL.Enable(EnableCap.ColorMaterial);
            GL.Enable(EnableCap.Normalize);
            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.ShadeModel(ShadingModel.Smooth);
            GL.Light(LightName.Light0, LightParameter.Ambient, Color4.Black);
            GL.Light(LightName.Light0, LightParameter.Diffuse, Color4.White);
            GL.Light(LightName.Light0, LightParameter.Position, new Color4(3, -3, -3, 0));
            GL.Enable(EnableCap.Light1);
            GL.Light(LightName.Light1, LightParameter.Ambient, Color4.Black);
            GL.Light(LightName.Light1, LightParameter.Diffuse, Color4.White);
            GL.Light(LightName.Light1, LightParameter.Position, new Color4(-1, -1, 3, 0));
        }

        private void glControl1_Resize(object sender, EventArgs e) { reshape(); }

        double aspectRatio = 1, scale = 0.02;
        void reshape()
        {
            aspectRatio = (double)glControl1.Width / glControl1.Height;
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            GL.Ortho(-scale * aspectRatio,
                    scale * aspectRatio,
                    -scale,
                    scale, -1000, 1000);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Enable(EnableCap.PointSmooth);
            GL.Hint(HintTarget.PointSmoothHint, HintMode.Nicest);
            GL.Enable(EnableCap.LineSmooth);
        }

        int lastX = 0, lastY = 0; // mouse
        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                theta += (e.X - lastX);
                phi += (e.Y - lastY);
                lastX = e.X;
                lastY = e.Y;
            }
            else if (e.Button == MouseButtons.Right)
            {
                // twist
                const int maxTwist = 180;
                twist += (e.X - lastX) * 0.1;
                if (twist > maxTwist) twist = maxTwist;
                else if (twist < -maxTwist) twist = -maxTwist;
                stretch += (e.Y - lastY)*0.00001;
                lastX = e.X;
                lastY = e.Y;

                model.ApplyDisplacementBoundaryConditions(stretch, twist);
            }
            glControl1.Invalidate();
        }

        private void tscbSurface1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tscbSurface1.SelectedItem == null) surfTag1 = 0;
            else surfTag1 = (int)tscbSurface1.SelectedItem;
            if (tscbSurface2.SelectedItem == null) surfTag2 = 0;
            else surfTag2 = (int)tscbSurface2.SelectedItem;
            glControl1.Invalidate();
        }

        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            lastX = e.X;
            lastY = e.Y;
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            // start computation
            mre.Set();
            useDoublePrecision = (string)tscbPrecision.SelectedItem == "double";
            useGPU = tscbDevice.SelectedIndex == 1;
            Trace.WriteLine($"double: {useDoublePrecision}; GPU: {useGPU}");
            tslStatus.Text = "running";
            tslStatus.ForeColor = Color.IndianRed;
        }

        void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            scale += 0.001 * scale * e.Delta;
            reshape();
            glControl1.Invalidate();
        }
        #endregion

        private void toolStripButton_Click(object sender, EventArgs e)
        {
            // replace current model with user's selection (1,2,3 or 4)
            ReadModel(int.Parse((string)((ToolStripButton)sender).Tag));
        }

        long msAssemble, msSolve;
        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            // initialize Paralution
            CSR_System.initParalution();
            do
            {
                mre.WaitOne(Timeout.Infinite);
                sw.Restart();
                model.AssembleLinearSystem();               // assemble
                msAssemble = sw.ElapsedMilliseconds;
                sw.Restart();
                model.Solve(useGPU, useDoublePrecision);    // solve
                sw.Stop();
                msSolve = sw.ElapsedMilliseconds;
                backgroundWorker1.ReportProgress(0);
            } while (true);

            // stop paralution
        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            glControl1.Invalidate();
            tslStatus.Text = "ready";
            tslStatus.ForeColor = Color.OliveDrab;
            tsslBenchmark.Text = $"assemble: {msAssemble} ms; solve: {msSolve} ms";
        }


    }
}
