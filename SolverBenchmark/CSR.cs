using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GeoLoader
{
    class CSR_System
    {
        public int[] rows, cols;  // structure of the sparse matrix
        public double[] vals;      // non-zero values
        public int N, nnz;

        public double[] rhs;
        public double[] x;

        public CSR_System(int N, int nnz)
        {
            this.N = N; this.nnz = nnz;
            rows = new int[N + 1];
            rows[N] = nnz;
            cols = new int[nnz];
            vals = new double[nnz];

            rhs = new double[N];
            x = new double[N];
        }
        
        public void Clear()
        {
            Array.Clear(vals, 0, vals.Length);
            Array.Clear(rhs, 0, rhs.Length);
        }

        public void SolveSingle(bool GPU)
        {
            float[] fv = new float[nnz];
            float[] frhs = new float[N];
            float[] fx = new float[N];
            Parallel.For(0, nnz, i => fv[i] = (float)vals[i]);
            Parallel.For(0, N, i => frhs[i] = (float)rhs[i]);
            if (GPU) solveCSRSingle(rows, cols, fv, nnz, N, frhs, fx);
            else CPUsolveCSRSingle(rows, cols, fv, nnz, N, frhs, fx);
            Parallel.For(0, N, i => x[i] = (double)fx[i]);
        }

        public void SolveDouble(bool GPU)
        {
            if(GPU) solveCSRDouble(rows, cols, vals, nnz, N, rhs, x);
            else CPUsolveCSRDouble(rows, cols, vals, nnz, N, rhs, x);
        }

        [DllImport("SolverWrapperNativeCode.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void initParalution();

        [DllImport("SolverWrapperNativeCode.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void stopParalution();

        [DllImport("SolverWrapperNativeCode.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double solveCSRSingle(int[] row_offset, int[] col, float[] val,
            int nnz, int N, float[] _rhs, float[] _x);

        [DllImport("SolverWrapperNativeCode.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double solveCSRDouble(int[] row_offset, int[] col, double[] val,
    int nnz, int N, double[] _rhs, double[] _x);

        [DllImport("SolverWrapperNativeCode.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double CPUsolveCSRSingle(int[] row_offset, int[] col, float[] val,
    int nnz, int N, float[] _rhs, float[] _x);

        [DllImport("SolverWrapperNativeCode.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double CPUsolveCSRDouble(int[] row_offset, int[] col, double[] val,
int nnz, int N, double[] _rhs, double[] _x);
    }
}
