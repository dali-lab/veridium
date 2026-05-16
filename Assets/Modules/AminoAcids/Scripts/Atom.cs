using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.EnhancedTouch;

namespace Veridium.Modules.AminoAcids {
    public class Atom : MonoBehaviour
    {
        public const float SizeH = 0.5f, SizeC = 1f, SizeN = 0.9f, SizeO = 0.85f, SizeS = 1.35f;

        private Material storedMaterial;
        private bool highlighted = false;
        private Renderer atomRenderer;
        public Element element;
        public int AtomicNumber => element.ToAtomicNumber();
        public string Name;
        public int NaturalValenceElectrons;
        public int MaxValenceElectrons => element == Element.H || element == Element.He ? 2 : 8;
        public int CurrentValenceElectrons => NaturalValenceElectrons + Bonds.Sum(bond => bond.electrons);
        public HashSet<Bond> Bonds = new HashSet<Bond>();
        public HashSet<Atom> Neighbors => new HashSet<Atom>(Bonds.Select(bond => bond.Other(this)));
        public Molecule Molecule;
        public bool acceptsBonds = true;
        public UnityEvent<bool> OnToggleHighlight;
        public bool pointsUpInMolecule;

        void Start()
        {
            Molecule = GetComponentInParent<Molecule>();
            Bonds = new HashSet<Bond>(Molecule.GetComponentsInChildren<Bond>().Where(b => b.atom1 == this || b.atom2 == this));
            GetComponentInChildren<AtomBondingPoint>().gameObject.SetActive(acceptsBonds);
            atomRenderer = GetComponentInChildren<Renderer>();
            OnToggleHighlight = new UnityEvent<bool>();
        }

        void Update()
        {
            transform.LookAt(Camera.main.transform);
        }

        public void BondWith(Atom other, int electrons, float? animDuration = null)
        {
            if (this == other) return;
            if (Neighbors.Contains(other)) return;

            string name = $"{this.name.Split(' ')[0]}-{other.name.Split(' ')[0]}";
            GameObject go = new GameObject(name);
            Bond bond = go.AddComponent<Bond>();
            bond.Create(this, other, electrons, animDuration);
        }

        public bool IsConnectedTo(Atom other, Atom forbiddenAtom)
        {
            if (Molecule != other.Molecule) return false;

            List<Atom> visited = new List<Atom>();
            Queue<Atom> queue = new Queue<Atom>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                Atom current = queue.Dequeue();
                visited.Add(current);

                if (current == other) return true;

                foreach (Atom neighbor in current.Neighbors)
                {
                    if (visited.Contains(neighbor) || queue.Contains(neighbor) || neighbor == forbiddenAtom) continue;
                    queue.Enqueue(neighbor);
                }
            }

            return false;
        }

        public bool AreNeighborsConnectedInOtherWay()
        {
            int connectedCount = Neighbors.Count;

            for (int i = 0; i < Neighbors.Count - 1; i++)
            {
                for (int j = i + 1; j < Neighbors.Count; j++)
                {
                    Atom atom1 = Neighbors.ElementAt(i);
                    Atom atom2 = Neighbors.ElementAt(j);

                    if (atom1.IsConnectedTo(atom2, this)) return true;
                }
            }

            return false;
        }

        public bool HasFullValence(int electrons = 0)
        {
            if (electrons == 0) return CurrentValenceElectrons >= MaxValenceElectrons;
            return CurrentValenceElectrons + electrons > MaxValenceElectrons;
        }

        public Bond GetBondTo(Atom other)
        {
            if (!Neighbors.Contains(other)) return null;

            return Bonds.FirstOrDefault(bond => bond.Other(this) == other);
        }

        public List<Atom> GetConnectedAtoms(Atom forbiddenAtom = null)
        {
            List<Atom> connectedAtoms = new List<Atom>();
            Queue<Atom> queue = new Queue<Atom>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                Atom current = queue.Dequeue();
                connectedAtoms.Add(current);

                foreach (Atom neighbor in current.Neighbors)
                {
                    if (connectedAtoms.Contains(neighbor) || queue.Contains(neighbor) || neighbor == forbiddenAtom) continue;
                    queue.Enqueue(neighbor);
                }
            }

            return connectedAtoms;
        }

        public List<Transform> GetConnectedTransforms(Atom forbiddenAtom = null)
        {
            HashSet<Transform> connectedTransforms = new HashSet<Transform>();
            Queue<Atom> queue = new Queue<Atom>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                Atom current = queue.Dequeue();
                connectedTransforms.Add(current.transform);

                foreach (Atom neighbor in current.Neighbors)
                {
                    if (connectedTransforms.Contains(neighbor.transform) || queue.Contains(neighbor) || neighbor == forbiddenAtom) continue;
                    queue.Enqueue(neighbor);
                    connectedTransforms.Add(current.GetBondTo(neighbor).transform);
                }
            }

            return connectedTransforms.ToList();
        }

        public void Highlight()
        {
            if (storedMaterial)
            {
                (storedMaterial, atomRenderer.material) = (atomRenderer.material, storedMaterial);
                OnToggleHighlight.Invoke(true);
                return;
            }

            Material highlightMat = new Material(Shader.Find("Shader Graphs/HighlightedAtom"));
            highlightMat.SetTexture("BaseTexture", atomRenderer.material.mainTexture as Texture2D);
            storedMaterial = atomRenderer.material;
            atomRenderer.material = highlightMat;
            OnToggleHighlight.Invoke(true);
        }

        public void Unhighlight()
        {
            if (!storedMaterial) return;
            (storedMaterial, atomRenderer.material) = (atomRenderer.material, storedMaterial);
            OnToggleHighlight.Invoke(false);
        }

        [ContextMenu("Highlight")]
        public void ToggleHighlight()
        {
            if (highlighted)
            {
                Unhighlight();
            }
            else
            {
                Highlight();
            }
            highlighted = !highlighted;
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