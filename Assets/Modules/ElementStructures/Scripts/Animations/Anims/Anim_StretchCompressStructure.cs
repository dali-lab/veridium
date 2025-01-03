using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Core;
using Veridium_Interaction;

namespace Veridium_Animation{
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


        // Somewhat lazy implementation, scales entire structure and then scales atoms back to original size
        // TODO: Fix bond scaling
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
        }

        public override void Pause()
        {
            base.Pause();
        }

    }
}
