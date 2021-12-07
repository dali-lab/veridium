using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using sib;

public class StructureBuilder : MonoBehaviour
{
    // Prefab used to draw Atoms
    public GameObject atomPrefab;

    // Prefab used to draw lines
    public GameObject linePrefab;

    /// Crystal object being drawn to scene
    private Crystal crystal;

    // Start is called before the first frame update
    void Start()
    {
        // BuildHex();
        BuildHexCrystal();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * @function DestoryCell
     * Removes the crystal from the scene.
     */
    public void DestroyCell() {
        this.crystal.ClearCrystal(this.gameObject);
    }

    /**
     * @function GetMillerAtoms
     * @input h         Miller index corresponding to x value of normal vector
     * @input k         Miller index corresponding to y value of normal vector
     * @input l         Miller index corresponding to z value of normal vector
     * @return HashSet  Set containing the atoms falling on the specified plane
     * Returns HashSet of atoms in the specified miller plane
     */
    public HashSet<Atom> GetMillerAtoms(int h, int k, int l) {
        return this.crystal.GetMillerAtoms(h, k, l);
    }

    /**
     * @function BuildCell
     * @input type          The type of the desired cell
     * @input variation     The cell variation of the cell
     * @input state         The draw state the crystal should use
     * @input sideLength    The square dimension of the unitcells
     * @input sphereRadius  The size of the Atoms
     * Creates a crystal according to the given input specificaitons and draws * it to the scene. Times the runtime of processes for benchmarking
     */
    public void BuildCell(CellType type, CellVariation variation, CrystalState state, float sideLength, float sphereRadius) {
        string debugString = "";

        Stopwatch stopwatch = new Stopwatch();

        // Instantiates the Crystal
        stopwatch.Start();
        this.crystal = new Crystal(gameObject.transform.position);
        stopwatch.Stop();

        TimeSpan ts = stopwatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        debugString += "Time elapsed in crystal initialization " + elapsedTime + "\n";

        // Sets the crystal's draw state
        stopwatch.Start();
        this.crystal.SetState(state);
        stopwatch.Stop();

        ts = stopwatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        debugString += "Time elapsed in crystal state setting" + elapsedTime + "\n";

        // Adds Atoms and bonds to the crystal
        stopwatch.Start();
        this.crystal.Construct(type, variation, sideLength, sideLength, sideLength, 90, 90, 90, 0);
        stopwatch.Stop();

        ts = stopwatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        debugString += "Time elapsed in crystal construction" + elapsedTime + "\n";

        // Draws the crystal
        stopwatch.Start();
        this.crystal.Draw(atomPrefab, linePrefab, gameObject);
        stopwatch.Stop();

        ts = stopwatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        debugString += "Time elapsed in crystal drawing" + elapsedTime + "\n";

        // HashSet<Atom> millerAtoms = this.crystal.GetMillerAtoms(1, 0, 0);
        // debugString = "Miller retreival did not crash\n";

        // if (millerAtoms.Count > 0) {
        //     foreach ( Atom atom in millerAtoms ) {
        //         debugString += atom.Debug();
        //     }
        // } else {
        //     debugString += "Miller retreival failed to find any atoms\n";
        // }

        // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugString; 
    }

    public void BuildHex() {
        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Builing Hex"; 
        UnitCell test = new UnitCell8(this.gameObject.transform.position, 0.2f, 0.2f, true);
        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Hex Initialized"; 
        test.AddVertices(new Dictionary<Vector3, Atom>(), 0, "");
        // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Vertices Added"; 
        test.AddBonds(new Dictionary<Vector3, Bond>());
        // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Bonds Added"; 
        test.Draw(this.atomPrefab, this.linePrefab, this.gameObject);
        // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Hex drawn"; 
    }

    public void BuildHexCrystal() {
        Crystal test = new Crystal(this.gameObject.transform.position);
        test.SetState(CrystalState.INFINITE);
        test.Construct(CellType.HEX, CellVariation.SIMPLE, 0.2f, 0.4f, 0, 0, 0, 0, 1);
        test.Draw(this.atomPrefab, this.linePrefab, this.gameObject);
    }
}
