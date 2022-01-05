using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using sib;
using System.Linq;

public class StructureBuilder : MonoBehaviour
{
    // Prefab used to draw Atoms
    public GameObject atomPrefab;

    // Prefab used to draw lines
    public GameObject linePrefab;

    /// Crystal object being drawn to scene
    public Crystal crystal {get; private set;}
    public CellType cellType;
    public CellVariation cellVariation;
    [HideInInspector] public int numPlanes;
    public bool initialized {get; private set;}
    public bool buildOnStart;

    // Start is called before the first frame update
    void Start()
    {

        if(buildOnStart) BuildCell(cellType, cellVariation, CrystalState.SINGLECELL, 0.5f, 0.075f, 23);
        
        // TESTS: Uncomment a test to run it at start

        // Tests.TestHex(this.atomPrefab, this.linePrefab, this.gameObject);
        // Tests.TestHexCrystal(this.atomPrefab, this.linePrefab, this.gameObject);
        // Tests.TestUnit6Millers(this.atomPrefab, this.linePrefab, this.gameObject);
        // Tests.TestUnit8Millers(this.atomPrefab, this.linePrefab, this.gameObject);
        // Tests.TestMillerCrystal(this.atomPrefab, this.linePrefab, this.gameObject);
        // Tests.TestMillerLists(this.atomPrefab, this.linePrefab, this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HighlightPlaneAtIndex(int index){
        
        if(!initialized) return;

        foreach(KeyValuePair<Vector3, Atom> atom in crystal.atoms){
            atom.Value.Unhighlight();
        }

        Vector3 millerIndices = Miller.GetMillerIndicesForCell(cellType, cellVariation)[index];
        
        foreach (Atom atom in GetMillerAtoms((int) millerIndices.x, (int) millerIndices.y, (int) millerIndices.z)){

            atom.Highlight();

        }

    }

    /**
     * @function DestroyCell
     * Removes the crystal from the scene.
     */
    public void DestroyCell() {
        this.crystal.ClearCrystal(this.gameObject);

        initialized = false;
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
    public void BuildCell(CellType type, CellVariation variation, CrystalState state, float sideLength, float sphereRadius, int atomicNumber = 0) {

        // Reset the transform of the structure when building it
        transform.parent.localPosition = Vector3.zero;
        transform.parent.localScale = Vector3.one;
        transform.parent.localRotation = Quaternion.identity;

        cellType = type;
        cellVariation = variation;
        numPlanes = Miller.GetMillerIndicesForCell(cellType, cellVariation).Count;
        initialized = true;

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
        this.crystal.Construct(type, variation, sideLength, sideLength, sideLength, 90, 90, 90, atomicNumber, 0);
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
    }
}
