/**
 * @author      Siddharth Hathi
 * @title       Crystal
 */

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Veridium.Modules.ElementStructures
{

    // Enum used to describe how the crystal is being rendered in the current 
    // display context
    public enum CrystalState {
        // SINGLECELL describes the state where only the central unit cell gets 
        // rendered
        SINGLECELL,
        // INFINITE describes the state where the crystal structure is 
        // generated recursively to a user-specificed recursion depth
        INFINITE,
        MULTICELL,
        // Need 2 different multicell views for hexagonal structures specifically
        MULTICELLHEX1,
        MULTICELLHEX2
    };

    public enum CrystalView {
        // SINGLECELL describes the state where only the central unit cell gets 
        // rendered
        BallAndStick,
        // INFINITE describes the state where the crystal structure is 
        // generated recursively to a user-specificed recursion depth
        ClosePacked
    };

    /**
     * @class Crystal
     * Object class that generates, stores and renders full Bravais crystaline 
     * latices using the Atom, Bond, and UnitCell6 subclasses.
     */
    public class Crystal {

        public CellType cellType {get; private set;}
        public CellVariation cellVariation {get; private set;}

        // The coordinates of the center of the crystal in world space
        private Vector3 centerPoint;

        // The Dictionary relating the posititions of each atom in the crystal  
        // structure to the atom itself
        public Dictionary<Vector3, Atom> atoms {get; private set;}

        // The Dictionary relating the positions of each bond in the crystal 
        // structure to the atom itself
        public Dictionary<Vector3, Bond> bonds {get; private set;}

        // The Dictionary relating the position of each cage in the crystal
        // structure to the cage itself
        // public Dictionary<Vector3, GameObject> cages {get; private set;}

        // The Dictionary relating the positions of each Unit cell in the crystal structure to the atom itself
        public Dictionary<Vector3, UnitCell> unitCells {get; private set;}

        // The current drawing context for the crystal
        public CrystalState drawMode;
        public GameObject builder;
        public GameObject infiniteObject;
        public int atomicNumber {get; private set;}

        /**
         * @constructor
         * @input centerPoint   The location of the crystal in space
         * Default constructor for Crystal. Creates an empty crystal.
         */
        public Crystal(Vector3 centerPoint, GameObject builder) {
            atoms = new Dictionary<Vector3, Atom>();
            bonds = new Dictionary<Vector3, Bond>();
            // cages = new Dictionary<Vector3, GameObject>();
            unitCells = new Dictionary<Vector3, UnitCell>();
            centerPoint = Vector3.zero;//centerPoint;
            drawMode = CrystalState.SINGLECELL;
            this.builder = builder;
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

            atoms = new Dictionary<Vector3, Atom>();
            bonds = new Dictionary<Vector3, Bond>();
            // cages = new Dictionary<Vector3, GameObject>();
            unitCells = new Dictionary<Vector3, UnitCell>();
            drawMode = CrystalState.SINGLECELL;

            if (infiniteObject != null) Debug.Log(infiniteObject.name);
            MonoBehaviour.Destroy(infiniteObject);

            infiniteObject = null;
        }

        /**
         * @function Draw
         * @input atomPrefab    GameObject for the atom prefab
         * @input linePrefab    GameObject for the line prefab
         * @input builder       GameObject parenting the crystal
         * Draws the crystal in the Unity scene
         */
        public void Draw() {

            foreach (Bond bond in bonds.Values)
            {
                if(bond.drawnObject != null) MonoBehaviour.Destroy(bond.drawnObject);
            }

            foreach (Atom atom in atoms.Values)
            {
                if(atom.drawnObject != null) MonoBehaviour.Destroy(atom.drawnObject);
            }

            foreach (Transform child in builder.transform)
            {
                if (child.tag == "cage") MonoBehaviour.Destroy(child.gameObject);
            }

            // foreach (GameObject cage in cages.Values)
            // {
            //     if(cage != null) MonoBehaviour.Destroy(cage);
            // }

            MonoBehaviour.Destroy(infiniteObject);

            switch (drawMode) {
                case CrystalState.SINGLECELL:
                    if (unitCells.ContainsKey(centerPoint)) {
                        unitCells[centerPoint].Draw();
                    }
                    break;
                case CrystalState.MULTICELL:

                // TODO for someone someday: what I did here is messy. If you make accessing unit cells easier and 
                // make structures heirarchical in the scene it should be easy to make it look nicer.

                    foreach (Atom atom in atoms.Values){
                        if(atom.drawnObject != null) MonoBehaviour.Destroy(atom.drawnObject);
                    }

                    foreach (Bond bond in bonds.Values){
                        if(bond.drawnObject != null) MonoBehaviour.Destroy(bond.drawnObject);
                    }
                    
                    switch (cellType){
                        case CellType.CUBIC:
                            for ( float i = 0; i < 2; i++){
                                for ( float j = 0; j < 2; j++){
                                    for ( float k = 0; k < 2; k++){
                                        Vector3 coord = new Vector3(i,j,k);
                                        UnitCell unitCell = GetUnitCellAtCoordinate(coord);
                                        if (unitCell != null){
                                            unitCell.builder = builder;
                                            unitCell.Draw();
                                        }
                                    }
                                }
                            }
                        break;
                        case CellType.HEX:
                        break;
                        default:
                        break;
                    } 

                    foreach (Atom atom in atoms.Values){
                        if(atom.drawnObject != null){
                            atom.drawnObject.transform.localPosition -= new Vector3(0.25f,0.25f,0.25f);
                            atom.drawnObject.transform.localPosition /= 2f;
                            atom.drawnObject.transform.localScale /= 2f;
                        }
                    }
                    foreach (Bond bond in bonds.Values){
                        if(bond.drawnObject != null){
                            bond.drawnObject.transform.localPosition -= new Vector3(0.25f,0.25f,0.25f);
                            bond.drawnObject.transform.localPosition /= 2f;
                            bond.drawnObject.transform.localScale /= 2f;
                        }
                    }
                    break;

                case CrystalState.MULTICELLHEX1:

                    // What we offset everything by
                    Vector3 hexagonalT = new Vector3(0.75f, 0.75f, Mathf.Sqrt(3)/4f);
                    for (int i=0; i < 2; i++)
                    {
                        for (int j=0; j < 2; j++)
                        {
                            for (int k=0; k < 2; k++)
                            {
                                Vector3 coord = new Vector3(i,j,k);
                                Matrix4x4 m = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1.5f, 0, 0), new Vector4(0.5f, 0, Mathf.Sqrt(3)/2, 0), new Vector4(0, 0, 0, 1)); //Shear
                                coord = m.MultiplyPoint(coord);
                                // Debug.Log("Coord: " + coord);
                                UnitCell unitCell = GetHexUnitCellAtCoordinate(coord);
                                if (unitCell != null){
                                    unitCell.builder = builder;
                                    unitCell.Draw();
                                    UnitCell2 cell2 = (UnitCell2) unitCell;
                                    cell2.RescaleCage(-hexagonalT * Constants.hexBaseLength, 0.5f * Vector3.one);
                                }  
                            }
                        }
                    }
                    foreach (Atom atom in atoms.Values){
                        if(atom.drawnObject != null){
                            atom.drawnObject.transform.localPosition -= hexagonalT * Constants.hexBaseLength;
                            atom.drawnObject.transform.localPosition /= 2f;
                            atom.drawnObject.transform.localScale /= 2f;
                        }
                    }
                    break;
                case CrystalState.MULTICELLHEX2:
                // What we offset everything by
                    hexagonalT = new Vector3(1.5f, 0.75f, Mathf.Sqrt(3)/2f);
                    for (int i=0; i < 3; i++)
                    {
                        for (int j=0; j < 2; j++)
                        {
                            for (int k=0; k < 3; k++)
                            {
                                Vector3 coord = new Vector3(i,j,k);
                                Matrix4x4 m = new Matrix4x4(new Vector4(1, 0, 0, 0), new Vector4(0, 1.5f, 0, 0), new Vector4(0.5f, 0, Mathf.Sqrt(3)/2, 0), new Vector4(0, 0, 0, 1)); //Shear
                                coord = m.MultiplyPoint(coord);
                                // Debug.Log("Coord: " + coord);
                                UnitCell unitCell = GetHexUnitCellAtCoordinate(coord);
                                if (unitCell != null){
                                    unitCell.builder = builder;
                                    unitCell.Draw();
                                    UnitCell2 cell2 = (UnitCell2) unitCell;
                                    cell2.RescaleCage(-hexagonalT * Constants.hexBaseLength, new Vector3(1/3f, 1/2f, 1/3f));
                                }  
                            }
                        }
                    }
                    foreach (Atom atom in atoms.Values){
                        if(atom.drawnObject != null){
                            atom.drawnObject.transform.localPosition -= hexagonalT * Constants.hexBaseLength;
                            atom.drawnObject.transform.localPosition = Vector3.Scale(atom.drawnObject.transform.localPosition, new Vector3(1/3f, 1/2f, 1/3f));
                            atom.drawnObject.transform.localScale /= 3f;
                        }
                    }
                    break;

                case CrystalState.INFINITE:

                    string fileName = "";

                    switch (cellType){
                        case CellType.CUBIC:
                            switch (cellVariation){
                                case CellVariation.FACE:
                                    fileName = "InfiniteFCC";
                                break;
                                case CellVariation.BODY:
                                    fileName = "InfiniteBCC";
                                break;
                            }
                        break;
                        case CellType.TETRA:
                            switch (cellVariation){
                                case CellVariation.BODY:
                                    fileName = "InfiniteBCT";
                                break;
                            }
                        break;
                        case CellType.HEX:
                            switch (cellVariation){
                                case CellVariation.BODY:
                                    fileName = "InfiniteBCH";
                                break;
                            }
                        break;
                        default:
                            throw new Exception("Invalid cell type for infinite view");
                        
                    }

                    infiniteObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("MeshPrefab"));
                    infiniteObject.layer = LayerMask.NameToLayer("InfiniteOnly");
                    // TODO: Bad practice to do this with hardcoded paths
                    infiniteObject.GetComponent<MeshFilter>().mesh = Resources.Load<Mesh>("InfiniteViews/" + fileName);
                    infiniteObject.GetComponent<Renderer>().material = Resources.Load<Material>("M_Atom_Infinite");
                    infiniteObject.transform.localScale = Vector3.one * 150f;
                    infiniteObject.transform.position = Vector3.up * MonoBehaviour.FindObjectsOfType<Camera>()[0].transform.position.y;
                    infiniteObject.GetComponent<Renderer>().material.color = Coloration.GetColorByNumber(atomicNumber);

                    if(true){
                        infiniteObject.GetComponent<Renderer>().material.SetFloat("_Metallic", 1.0f);
                        infiniteObject.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0.65f);
                    }
                    infiniteObject.transform.SetParent(builder.transform);
                    
                    break;
            }
        }

        public UnitCell GetUnitCellAtCoordinate(Vector3 pos)
        {
            Vector3 corrected = pos * 0.5f;

            foreach (KeyValuePair<Vector3, UnitCell> a in unitCells){
                if((a.Key - corrected).magnitude < 0.1){
                    return a.Value;
                }
            }
            return null;
        }

        public UnitCell GetHexUnitCellAtCoordinate(Vector3 pos)
        {
            Vector3 corrected = pos * Constants.hexBaseLength;

            foreach (KeyValuePair<Vector3, UnitCell> a in unitCells){
                if((a.Key - corrected).magnitude < 0.1){
                    return a.Value;
                }
            }
            return null;
        }

        /**
         * @function SetState
         * @input newState  The new drawState of the crystal
         * Changes the current CrystalState for the Crystal object
         */
        public void SetState(CrystalState newState) {
            drawMode = newState;
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
            int atomicNumber, int constructionDepth) {

            this.atomicNumber = atomicNumber;
            cellType = type;
            cellVariation = variation;

            UnitCell originCell;
            if (type == CellType.HEX) {
                originCell = new UnitCell2(atomicNumber, centerPoint, Constants.hexBaseLength, Constants.hexBaseLength, false); // need to scale the unit cell of hex structures to be smaller
                builder.GetComponentInParent<BoxCollider>().size = Vector3.one * (Constants.hexBaseLength * 1.5f);
            } else {
                originCell = new UnitCell6(atomicNumber, type, variation, 
                    centerPoint, a, b, c, alpha, beta, gamma);
                builder.GetComponentInParent<BoxCollider>().size = Vector3.one * 0.6f;
            }
            originCell.builder = builder;
            unitCells[centerPoint] = originCell;

            originCell.AddVertices(atoms);
            originCell.AddBonds(bonds);

            HashSet<Vector3> constructedPositions = new HashSet<Vector3>();

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
                            cell.GenerateNeighbors(atoms, bonds, unitCells);
                        }
                    }
                }
            }
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
    }
}