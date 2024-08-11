using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Core;

namespace Veridium_Animation{
    public class Anim_FadeAtoms : AnimationBase
    {

        public List<ListWrapper<Vector3>> steps;
        public StructureBuilder structureBuilder;
        private int currentStep = -1;

        [System.Serializable]
        public class ListWrapper<T>
        {
            public List<T> list;
            public bool fadeIn;
        }
        
        public override void Play()
        {
            base.Play();
            currentStep = -1;
        }

        protected override void ResetChild()
        {
            base.ResetChild();
            currentStep = -1;
        }

        public override void Pause()
        {
            base.Pause();
        }
        
        protected override void UpdateAnim()
        {
            base.UpdateAnim();

            int step = (int) Mathf.Floor(elapsedTimePercent * steps.Count);

            if(step != currentStep){
                currentStep = step;

                foreach (Vector3 pos in steps[currentStep].list)
                {
                    Atom atom = structureBuilder.GetAtomAtCoordinate(pos, structureBuilder.cellType);
                    if(atom.drawnObject != null){
                        Anim_Fade anim = atom.drawnObject.AddComponent<Anim_Fade>() as Anim_Fade;
                        anim.easingType = EasingType.Exponential;
                        if(!steps[currentStep].fadeIn){
                            anim.startingOpacity = 1f;
                            anim.endingOpacity = 0f;
                        } else {
                            anim.startingOpacity = 0f;
                            anim.endingOpacity = 1f;
                        }
                        anim.Play();
                        anim.selfDestruct = true;
                    }
                }
            }
        }
    }
}

