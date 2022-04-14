using System;
using System.Collections.Generic;
using UnityEngine;

public interface IShape {}

[Serializable]
public class Cube : IShape
{
    public Vector3 size;
}

[Serializable]
public class UnitCube : Cube
{
    public float sideLength;
}

[Serializable]
public class Thing
{
    public int weight;
}

[ExecuteInEditMode]
public class BuildingBlocks : MonoBehaviour
{
    [SerializeReference]
    public List<IShape> inventory;

    [SerializeReference]
    public System.Object bin;

    [SerializeReference]
    public List<System.Object> bins;

    void OnEnable()
    {
        if (inventory == null)
        {
            inventory = new List<IShape>()
            {
                new Cube() {size = new Vector3(1.0f, 1.0f, 1.0f)},
                new UnitCube() {sideLength = 1.0f}
            };
        }

        if (bins == null)
        {
            // This is supported, the 'bins' serialized field is declared as holding a collection type.
            bins = new List<System.Object>() { new Cube(), new Thing(), new UnitCube() };
        }
    }
}
