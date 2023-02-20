using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Veridium_Core;
using System.Linq;

public class StructureBuilder : MonoBehaviour
{

    /// Crystal object being drawn to scene
    public Crystal crystal {get; private set;}
    public CellType cellType;
    public CellVariation cellVariation;
    [HideInInspector] public int numPlanes;
    public bool initialized {get; private set;}
    public bool buildOnStart;
    public Queue<Atom> atomsToDraw = new Queue<Atom>();

    // Start is called before the first frame update
    void Start()
    {

        if(buildOnStart) BuildCell(cellType, cellVariation, CrystalState.INFINITE, 0.5f, 0.075f, 23);
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.startWidth = Constants.cageLineWidth;
        lr.endWidth = Constants.cageLineWidth;

    }

    void Update()
    {

        if(atomsToDraw.Count > 0){
            Atom atom = atomsToDraw.Dequeue();
            Debug.Log(atom);
            atom.Draw();
        }

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

    public void Redraw(CrystalState state){
        crystal.drawMode = state;
        crystal.Draw();
    }

    /**
     * @function DestroyCell
     * Removes the crystal from the scene.
     */
    public void DestroyCell() {
        crystal.ClearCrystal(gameObject);
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
        lr.startWidth = Constants.cageLineWidth;
        lr.endWidth = Constants.cageLineWidth;

        initialized = false;
    }

    public Atom GetAtomAtCoordinate(Vector3 pos){

        Vector3 corrected = pos * 0.25f; //+ transform.position;

        foreach (KeyValuePair<Vector3, Atom> a in crystal.atoms){
            
            if((a.Key - corrected).magnitude < 0.1){
                return a.Value;
            }
        }
        return null;
    }

     public Vector3 GetCoordinateAtAtom(Atom a){
          foreach (KeyValuePair<Vector3, Atom> pair in crystal.atoms){
              if(GameObject.ReferenceEquals(a, pair.Value)){
                  return pair.Key / 0.25f;   // Revert vector from corrected value from GetUnitCellAtCoordinate
              }
          }
          return Vector3.zero;
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
        return crystal.GetMillerAtoms(h, k, l);
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

        crystal = new Crystal(gameObject.transform.position, gameObject);
        crystal.SetState(state);
        crystal.Construct(type, variation, Constants.defaultA, Constants.defaultB, Constants.defaultC, Constants.defaultAlpha, Constants.defaultBeta, Constants.defaultGamma, atomicNumber, 6);
        crystal.Draw();
    }
}
