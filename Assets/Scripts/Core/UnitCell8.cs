using System;
using System.Collections.Generic;
using UnityEngine;

namespace sib
{
    public class UnitCell8 : UnitCell
    {
        private int atomicNumber;
        private int numVertices;
        private Vector3 worldPosition;
        private float baseLength;
        private float height;
        private Atom[] vertices;
        private List<Bond> bonds;
        private bool inverted;

        public UnitCell8() {
            this.atomicNumber = 0;
            this.numVertices = 0;
            this.baseLength = 0;
            this.height = 0;
            this.vertices = null;
            this.bonds = null;
            this.worldPosition = new Vector3(0, 0, 0);
            this.inverted = false;
        }

        public UnitCell8(int atomicNumber, Vector3 position, float baseLength, float height, bool inverted) {
            this.worldPosition = position;
            this.baseLength = baseLength;
            this.height = height;
            this.numVertices = 14;
            this.vertices = new Atom[numVertices];
            this.bonds = new List<Bond>();
            this.inverted = inverted;
        }

        public override List<Atom> GetMillerAtoms(int h, int k, int l) {
            if (this.numVertices < 8 || this.vertices[0] == null || this.vertices[1] == null|| this.vertices[2] == null || this.vertices[7] == null) {
                return null;
            }

            Vector3 cellOrigin = this.vertices[0].GetPosition();
            Vector3 a1, a2, a3;
            a1 = this.vertices[1].GetPosition() - cellOrigin;
            a2 = this.vertices[2].GetPosition() - cellOrigin;
            a3 = this.vertices[7].GetPosition() - cellOrigin;

            float planarSeparation = this.baseLength/(Mathf.Sqrt((4.0f/3.0f)*(h^2 + k^2 + h*k) + (Mathf.Pow(this.baseLength, 2)/Mathf.Pow(this.height, 2))*(l^2)));

            List<Atom> atoms = new List<Atom>();
            for ( int i = 0; i < this.numVertices; i ++ ) {
                if (Miller.PointInMillerPlane(this.vertices[i].GetPosition(), h, k, l, cellOrigin, a1, a2, a3, planarSeparation)) {
                    atoms.Add(this.vertices[i]);
                }
            }
            return atoms;
        }

        public override void AddVertices(Dictionary<Vector3, Atom> crystalAtoms) {
            if (this.numVertices == 0) {
                return;
            }

            for ( int i = 0; i < this.numVertices; i ++ ) {
                Vector3 relPosition = Constants.cell8BasicPositions[i];
                Vector3 atomPosition = GenerateVertexPosition(relPosition);
                Atom newAtom = new Atom(this.atomicNumber, atomPosition);

                bool overlaps = false;
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
            }
        }

        public Vector3 GenerateVertexPosition(Vector3 vertexPositionRel) {
            float x, y, z;
            if (this.inverted) {
                x = this.worldPosition.x + (vertexPositionRel.y * this.baseLength);
                y = this.worldPosition.y + (vertexPositionRel.x * this.baseLength);
            } else {
                x = this.worldPosition.x + (vertexPositionRel.x * this.baseLength);
                y = this.worldPosition.y + (vertexPositionRel.y * this.baseLength);
            }
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

        public override void GenerateNeighbors(Dictionary<Vector3, Atom> crystalAtoms, Dictionary<Vector3, Bond> crystalBonds, Dictionary<Vector3, UnitCell> crystalCells)
        {
            Vector3[] neighborRelativeLocations = new Vector3[] {
                Vector3.forward,
                Vector3.back,
                new Vector3((float)(Math.Sqrt(3)/2), 1.5f, 0),
                new Vector3(-(float)(Math.Sqrt(3)/2), 1.5f, 0),
                new Vector3(-(float)(Math.Sqrt(3)/2), -1.5f, 0),
                new Vector3((float)(Math.Sqrt(3)/2), -1.5f, 0),
                new Vector3((float)(-(Math.Sqrt(3))), 0, 0),
                new Vector3((float)(Math.Sqrt(3)), 0, 0)
            };

            foreach (Vector3 direction in neighborRelativeLocations) {
                GenerateNeighborInDirection(direction, this.inverted, crystalCells, crystalAtoms, crystalBonds);
            }
        }

        public void GenerateNeighborInDirection(Vector3 direction, bool invert, Dictionary<Vector3, UnitCell> crystalCells, Dictionary<Vector3, Atom> crystalAtoms, Dictionary<Vector3, Bond> crystalBonds) {
            float xTranslation, yTranslation, zTranslation;
            if (inverted) {
                xTranslation = direction.y*this.baseLength;
                yTranslation = direction.x*this.baseLength;
            } else {
                xTranslation = direction.x*this.baseLength;
                yTranslation = direction.y*this.baseLength;
            }
            zTranslation = direction.z*this.height;

            Vector3 translationVector = new Vector3(xTranslation, yTranslation, zTranslation);
            Vector3 newCellPos = this.worldPosition + translationVector;

            UnitCell possibleDuplicate;
            if (crystalCells.TryGetValue(newCellPos, out possibleDuplicate)) {
                return;
            } else {
                UnitCell8 newCell = new UnitCell8(this.atomicNumber, newCellPos, this.baseLength, this.height, invert);
                newCell.AddVertices(crystalAtoms);
                newCell.AddBonds(crystalBonds);
                crystalCells[newCellPos] = newCell;
            }
        }
    }
}