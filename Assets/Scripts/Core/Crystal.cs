using UnityEngine;
using System;
using System.Collections.Generic;

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
        }

        public void Draw(GameObject atomPrefab, GameObject linePrefab, GameObject builder) {
            ClearCrystal(builder);
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

            UnitCell6 originCell = new UnitCell6(type, variation, 
                this.centerPoint, a, b, c, alpha, beta, gamma);
            this.unitCells[this.centerPoint] = originCell;

            originCell.AddVertices(this.atoms, 0, "");
            originCell.AddBonds(this.bonds);

            string debugInfo = originCell.Debug();
            // Debug.Log(debugInfo);

            //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;

            HashSet<Vector3> constructedPositions = new HashSet<Vector3>();

            debugInfo += "Construction depth : " + constructionDepth.ToString() + "\n";

            for ( int i = 0; i < constructionDepth; i ++ ) {
                int index = 0;
                debugInfo += "New pass starting at depth " + i.ToString() + "\n";
                //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;
                UnitCell6[] cells = new UnitCell6[unitCells.Count];
                Vector3[] positions = new Vector3[unitCells.Count];
                unitCells.Values.CopyTo(cells, 0);
                unitCells.Keys.CopyTo(positions, 0);
                for ( int cellIndex = 0; cellIndex < cells.Length; cellIndex ++ ) {
                    debugInfo += "Checking unit cell construction at position\n";
                    //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;
                    UnitCell6 cell = cells[cellIndex];
                    Vector3 position = positions[cellIndex];
                    if (!constructedPositions.Contains(position)) {
                        constructedPositions.Add(position);
                        debugInfo += "New positon validated\n";
                        if (cell != null) {
                            cell.GenerateNeighbors(this.atoms, this.bonds, this.unitCells);
                            debugInfo += "Neighbors generated in pass " + i.ToString() + " for vertex " + index.ToString();
                            //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;
                        } else {
                            debugInfo += "No entry found in unitCells for requested position";
                            //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;
                        }
                    }
                }
                debugInfo += "Pass complete\n";
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