/**
 * @author Siddharth Hathi
 * @title Unit Cell 8
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium_Core
{
    /**
     * @class UnitCell8
     * Object class used to store all associated information for an 8 sided unit
     * cell. Implements functionality for building the hexagonal cell, adding
     * vertices, creating bonds, and generating duplicate neighbor vertices.
     * Designed for use within the Crystal object superstructure.
     */
    public class UnitCell2 : UnitCell
    {
        // The atomic number of the atoms within the cell
        private int atomicNumber;

        // The number of vertices in the cell
        private int numVertices;

        // The world position of the cell
        private Vector3 worldPosition;

        // The length of the hexagonal base's side lengths
        private float baseLength;

        // The height of the hexagonal cell
        private float height;

        // The array of vertices in the cell
        private Atom[] vertices;

        // The list of bonds in the cell
        private List<Bond> bonds;

        // Is the hexagon rotated 60 degrees
        private bool inverted;

        /**
         * Default Constructor
         */
        public UnitCell2()
        {
            this.atomicNumber = 0;
            this.numVertices = 0;
            this.baseLength = 0;
            this.height = 0;
            this.vertices = null;
            this.bonds = null;
            this.worldPosition = new Vector3(0, 0, 0);
            this.inverted = false;
        }

        /**
         * @constructor
         * @input atomicNumber  The atomic number of the atoms in the cell
         * @input position      The position of the center of the cell
         * @input baseLength    The length of the hexagonal sides
         * @input height        The height of the cell
         * @input inverted      Is the cell rotated 60 degrees?
         * Constructs the hexagonal cell based on the given input parameters.
         */
        public UnitCell2(int atomicNumber, Vector3 position, float baseLength, float height, bool inverted)
        {
            this.worldPosition = position;
            this.baseLength = baseLength;
            this.height = height;
            this.numVertices = 2;
            this.vertices = new Atom[numVertices];
            this.bonds = new List<Bond>();
            this.inverted = inverted;
        }

        /**
         * @function GetMillerAtoms
         * @input h, k, l   The miller indices
         * @return atoms    The atoms in the miller plane
         * Uses the miller indices to generate a reciprocal lattice describing
         * a cartesian plane. Returns all the atoms in the cell that lie on the 
         * plane.
         */
        public override List<Atom> GetMillerAtoms(int h, int k, int l)
        {
            if (this.numVertices < 8 || this.vertices[0] == null || this.vertices[1] == null || this.vertices[2] == null || this.vertices[7] == null)
            {
                return null;
            }

            // Extracts the primitive vectors from the coordinates of the 
            // cell's vertices. 
            Vector3 cellOrigin = this.vertices[0].GetPosition();
            Vector3 a1, a2, a3;
            a1 = this.vertices[1].GetPosition() - cellOrigin;
            a2 = this.vertices[2].GetPosition() - cellOrigin;
            a3 = this.vertices[7].GetPosition() - cellOrigin;

            // Calculates the distance between parallel unit cells
            float planarSeparation = this.baseLength / (Mathf.Sqrt((4.0f / 3.0f) * (h ^ 2 + k ^ 2 + h * k) + (Mathf.Pow(this.baseLength, 2) / Mathf.Pow(this.height, 2)) * (l ^ 2)));

            // Identifies the atoms on the miller plane
            List<Atom> atoms = new List<Atom>();
            for (int i = 0; i < this.numVertices; i++)
            {
                if (Miller.PointInMillerPlane(this.vertices[i].GetPosition(), h, k, l, cellOrigin, a1, a2, a3, planarSeparation))
                {
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
        public override void AddVertices(Dictionary<Vector3, Atom> crystalAtoms)
        {
            if (this.numVertices == 0)
            {
                return;
            }

            for (int i = 0; i < this.numVertices; i++)
            {
                Vector3 relPosition = Constants.cell2BasicPositions[i];
                Vector3 atomPosition = GenerateVertexPosition(relPosition);
                Debug.Log("relative: " + relPosition + " atomPos: " + atomPosition);
                Atom newAtom = new Atom(this.atomicNumber, atomPosition);
                newAtom.builder = builder;

                // Makes sure that the atom hasn't already been rendered in 
                // another cell in the crystal
                bool overlaps = false;
                Atom duplicate;
                if (crystalAtoms.TryGetValue(atomPosition, out duplicate))
                {
                    overlaps = true;
                    this.vertices[i] = duplicate;
                }
                else
                {
                    crystalAtoms[atomPosition] = newAtom;
                }
                if (!overlaps)
                {
                    this.vertices[i] = newAtom;
                }
            }
        }

        /**
         * @function GenerateVertexPosition
         * @input unitCellPosition       world space position of the unit cell
         * @input vertexPosRel           Some cartesian position relative to the 
         *                               center of the unit cell
         * @return Vector3               The world space position of the vertex
         * Calculates the world space position of an atom specified in 
         * Constants.cell8BasicPosition
         */
        public Vector3 GenerateVertexPosition(Vector3 vertexPositionRel)
        {
            float x, y, z;
            if (this.inverted)
            {
                x = this.worldPosition.x + (vertexPositionRel.y * this.baseLength);
                y = this.worldPosition.y + (vertexPositionRel.x * this.baseLength);
            }
            else
            {
                x = this.worldPosition.x + (vertexPositionRel.x * this.baseLength);
                y = this.worldPosition.y + (vertexPositionRel.y * this.baseLength);
            }
            z = this.worldPosition.z + (vertexPositionRel.z * this.height);
            return new Vector3(x, y, z);
        }

        /**
         * @function AddBonds
         * @input crystalBonds  The Crystal object's bonding hashmap - relates 
         *                      the mid point positions of bonds already in the 
         *                      Crystal structure to the bonds themselves
         * Adds bonds to the unit cell taking special care not to add the same 
         * bonds twice
         */
        public override void AddBonds(Dictionary<Vector3, Bond> crystalBonds)
        {
            return;
            // Loops through each vertex in the unit cell
            for (int startIndex = 0; startIndex < this.numVertices; startIndex++)
            {
                if (startIndex >= Constants.cell8BondMap.Length)
                {
                    continue;
                }

                Atom startVertex = this.vertices[startIndex];
                int[] endIndices = Constants.cell8BondMap[startIndex];

                // Loops through indices of all vertices that should be bound to the startVertex
                foreach (int endIndex in endIndices)
                {
                    if (endIndex >= this.numVertices)
                    {
                        continue;
                    }

                    // Ensure no duplicate bonds are created within the unit cell
                    bool duplicate = false;
                    Atom endVertex = vertices[endIndex];
                    Bond newBond = new Bond(startVertex, endVertex);
                    newBond.builder = builder;

                    foreach (Bond bond in bonds)
                    {
                        if (bond.Equals(newBond))
                        {
                            duplicate = true;
                        }
                    }

                    if (!duplicate)
                    {
                        Vector3 midpoint = (newBond.GetStartPos() + newBond.GetEndPos()) / 2;
                        // If an equivalent bond already exists within the Crystal structure, use it instead
                        Bond crystalDuplicate;
                        if (crystalBonds.TryGetValue(midpoint, out crystalDuplicate))
                        {
                            if (crystalDuplicate.Equals(newBond))
                            {
                                bonds.Add(crystalDuplicate);
                            }
                            else
                            {
                                bonds.Add(newBond);
                                crystalBonds[midpoint] = newBond;
                            }
                        }
                        else
                        {
                            bonds.Add(newBond);
                            crystalBonds[midpoint] = newBond;
                        }
                    }
                }
            }
        }

        // Returns the vertex array
        public override Atom[] GetVertices()
        {
            return this.vertices;
        }

        // Returns the List of bonds
        public override List<Bond> GetBonds()
        {
            return this.bonds;
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
        public override void Draw()
        {

            // Draw the atoms
            for (int i = 0; i < this.numVertices; i++)
            {
                if (this.vertices[i] != null)
                {
                    this.vertices[i].Draw();
                }
            }

            // Draws the bonds
            foreach (Bond bond in this.bonds)
            {
                bond.Draw();
            }

            // Draw CAGE

            Vector3[] positions = new Vector3[] 
            { 
                Constants.cell2CagePositions[0],
                Constants.cell2CagePositions[1],
                Constants.cell2CagePositions[5],
                Constants.cell2CagePositions[4],
                Constants.cell2CagePositions[0],
                Constants.cell2CagePositions[3],
                Constants.cell2CagePositions[7],
                Constants.cell2CagePositions[6],
                Constants.cell2CagePositions[2],
                Constants.cell2CagePositions[1],
                Constants.cell2CagePositions[5],
                Constants.cell2CagePositions[6],
                Constants.cell2CagePositions[2],
                Constants.cell2CagePositions[3],
                Constants.cell2CagePositions[7],
                Constants.cell2CagePositions[4]
            };

            // SCALE CAGE POSITIONS BASED ON BASELENGTH
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] *= baseLength;
            }


            LineRenderer lr = builder.GetComponent<LineRenderer>();
            lr.positionCount = positions.Length;
            lr.SetPositions(positions);

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

            foreach (Vector3 direction in neighborRelativeLocations)
            {
                GenerateNeighborInDirection(direction, this.inverted, crystalCells, crystalAtoms, crystalBonds);
            }
        }

        /**
         * @function GenerateNeighborInDirection
         * @input direction     A unit vector in the direction of the neighbor
         * @bool invert         Should the neighbors be inverted relative to 
         *                      this cell?
         * @input crystallCells Dictionary mapping positions in world space to 
         *                      unit cells centered at those positions
         * @input cystalAtoms   Dictionary mapping positions in world space to 
         *                      the atoms at those positions
         * @input crystalBonds  Dictionary mapping positions in world space to 
         *                      Bonds at those positions
         * Generates an adjacent duplicate of the current cell instance in the 
         * specified unit direction.
         */
        public void GenerateNeighborInDirection(Vector3 direction, bool invert, Dictionary<Vector3, UnitCell> crystalCells, Dictionary<Vector3, Atom> crystalAtoms, Dictionary<Vector3, Bond> crystalBonds)
        {
            // Builds the translation vector components
            float xTranslation, yTranslation, zTranslation;
            if (inverted)
            {
                xTranslation = direction.y * this.baseLength;
                yTranslation = direction.x * this.baseLength;
            }
            else
            {
                xTranslation = direction.x * this.baseLength;
                yTranslation = direction.y * this.baseLength;
            }
            zTranslation = direction.z * this.height;

            // Applies the translation
            Vector3 translationVector = new Vector3(xTranslation, yTranslation, zTranslation);
            Vector3 newCellPos = this.worldPosition + translationVector;

            // Checks that the cell hasn't already been drawn
            UnitCell possibleDuplicate;
            if (crystalCells.TryGetValue(newCellPos, out possibleDuplicate))
            {
                return;
            }
            else
            {
                UnitCell2 newCell = new UnitCell2(this.atomicNumber, newCellPos, this.baseLength, this.height, invert);
                newCell.builder = builder;
                newCell.AddVertices(crystalAtoms);
                //newCell.AddBonds(crystalBonds);
                crystalCells[newCellPos] = newCell;
                
            }
        }
    }
}