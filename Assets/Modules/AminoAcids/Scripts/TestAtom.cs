using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TestAtom : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out TestAtom otherAtom)) return;
        if (transform.IsChildOf(otherAtom.transform) || otherAtom.transform.IsChildOf(transform)) return;
        if (GetInstanceID() < otherAtom.GetInstanceID()) return;

        transform.SetParent(otherAtom.transform, true);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.transform.position = (transform.position + otherAtom.transform.position) / 2;
        cylinder.transform.localScale = new Vector3(0.02f, Vector3.Distance(transform.position, otherAtom.transform.position) / 2, 0.02f);
        cylinder.transform.up = otherAtom.transform.position - transform.position;
        cylinder.transform.parent = otherAtom.transform;
    }
}
