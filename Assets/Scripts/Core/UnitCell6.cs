/**
 * @author      Siddharth Hathi
 * @title       Unit Cell 6
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace sib
{
    // Enum used to enumerate unit cell types
    public enum CellType {
        CUBIC,
        ORTHO,
        TETRA,
        MONO,
        TRI,
        RHOMBO,
        HEX
    };

    // Enum used to enumerate cell variations
    public enum CellVariation {
        SIMPLE,
        FACE,
        BODY,
        BASE
    }

    /**
    * @class UnitCell6
    * Object class used to store all associated information for a 6 sided unit 
    * cell. Implements functionality for building the cell, adding vertices, 
    * creating bonds, and generating duplicate neighbor vertices. Designed to be
    * used within the Crystal object superstructure.
    */
    public class UnitCell6 : UnitCell {
        // Number of vertices in the cell
        private int numVertices;

        // Atomic number of atoms within unit cell
        private int atomicNumber;

        // The unit cell's position in world space
        private Vector3 worldPosition;

        // The angles within the cell
        private float alpha, beta, gamma;

        // The side lengths of cell
        private float a, b, c;

        // The type of the unit cell
        private CellType type;

        // The variation of the structure
        private CellVariation structure;

        // The array containing the cell's vertices
        private Atom[] vertices;

        // The bonds connecting the vertices
        private List<Bond> bonds;

        // Default constructor
        public UnitCell6() {
            this.worldPosition = new Vector3(0, 0, 0);

            alpha = 90;
            beta = 90;
            gamma = 90;

            a = 1;
            b = 1;
            c = 1;

            type = CellType.CUBIC;
            structure = CellVariation.SIMPLE;

            vertices = new Atom[Constants.cell6Vertices];
            bonds = new List<Bond>();
        }

        /**
         * @constructor
         * Builds a unit cell with a given type, structure, worldPosition, side 
         * lengths, and angles.
         */
        public UnitCell6(int atomicNumber, CellType type, CellVariation structure, Vector3 worldPosition,
            float a, float b, float c, float alpha, float beta, float gamma) {

            bool valid = false;
            this.worldPosition = worldPosition;
            this.type = type;
            this.structure = structure;
            this.vertices = new Atom[Constants.cell6Vertices];
            this.bonds = new List<Bond>();
            this.atomicNumber = atomicNumber;

            // Checks that the variation + cell type combination is valid
            foreach (CellVariation allowedVar in Constants.validCells[type]) {
                if (allowedVar == structure) {
                    valid = true;
                }
            }

            if (!valid) {
                return;
            }

            // Modifies side lengths and angles based on cell type
            switch(type) {
                case CellType.CUBIC:
                    this.a = a;
                    this.b = a;
                    this.c = a;
                    
                    this.alpha = 90;
                    this.beta = 90;
                    this.gamma = 90;

                    break;
                case CellType.ORTHO:
                    this.a = a;
                    this.b = b;
                    this.c = c;

                    this.alpha = 90;
                    this.beta = 90;
                    this.gamma = 90;

                    break;
                case CellType.TETRA:
                    this.a = a;
                    this.b = b;
                    this.c = c;

                    if (b != c && b != a && a != c) {
                        this.b = a;
                    }

                    this.alpha = 90;
                    this.beta = 90;
                    this.gamma = 90;

                    break;
                case CellType.RHOMBO:
                    this.a = a;
                    this.b = a;
                    this.c = a;

                    this.alpha = alpha;
                    this.beta = alpha;
                    this.gamma = alpha;

                    break;
                case CellType.MONO:
                    this.a = a;
                    this.b = b;
                    this.c = c;

                    this.alpha = 90;
                    this.beta = 90;
                    this.gamma = gamma;

                    break;
                case CellType.TRI:
                    this.a = a;
                    this.b = b;
                    this.c = c;

                    this.alpha = alpha;
                    this.beta = beta;
                    this.gamma = gamma;

                    break;
                case CellType.HEX:
                    return;
                default:
                    this.a = a;
                    this.b = a;
                    this.c = a;
                    
                    this.alpha = 90;
                    this.beta = 90;
                    this.gamma = 90;

                    break;
            }

            // Sets number of vertices
            switch(structure) {
                case CellVariation.SIMPLE:
                    this.numVertices = 8;
                    break;
                case CellVariation.FACE:
                    this.numVertices = 14;
                    break;
                case CellVariation.BODY:
                    this.numVertices = 9;
                    break;
                case CellVariation.BASE:
                    this.numVertices = 10;
                    break;
                default:
                    this.numVertices = 0;
                    break;
            }
        }

        /**
         * @function GetMillerAtoms
         * @input h, k, l   The miller indices
         * @return atoms    The atoms in the miller plane
         * Uses the miller indices to generate a reciprocal lattice describing
         * a cartesian plane. Returns all the atoms in the cell that lie on the 
         * plane.
         */
        public override List<Atom> GetMillerAtoms(int h, int k , int l) {
            if (this.numVertices < 4 || this.vertices[0] == null || this.vertices[1] == null|| this.vertices[2] == null || this.vertices[3] == null) {
                return null;
            }

            // Extracts the primitive vectors from the coordinates of the 
            // cell's vertices. 
            Vector3 bottomCorner = this.vertices[0].GetPosition();
            Vector3 a1, a2, a3;
            a1 = this.vertices[3].GetPosition() - bottomCorner;
            a2 = this.vertices[2].GetPosition() - bottomCorner;
            a3 = this.vertices[1].GetPosition() - bottomCorner;

            // Calculates the distance between parallel unit cells
            float planarSeparation = Mathf.Sqrt(1/((h^2)/(Mathf.Pow(this.a, 2)) + (k^2)/(Mathf.Pow(this.b, 2)) + (l^2)/(Mathf.Pow(this.c, 2))));

            // Identifies the atoms on the miller plane
            List<Atom> atoms = new List<Atom>();
            for ( int i = 0; i < this.numVertices; i ++ ) {
                if (Miller.PointInMillerPlane(this.vertices[i].GetPosition(), h, k, l, bottomCorner, a1, a2, a3, planarSeparation)) {
                    atoms.Add(this.vertices[i]);
                }
            }
            return atoms;
        }

        /**
         * @function AddVertices
         * @input crystalAtoms  The Crystal's hashmap relating positions to
         *                      atoms.
         * Adds vertices to the unit cell.
         */
        public override void AddVertices(Dictionary<Vector3, Atom> crystalAtoms) {
            if (this.numVertices < 0) {
                return;
            }
            int[] vertexIndices = Constants.cell6VariationMap[this.structure];
            int cellIndex = 0;
            foreach ( int index in vertexIndices ) {
                Vector3 atomPosition = GenerateVertexPosition(this.worldPosition, Constants.cell6BasicPositions[index], 
                        this.a, this.b, this.c, this.alpha, this.beta, this.gamma);

                Atom newAtom = new Atom(this.atomicNumber, atomPosition);

                // Makes sure that the atom hasn't already been rendered in 
                // another cell in the crystal
                bool overlaps = false;
                Atom duplicate;
                if (crystalAtoms.TryGetValue(atomPosition, out duplicate)) {
                    overlaps = true;
                    this.vertices[cellIndex] = duplicate;
                } else {
                    crystalAtoms[atomPosition] = newAtom;
                }
                if (!overlaps) {
                    this.vertices[cellIndex] = newAtom;
                }
                cellIndex ++;
            }
        }

       /**
        * @function GenerateVertexPosition
        * @input unitCellPosition       world space position of the unit cell
        * @input vertexPosRel           Some cartesian position relative to the 
        *                               center of the unit cell
        * @inputs a, b, c               Unit cell sizing parameters
        * @inputs alpha, beta, gamma    Unit cell angle parameters
        * @return Vector3               The world space position of the vertex
        * Calculates the world space position of an atom specified in 
        * Constants.cell6BasicPosition
        */
        public Vector3 GenerateVertexPosition(Vector3 unitCellPosition, Vector3 vertexPosRel, 
            float a, float b, float c, float alpha, float beta, float gamma)
        {
            // The cell's primitive vectors
            Vector3 a1, a2, a3;

            // Calculates the components of the third primitive vector
            float cx = c * Mathf.Cos(Mathf.Deg2Rad * beta);
            float cy = c * (Mathf.Cos(Mathf.Deg2Rad * alpha) - Mathf.Cos(Mathf.Deg2Rad * beta) * Mathf.Cos(Mathf.Deg2Rad * gamma)) / (Mathf.Sin(Mathf.Deg2Rad * gamma));
            float cz = Mathf.Sqrt(Mathf.Abs(Mathf.Pow(c, 2) - Mathf.Pow(cx, 2) - Mathf.Pow(cy, 2)));

            // Calculates the primitive vectors
            a1 = Vector3.right * a;
            a2 = (Vector3.right * (b * Mathf.Cos(Mathf.Deg2Rad * gamma))) + (Vector3.up * (b * Mathf.Sin(Mathf.Deg2Rad * gamma)));
            a3 = new Vector3(cx, cy, cz);

            // Applies a transform to the relative position based on the 
            // primitive vectors
            Vector3 origin = -(1.0f / 2.0f) * (a1 + a2 + a3);
            Vector3 primitiveCoordinateRel = vertexPosRel + new Vector3(1, 1, 1);
            Vector3 primitiveCoordinate = (1.0f/2.0f) * (primitiveCoordinateRel.x * a1 + primitiveCoordinateRel.y * a2 + primitiveCoordinateRel.z * a3);
            Vector3 normalizedCoordinate = primitiveCoordinate + origin;
            
            return unitCellPosition + normalizedCoordinate;
        }

        /**
         * @function AddBonds
         * @input crystalBonds  The Crystal object's bonding hashmap - relates 
         *                      the mid point positions of bonds already in the 
         *                      Crystal structure to the bonds themselves
         * Adds bonds to the unit cell taking special care not to add the same 
         * bonds twice
         */
        public override void AddBonds(Dictionary<Vector3, Bond> crystalBonds) {
            // Loops through each vertex in the unit cell
            for ( int startIndex = 0; startIndex < this.numVertices; startIndex ++ ) {
                if ( startIndex >= Constants.cell6BondMap.Length ) {
                    continue;
                }

                // The start point of the bond
                Atom startVertex = this.vertices[startIndex];

                // The possible indices of the end point of the bond
                int[] endIndices;
                if ( this.structure == CellVariation.BODY && startIndex == 8) {
                    endIndices = Constants.cell6BondMap[startIndex + 6];
                } else {
                    endIndices = Constants.cell6BondMap[startIndex];
                }

                //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = 
                //            ("Start vertex and possible ends retreived. \n start vertex: " + startVertex.Debug());

                // Loops through indices of all vertices that should be bound to the startVertex
                foreach (int endIndex in endIndices) {
                    if (endIndex >= this.numVertices) {
                        continue;
                    }

                    // Ensure no duplicate bonds are created locally
                    bool duplicate = false;
                    Atom endVertex = vertices[endIndex];
                    Bond newBond = new Bond(startVertex, endVertex);foreach (Bond bond in bonds) {
                        if (bond.Equals(newBond)) {
                            duplicate = true;
                        }
                    }

                    if (!duplicate) {
                        Vector3 midpoint = (newBond.GetStartPos() + newBond.GetEndPos())/2;
                        // If an equivalent bond already exists within the 
                        // Crystal structure, use it instead
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

        // Returns the vertex array
        public override Atom[] GetVertices() {
            return this.vertices;
        }

        // Returns the List of bonds
        public override List<Bond> GetBonds() {
            return this.bonds;
        }

        // Debugging function - prints the unit cell to console
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

        /**
         * @function Draw
         * @input atomPrefab    GameObject reference to the prefab describing 
         *                      each Atom's appearance
         * @input linepPrefab   GameObject reference to the prefab describing 
         *                      each Bond's appearance
         * @input builder       Reference to StructureBuilder's instance
         * Draws the UnitCell's Atoms and bonds to the scene.
         */
        public override void Draw(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {

            //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Drawing unit cell";

            string debugOutput = "";
            
            // Draws the atoms
            for ( int i = 0; i < this.numVertices; i ++ ) {
                if (this.vertices[i] != null) {
                    this.vertices[i].Draw(atomPrefab, builder);
                }
            }

            //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugOutput;            

            // Draws the bonds
            foreach ( Bond bond in this.bonds ) {
                bond.Draw(linePrefab, builder);
            }
        }

        /**
         * @function GenerateNeighbors
         * @input cystalAtoms   Dictionary mapping positions in world space to 
         *                      the atoms at those positions
         * @input crystalBonds  Dictionary mapping positions in world space to 
         *                      Bonds at those positions
         * @input crystallCells Dictionary mapping positions in world space to 
         *                      unit cells centered at those positions
         * Creates duplicates of itself that exist exactly adjacent to itself 
         * in world space. Adds the duplicates to the crystalCells hashmap
         */
        public override void GenerateNeighbors(Dictionary<Vector3, Atom> crystalAtoms, Dictionary<Vector3, Bond> crystalBonds, Dictionary<Vector3, UnitCell> crystalCells) {

            //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Generating Unit Cell Neighbors";     

            string debugString = "";

            // A list of the directions in which neighbors can be generated 
            // relative to the starting position/orientation of the cell
            Vector3[] possibleDirections = new Vector3[] {
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left,
                Vector3.up,
                Vector3.down
            };

            Vector3 a1, a2, a3;

            float cx = c * Mathf.Cos(Mathf.Deg2Rad * this.beta);
            float cy = c * (Mathf.Cos(Mathf.Deg2Rad * this.alpha) - Mathf.Cos(Mathf.Deg2Rad * this.beta) * Mathf.Cos(Mathf.Deg2Rad * this.gamma)) / (Mathf.Sin(Mathf.Deg2Rad * this.gamma));
            float cz = Mathf.Sqrt(Mathf.Pow(c, 2) - Mathf.Pow(cx, 2) - Mathf.Pow(cy, 2));

            a1 = Vector3.right * this.a;
            a2 = (Vector3.right * (this.b * Mathf.Cos(Mathf.Deg2Rad * this.gamma))) + (Vector3.up * (this.b * Mathf.Sin(Mathf.Deg2Rad * this.gamma)));
            a3 = new Vector3(cx, cy, cz);

            // Loop that creates and verfies validity of new unit cells in each 
            // possible adjacent position
            int index = 0;

            foreach (Vector3 direction in possibleDirections) {
                // Determines position of new Unit Cell
                Vector3 translation = new Vector3(0, 0, 0);

                if (direction == Vector3.right || direction == Vector3.left) {
                    translation = a1*direction.x;
                } else if (direction == Vector3.up || direction == Vector3.down) {
                    translation = -a2*direction.y;
                } else if (direction == Vector3.forward || direction == Vector3.back) {
                    translation = a3*direction.z;
                }

                Vector3 newCellPos = this.worldPosition + translation;

                // Verifies that the unit cell is not a duplicate
                UnitCell possibleDuplicate;
                if (crystalCells.TryGetValue(newCellPos, out possibleDuplicate)) {
                    continue;
                } else {
                    // Builds the unit cell and adds it to the crystal
                    UnitCell6 newCell = new UnitCell6(this.atomicNumber, this.type, this.structure, newCellPos, 
                        this.a, this.b, this.c, this.alpha, this.beta, this.gamma);
                    newCell.AddVertices(crystalAtoms);
                    newCell.AddBonds(crystalBonds);
                    crystalCells[newCellPos] = newCell;
                }

                index ++;

                //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString;  
            }

            debugString += "Exited Successfuly";

            //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString;  
        }
    }
}