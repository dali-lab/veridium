using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Core;
using Veridium.Interaction;

namespace Veridium.Animation{
    public class Anim_StretchCompressStructure : AnimationBase
    {
        public Vector3 endScale = new Vector3(1,1,1);
        public EasingType easingType = EasingType.Linear;
        private Vector3 startingScale;
        [HideInInspector] public bool easeOutOnly = false;

        public StructureBase structureBase;

        private GameObject target;
        
        public override void Play()
        {
            base.Play();

            target = structureBase.structureController.gameObject;
            startingScale = target.transform.localScale;

        }


        // Somewhat lazy implementation, scales entire structure and then scales atoms and bonds back to original size
        // Only works when bonds are either perpendicular or parallel to the squeezing direction
        protected override void UpdateAnim()
        {
            base.UpdateAnim();

            float alpha;

            if(!easeOutOnly){
                alpha = Easing.EaseFull(elapsedTimePercent, easingType);
            } else {
                alpha = Easing.EaseOut(elapsedTimePercent, easingType);
            }

            target.transform.localScale = (endScale - startingScale) * alpha + startingScale;


            Vector3 reverseScale = new Vector3(
                1 / target.transform.localScale.x,
                1 / target.transform.localScale.y,
                1 / target.transform.localScale.z 
            );

            // apply inverse scaling to atoms
            foreach (Atom atom in structureBase.structureBuilder.crystal.atoms.Values)
            {
                if (atom.drawnObject != null)
                {
                    atom.drawnObject.transform.localScale = reverseScale * 0.15f;
                }
            }
            
            // apply inverse scaling to bonds
            foreach (Bond bond in structureBase.structureBuilder.crystal.bonds.Values)
            {
                if (bond.drawnObject != null)
                {
                    Vector3 right = target.transform.InverseTransformDirection(bond.drawnObject.transform.right).normalized;
                    Vector3 up = target.transform.InverseTransformDirection(bond.drawnObject.transform.up).normalized;

                    Vector3 newLocalScale = new Vector3(
                        Mathf.Abs(Vector3.Dot(right, reverseScale)),
                        Mathf.Abs(Vector3.Dot(up, reverseScale)),
                        1
                    );

                    bond.drawnObject.transform.localScale = newLocalScale;
                }
            }
        }

        public override void Pause()
        {
            base.Pause();
        }

    }
}
