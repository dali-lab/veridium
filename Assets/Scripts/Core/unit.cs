/**
 * @author      Siddharth Hathi
 * @title       Unit Cells
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace sib
{
    // Struct for storing 3 floats in a vector
    public struct vec3 {
        public float x, y, z;
    }

    // // Struct used to enumerate a bond - made up of two vertex indices 
    // // where the bonds starts and ends respsectively
    // public struct bond {
    //     public int startIndex, endIndex;
    // }

    // public struct Line {
    //     public Vector3 start, end;
    // }

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

    // Static class the wraps global constants pertinent to the unit cell classes
    public static class Constants {
        // The maximum number of vertices in a 6-sided unit cell
        public static int cell6Vertices = 15;
        // The maximum number of vertices in an 8-sided unit cell
        public static int cell8Vertices = 14;

        // A list of the 15 points in a 6-sided cell - the first 
        // 8 are found in all 6-sided unit cells, the next 8 are found
        // in face-centered cells, and the last one is a center point
        // for body-centered cells.
        /*************************************
        * UNIT CELL ARRAY POSITION-INDEX REF:
        * (Assumes 1x1x1 cubic centered at origin)
        * 0: (-1, -1, -1),
        * 1: (-1, -1, 1),
        * 2: (-1, 1, -1),
        * 3: (1, -1, -1),
        * 4: (1, 1, -1),
        * 5: (1, -1, 1),
        * 6: (-1, 1, 1),
        * 7: (1, 1, 1),
        * 8: (0, 0, -1),
        * 9: (0, -1, 0),
        * 10: (-1, 0, 0),
        * 11: (0, 0, 1),
        * 12: (0, 1, 0),
        * 13: (1, 0, 0),
        * 14: (0, 0, 0)
        **************************************/
        // public static vec3[] cell6BasicPositions = new vec3[] {
        //     new vec3() { x=-1, y=-1, z=-1 },
        //     new vec3() { x=-1, y=-1, z=1 },
        //     new vec3() { x=-1, y=1, z=-1 },
        //     new vec3() {x=1, y=-1, z=-1},
        //     new vec3() {x=1, y=1, z=-1},
        //     new vec3() {x=1, y=-1, z=1},
        //     new vec3() {x=-1, y=1, z=1},
        //     new vec3() {x=1, y=1, z=1},
        //     new vec3() {x=0, y=0, z=-1},
        //     new vec3() {x=0, y=-1, z=0},
        //     new vec3() {x=-1, y=0, z=0},
        //     new vec3() {x=0, y=0, z=1},
        //     new vec3() {x=0, y=1, z=0},
        //     new vec3() {x=1, y=0, z=0},
        //     new vec3() {x=0, y=0, z=0}
        // };

        public static Vector3[] cell6BasicPositions = new Vector3[] {
            // Basic Vertices
            new Vector3(-1, -1, -1),
            new Vector3(-1, -1, 1),
            new Vector3(-1, 1, -1),
            new Vector3(1, -1, 1),
            new Vector3(1, 1, -1),
            new Vector3(1, -1, 1),
            new Vector3(-1, 1, 1),
            new Vector3(1, 1, 1),
            // Face Centered Vertices
            new Vector3(0, 0, -1),
            new Vector3(0, -1, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0),
            // Body centered vertex
            new Vector3(0, 0, 0)
        };

        // A static dictionary reference that maps each unit cell type to the valid variations associated with it.
        public static Dictionary<CellType, CellVariation[]> validCells = new Dictionary<CellType, CellVariation[]> {
            { CellType.CUBIC, new CellVariation[] { CellVariation.SIMPLE, CellVariation.FACE, CellVariation.BODY } },
            { CellType.ORTHO, new CellVariation[] { CellVariation.SIMPLE, CellVariation.FACE, CellVariation.BODY, CellVariation.BASE } },
            { CellType.TETRA, new CellVariation[] { CellVariation.SIMPLE, CellVariation.BODY } },
            { CellType.MONO, new CellVariation[] { CellVariation.SIMPLE, CellVariation.BASE } },
            { CellType.TRI, new CellVariation[] { CellVariation.SIMPLE } },
            { CellType.RHOMBO, new CellVariation[] { CellVariation.SIMPLE } },
            { CellType.HEX, new CellVariation[] { CellVariation.SIMPLE } }
        };

        // public static bond[] cell6Bonds = new bond[] {
        //     new bond() { startIndex=0, endIndex=1 }, //AB
        //     new bond() { startIndex=0, endIndex=2 }, //AC
        //     new bond() { startIndex=0, endIndex=3 }, //AD
        //     new bond() { startIndex=1, endIndex=5 }, //BF
        //     new bond() { startIndex=1, endIndex=6 }, //BG
        //     new bond() { startIndex=2, endIndex=4 }, //CE
        //     new bond() { startIndex=2, endIndex=6 }, //CG
        //     new bond() { startIndex=3, endIndex=4 }, //DE
        //     new bond() { startIndex=3, endIndex=5 }, //DF
        //     new bond() { startIndex=4, endIndex=7 }, //EH
        //     new bond() { startIndex=5, endIndex=7 }, //FH
        //     new bond() { startIndex=6, endIndex=7 } //GH
        // };

        public static int[,] cell6BondMap = new int[,] {
            { 1, 2, 3 },
            { 0, 5, 6 },
            { 0, 4, 6 },
            { 0, 4, 5 },
            { 2, 3, 7 },
            { 1, 3, 7 },
            { 1, 2, 7 },
            { 4, 5, 6}
        };
    }

    /**
    * @class Atom
    * Object class used to store positioning data and distinguishing characteristics for an atom
    */
    public class Atom {
        // The Atom's position in global-space
        private Vector3 position;
        
        // The number of protons in the atom
        private int atomicNumber;

        // The name of the element
        private string element;

        /**
         * Constructor - creates a new Atom object
         */
        public Atom(int atomicNumber, string element, Vector3 position) {
            this.atomicNumber = atomicNumber;
            this.element = element;
            this.position = position;
        }
        
        // override object.GetHashCode
        public override int GetHashCode()
        {
            // TODO: write your implementation of GetHashCode() here
            throw new System.NotImplementedException();
            return base.GetHashCode();
        }

        /**
         * @function compare
         * @input   otherAtom   the atom being compared
         * @return  bool        it the atom equivalent to this
         * Compares another atom to itself. Returns true if it has the same
         * position and atomic number.
         */
        public bool Equals(Atom otherAtom) {
            if (otherAtom.getAtomicNumber() == this.atomicNumber && 
                otherAtom.getPosition().x == this.position.x &&
                otherAtom.getPosition().y == this.position.y &&
                otherAtom.getPosition().z == this.position.z) {
                    return true;
            }
            return false;
        }

        /**
         * @function getAtomicNumber
         * @return  int     The Atom's atomic number
         * Returns the atomic number for the Atom
         */
        public int GetAtomicNumber() {
            return this.atomicNumber;
        }

        /**
         * @function getElement
         * @return  string     The Atom's name
         * Returns the name the Atom
         */
        public string GetElement() {
            return this.element;
        }

        /**
         * @function getPosition
         * @return  Vector3     The Atom's  position
         * Returns the position of the Atom
         */
        public Vector3 GetPosition() {
            return this.position;
        }

        public string Debug() {
            string output = "";
            output += "Atom " + this.element + " Atomic Number: " + this.atomicNumber.ToString() + " Position: (" + this.position.x.ToString() + ", " + this.position.y.ToString() + ", " + this.position.z.ToString() + ")\n";
            return output;
        }
    }

    public class Bond {
        private Atom start;
        private Atom end;

        public Bond(Atom start, Atom end) {
            this.start = start;
            this.end = end;
        }

        public Atom GetStart() {
            return this.start;
        }

        public Atom GetEnd() {
            return this.end;
        }

        public bool Equals(Bond other) {
            if (this.start.Equals(other.GetStart()) && this.end.Equals(other.GetEnd())) {
                return true;
            } else if (this.end.Equals(other.GetStart()) && this.start.Equals(other.GetEnd())) {
                return true;
            }
            return false;
        }

        public Vector3 GetStartPos() {
            return this.start.GetPosition();
        }

        public Vector3 GetEndPos() {
            return this.end.GetPosition();
        }

        public string Debug() {
            string output = "";
            output += "Start : " + start.Debug() + "End : " + end.Debug();
            return output;
        }
    }

    /**
    * @class UnitCell6
    * Object class used to store all associated information for a 6 sided unit cell.
    * Implements functionality for building the cell, adding vertices, and implementing
    * modifications based on cell type and variation.
    */
    public class UnitCell6 {
        // Number of vertices in the cell
        private int numVertices;
        private int numBonds;

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
        private Bond[] bonds;

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
        }

        /**
         * @Constructor
         * Builds a unit cell with a given type, structure, worldPosition, side lengths,
         * and angles.
         */
        public UnitCell6(CellType type, CellVariation structure, Vector3 worldPosition,
            float a, float b, float c, float alpha, float beta, float gamme) {

            bool valid = false;
            this.worldPosition = worldPosition;
            this.type = type;
            this.structure = structure;
            this.vertices = new Atom[Constants.cell6Vertices];

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
                case CellType.MONO:
                    this.a = a;
                    this.b = b;
                    this.c = c;

                    this.alpha = alpha;
                    this.beta = beta;
                    this.gamma = gamma;

                    if ((alpha != 90 && beta != 90)) {
                        this.alpha = 90;
                    } else if (alpha != 90 && gamma != 90) {
                        this.beta = 90;
                    } else if (beta != 90 && gamma != 90) {
                        this.gamma = 90;
                    }

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

                    break;
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
                    this.numBonds = 12;
                    break;
                case CellVariation.FACE:
                    this.numVertices = 14;
                    // temporary value
                    this.numBonds = 12;
                    break;
                case CellVariation.BODY:
                    this.numVertices = 9;
                    // temporary value
                    this.numBonds = 12;
                    break;
                case CellVariation.BASE:
                    this.numVertices = 10;
                    // temporary value
                    this.numBonds = 12;
                    break;
                default:
                    this.numVertices = 0;
                    // temporary value
                    this.numBonds = 12;
                    break;
            }
        }

        /**
         * @function    addVertices
         * @input overlap           An array of atoms that are already enclosed in other unit cells
         * @input atomicNumber      The number of protons in the number
         * @input elementName       The name of the element
         * Creates and adds atoms to the unit cell. Checks against the overlap array to make sure
         * that the unit cell doesn't create duplicate atoms.
         */
        public List<Atom> AddVertices(Atom[] overlap, int atomicNumber, string elementName) {
            if (this.numVertices < 0) {
                return null;
            }
            List<Atom> newVertices = new List<Atom>();
            for ( int i = 0; i < this.numVertices; i ++ ) {
                Vector3 atomPosition = GenerateVertexPosition(this.worldPosition, Constants.cell6BasicPositions[i], 
                    this.a, this.b, this.c, this.alpha, this.beta, this.gamma);
                Atom newAtom = new Atom(atomicNumber, elementName, atomPosition);
                bool overlaps = false;
                if (overlap.Length > 0) {
                    for ( int j = 0; j < overlap.Length; j ++ ) {
                        if (newAtom.Equals(overlap[j])) {
                            overlaps = true;
                            this.vertices[i] = overlap[i];
                            break;
                        }
                    }
                }
                if (!overlaps) {
                    this.vertices[i] = newAtom;
                    newVertices.Add(newAtom);
                }
            }

            // NOT FINAL IMPLEMENTATION
            for ( int i = 0; i < this.numBonds; i ++ ) {
                this.bonds[i] = Constants.cell6Bonds[i];
            }

            return newVertices;
        }

        // Transforms the relative position of unit cell vertex to world space coordinates
        public Vector3 GenerateVertexPosition(Vector3 unitCellPosition, Vector3 vertexPosRel, 
            float a, float b, float c, float alpha, float beta, float gamma)
        {
            float x, y, z;
            x = unitCellPosition.x + (vertexPosRel.x * (a/2));
            y = unitCellPosition.y + (vertexPosRel.y * (b/2));
            z = unitCellPosition.z + (vertexPosRel.z * (c/2));
            return new Vector3(x, y, z);
        }

        public void AddBonds() {
            List<Bond> generatedBonds = new List<Bond>();
            for ( int i = 0; i < this.numVertices; i ++ ) {
                if ( i >= Constants.cell6BondMap.Length ) {
                    continue;
                }

                Atom startVertex = this.vertices[i];
                int[] endIndices = Constants.cell6BondMap[i];
                foreach (int endIndex in endIndices) {
                    if (endIndex >= this.numVertices) {
                        continue;
                    }

                    bool duplicate = false;
                    Atom endVertex = vertices[endIndex];
                    Bond newBond = new Bond(startVertex, endVertex);
                    foreach (Bond bond in generatedBonds) {3
                        if (bond.Equals(newbond)) {
                            duplicate = true;
                        }
                    }
                    if (!duplicate) {
                        generatedBonds.Add(newBond);
                        this.bonds[i] = newBond;
                    }
                }
            }
        }

        // public Line[] GetBonds() {
        //     Line[] lines = new Line[this.numBonds];
        //     for ( int i = 0 ; i < this.numBonds; i ++ ) {
        //         bond b = this.bonds[i];
        //         lines[i] = new Line(){ start=this.vertices[b.startIndex].GetPosition(), end=this.vertices[b.endIndex].GetPosition() };
        //     }
        //     return lines;
        // }

        // Returns the vertex array
        public Atom[] GetVertices() {
            return this.vertices;
        }

        // Debugging function - prints the unit cell to console
        public string Debug() {
            string debuginfo = "Vertices : \n";
            
            for ( int i = 0; i < numVertices; i ++ ) {
                debuginfo += "v" + i.ToString() + " (" + this.vertices[i].GetPosition().x.ToString() + ", " + this.vertices[i].GetPosition().y.ToString() + ", " + this.vertices[i].getPosition().z.ToString() + ")\n";
            }

            return debuginfo;
        }
    }
}