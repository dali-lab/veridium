using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;

namespace Veridium.Modules.ElementStructures
{
    [System.Serializable]
    public struct AngleData {
        public Vector3 rootAtom;
        public Vector3 incidentAtom1;
        public Vector3 incidentAtom2;
    }

    public class Anim_DrawStructureAngles : AnimationBase
    {
        public StructureBuilder structureBuilder; 
        public GameObject anglePrefab;

        public bool addAngleText = false;
        public bool shouldDrawAllAngles = false;

        public List<AngleData> anglesToDraw;

        public override void Play()
        {
            if (shouldDrawAllAngles) drawAllAngles();
            else drawSpecificAngles();

            base.Play();
        }

        private void drawAllAngles() {
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
                        Atom incident1 = incidentBonds[i].GetStart() == atom ? incidentBonds[i].GetEnd() : incidentBonds[i].GetStart();
                        Atom incident2 = incidentBonds[j].GetStart() == atom ? incidentBonds[j].GetEnd() : incidentBonds[j].GetStart();

                        drawAngle(atom, incident1, incident2);
                    }
                }
            }
        }

        private void drawSpecificAngles() {
            foreach (AngleData angleData in anglesToDraw)
            {
                Atom root = structureBuilder.crystal.GetAtomAtPosition(angleData.rootAtom);
                Atom incident1 = structureBuilder.crystal.GetAtomAtPosition(angleData.incidentAtom1);
                Atom incident2 = structureBuilder.crystal.GetAtomAtPosition(angleData.incidentAtom2);

                drawAngle(root, incident1, incident2);
            }
        }

        private void drawAngle(Atom root, Atom incidentAtom1, Atom incidentAtom2) {
            
            /*Vector3 b = root.drawnObject.transform.InverseTransformPoint(incidentAtom1.drawnObject.transform.position);
            Vector3 c = root.drawnObject.transform.InverseTransformPoint(incidentAtom2.drawnObject.transform.position);

            GameObject angleSymbol = Instantiate(anglePrefab);
            angleSymbol.transform.SetParent(root.drawnObject.transform);

            angleSymbol.transform.localPosition = Vector3.zero;
            angleSymbol.transform.localRotation = Quaternion.LookRotation(Vector3.Cross(-b, c).normalized, c);
            angleSymbol.transform.localScale = new Vector3(300, 300, 300);
            angleSymbol.tag = "angleSymbol";

            Anim_Fade anim = angleSymbol.GetComponentInChildren<Renderer>().gameObject.AddComponent<Anim_Fade>() as Anim_Fade;
            anim.easingType = EasingType.Exponential;
            anim.startingOpacity = 0f;
            anim.endingOpacity = .4f;
            anim.duration = duration;

            angleSymbol.GetComponentInChildren<a*/

            GameObject angleSymbolGO = Instantiate(anglePrefab);
            angleSymbolGO.transform.SetParent(root.drawnObject.transform);
            angleSymbolGO.transform.localPosition = Vector3.zero;

            DynamicAngleVisualization angleSymbol = angleSymbolGO.GetComponent<DynamicAngleVisualization>();
            angleSymbol.rootAtom = root;
            angleSymbol.incidentAtom1 = incidentAtom1;
            angleSymbol.incidentAtom2 = incidentAtom2;
            angleSymbol.shouldShowText = addAngleText;

            Anim_Fade anim = angleSymbolGO.GetComponentInChildren<Renderer>().gameObject.AddComponent<Anim_Fade>() as Anim_Fade;
            anim.easingType = EasingType.Exponential;
            anim.startingOpacity = 0f;
            anim.endingOpacity = .4f;
            anim.duration = duration;

            anim.Play();
        }
    }
}
