using System;
using System.Collections.Generic;
using UnityEngine;

namespace sib
{
    public class UnitCell8 : UnitCell
    {
        private int numVertices;
        private Vector3 worldPosition;
        private float baseLength;
        private float height;
        private Atom[] vertices;
        private List<Bond> bonds;

        public UnitCell8() {
            this.numVertices = 0;
            this.baseLength = 0;
            this.height = 0;
            this.vertices = null;
            this.bonds = null;
            this.worldPosition = new Vector3(0, 0, 0);
        }

        public UnitCell8(Vector3 position, float baseLength, float height) {
            this.worldPosition = position;
            this.baseLength = baseLength;
            this.height = height;
            this.numVertices = 14;
            this.vertices = new Atom[numVertices];
            this.bonds = new List<Bond>();
        }

        public override List<Atom> GetMillerAtoms(int h, int k, int l) {
            return null;
        }

        public override void AddVertices(Dictionary<Vector3, Atom> crystalAtoms, int atomicNumber, string elementName) {
            if (this.numVertices == 0) {
                return;
            }
            string debugString = "Adding Vertices\n";
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
            for ( int i = 0; i < this.numVertices; i ++ ) {
                debugString += "Loop pass " + i.ToString() + "\n";
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
                Vector3 relPosition = Constants.cell8BasicPositions[i];
                debugString += "Relative position " + relPosition.ToString() + "\n";
                Vector3 atomPosition = GenerateVertexPosition(relPosition);
                debugString += "Calculated atom position " + atomPosition.ToString() + "\n";
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
                Atom newAtom = new Atom(atomicNumber, elementName, atomPosition);
                debugString += "New atom created \n";
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
                bool overlaps = false;
                debugString += "Duplication checking starting\n";
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
                Atom duplicate;
                if (crystalAtoms.TryGetValue(atomPosition, out duplicate)) {
                    overlaps = true;
                    this.vertices[i] = duplicate;
                } else {
                    crystalAtoms[atomPosition] = newAtom;
                }
                if (!overlaps) {
                    this.vertices[i] = newAtom;
                }
                debugString += "Duplication checking finished\n";
                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
            }
            debugString += "Function exited\n";
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
        }

        public Vector3 GenerateVertexPosition(Vector3 vertexPositionRel) {
            float x, y, z;
            x = this.worldPosition.x + (vertexPositionRel.x * this.baseLength);
            y = this.worldPosition.y + (vertexPositionRel.y * this.baseLength);
            z = this.worldPosition.z + (vertexPositionRel.z * this.height);
            return new Vector3(x, y, z);
        }

        public override void AddBonds(Dictionary<Vector3, Bond> crystalBonds) {
            // Loops through each vertex in the unit cell
            for ( int startIndex = 0; startIndex < this.numVertices; startIndex ++ ) {
                if ( startIndex >= Constants.cell6BondMap.Length ) {
                    continue;
                }

                Atom startVertex = this.vertices[startIndex];
                int[] endIndices = Constants.cell8BondMap[startIndex];

                // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = 
                //             ("Start vertex and possible ends retreived. \n start vertex: " + startVertex.Debug());

                // Loops through indices of all vertices that should be bound to the startVertex
                foreach (int endIndex in endIndices) {
                    if (endIndex >= this.numVertices) {
                        continue;
                    }

                    // Ensure no duplicate bonds are created within the unit cell
                    bool duplicate = false;
                    Atom endVertex = vertices[endIndex];
                    Bond newBond = new Bond(startVertex, endVertex);foreach (Bond bond in bonds) {
                        if (bond.Equals(newBond)) {
                            duplicate = true;
                        }
                    }

                    if (!duplicate) {
                        Vector3 midpoint = (newBond.GetStartPos() + newBond.GetEndPos())/2;
                        // If an equivalent bond already exists within the Crystal structure, use it instead
                        Bond crystalDuplicate;;
                        if (crystalBonds.TryGetValue(midpoint, out crystalDuplicate)) {
                            if (crystalDuplicate.Equals(newBond)) {
                                bonds.Add(crystalDuplicate);
                            } else {
                                bonds.Add(newBond);
                                crystalBonds[midpoint] = newBond;
                            }
                        } else {
                            bonds.Add(newBond);
                            crystalBonds[midpoint] = newBond;
                        }
                    }
                }
            }
            // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Bonds added " + bonds.Count.ToString() + "\n";
        }

        public override Atom[] GetVertices() {
            return this.vertices;
        }

        public override List<Bond> GetBonds() {
            return this.bonds;
        }

        public override string Debug() {
            string debuginfo = "";

            debuginfo += "WorldPosition : " + this.worldPosition.ToString() + "\n";

            debuginfo += "Bonds : \n";
            for ( int i = 0; i < this.bonds.Count; i ++ ) {
                // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "printing bond at index " + i.ToString();
                // Vector3 bondStart = this.bonds[i].GetStartPos();
                debuginfo += "b " + this.bonds[i].Debug();
            }
            
            debuginfo += "Vertices : \n";
            
            for ( int i = 0; i < numVertices; i ++ ) {
                debuginfo += "v" + i.ToString() + " (" + this.vertices[i].GetPosition().x.ToString() + ", " + this.vertices[i].GetPosition().y.ToString() + ", " + this.vertices[i].GetPosition().z.ToString() + ")\n";
            }

            return debuginfo;
        }

        public override void Draw(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            for ( int i = 0; i < this.numVertices; i ++ ) {
                if (this.vertices[i] != null) {
                    this.vertices[i].Draw(atomPrefab, builder);
                }
            }

            foreach (Bond bond in this.bonds) {
                bond.Draw(linePrefab, builder);
            }
        }

        public override void GenerateNeighbors(Dictionary<Vector3, Atom> crystalAtoms, Dictionary<Vector3, Bond> crystalBonds, Dictionary<Vector3, UnitCell6> crystalCells)
        {
            throw new NotImplementedException();
        }
    }
}