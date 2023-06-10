using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Core;

namespace Veridium_Animation{
    public class Anim_ChangeIndium : AnimationBase
    {

        public StructureBuilder structureBuilder;
        private int currentStep = -1;
        public List<ListWrapper<Vector3>> steps;

        [System.Serializable]
        public class ListWrapper<T>
        {
            public List<T> atomList;
            public List<T> bondList;
            public bool isElongating;
            public bool isCompressing;
        }

        public Anim_ChangeIndium(){
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
                foreach (Vector3 pos in steps[currentStep].bondList)
                {
                    Bond bond = structureBuilder.GetBondAtCoordinate(pos, structureBuilder.cellType);

                    if(bond != null && bond.drawnObject != null){
                        Anim_MoveTo anim = bond.drawnObject.AddComponent<Anim_MoveTo>() as Anim_MoveTo;
                        anim.target = bond.drawnObject;
                        anim.endScale = new Vector3(1f, 1f, 0.8f);
                        anim.Play();
                        anim.Pause();
                    }
                    // } else {
                    //     Anim_Glow anim = bond.cylinderChild.GetComponent<Anim_Glow>();
                    //     if(anim != null) anim.Pause();
                    // }
                }
            }
        }
    }
}
