using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Veridium.Modules.AminoAcids {
    public class Atom : MonoBehaviour
    {
        public Element element;
        public int AtomicNumber => element.ToAtomicNumber();
        public string Name;
        public int NaturalValenceElectrons;
        public int MaxValenceElectrons => element == Element.H || element == Element.He ? 2 : 8;
        public int CurrentValenceElectrons => NaturalValenceElectrons - Bonds.Count + Bonds.Sum(bond => bond.electrons);
        public HashSet<Bond> Bonds = new HashSet<Bond>();
        public HashSet<Atom> Neighbors => new HashSet<Atom>(Bonds.Select(bond => bond.Other(this)));
        public Molecule Molecule;
        public bool acceptsBonds = true;

        void Start() {
            Molecule = GetComponentInParent<Molecule>();
            Bonds = new HashSet<Bond>(Molecule.GetComponentsInChildren<Bond>().Where(b => b.atom1 == this || b.atom2 == this));
            GetComponentInChildren<AtomBondingPoint>().gameObject.SetActive(acceptsBonds);
        }

        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }

        public void BondWith(Atom other, int electrons = 2) {
            if (Neighbors.Contains(other)) return;

            string name = $"{this.name.Split(' ')[0]}-{other.name.Split(' ')[0]}";
            GameObject go = new GameObject(name);
            Bond bond = go.AddComponent<Bond>();
            bond.Create(this, other, electrons);
        }

        public bool IsConnectedTo(Atom other) {
            if (Molecule != other.Molecule) return false;

            List<Atom> visited = new List<Atom>();
            Queue<Atom> queue = new Queue<Atom>();
            queue.Enqueue(this);

            while (queue.Count > 0) {
                Atom current = queue.Dequeue();
                visited.Add(current);

                if (current == other) return true;

                foreach (Atom neighbor in current.Neighbors) {
                    if (!visited.Contains(neighbor) && !queue.Contains(neighbor)) {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return false;
        }

        public List<Atom> GetConnectedAtoms() {
            List<Atom> connectedAtoms = new List<Atom>();
            Queue<Atom> queue = new Queue<Atom>();
            queue.Enqueue(this);

            while (queue.Count > 0) {
                Atom current = queue.Dequeue();
                connectedAtoms.Add(current);

                foreach (Atom neighbor in current.Neighbors) {
                    if (!connectedAtoms.Contains(neighbor) && !queue.Contains(neighbor)) {
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return connectedAtoms;
        }

        // public bool TryGetBondTo(Atom other, out Bond bond) {
        //     bond = null;
        //     if (!Neighbors.Contains(other)) return false;
        //     bond = bonds.First(bond => bond.Other(this) == other);
        //     return true;
        // }

        // public void OnTriggerEnter(Collider collider) {
        //     if (!collider.TryGetComponent(out Atom otherAtom)) return;

        //     Molecule.MergeWith(otherAtom.Molecule, this, otherAtom);
        // }
    }
}