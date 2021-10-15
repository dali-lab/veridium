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
        BuildStructure();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.up* Time.deltaTime*10);
    }

    public void BuildStructure()
    {
        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "BuildStructure called";

        UnitCell6 test = new UnitCell6(CellType.CUBIC, CellVariation.FACE,
            gameObject.transform.position, 0.66f, 0.66f, 0.66f, 90, 90, 90);


        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "test Atom instantiated";
        test.AddVertices(new Dictionary<Vector3, Atom>(), 0, null);

        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "vertices added";

        test.AddBonds();

        // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = "bonds added";

        string debugInfo = test.Debug();
        Debug.Log(debugInfo);

        (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = debugInfo;

        Atom[] vertices = test.GetVertices();

        List<Bond> bonds = test.GetBonds();

        foreach (Atom vert in vertices) {
            Instantiate(atomPrefab, vert.GetPosition(), Quaternion.identity).transform.SetParent(gameObject.transform);
        }

        foreach (Bond bond in bonds) {
            Vector3 start = bond.GetStartPos();
            Vector3 end = bond.GetEndPos();
            Vector3 midpoint = (start + end)/2;
            Instantiate(linePrefab, midpoint/3 + gameObject.transform.position, Quaternion.LookRotation(end-start, Vector3.up));
        }
        /*
        foreach (Line edge in lines) {
            Vector3 start = edge.start;
            Vector3 end = edge.end;
            Vector3 midpoint = (start + end)/2;
            Instantiate(linePrefab, midpoint, Quaternion.LookRotation((end-start), Vector3.up));
        }*/
    }
}
