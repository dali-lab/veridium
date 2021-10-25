using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrabCollider : MonoBehaviour
{

    public MeshFilter meshFilter;
    public MeshCollider meshCollider;
    public HandDistanceGrabbable handDistanceGrabbable;

    // Start is called before the first frame update
    void Awake()
    {

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = meshFilter.mesh;
        
    }

}
