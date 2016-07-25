using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GeoLoader
{
    class Node
    {


        public int altId;       // altId is sequential numbering of non-anchored nodes
        public double x, y, z;      // original positions
        public double ux, uy, uz;   // displacements
        public double cx, cy, cz;   // current positions

        public List<Element> elems;         // elems that are connected to this node
        public HashSet<int> neighbors;      // set of nodes that are adjacent to this node (altId is used)
        public SortedList<int, int> pcsr;   // indices in the CSR value array that correspond to neighbor information (also altId)
        public int row_nnz;                 // number of non-zero entries in the CSR row for this node
        public bool anchored = false;       // location of the anchored node does not change, hence displacements are not variables

        public Node(double x, double y, double z)
        {
//            this.id = id;
            this.x = cx = x;
            this.y = cy = y;
            this.z = cz = z;
            elems = new List<Element>(20);
        }

        public void AddElem(Element elem)
        {
            Trace.Assert(!elems.Contains(elem), "two identical elements are being added to node");
            elems.Add(elem); // if an element contains this node, it will be added 
        }

        public void InferConnectivityInformation()
        {
            neighbors = new HashSet<int>();

            // "neighbors" will contain nodes who connect to this node through edges of adjacent elements.
            // "neighbors" is a set, because some elements have coinciding edges, and we don't want duplicate entries
            // It is computationally easier to insert into unordered container, hence HashSet is used.
            // In the next stage, order will matter, and we will sort these values.
            // Anchored nodes _do_ participate in the simulation, but they do not go into CSR.
            // altId must be assigned to non-anchored nodes before this funcion is called.
            // this.altId also goes into neighbors
            // Basically, all nodes that directly exert forces on current node go in the list, including self.
            foreach (Element elem in elems)
                foreach (Node nd in elem.vertices)
                    if (!nd.anchored) neighbors.Add(nd.altId); // using altId here!
        }

        /// <summary>
        /// This function is used in the process of constructing the structure of CSR.
        /// It identifies the locations in the matrix where non-zero values will be present.
        /// This function is executed for each node whose displacement is variable (non-anchored node),
        /// in sequential order starting from 0.
        /// Each execution of CreateCSRIndices adds 3 row entries to CSR, corresponding to (x,y,z).
        /// In each row, every non-zero entry corresponds to a neighbor of current node. 
        /// The non-zero entries in each row also come in (x,y,z) triples.
        /// </summary>
        /// <param name="startIndex">number of non-zero elements in the CSR that have been already initialized</param>
        /// <param name="cols">column structure of CSR that is being created</param>
        /// <returns></returns>
        public int CreateCSRIndices(int startIndex, int[] cols)
        {
            // number of non-zero entries (nnz) in this row is (number of neighbors)*(3 coordinates)
            // we save this value, because it will be used to look up indices in CSR.vals[] at assembly stage
            row_nnz = neighbors.Count * 3;

            // In each row of the CSR matrix, non-zero values are stored left-to-write,
            // therefore we transfer neighboring nodes from HashSet to SortedList (sort by altId before inserting into CSR).
            // Creating SortedSet from HashSet should speed up insertions into SortedList (not benchmarked)
            SortedSet<int> sortedNeighbors = new SortedSet<int>(neighbors);

            // PCSR stands for "Position in the CSR matrix value array".
            // In pcsr, "key" is the altId of the neighbor node (self included in neighbor list), and
            // "value" is the index in CSR.vals[] where the stiffness value will go.
            // If idx is such index, then the values will also go to (idx+1) and (idx+2), but
            // we do not store (idx+1) and (idx+2) explicitly in pcsr - only idx is stored.
            // The stiffness values will also be written to (idx + row_nnz), (idx + 2*row_nnz), (idx+row_nnz+1),...,(idx+2*row_nnz+1).
            pcsr = new SortedList<int, int>(neighbors.Count);

            // for each neighbor of the current node, a 3x3 entry is created in the CSR matrix
            // the number of each new entry is stored in pcsr, where key=altId, value=index
            foreach (int _altId in sortedNeighbors)
            {
                pcsr.Add(_altId, startIndex);
                for (int i = 0; i < 3; i++)
                {
                    cols[startIndex + i] =
                    cols[startIndex + i + row_nnz] =
                    cols[startIndex + i + row_nnz * 2] = _altId * 3 + i;
                }
                startIndex += 3;
            }
            neighbors = null; 
            return row_nnz;
        }
    }
}
