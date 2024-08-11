using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Core;

namespace Veridium_Animation{
    public class Anim_GlowAtomsConstant : AnimationBase
    {
        public List<ListWrapper<Vector3>> steps;
        public StructureBuilder structureBuilder;
        private int currentStep = -1;
        public int TimeDelay;

        [System.Serializable]
        public class ListWrapper<T>
        {
            public List<T> list;
            public bool unglow;
            public Color glowColor;
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

        IEnumerator waiter()
        {
            yield return new WaitForSeconds(TimeDelay);
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
/*                    Debug.Log("Position " + pos + " has atom object of: " + atom.drawnObject);*/
                    if(atom.drawnObject != null){

                        if(!steps[currentStep].unglow){
                            Anim_Glow anim = atom.drawnObject.AddComponent<Anim_Glow>() as Anim_Glow;
                            anim.easingType = EasingType.Exponential;
                            anim.selfDestruct = true;
                            anim.emissionColor = steps[currentStep].glowColor;
                            anim.fadeTime = 1f;
                            anim.Play();
                        } else {
                            Anim_Glow anim = atom.drawnObject.GetComponent<Anim_Glow>();
                            if(anim != null) anim.Pause();
                        }
                    }
                }
            }
        }
    }
}
