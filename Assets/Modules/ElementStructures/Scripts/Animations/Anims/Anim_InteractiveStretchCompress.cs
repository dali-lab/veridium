using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;

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
        
        // Called when animation is started
        public override void Play()
        {
            base.Play();

            Transform structureTransform = structureBase.structureBuilder.transform;

            GameObject handleParent = new GameObject("StretchCompressParent");
            handleParent.transform.parent = structureTransform;

            handleParent.transform.localRotation = Quaternion.LookRotation(stretchCompressDirection);
            

            foreach (Atom atom in structureBase.structureBuilder.crystal.atoms.Values) {
                if (atom.drawnObject) {
                    GameObject handleGO = Instantiate(handlePrefab, handleParent.transform);
                    handleGO.transform.position = atom.drawnObject.transform.position;

                    StructureCompressionHandle handle = handleGO.GetComponent<StructureCompressionHandle>();

                    handle.atomToTrack = atom;
                    handle.structureBase = structureBase;
                    handle.compressDirection = stretchCompressDirection;
                    handle.snapPoints = snapPoints;
                    handle.maxStretch = maxStretch;
                    handle.minStretch = minStretch;
                }
            }
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
