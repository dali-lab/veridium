using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;

namespace Veridium.Modules.ElementStructures
{
    public class Anim_Draw90DegAngles : AnimationBase
    {
        public StructureBuilder structureBuilder; 
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
                        angleSymbol.tag = "angleSymbol";

                        Anim_Fade anim = angleSymbol.GetComponentInChildren<Renderer>().gameObject.AddComponent<Anim_Fade>() as Anim_Fade;
                        anim.easingType = EasingType.Exponential;
                        anim.startingOpacity = 0f;
                        anim.endingOpacity = .4f;
                        anim.duration = duration;

                        anim.Play();

                    }
                }
            }


            base.Play();
            

        }
    }
}
