#include <string>
#include <paralution.hpp>
using namespace paralution;
using namespace std;

extern "C" __declspec(dllexport) void initParalution(void) {
	init_paralution();
}

extern "C" __declspec(dllexport) void stopParalution(void) {
	stop_paralution(); 
}

extern "C" __declspec(dllexport) void test1() {
	init_paralution();
}


extern "C" __declspec(dllexport) void solveCSRSingle(int *row_offset, int *col, float *val, 
	const int nnz, const int N, float *_rhs, float *_x) {

	std::string name = "s";
	LocalVector<float> x;
	LocalVector<float> rhs;
	LocalMatrix<float> mat;

	x.Allocate(name, N);
	x.Zeros();
	rhs.Allocate(name, N);
	mat.AllocateCSR(name, nnz, N, N);
	mat.CopyFromCSR(row_offset, col, val);
	rhs.CopyFromData(_rhs);
//	mat.Check();

/*	rhs.SetDataPtr(&_rhs, name, N);
	x.SetDataPtr(&_x, name, N);
	mat.SetDataPtrCSR(&row_offset, &col, &val, name, nnz, N, N);
*/

	mat.MoveToAccelerator();
	x.MoveToAccelerator();
	rhs.MoveToAccelerator();
	CG<LocalMatrix<float>, LocalVector<float>, float> ls;
	MultiColoredILU<LocalMatrix<float>, LocalVector<float>, float> p;
	ls.SetOperator(mat);
	ls.SetPreconditioner(p);
	ls.Build();

	ls.Solve(rhs, &x);

	mat.MoveToHost();
	x.MoveToHost();
	rhs.MoveToHost();

	/*
	mat.LeaveDataPtrCSR(&row_offset, &col, &val);
	rhs.LeaveDataPtr(&_rhs);
	x.LeaveDataPtr(&_x);
*/

	x.CopyToData(_x);
	mat.Clear();
	x.Clear();
	rhs.Clear();
	
	ls.Clear();
}


extern "C" __declspec(dllexport) void solveCSRDouble(int *row_offset, int *col, double *val,
	const int nnz, const int N, double *_rhs, double *_x) {
	std::string name = "s";
	LocalVector<double> x;
	LocalVector<double> rhs;
	LocalMatrix<double> mat;

	x.Allocate(name, N);
	x.Zeros();
	rhs.Allocate(name, N);
	mat.AllocateCSR(name, nnz, N, N);
	mat.CopyFromCSR(row_offset, col, val);
	rhs.CopyFromData(_rhs);

	mat.MoveToAccelerator();
	x.MoveToAccelerator();
	rhs.MoveToAccelerator();
	CG<LocalMatrix<double>, LocalVector<double>, double> ls;
	MultiColoredILU<LocalMatrix<double>, LocalVector<double>, double> p;
	ls.SetOperator(mat);
	ls.SetPreconditioner(p);
	ls.Build();

	ls.Solve(rhs, &x);

	mat.MoveToHost();
	x.MoveToHost();
	rhs.MoveToHost();

	x.CopyToData(_x);
	mat.Clear();
	x.Clear();
	rhs.Clear();

	ls.Clear();
}


extern "C" __declspec(dllexport) void CPUsolveCSRSingle(int *row_offset, int *col, float *val,
	const int nnz, const int N, float *_rhs, float *_x) {

	std::string name = "s";
	LocalVector<float> x;
	LocalVector<float> rhs;
	LocalMatrix<float> mat;

	x.Allocate(name, N);
	x.Zeros();
	rhs.Allocate(name, N);
	mat.AllocateCSR(name, nnz, N, N);
	mat.CopyFromCSR(row_offset, col, val);
	rhs.CopyFromData(_rhs);

	CG<LocalMatrix<float>, LocalVector<float>, float> ls;
	Jacobi<LocalMatrix<float>, LocalVector<float>, float> p;
	ls.SetOperator(mat);
	ls.SetPreconditioner(p);
	ls.Build();
	ls.Solve(rhs, &x);

	x.CopyToData(_x);
	mat.Clear();
	x.Clear();
	rhs.Clear();

	ls.Clear();
}

extern "C" __declspec(dllexport) void CPUsolveCSRDouble(int *row_offset, int *col, double *val,
	const int nnz, const int N, double *_rhs, double *_x) {

	std::string name = "s";
	LocalVector<double> x;
	LocalVector<double> rhs;
	LocalMatrix<double> mat;

	x.Allocate(name, N);
	x.Zeros();
	rhs.Allocate(name, N);
	mat.AllocateCSR(name, nnz, N, N);
	mat.CopyFromCSR(row_offset, col, val);
	rhs.CopyFromData(_rhs);

	CG<LocalMatrix<double>, LocalVector<double>, double> ls;
	Jacobi<LocalMatrix<double>, LocalVector<double>, double> p;
	ls.SetOperator(mat);
	ls.SetPreconditioner(p);
	ls.Build();
	ls.Solve(rhs, &x);

	x.CopyToData(_x);
	mat.Clear();
	x.Clear();
	rhs.Clear();

	ls.Clear();
}
