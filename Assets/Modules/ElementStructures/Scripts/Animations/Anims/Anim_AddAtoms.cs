using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Core;

namespace Veridium.Animation{
    public class Anim_AddAtoms : AnimationBase
    {

        ///<summary>
        /// Draws existing atoms at all of the existing coordinates
        /// If more than one step is filled in, the steps will be evenly 
        /// spread across the duration of the animation.
        ///</summary>

        public List<ListWrapper<Vector3>> steps;        // A list of steps. Each step contains a list of coordinates of atoms
        public StructureBuilder structureBuilder;       // The structureBuilder of the atoms to add
        private int currentStep = -1;                   // Current step of the animation. Steps will progress for the length of the steps list

        // A list wrapper class that allows nested lists to be edited in the inspector
        [System.Serializable]
        public class ListWrapper<T>
        {
            public List<T> list;
        }

        // Constructor
        public Anim_AddAtoms(){
            duration = 2f;
        }
        
        // Called when started
        public override void Play()
        {
            base.Play();
            currentStep = -1;
        }

        // Called when restarted 
        protected override void ResetChild()
        {
            base.ResetChild();
            currentStep = -1;
        }

        // Called when paused or ended
        public override void Pause()
        {
            base.Pause();
        }
        
        // Called each frame the animation is playing
        protected override void UpdateAnim()
        {
            base.UpdateAnim();

            // Find the current step of the sequence
            int step = (int) Mathf.Floor(elapsedTimePercent * steps.Count);

            // Spawn new atoms when the step increases
            if(step != currentStep){
                currentStep = step;

                foreach (Vector3 pos in steps[currentStep].list)
                {
                    // Draw the atom at the coordinates
                    Atom atom = structureBuilder.GetAtomAtCoordinate(pos, structureBuilder.cellType);
                    atom.builder = structureBuilder.gameObject;
                    atom.Draw();

                    // Play a fade in animation 
                    Anim_Fade anim = atom.drawnObject.AddComponent<Anim_Fade>() as Anim_Fade;
                    anim.easingType = EasingType.Exponential;
                    anim.Play();
                    anim.selfDestruct = true;
                }
            }
        }
    }
}
