using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using sib;

public class StructureBuilder : MonoBehaviour
{
    public GameObject atomPrefab;
    public GameObject linePrefab;
    Crystal crystal;

    // Start is called before the first frame update
    void Start()
    {
        //BuildStructure();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HighlightPlane(int index){

        foreach (Atom atom in GetPlanarAtoms(index)){
            atom.GetDrawnObject().GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Coloration.GetColorByNumber(29));
            atom.GetDrawnObject().GetComponentInChildren<Renderer>().material.EnableKeyword("_EMISSION");
        }

    }

    public void DestroyCell() {
        this.crystal.ClearCrystal(this.gameObject);
    }

    public HashSet<Atom> GetPlanarAtoms(int planeIndex) {
        return this.crystal.GetPlanarAtoms(planeIndex);
    }

    public void BuildCell(CellType type, CellVariation variation, CrystalState state, float sideLength, float sphereRadius/*, string element*/) {

        gameObject.transform.parent.localPosition = Vector3.zero;
        gameObject.transform.parent.localRotation = Quaternion.identity;
        gameObject.transform.parent.localScale = Vector3.one;

        string debugString = "";

        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
        this.crystal = new Crystal(gameObject.transform.position);
        stopwatch.Stop();

        TimeSpan ts = stopwatch.Elapsed;
        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        debugString += "Time elapsed in crystal initialization " + elapsedTime + "\n";

        stopwatch.Start();
        this.crystal.SetState(state);
        stopwatch.Stop();

        ts = stopwatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        debugString += "Time elapsed in crystal state setting" + elapsedTime + "\n";

        stopwatch.Start();
        this.crystal.Construct(type, variation, sideLength, sideLength, sideLength, 90, 90, 90, 0);
        stopwatch.Stop();

        ts = stopwatch.Elapsed;
        elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
        debugString += "Time elapsed in crystal construction" + elapsedTime + "\n";

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
