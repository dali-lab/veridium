using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Veridium_Core;
using Veridium_Interaction;

namespace Veridium_Animation
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
                            Destroy(child.gameObject);
                        }
                    }
                }
            }


            base.Play();
        }
    }
}
