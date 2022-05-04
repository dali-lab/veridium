using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Core;

namespace Veridium_Animation{
    public class Anim_HightlightLayers : AnimationBase
    {

        public StructureBuilder structureBuilder;
        private int currentStep = -1;
        /*
        private Vector3[][] steps = new Vector3[][]{
            new Vector3[] {
                new Vector3(-1,-1,-1)
            },
            new Vector3[] {
                new Vector3(-1,-1,1),
                new Vector3(-1,1,-1),
                new Vector3(1,-1,-1),
                new Vector3(-1,0,0),
                new Vector3(0,-1,0),
                new Vector3(0,0,-1)
            },
            new Vector3[] {
                new Vector3(-1,1,1),
                new Vector3(1,-1,1),
                new Vector3(1,1,-1),
                new Vector3(0,0,1),
                new Vector3(0,1,0),
                new Vector3(1,0,0)
            },
            new Vector3[] {
                new Vector3(1,1,1)
            }
        };
        */
        private Vector3[][] steps = new Vector3[][]{
            new Vector3[] {
                new Vector3(0,-1, 1),
                new Vector3(-1,-1,0.5f),
                new Vector3(0,1, 1),
                new Vector3(-1,1,0.5f)
                
            },
            new Vector3[] {
                new Vector3(-1,-1,0),
                new Vector3(0,-1,0),
                new Vector3(1,-1,0),
                new Vector3(-1,1,0),
                new Vector3(0,1,0),
                new Vector3(1,1,0)
            },
            new Vector3[] {
                new Vector3(-0.5f,-1,-1),
                new Vector3(0.5f,-1,-1),
                new Vector3(-0.5f,1,-1),
                new Vector3(0.5f,1,-1)
            }
        };

        public Anim_HightlightLayers(){
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

            int step = (int) Mathf.Floor(elapsedTimePercent * steps.Length);
            
            if(step != currentStep){
                currentStep = step;

                foreach (Vector3 pos in steps[currentStep])
                {
                    Atom atom = structureBuilder.GetAtomAtCoordinate(pos);

                    if(atom != null){
                        if(atom.drawnObject.GetComponent<Anim_GlowPulse>() != null) Destroy(atom.drawnObject.GetComponent<Anim_GlowPulse>());
                        Anim_GlowPulse anim = atom.drawnObject.AddComponent<Anim_GlowPulse>() as Anim_GlowPulse;
                        anim.emissionColor = new Color(1,1,0);
                        anim.blinksPerSecond = steps.Length / (2 * duration);
                        anim.maxIntensity = 0.4f;
                        anim.selfDestruct = true;
                        anim.Play();
                        anim.Pause();
                    }
                }
            }
        }
    }
}
