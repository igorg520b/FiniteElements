using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace GeoLoader
{
    class Element
    {
        public Node[] vertices;
        public int[] surfaceTags;
        public double[,] K; // stiffness matrix of the element of size 12x12
        public static double[,] E;

        static Element()
        {
            // initialize elasticity matrix
            E = new double[6, 6];
            double nu = 0.35;   // Poisson ratio
            double E1 = 1;      // Young's modulus
            double coeff1 = E1 / ((1D + nu) * (1D - 2D * nu));
            E[0, 0] = E[1, 1] = E[2, 2] = (1D - nu) * coeff1;
            E[0, 1] = E[0, 2] = E[1, 2] = E[1, 0] = E[2, 0] = E[2, 1] = nu * coeff1;
            E[3, 3] = E[4, 4] = E[5, 5] = (0.5 - nu) * coeff1;
        }

        public void ComputeStiffnessMatrix()
        {
            Node n1 = vertices[0];
            Node n2 = vertices[1];
            Node n3 = vertices[2];
            Node n4 = vertices[3];

            // jacobian of the tetrahedral element
            double[,] J = new double[4, 4] 
            {{ 1, 1, 1, 1}, 
            { n1.x, n2.x, n3.x, n4.x },
            { n1.y, n2.y, n3.y, n4.y },
            { n1.z, n2.z, n3.z, n4.z }};

            double[,] Ji = J.Inverse();
            double a1 = Ji[0, 1], a2 = Ji[1, 1], a3 = Ji[2, 1], a4 = Ji[3, 1];
            double b1 = Ji[0, 2], b2 = Ji[1, 2], b3 = Ji[2, 2], b4 = Ji[3, 2];
            double c1 = Ji[0, 3], c2 = Ji[1, 3], c3 = Ji[2, 3], c4 = Ji[3, 3];

            // strain-displacement matrix
            double[,] B = new double[6, 12] {
        { a1, 0, 0, a2, 0, 0, a3, 0, 0, a4, 0, 0 },
        { 0, b1, 0, 0, b2, 0, 0, b3, 0, 0, b4, 0 },
        { 0, 0, c1, 0, 0, c2, 0, 0, c3, 0, 0, c4 },
        { b1, a1, 0, b2, a2, 0, b3, a3, 0, b4, a4, 0 },
        { 0, c1, b1, 0, c2, b2, 0, c3, b3, 0, c4, b4 },
        { c1, 0, a1, c2, 0, a2, c3, 0, a3, c4, 0, a4 } };

            K = B.TransposeAndMultiply(E).Multiply(B).Multiply(J.Determinant() / 6D);
        }
    }
}
