using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Core;

namespace Veridium.Animation{
    public class Anim_GlowIndiumBonds : AnimationBase
    {

        public StructureBuilder structureBuilder;
        private int currentStep = -1;
        public List<ListWrapper<Vector3>> steps;

        [System.Serializable]
        public class ListWrapper<T>
        {
            public List<T> list;
            public bool unglow;
            public Color glowColor;
        }

        public Anim_GlowIndiumBonds(){
            duration = 4f;
        }
        
        public override void Play()
        {
            base.Play();
            currentStep = -1;
        }

        public override void Pause()
        {
            base.Pause();
        }

        protected override void ResetChild()
        {
            base.ResetChild();
            currentStep = -1;
        }

        protected override void UpdateAnim()
        {
            base.UpdateAnim();
            // print("inside update anim");
            int step = (int) Mathf.Floor(elapsedTimePercent * steps.Count);
            
            if(step != currentStep){
                currentStep = step;
                // print("inside steps");
                foreach (Vector3 pos in steps[currentStep].list)
                {
                    Bond bond = structureBuilder.GetBondAtCoordinate(pos, structureBuilder.cellType);

                    if(!steps[currentStep].unglow){
                            Anim_Glow anim = bond.cylinderChild.AddComponent<Anim_Glow>() as Anim_Glow;
                            anim.easingType = EasingType.Exponential;
                            anim.selfDestruct = true;
                            anim.emissionColor = steps[currentStep].glowColor;
                            anim.fadeTime = 1f;
                            anim.Play();
                        } else {
                            Anim_Glow anim = bond.cylinderChild.GetComponent<Anim_Glow>();
                            if(anim != null) anim.Pause();
                        }
                }
            }
        }
    }
}
