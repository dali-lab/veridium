using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace sib
{

    public enum CrystalState {
        SINGLECELL,
        MULTICELL,
        INFINITE
    };

    class Crystal {

        private Vector3 centerPoint;
        private Dictionary<Vector3, Atom> atoms;
        private Dictionary<Vector3, Bond> bonds;
        private Dictionary<Vector3, UnitCell6> unitCells;
        private CrystalState drawMode;

        public Crystal(Vector3 centerPoint) {
            this.atoms = new Dictionary<Vector3, Atom>();
            this.bonds = new Dictionary<Vector3, Bond>();
            this.unitCells = new Dictionary<Vector3, UnitCell6>();
            this.centerPoint = centerPoint;
            this.drawMode = CrystalState.SINGLECELL;
        }

        public void ClearCrystal(GameObject builder) {
            foreach (Transform child in builder.transform) {
                MonoBehaviour.Destroy(child.gameObject);
            }

            this.atoms = new Dictionary<Vector3, Atom>();
            this.bonds = new Dictionary<Vector3, Bond>();
            this.unitCells = new Dictionary<Vector3, UnitCell6>();
            this.drawMode = CrystalState.SINGLECELL;
        }

        public void Draw(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            ClearCrystal(builder);
            switch (this.drawMode) {
                case CrystalState.SINGLECELL:
                    if (this.unitCells.ContainsKey(centerPoint)) {
                        this.unitCells[centerPoint].Draw(atomPrefab, linePrefab, builder);
                    } else {
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
                        bond.Draw(atomPrefab, builder);
                    }
                    
                    break;
            }
        }

        public void SetState(CrystalState newState) {
            this.drawMode = newState;
        }

        public void Construct(CellType type, CellVariation variation,
            float a, float b, float c, float alpha, float beta, float gamma, 
            int constructionDepth) {

            Stopwatch stopwatch = new Stopwatch();
            string debugString = "";
            
            stopwatch.Start();
            UnitCell6 originCell = new UnitCell6(type, variation, 
                this.centerPoint, a, b, c, alpha, beta, gamma);
            this.unitCells[this.centerPoint] = originCell;
            stopwatch.Stop();

            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            debugString += "Time elapsed in cell initialization " + elapsedTime + "\n";

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

            // debugString += "Construction depth : " + constructionDepth.ToString() + "\n";

            stopwatch.Start();
            for ( int i = 0; i < constructionDepth; i ++ ) {
                // int index = 0;
                // debugString += "New pass starting at depth " + i.ToString() + "\n";
                UnitCell6[] cells = new UnitCell6[unitCells.Count];
                Vector3[] positions = new Vector3[unitCells.Count];
                unitCells.Values.CopyTo(cells, 0);
                unitCells.Keys.CopyTo(positions, 0);
                for ( int cellIndex = 0; cellIndex < cells.Length; cellIndex ++ ) {
                    // debugString += "Checking unit cell construction at position\n";
                    UnitCell6 cell = cells[cellIndex];
                    Vector3 position = positions[cellIndex];
                    if (!constructedPositions.Contains(position)) {
                        constructedPositions.Add(position);
                        // debugString += "New positon validated\n";
                        if (cell != null) {
                            cell.GenerateNeighbors(this.atoms, this.bonds, this.unitCells);
                            // debugString += "Neighbors generated in pass " + i.ToString() + " for vertex " + index.ToString();
                        } else {
                            // debugString += "No entry found in unitCells for requested position";
                        }
                    }
                }

                // debugString += "Pass complete\n";
                //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;

            //     foreach (KeyValuePair<Vector3, UnitCell6> item in this.unitCells) {
            //         debugInfo += "Checking unit cell construction at position\n";
            //         (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;
            //         if (!constructedPositions.Contains(item.Key)) {
            //             debugInfo += "New positon validated\n";
            //             if (this.unitCells.ContainsKey(item.Key) && this.unitCells[item.Key] != null) {
            //                 this.unitCells[item.Key].GenerateNeighbors(this.atoms, this.bonds, this.unitCells);
            //                 debugInfo += "Neighbors generated in pass " + i.ToString() + " for vertex " + index.ToString();
            //                 (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;
            //             } else {
            //                 debugInfo += "No entry found in unitCells for requested position";
            //                 (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;
            //             }
            //         }
            //         index ++;
            //     }
            }

            stopwatch.Stop();
            ts = stopwatch.Elapsed;
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            debugString += "Time elapsed in crystal building" + elapsedTime + "\n";

            (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString;
        }

        public HashSet<Atom> GetPlanarAtoms(int planeIndex) {
            HashSet<Atom> atomList = new HashSet<Atom>();
            UnitCell6[] cells = new UnitCell6[unitCells.Count];
            unitCells.Values.CopyTo(cells, 0);
            for ( int cellIndex = 0; cellIndex < cells.Length; cellIndex ++ ) {
                if (cells[cellIndex] != null) {
                    List<Atom> planeAtoms = cells[cellIndex].GetPlaneAtIndex(planeIndex);
                    for ( int atomIndex  = 0; atomIndex < planeAtoms.Count; i ++ ) {
                        atomList.Add(planeAtoms[atomIndex]);
                    }
                }
            }
            return atomList;
        }

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