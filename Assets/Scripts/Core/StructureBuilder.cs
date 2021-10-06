using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using sib;

public class StructureBuilder : MonoBehaviour
{
    public GameObject atomPrefab;
    // Start is called before the first frame update
    void Start()
    {
        BuildStructure();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildStructure()
    {
        UnitCell6 test = new UnitCell6(CellType.CUBIC, CellVariation.SIMPLE,
            new Vector3(0, 0, 0), 2, 2, 2, 90, 90, 90);
        test.addVertices(new Atom[0], 0, null);

        Atom[] vertices = test.getVertices();

        foreach (Atom vert in vertices) {
            Instantiate(atomPrefab, vert.getPosition() + gameObject.transform.position, Quaternion.identity);
        }
    }
}
