using System;
using System.Collections.Generic;
using UnityEngine;

namespace sib
{
    public abstract class UnitCell
    {
        public abstract List<Atom> GetMillerAtoms(int h, int k, int l);
        public abstract void AddVertices(Dictionary<Vector3, Atom> crystalAtoms, int atomicNumber, string elementName);
        public abstract void AddBonds(Dictionary<Vector3, Bond> crystalBonds);
        public abstract Atom[] GetVertices();
        public abstract List<Bond> GetBonds();
        public abstract void Draw(GameObject atomPrefab, GameObject linePrefab, GameObject builder);
        public abstract void GenerateNeighbors(Dictionary<Vector3, Atom> crystalAtoms, Dictionary<Vector3, Bond> crystalBonds, Dictionary<Vector3, UnitCell> crystalCells);
        public abstract string Debug();
    }
}