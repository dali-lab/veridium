using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;
using Veridium.ElementStructures.Interaction;
using Veridium.Interaction;

namespace Veridium.Modules.ElementStructures {
    public class Anim_StopInteractiveStretchCompress : AnimationBase
    {
        ///<summary>
        /// Stop Interactive Stretch Compress Animation
        ///</summary>
        ///
        
        public StructureBase structureBase;

        // Called when animation is started
        public override void Play()
        {
            base.Play();
            Destroy(structureBase.GetComponentInChildren<StretchCompressGrabModifier>().gameObject);
            structureBase.GetComponentInChildren<OverridableGrabTransformer>().grabModifiers = new GrabModifier[0];

            structureBase.structureController.Lock();
        }

        // Called when animation ends
        public override void End()
        {
            base.End();
        }

        // Called when animation is paused
        public override void Pause()
        {
            base.Pause();
        }

        // Called when animation restarts
        protected override void ResetChild()
        {
            base.ResetChild();
        }

        // Called every frame while animation is playing
        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }

    }
}
