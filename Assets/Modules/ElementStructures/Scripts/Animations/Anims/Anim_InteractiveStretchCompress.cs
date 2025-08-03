using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;
using Veridium.ElementStructures.Interaction;
using Veridium.Interaction;

namespace Veridium.Modules.ElementStructures {
    public class Anim_InteractiveStretchCompress : AnimationBase
    {
        ///<summary>
        /// Enable interactive stretching/compression of the structure
        /// Spawns handles to enable the interaction
        ///</summary>
        
        public StructureBase structureBase;

        public Vector3 stretchCompressDirection = Vector3.up;

        public GameObject handlePrefab;

        public float[] snapPoints;

        
        public float maxStretch = 2.0f;
        public float minStretch = 0.5f;

        public float startStretchFactor = 1.0f;

        // Called when animation is started
        public override void Play()
        {
            base.Play();

            GameObject grabModifierGO = new GameObject("StretchCompressGrabModifier");
            grabModifierGO.transform.parent = structureBase.structureBuilder.transform;

            StretchCompressGrabModifier grabModifier = grabModifierGO.AddComponent<StretchCompressGrabModifier>();
            grabModifier.structureBase = structureBase;
            grabModifier.compressDirection = stretchCompressDirection;
            grabModifier.snapPoints = snapPoints;
            grabModifier.maxStretch = maxStretch;
            grabModifier.minStretch = minStretch;

            grabModifier.lastStretchFactor = startStretchFactor;

            OverridableGrabTransformer grabTransformer = structureBase.structureController.GetComponent<OverridableGrabTransformer>();
            grabTransformer.grabModifiers = new GrabModifier[] { grabModifier };

            structureBase.structureController.Unlock();
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
