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
        UnitCell6 test = new UnitCell6(CellType.CUBIC, CellVariation.SIMPLE,
            new Vector3(0, 0, 0), 2, 2, 2, 90, 90, 90);
        test.addVertices(new Atom[0], 0, null);

        Atom[] vertices = test.getVertices();

        //Line[] lines = test.getLines();

        foreach (Atom vert in vertices) {
            Instantiate(atomPrefab, vert.getPosition()/3 + gameObject.transform.position, Quaternion.identity).transform.SetParent(gameObject.transform);
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
