using UnityEngine;
using Veridium.Animation;
using Veridium.Core;

namespace Veridium.Modules.ElementStructures
{
    public class Anim_Destroy90DegAngles : AnimationBase
    {
        public StructureBuilder structureBuilder; 

        public override void Play()
        {
            foreach (Atom atom in structureBuilder.crystal.atoms.Values)
            {
                if (atom.drawnObject)
                {
                    foreach (Transform child in atom.drawnObject.transform)
                    {
                        if (child.gameObject.tag == "angleSymbol")
                        {
                            Anim_Fade anim = child.gameObject.GetComponentInChildren<Anim_Fade>() as Anim_Fade;

                            anim.easingType = EasingType.Exponential;
                            anim.startingOpacity = .4f;
                            anim.endingOpacity = 0f;
                            anim.duration = duration;

                            anim.Play();
                        }
                    }
                }
            }
            


            base.Play();
        }

        public override void End()
        {
            foreach (Atom atom in structureBuilder.crystal.atoms.Values)
            {
                if (atom.drawnObject)
                {
                    foreach (Transform child in atom.drawnObject.transform)
                    {
                        if (child.gameObject.tag == "angleSymbol")
                        {
                            Destroy(child.gameObject);
                        }
                    }
                }
            }

            base.End();
        }
    }
}
