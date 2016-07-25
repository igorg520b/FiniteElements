using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace GeoLoader
{
    class Model
    {
        public List<Node> nodes;                // all nodes
        public List<Node> activeNodes;          // nodes that are not anchored
        public List<Node> anchoredNodes2;       // nodes to which displacement boundary conditions are applied
        public List<Node> drawNodes;            // triples of nodes that form triangles of the surface, for drawing
        public List<int> drawTags;

        public List<Element> elems;
        public HashSet<int> surfaceTags;    // outer surfaces of the elements are tagged with integers (used to apply BC)
        public CSR_System csr;

        public Model(Stream str)
        {
            nodes = new List<Node>(100000);
            drawNodes = new List<Node>(100000);
            drawTags = new List<int>(30000);
            elems = new List<Element>(300000);
            surfaceTags = new HashSet<int>();

            // load mesh from stream
            StreamReader sr = new StreamReader(str);
            string s;
            string[] separators = new string[] { " ", "," };
            string[] parts;

            // go to first node
            do {
                s = sr.ReadLine();
                parts = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            } while (parts[0] != "ND");

            // read nodes in the format:
            // " ND, 8545,    0.0089999996    -0.004416103     -0.01005054"
            do
            {
                double x = double.Parse(parts[2]);
                double y = double.Parse(parts[3]);
                double z = double.Parse(parts[4]);
                nodes.Add(new Node(x, y, z));
                s = sr.ReadLine();
                parts = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            } while (parts[0] == "ND");

            // go to first element
            while(parts[0] != "EL")
            {
                s = sr.ReadLine();
                parts = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            }

            // read elements in the format:
            //  " EL, 198708,  VL      1,   4  7731 48584  7730  7815     0     0     0  -207     0     0"
            do
            {
                const int nVertices = 4;
                int[] tags = new int[nVertices];
                Node[] nds = new Node[nVertices];
                for(int i=0;i< nVertices; i++)
                {
                    nds[i] = nodes[int.Parse(parts[5+i])-1];
                    tags[i] = int.Parse(parts[9+i]);
                    surfaceTags.Add(tags[i]);
                }
                Element elem = new Element() { vertices = nds, surfaceTags = tags };
                elems.Add(elem);

                // add tagged faces to the drawing array in the form of node triples
                if (tags[0] != 0) { drawNodes.Add(nds[2]); drawNodes.Add(nds[0]); drawNodes.Add(nds[1]); drawTags.Add(tags[0]); }
                if (tags[1] != 0) { drawNodes.Add(nds[3]); drawNodes.Add(nds[1]); drawNodes.Add(nds[0]); drawTags.Add(tags[1]); }
                if (tags[2] != 0) { drawNodes.Add(nds[1]); drawNodes.Add(nds[3]); drawNodes.Add(nds[2]); drawTags.Add(tags[2]); }
                if (tags[3] != 0) { drawNodes.Add(nds[2]); drawNodes.Add(nds[3]); drawNodes.Add(nds[0]); drawTags.Add(tags[3]); }

                s = sr.ReadLine();
                parts = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            } while (parts[0] == "EL");

            str.Close();
            Parallel.ForEach(elems, elem => elem.ComputeStiffnessMatrix());
        }

        public void MarkAnchoredNodes(int tag1, int tag2)
        {
            anchoredNodes2 = new List<Node>();
            for (int i = 0; i < drawTags.Count; i++)
            {
                if(drawTags[i] == tag1 || drawTags[i] == tag2)
                {
                    drawNodes[i * 3 + 0].anchored = true;
                    drawNodes[i * 3 + 1].anchored = true;
                    drawNodes[i * 3 + 2].anchored = true;
                }
                if (drawTags[i] == tag2)
                {
                    anchoredNodes2.Add(drawNodes[i * 3 + 0]);
                    anchoredNodes2.Add(drawNodes[i * 3 + 1]);
                    anchoredNodes2.Add(drawNodes[i * 3 + 2]);
                }
            }
        }

        public void InitializeCSR()
        {
            // list non-anchored nodes and give them sequential ids
            activeNodes = nodes.FindAll(nd => !nd.anchored);
            int id = 0;
            foreach (Node nd in activeNodes) { nd.altId = id++; }

            // in each node make a list of elements to which it belongs
            foreach (Element elem in elems)
                foreach (Node nd in elem.vertices) nd.AddElem(elem);

            // find neighbour nodes
            Parallel.ForEach(nodes, nd => nd.InferConnectivityInformation());

            // count total number of neighbours in all nodes 
            int count = 0;
            foreach (Node nd in activeNodes) count += nd.neighbors.Count;

            // allocate CSR
            // each neighbor contributes 3 rows and 3 columns to CSR matrix, so nnz = count * 9
            // the size of the matrix is (number of active nodes)*(3 coordinates)
            csr = new CSR_System(activeNodes.Count * 3, count * 9);

            // 3) create CSR indices
            count = 0;
            foreach(Node nd in activeNodes)
            {
                int row_nnz = nd.CreateCSRIndices(count, csr.cols);
                csr.rows[nd.altId * 3] = count;
                csr.rows[nd.altId * 3 + 1] = count + row_nnz;
                csr.rows[nd.altId * 3 + 2] = count + row_nnz * 2;
                count += row_nnz * 3;
            }
        }

        public void ApplyDisplacementBoundaryConditions(double stretch, double twist)
        {
            double alpha = twist * Math.PI / 180D;
            foreach (Node nd in anchoredNodes2)
            {
                nd.ux = stretch;
                double new_y = Math.Cos(alpha) * nd.y + Math.Sin(alpha) * nd.z;
                double new_z = Math.Cos(alpha) * nd.z - Math.Sin(alpha) * nd.y;
                nd.uy = new_y - nd.y;
                nd.uz = new_z - nd.z;

                nd.cx = nd.x + nd.ux;
                nd.cy = nd.y + nd.uy;
                nd.cz = nd.z + nd.uz;
            }
        }

        public void AssembleLinearSystem()
        {
            csr.Clear();
            double[] v = csr.vals;
            double[] rhs = csr.rhs;

            foreach(Element elem in elems)
            {
                double[,] K = elem.K;
                for(int i=0;i<4;i++) 
                    for(int j=0;j<4;j++)
                    {
                        Node ni = elem.vertices[i];
                        Node nj = elem.vertices[j];
                        if(!ni.anchored)
                        {
                            if (!nj.anchored)
                            {
                                // write the corresponding 3x3 fragment to CSR
                                int idx1 = ni.pcsr[nj.altId]; // there is a room for optimization here
                                for(int m=0;m<3;m++) for(int n=0;n<3;n++) v[idx1 + ni.row_nnz * n + m] += K[i * 3 + n, j * 3 + m];
                            }
                            else {
                                // write the triple to RHS with negative sign
                                int idx = ni.altId * 3;
                                for(int m=0;m<3;m++)
                                    rhs[idx + m] -= K[i * 3 + m, j * 3] * nj.ux + K[i * 3 + m, j * 3 + 1] * nj.uy + K[i * 3 + m, j * 3 + 2] * nj.uz;
                            }
                        } 
                    }
            }

        }


        public void Solve(bool useGPU, bool useDoublePrecision)
        {
            if (useDoublePrecision) csr.SolveDouble(useGPU);
            else csr.SolveSingle(useGPU);
            foreach(Node nd in activeNodes)
            {
                nd.ux = csr.x[nd.altId * 3];
                nd.uy = csr.x[nd.altId * 3+1];
                nd.uz = csr.x[nd.altId * 3+2];
                nd.cx = nd.x + nd.ux;
                nd.cy = nd.y + nd.uy;
                nd.cz = nd.z + nd.uz;
            }
        }
    }
}
