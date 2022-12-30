using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Veridium_Core;

namespace Veridium_Animation
{

    [System.Serializable]
    public class Anim_PlayOnAtoms : AnimationBase
    {

        public AnimationScript animationType;
        [HideInInspector] public AnimationScript currentAnimation;
        [SerializeReference] public AnimationBase animScript;
        public bool allAtoms;
        public List<ListWrapper<Vector3>> steps;
        public StructureBuilder structureBuilder;
        private int currentStep = -1;

        [System.Serializable]
        public class ListWrapper<T>
        {
            public List<T> list;
        }

        // Called when animation is started
        public override void Play()
        {
            base.Play();
            currentStep = -1;

            if (allAtoms){
                foreach (Atom atom in structureBuilder.crystal.atoms.Values)
                {
                    AnimPlayer animPlayer = atom.drawnObject.AddComponent<AnimPlayer>() as AnimPlayer;
                    animPlayer.animScript = animScript.Clone();
                    animPlayer.animScript.OnValidate(animPlayer);
                    animPlayer.animScript.selfDestruct = true;
                    animPlayer.animScript.Play();
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
            currentStep = -1;
        }

        // Called every frame while animation is playing
        protected override void UpdateAnim()
        {
            base.UpdateAnim();

            if(!allAtoms){
                int step = (int) Mathf.Floor(elapsedTimePercent * steps.Count);

                if(step != currentStep){
                    currentStep = step;

                    foreach (Vector3 pos in steps[currentStep].list)
                    {
                        Atom atom = structureBuilder.GetAtomAtCoordinate(pos);
                        if(atom.drawnObject != null){

                            AnimPlayer animPlayer = atom.drawnObject.AddComponent<AnimPlayer>() as AnimPlayer;
                            animPlayer.animScript = animScript.Clone();
                            animPlayer.animScript.OnValidate(animPlayer);
                            animPlayer.animScript.selfDestruct = true;
                            animPlayer.animScript.Play();

                        }
                    }
                }
            }
        }

        public override void OnValidate(AnimationManager parent){

            base.OnValidate(parent);

            if(currentAnimation != animationType){
                if(animationType != AnimationScript.Anim_PlayOnAtoms){
                    animScript = AnimPlayer.CreateAnimation(animationType);
                    animScript.OnValidate(parent);
                    currentAnimation = animationType;
                } else {
                    animScript = null;
                }
            }
        }
    }
}