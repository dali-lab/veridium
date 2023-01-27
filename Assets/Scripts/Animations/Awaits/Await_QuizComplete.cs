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

        // 6 atoms of the same layer
        public Vector3[] solution = 
            new Vector3[] {
            new Vector3(-1,-1,1),
            new Vector3(-1,1,-1),
            new Vector3(1,-1,-1),
            new Vector3(-1,0,0),
            new Vector3(0,-1,0),
            new Vector3(0,0,-1)
        };

        // // 14 atoms that form the cubic face centered unit cell
        // private Vector3[] solution2 =
        //     new Vector3[] {
        //     new Vector3(-1, 0, 0), // left
        //     new Vector3(1, 0, 0), // right
        //     new Vector3(0, 0, -1), // front
        //     new Vector3(0, 0, 1), // back
        //     new Vector3(0, -1, 0), // bottom
        //     new Vector3(0, 1, 0), // top
        //     new Vector3(-1, 1, -1), // front top left
        //     new Vector3(1, 1, -1), // front top right
        //     new Vector3(-1, 1, 1), // back top left
        //     new Vector3(1, 1, 1), // back top right
        //     new Vector3(-1, -1, -1), // front bottom left
        //     new Vector3(1, -1, -1), // front bottom right
        //     new Vector3(-1, -1, 1), // back bottom left
        //     new Vector3(1, -1, 1), // back bottom right
        // };

        private HashSet<GameObject> solutionSet = new HashSet<GameObject>();
        private HashSet<GameObject> answer = new HashSet<GameObject>();

        private Color initialColor;
        public Color glowColor;

        public GameObject feedbackManagerGO;
        public GameObject pointer;
        public GameObject submitButton;
        public GameObject backdrop;

        // private SegmentPlay segmentPlay;
        private PointerSelector pointerSelector;
        private AudioFeedbackManager feedbackManager;

        private float FADE_DURATION = 0.5f;

        //  listener added to selector tool
        public void CollisionWithAtom(GameObject atom)
        {
            if(atom.GetComponent<Anim_GlowPulse>() != null) Destroy(atom.GetComponent<Anim_GlowPulse>());
            
            atom.TryGetComponent<Anim_Glow>(out Anim_Glow anim);

            if(answer.Contains(atom))
            {
                answer.Remove(atom);
                anim = atom.AddComponent<Anim_Glow>() as Anim_Glow;
                anim.easingType = EasingType.Exponential;
                anim.fadeTime = 0.5f;
                anim.selfDestruct = true;
                anim.emissionColor = initialColor;
                anim.Play();
                Debug.Log("Unhighlighted atom");
            }
            else
            {
                answer.Add(atom);
                Debug.Log("Selected atom at position: " + atom.transform.position);
                anim = atom.AddComponent<Anim_Glow>() as Anim_Glow;
                anim.easingType = EasingType.Exponential;
                anim.fadeTime = 0.5f;
                initialColor = atom.GetComponent<Renderer>().materials[0].GetColor("_EmissionColor");
                anim.selfDestruct = true;
                anim.emissionColor = glowColor;
                anim.Play();
                Debug.Log("Highlighted atom");
            }

        }

        public void OnAnswerSubmit()
        {
            Debug.Log("answer count:" + answer.Count);
            Debug.Log("solution count: " + solutionSet.Count);

            // Correct answer
            if(answer.SetEquals(solutionSet))
            {
                VeridiumButton.Instance.Disable();
                foreach (GameObject atom in answer)
                {
                    atom.TryGetComponent<Anim_Glow>(out Anim_Glow anim);

                    if (anim != null)
                    {
                        anim.emissionColor = initialColor;
                        anim.selfDestruct = true;
                        anim.Play();

                        // Making sure animation gets destroyed
                        if (anim != null) Destroy(anim);
                    }
                }
                
                pointerSelector.onAtomSelect.RemoveListener(CollisionWithAtom);
                // segmentPlay.onInteractionStart.RemoveListener(OnAnswerSubmit);

                // pointer.SetActive(false);
                feedbackManager.PlayCorrectAudio();
                StartCoroutine(WaitToCompleteAction());
                //*** StartCoroutine(FadeOutBackdrop());
                

            }
            else // Wrong answer
            {
                foreach(GameObject atom in answer)
                {
                    atom.TryGetComponent<Anim_Glow>(out Anim_Glow anim);
                    if (anim != null)
                    {
                        anim.emissionColor = initialColor;
                        anim.Play();
                    }

                    // if(atomGameObj.GetComponent<Anim_GlowPulse>() != null) Destroy(atomGameObj.GetComponent<Anim_GlowPulse>());
                }
                answer.Clear();
                Debug.Log("WRONG WRONG WRONG WRONG WRONG!!!");
                feedbackManager.PlayWrongAudio();
                VeridiumButton.Instance.Enable();
                // segmentPlay.sphereAnim.SetBool("respawnSphere", true);
            }
        }

        IEnumerator WaitToCompleteAction()
        {
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => feedbackManager.finishedAudio == true);
            yield return new WaitForSeconds(0.2f);
            CompleteAction();
        }


        public override void Play()
        {
            base.Play();

            // gets the associated gameobjects for the atoms in solution
            FillSolutionSet(solutionSet, solution);

            feedbackManagerGO.SetActive(true);
            feedbackManager = feedbackManagerGO.GetComponent<AudioFeedbackManager>();
            pointer.SetActive(true);
            pointerSelector = pointer.GetComponentInChildren<PointerSelector>();
            pointerSelector.onAtomSelect.AddListener(CollisionWithAtom);
            // StartCoroutine(EnableSubmitButton());
            VeridiumButton.Instance.SwitchType(VeridiumButton.ButtonType.SUBMIT);
            VeridiumButton.Instance.Enable();
            VeridiumButton.Instance.onInteracted.AddListener(OnAnswerSubmit);
            Debug.Log("added listeners");

            //*** StartCoroutine(FadeBackdrop());

        }

        // IEnumerator EnableSubmitButton()
        // {
        //     yield return new WaitForSeconds(1f);
        //     submitButton.SetActive(true);
        //     segmentPlay = submitButton.GetComponentInChildren<SegmentPlay>();
        //     Debug.Log("segment play: " + segmentPlay);
        //     segmentPlay.onInteractionStart.AddListener(OnAnswerSubmit);
        //     segmentPlay.sphereAnim.Rebind();
        //     segmentPlay.sphereAnim.Update(0f);
        // }

        // Fill the solution set with the right solutions
        public void FillSolutionSet(HashSet<GameObject> solutionSet, Vector3[] solution)
        {
            solutionSet.Clear();

            // Debug.Log("ATOMS IN CRYSTAL STRUCTURE: " + structureBuilder.crystal.atoms.Count);

            foreach (Vector3 vec in solution)
            {
                solutionSet.Add(structureBuilder.GetAtomAtCoordinate(vec).drawnObject);
                Debug.Log("added vector: " + vec);
            }
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
