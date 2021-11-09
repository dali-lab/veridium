using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sib;

public class StructureBuilder : MonoBehaviour
{
    public GameObject atomPrefab;
    public GameObject linePrefab;

    // Start is called before the first frame update
    void Start()
    {
        //BuildStructure();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildCell(CellType type, CellVariation variation, CrystalState state, float sideLength, float sphereRadius) {
        Crystal crystal = new Crystal(gameObject.transform.position);

        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Crystal initialized";

        crystal.SetState(state);

        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Crystal state set";

        crystal.Construct(type, variation, sideLength, sideLength, sideLength, 90, 90, 90, 10);

        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Crystal constructed";

        crystal.Draw(atomPrefab, linePrefab, gameObject);
    }

    public void BuildStructure()
    {
        // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "BuildStructure called";

        UnitCell6 test = new UnitCell6(CellType.CUBIC, CellVariation.FACE,
            gameObject.transform.position, 0.66f, 0.66f, 0.66f, 90, 90, 90);


        //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "test Atom instantiated";
        test.AddVertices(new Dictionary<Vector3, Atom>(), 0, null);

        //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "vertices added";

        test.AddBonds(new Dictionary<Vector3, Bond>());

        // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "bonds added";

        string debugInfo = test.Debug();
        Debug.Log(debugInfo);

        //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;


        // test.Draw(atomPrefab, linePrefab, gameObject);
        // Atom[] vertices = test.GetVertices();

        // List<Bond> bonds = test.GetBonds();

        // foreach (Atom vert in vertices) {
        //     vert.Draw(atomPrefab, gameObject);
        // }

        // foreach (Bond bond in bonds) {
        //     Vector3 start = bond.GetStartPos();
        //     Vector3 end = bond.GetEndPos();
        //     Vector3 midpoint = (start + end)/2;
        //     Instantiate(linePrefab, midpoint/3 + gameObject.transform.position, Quaternion.LookRotation(end-start, Vector3.up));
        // }

        Crystal crystal = new Crystal(gameObject.transform.position);

        //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Crystal initialized";

        crystal.SetState(CrystalState.SINGLECELL);

        //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Crystal state set";

        crystal.Construct(CellType.CUBIC, CellVariation.FACE, 0.66f, 0.66f, 0.66f, 90, 90, 90, 0);

        //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Crystal constructed";

        crystal.SetState(CrystalState.SINGLECELL);

        crystal.Draw(atomPrefab, linePrefab, gameObject);

        debugInfo = crystal.Debug();

        //(GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;

        // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "Crystal drawn";
    }
}
