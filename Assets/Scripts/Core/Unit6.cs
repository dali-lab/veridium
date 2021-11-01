/**
 * @author      Siddharth Hathi
 * @title       Unit Cells
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
    * Object class used to store all associated information for a 6 sided unit cell.
    * Implements functionality for building the cell, adding vertices, and implementing
    * modifications based on cell type and variation.
    */
    public class UnitCell6 {
        // Number of vertices in the cell
        private int numVertices;

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
         * @Constructor
         * Builds a unit cell with a given type, structure, worldPosition, side lengths,
         * and angles.
         */
        public UnitCell6(CellType type, CellVariation structure, Vector3 worldPosition,
            float a, float b, float c, float alpha, float beta, float gamma) {

            bool valid = false;
            this.worldPosition = worldPosition;
            this.type = type;
            this.structure = structure;
            this.vertices = new Atom[Constants.cell6Vertices];
            this.bonds = new List<Bond>();

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
         * @function    AddVertices
         * @input overlap           An array of atoms that are already enclosed in other unit cells
         * @input atomicNumber      The number of protons in the number
         * @input elementName       The name of the element
         * Creates and adds atoms to the unit cell. Checks against the overlap array to make sure
         * that the unit cell doesn't create duplicate atoms.
         */
        public List<Atom> AddVertices(Dictionary<Vector3, Atom> crystalAtoms, int atomicNumber, string elementName) {
            if (this.numVertices < 0) {
                return null;
            }
            List<Atom> newVertices = new List<Atom>();
            int[] vertexIndices = Constants.cell6VariationMap[this.structure];
            int cellIndex = 0;
            foreach ( int index in vertexIndices ) {
                Vector3 atomPosition = GenerateVertexPosition(this.worldPosition, Constants.cell6BasicPositions[index], 
                        this.a, this.b, this.c, this.alpha, this.beta, this.gamma);

                Atom newAtom = new Atom(atomicNumber, elementName, atomPosition);
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
                    newVertices.Add(newAtom);
                }
                cellIndex ++;
            }
            return newVertices;
        }

       /**
        * @function GenerateVertexPosition
        * @input unitCellPosition world space position of the unit cell
        * @input vertexPosRel Some cartesian position relative to the center of the unit cell
        * @inputs a, b, c Unit cell sizing parameters
        * @inputs alpha, beta, gamma Unit cell angle parameters
        * @return Vector3 The world space position of the provided unity space Vector3
        * Converts a Vector from unit cell coordinates to world space
        */
        public Vector3 GenerateVertexPosition(Vector3 unitCellPosition, Vector3 vertexPosRel, 
            float a, float b, float c, float alpha, float beta, float gamma)
        {
            float x, y, z;
            x = unitCellPosition.x + (vertexPosRel.x * (a/2));
            y = unitCellPosition.y + (vertexPosRel.y * (b/2));
            z = unitCellPosition.z + (vertexPosRel.z * (c/2));
            return new Vector3(x, y, z);
        }

        /**
         * @function AddBonds
         * @input crystalBonds The Crystal object's bonding hashmap - relates bond
         * mid point position to bonds already in the Crystal structure
         * Adds bonds to the unit cell taking special care not to add the same bonds twice
         */
        public void AddBonds(Dictionary<Vector3, Bond> crystalBonds) {
            // Loops through each vertex in the unit cell
            for ( int startIndex = 0; startIndex < this.numVertices; startIndex ++ ) {
                if ( startIndex >= Constants.cell6BondMap.Length ) {
                    continue;
                }

                Atom startVertex = this.vertices[startIndex];
                int[] endIndices = Constants.cell6BondMap[startIndex];

                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = 
                            ("Start vertex and possible ends retreived. \n start vertex: " + startVertex.Debug());

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
            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Bonds added " + bonds.Count.ToString() + "\n";
        }

        // Returns the vertex array
        public Atom[] GetVertices() {
            return this.vertices;
        }

        // Returns the List of bonds
        public List<Bond> GetBonds() {
            return this.bonds;
        }

        // Debugging function - prints the unit cell to console
        public string Debug() {
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

        public void Draw(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Drawing unit cell";

            string debugOutput = "";
            // int index = 0;

            // foreach ( Atom atom in this.vertices ) {
            //     if (atom != null) {
            //         atom.Draw(atomPrefab, builder);
            //     } else {
            //         debugOutput += "Null atom at index " + index.ToString();
            //     }
            //     index ++;
            // }

            for ( int i = 0; i < this.numVertices; i ++ ) {
                if (this.vertices[i] != null) {
                    this.vertices[i].Draw(atomPrefab, builder);
                }
            }

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugOutput;            

            foreach ( Bond bond in this.bonds ) {
                bond.Draw(linePrefab, builder);
            }
        }

        /**
         * @function GenerateNeighbors
         * @input cystalAtoms Dictionary mapping positions in world space to the atoms at those positions
         * @input crystalBonds Dictionary mapping positions in world space to Bonds at those positions
         * @input crystallCells Dictionary mapping positions in world space to unit cells centered at those
         * positions
         */
        public void GenerateNeighbors(Dictionary<Vector3, Atom> crystalAtoms, Dictionary<Vector3, Bond> crystalBonds, Dictionary<Vector3, UnitCell6> crystalCells) {

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Generating Unit Cell Neighbors";     

            string debugString = "";

            Vector3[] possibleDirections = new Vector3[] {
                Vector3.forward,
                Vector3.back,
                Vector3.right,
                Vector3.left,
                Vector3.up,
                Vector3.down
            };

            int index = 0;

            foreach (Vector3 direction in possibleDirections) {
                float translation = 1;

                if (direction == Vector3.forward || direction == Vector3.back) {
                    translation = this.a;
                } else if (direction == Vector3.right || direction == Vector3.left) {
                    translation = this.b;
                } else if (direction == Vector3.up || direction == Vector3.down) {
                    translation = this.c;
                }

                Vector3 newCellPos = this.worldPosition + (direction * translation);

                debugString += "Pass " + index.ToString() + "new Cell Position " + newCellPos.ToString();

                UnitCell6 possibleDuplicate;
                if (crystalCells.TryGetValue(newCellPos, out possibleDuplicate)) {
                    continue;
                } else {
                    UnitCell6 newCell = new UnitCell6(this.type, this.structure, newCellPos, 
                        this.a, this.b, this.c, this.alpha, this.beta, this.gamma);
                    newCell.AddVertices(crystalAtoms, 0, "");
                    newCell.AddBonds(crystalBonds);
                    crystalCells[newCellPos] = newCell;
                }

                debugString += "\n";
                index ++;

                (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString;  
            }

            debugString += "Exited Successfuly";

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString;  
        }
    }
}