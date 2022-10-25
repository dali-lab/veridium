using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium_Interaction;
using System.Linq;

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

        public GameObject pointer;
        public GameObject submitButton;
        public GameObject backdrop;

        private float FADE_DURATION = 0.5f;

        //  listener added to selector tool
        public void CollisionWithAtom(GameObject atom)
        {
            if(atom.GetComponent<Anim_GlowPulse>() != null) Destroy(atom.GetComponent<Anim_GlowPulse>());

            if(answer.Contains(atom)) answer.Remove(atom);
            else
            {
                answer.Add(atom);
                Anim_Glow anim = atom.AddComponent<Anim_Glow>() as Anim_Glow;
                anim.easingType = EasingType.Exponential;
                anim.selfDestruct = true;
                anim.emissionColor = glowColor;
                anim.fadeTime = 0.5f;
                anim.Play();
                Debug.Log("Here");
            }

        }

        public void OnAnswerSubmit()
        {
            //if(answer == solutionSet)
            if(answer.SetEquals(solutionSet))
            {
                pointer.SetActive(false);
                CompleteAction();
                //*** StartCoroutine(FadeOutBackdrop());
                //pointer.GetComponentInChildren<PointerSelector>().onAtomSelect.RemoveListener(CollisionWithAtom);
                //submitButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.RemoveListener(OnAnswerSubmit);

            }
            else
            {
                foreach(GameObject atomGameObj in answer)
                {
                    if(atomGameObj.GetComponent<Anim_GlowPulse>() != null) Destroy(atomGameObj.GetComponent<Anim_GlowPulse>());
                }
                answer.Clear();
            }
        }


        public override void Play()
        {
            base.Play();

            // gets the associated gameobjects for the atoms in solution
            foreach(Vector3 vec in solution)
            {
                solutionSet.Add(structureBuilder.GetAtomAtCoordinate(vec).drawnObject);
            }

            pointer.SetActive(true);
            pointer.GetComponentInChildren<PointerSelector>().onAtomSelect.AddListener(CollisionWithAtom);

            submitButton.SetActive(true);
            submitButton.GetComponentInChildren<SegmentPlay>().onInteractionStart.AddListener(OnAnswerSubmit);

            //*** StartCoroutine(FadeBackdrop());

        }

        private IEnumerator FadeBackdrop()
        {
            RenderSettings.fog = true;
            backdrop.SetActive(true);
            // to do
            Material mat = backdrop.GetComponent<Renderer>().material;
            Color initialColor = mat.color;
            Color finalColor = new Color(initialColor.r, initialColor.g, initialColor.b, 1f);

            float elapsedTime = 0f;

            while (elapsedTime < FADE_DURATION)
            {
                elapsedTime += Time.deltaTime;
                mat.color = Color.Lerp(initialColor, finalColor, elapsedTime / FADE_DURATION);
                RenderSettings.fogDensity = Mathf.Lerp(0f, 0.18f, elapsedTime / FADE_DURATION); // fades fog from 0 to 0.18 (use exponential squared)
                yield return null;
            }
        }

        private IEnumerator FadeOutBackdrop()
        {
            // to do
            Material mat = backdrop.GetComponent<Renderer>().material;
            Color initialColor = mat.color;
            Color finalColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

            float elapsedTime = 0f;

            while (elapsedTime < FADE_DURATION)
            {
                elapsedTime += Time.deltaTime;
                mat.color = Color.Lerp(initialColor, finalColor, elapsedTime / FADE_DURATION);
                RenderSettings.fogDensity = Mathf.Lerp(0.18f, 0f, elapsedTime / FADE_DURATION); // fades fog from 0 to 0.18 (use exponential squared)
                yield return null;
            }

            RenderSettings.fog = false;
            backdrop.SetActive(false);
            submitButton.SetActive(false);
        }

        protected override void UpdateAnim(){

            base.UpdateAnim();

        }
    }
}
