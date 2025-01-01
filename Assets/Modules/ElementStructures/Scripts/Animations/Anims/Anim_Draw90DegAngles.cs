using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Veridium_Core;
using Veridium_Interaction;

namespace Veridium_Animation
{
    public class Anim_Draw90DegAngles : AnimationBase
    {
        public StructureBuilder structureBuilder; 
        public StructureBase structureBase;
        public GameObject anglePrefab;

        public override void Play()
        {
            // find visible atoms
            List<Atom> visibleAtoms = new List<Atom>();
            foreach (Atom atom in structureBuilder.crystal.atoms.Values)
            {
                if (atom.drawnObject)
                {
                    visibleAtoms.Add(atom);
                }
            }
            
            Debug.Log("Number of visible atoms: " + visibleAtoms.Count);

            // find incident bonds for each atom
            foreach (Atom atom in visibleAtoms)
            {

                List<Bond> incidentBonds = new List<Bond>();
                foreach (Bond bond in structureBuilder.crystal.bonds.Values)
                {
                    if (bond.GetStart() == atom || bond.GetEnd() == atom)
                    {
                        if (incidentBonds.Contains(bond)) continue;
                        if (!bond.drawnObject) continue;

                        incidentBonds.Add(bond);
                    }
                }

                Debug.Log("Number of incident bonds: " + incidentBonds.Count);
                

                // find the angle between each pair of incident bonds
                for (int i = 0; i < incidentBonds.Count; i++)
                {
                    for (int j = i + 1; j < incidentBonds.Count; j++)
                    {
                        Vector3 b = incidentBonds[i].GetStart() == atom ? incidentBonds[i].GetEnd().drawnObject.transform.position : incidentBonds[i].GetStart().drawnObject.transform.position;
                        Vector3 c = incidentBonds[j].GetStart() == atom ? incidentBonds[j].GetEnd().drawnObject.transform.position : incidentBonds[j].GetStart().drawnObject.transform.position;

                        b = atom.drawnObject.transform.InverseTransformPoint(b);
                        c = atom.drawnObject.transform.InverseTransformPoint(c);

                        GameObject angleSymbol = Instantiate(anglePrefab);
                        angleSymbol.transform.SetParent(atom.drawnObject.transform);

                        angleSymbol.transform.localPosition = Vector3.zero;
                        angleSymbol.transform.localRotation = Quaternion.LookRotation(Vector3.Cross(-b, c).normalized, c);
                        angleSymbol.transform.localScale = new Vector3(300, 300, 300);

                    }
                }
            }


            base.Play();
            

        }
    }
}
