using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Interaction;
using Veridium_Core;

namespace Veridium_Animation{
    public class Anim_ViewMode : AnimationBase
    {

        public bool updateCrystalState = true;
        public CrystalState crystalState;
        public bool updateViewMode = true;
        public CrystalView crystalView;

        public StructureBase structureBase;
        
        public override void Play(){
            base.Play();

            if(updateCrystalState){
                switch (crystalState){
                    case CrystalState.SINGLECELL:
                        structureBase.SingleCellView();
                        break;
                    case CrystalState.MULTICELL:
                        structureBase.MultiCellView();
                        break;
                    case CrystalState.INFINITE:
                        structureBase.InfiniteView();
                        break;
                }
            }

            if(updateViewMode){
                switch (crystalView){
                    case CrystalView.BallAndStick:
                        structureBase.BallAndStickView();
                        break;
                    case CrystalView.ClosePacked:
                        structureBase.ClosePackedView();
                        break;
                }
            }
        }

        public override void Pause(){
            base.Pause();
        }

        protected override void ResetChild()
        {
            base.ResetChild();
        }

        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }
    }
}
