using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium_Interaction;

namespace Veridium_Animation{
    public class Await_QuizComplete : AwaitUserBase
    {

        public StructureBuilder structureBuilder;
        private Vector3[] solution = 
            new Vector3[] {
            new Vector3(-1,-1,1),
            new Vector3(-1,1,-1),
            new Vector3(1,-1,-1),
            new Vector3(-1,0,0),
            new Vector3(0,-1,0),
            new Vector3(0,0,-1)
        };

        private HashSet<GameObject> solutionSet = new HashSet<GameObject>();
        private HashSet<GameObject> answer = new HashSet<GameObject>();

        public Color glowColor;

        //  listener added to selector tool
        public void CollisionWithAtom(GameObject atom)
        {
            answer.Add(atom);
            Anim_Glow anim = atom.AddComponent<Anim_Glow>() as Anim_Glow;
            anim.easingType = EasingType.Exponential;
            anim.selfDestruct = true;
            anim.emissionColor = glowColor;
            anim.fadeTime = 0f;
        }

        public void OnAnswerSubmit()
        {
            if(answer == solutionSet) CompleteAction();
        }


        public override void Play()
        {
            base.Play();

            foreach(Vector3 vec in solution)
            {
                solutionSet.Add(structureBuilder.GetAtomAtCoordinate(vec).drawnObject);
            }

        }

        protected override void UpdateAnim(){

            base.UpdateAnim();

        }
    }
}
