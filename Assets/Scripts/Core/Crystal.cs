/**
 * @author      Siddharth Hathi
 * @title       Crystal
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace sib
{

    // Enum used to describe how the crystal is being rendered in the current 
    // display context
    public enum CrystalState {
        // SINGLECELL describes the state where only the central unit cell gets 
        // rendered
        SINGLECELL,
        // MULTICELL describes the state where 4 core cells get rendered
        MULTICELL,
        // INFINITE describes the state where the crystal structure is 
        // generated recursively to a user-specificed recursion depth
        INFINITE
    };

    /**
     * @class Crystal
     * Object class that generates, stores and renders full Bravais crystaline 
     * latices using the Atom, Bond, and UnitCell6 subclasses.
     */
    public class Crystal {

        // The coordinates of the center of the crystal in world space
        private Vector3 centerPoint;

        // The Dictionary relating the posititions of each atom in the crystal  
        // structure to the atom itself
        private Dictionary<Vector3, Atom> atoms;

        // The Dictionary relating the positions of each bond in the crystal 
        // structure to the atom itself
        private Dictionary<Vector3, Bond> bonds;

        // The Dictionary relating the positions of each Unit cell in the crystal structure to the atom itself
        private Dictionary<Vector3, UnitCell> unitCells;

        // The current drawing context for the crystal
        private CrystalState drawMode;

        /**
         * @constructor
         * @input centerPoint   The location of the crystal in space
         * Default constructor for Crystal. Creates an empty crystal.
         */
        public Crystal(Vector3 centerPoint) {
            this.atoms = new Dictionary<Vector3, Atom>();
            this.bonds = new Dictionary<Vector3, Bond>();
            this.unitCells = new Dictionary<Vector3, UnitCell>();
            this.centerPoint = centerPoint;
            this.drawMode = CrystalState.SINGLECELL;
        }

        /**
         * @function ClearCrystal
         * @input builder   Reference to the StructureBuilder GameObject 
         * that's creating and destroying crystals
         * Removes every child of the crystal gameobject from the scene and 
         * clears the atoms, bonds, and unit cells from the object's hashmaps
         */
        public void ClearCrystal(GameObject builder) {
            foreach (Transform child in builder.transform) {
                MonoBehaviour.Destroy(child.gameObject);
            }

            this.atoms = new Dictionary<Vector3, Atom>();
            this.bonds = new Dictionary<Vector3, Bond>();
            this.unitCells = new Dictionary<Vector3, UnitCell>();
            this.drawMode = CrystalState.SINGLECELL;
        }

        /**
         * @function Draw
         * @input atomPrefab    GameObject for the atom prefab
         * @input linePrefab    GameObject for the line prefab
         * @input builder       GameObject parenting the crystal
         * Draws the crystal in the Unity scene
         */
        public void Draw(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            switch (this.drawMode) {
                case CrystalState.SINGLECELL:
                    if (this.unitCells.ContainsKey(centerPoint)) {
                        //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Drawing unit cell at centerpoint";
                        this.unitCells[centerPoint].Draw(atomPrefab, linePrefab, builder);
                    } else {
                        //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "no atom at centerpoint";
                    }
                    break;
                case CrystalState.MULTICELL:
                    this.unitCells[centerPoint].Draw(atomPrefab, linePrefab, builder);
                    break;
                case CrystalState.INFINITE:
                    foreach ( Atom atom in this.atoms.Values ) {
                        atom.Draw(atomPrefab, builder);
                    }

                    foreach ( Bond bond in this.bonds.Values ) {
                        bond.Draw(linePrefab, builder);
                    }
                    
                    break;
            }
        }

        /**
         * @function SetState
         * @input newState  The new drawState of the crystal
         * Changes the current CrystalState for the Crystal object
         */
        public void SetState(CrystalState newState) {
            this.drawMode = newState;
        }

        /**
         * @function Construct
         * @input type              The type of the UnitCells for this crystal
         * @input vatiation         The CellVariation of the Unit cells
         * @input a                 The a side dimension of the Unit cells
         * @input b                 The b side dimension of the Unit cells
         * @input c                 The c side dimension of the Unit cells
         * @input alpha             The alpha angle of the Unit cells
         * @input beta              The beta angle of the Unit cells
         * @input gamma             The gamma angle of the Unit cells
         * @input constructionDepth How many times the Crystal should 
         * recursively generate Unit Cells for the Crystal view
         * Populates the UnitCell, atom, and bond arrays for the Crystal 
         * depending on what type of unitcells are in the crystal and how far 
         * the user wnats to recursively generate them. For debugging purposes,
         * the processes in this function are timed using System stopwatch.
         */
        public void Construct(CellType type, CellVariation variation,
            float a, float b, float c, float alpha, float beta, float gamma, 
            int constructionDepth) {

            Stopwatch stopwatch = new Stopwatch();
            string debugString = "";
            
            // Construct the origin cell
            stopwatch.Start();
            UnitCell originCell;
            if (type == CellType.HEX) {
                originCell = new UnitCell8(this.centerPoint, a, b, false);
            } else {
                originCell = new UnitCell6(type, variation, 
                    this.centerPoint, a, b, c, alpha, beta, gamma);
            }
            this.unitCells[this.centerPoint] = originCell;
            stopwatch.Stop();

            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            debugString += "Time elapsed in cell initialization " + elapsedTime + "\n";

            // Add vertices and bonds to origin cell
            stopwatch.Start();
            originCell.AddVertices(this.atoms, 0, "");
            stopwatch.Stop();

            ts = stopwatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            debugString += "Time elapsed in AddVertices" + elapsedTime + "\n";

            stopwatch.Start();
            originCell.AddBonds(this.bonds);
            stopwatch.Stop();

            ts = stopwatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            debugString += "Time elapsed in AddBonds" + elapsedTime + "\n";

            HashSet<Vector3> constructedPositions = new HashSet<Vector3>();

            // Recursively add surrounding unit cells using Unit6's GenerateNeighbors function
            stopwatch.Start();
            for ( int i = 0; i < constructionDepth; i ++ ) {
                UnitCell[] cells = new UnitCell[unitCells.Count];
                Vector3[] positions = new Vector3[unitCells.Count];
                unitCells.Values.CopyTo(cells, 0);
                unitCells.Keys.CopyTo(positions, 0);
                for ( int cellIndex = 0; cellIndex < cells.Length; cellIndex ++ ) {
                    UnitCell cell = cells[cellIndex];
                    Vector3 position = positions[cellIndex];
                    if (!constructedPositions.Contains(position)) {
                        constructedPositions.Add(position);
                        if (cell != null) {
                            cell.GenerateNeighbors(this.atoms, this.bonds, this.unitCells);
                        }
                    }
                }
            }

            stopwatch.Stop();
            ts = stopwatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            debugString += "Time elapsed in crystal building" + elapsedTime + "\n";

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString;
        }

        public HashSet<Atom> GetMillerAtoms(int h, int k , int l) {
            HashSet<Atom> atomList = new HashSet<Atom>();
            UnitCell[] cells = new UnitCell[unitCells.Count];
            unitCells.Values.CopyTo(cells, 0);
            for ( int cellIndex = 0; cellIndex < cells.Length; cellIndex ++ ) {
                if (cells[cellIndex] != null) {
                    List<Atom> planeAtoms = cells[cellIndex].GetMillerAtoms(h, k, l);
                    foreach ( Atom atom in planeAtoms ) {
                        if (atom != null) {
                            atomList.Add(atom);
                        } 
                    }
                }
            }
            return atomList;
        }

        /**
         * @funciton Debug
         * @return string   A string describing the state of the crystal
         * Returns a string respresentation of the objects's state for debugging
         */
        public string Debug() {
            string debugInfo = "";
            if (this.unitCells.ContainsKey(this.centerPoint)) {
                debugInfo += "Center Vertex Info: " + this.unitCells[this.centerPoint].Debug();
            } else {
                debugInfo += "No center point available";
            }
            return debugInfo;
        }

    }
}