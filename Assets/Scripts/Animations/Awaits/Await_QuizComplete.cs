using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium.Interaction;
using System.Linq;
using Veridium.Core;
using System;

namespace Veridium.Animation
{
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

        public bool useSolutionSet = false;
        public bool isMGQ2 = false;
        private XRBaseInteractor interactor;

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
        public int atomsOnLayer;

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
                // Debug.Log("Selected atom at position: " + atom.transform.position);
                anim = atom.AddComponent<Anim_Glow>() as Anim_Glow;                                                                 
                anim.easingType = EasingType.Exponential;
                anim.fadeTime = 0.5f;
                initialColor = atom.GetComponent<Renderer>().materials[0].GetColor("_EmissionColor");
                anim.selfDestruct = true;
                anim.emissionColor = glowColor;
                anim.Play();
                // Debug.Log("Highlighted atom");
            }

        }

        // Confirm answer set lies on the same plane/layer
        private bool onSamePlane(HashSet<GameObject> answer)
        {
            // Planar Equation: ax + by + cz = d
            // where the normal vector N = (a, b, c)
            Vector3 N = Vector3.zero; // Normal vector
            int d = 0;
            int i = 0;
            Vector3[] ABC = new Vector3[3];
            foreach (GameObject atom in answer)
            {  
                Vector3 atomPos = Vector3.zero;
                foreach (Atom a in structureBuilder.crystal.atoms.Values)
                {
                    if (a.drawnObject == atom) {
                        atomPos = structureBuilder.GetCoordinateAtAtom(a);
                        break;
                    }
                }
                // Compute planar equation
                if (i < 3)
                {
                    ABC[i] = atomPos;
                }
                else // Check the rest of the atoms lie on the plane
                {
                    if (i == 3)
                    {
                        Vector3 AB = ABC[1] - ABC[0];
                        Vector3 AC = ABC[2] - ABC[0];
                        N = new Vector3((AB.y * AC.z - AB.z * AC.y), (AB.z * AC.x - AB.x * AC.z), (AB.x * AC.y - AB.y * AC.x));
                    }
                    Debug.Log("Plane" + Math.Abs(N.x * (atomPos.x - ABC[2].x) + N.y * (atomPos.y - ABC[2].y) + N.z * (atomPos.z - ABC[2].z)));
                    if (Math.Abs(N.x * (atomPos.x - ABC[2].x) + N.y * (atomPos.y - ABC[2].y) + N.z * (atomPos.z - ABC[2].z)) > 0.05)
                    {
                        return false;
                    }
                }
                i++;
            }
            return true;
        }
        
        public void OnAnswerSubmit()
        {
            Debug.Log("answer count:" + answer.Count);
            Debug.Log("solution count: " + solutionSet.Count);

            // First, force user to drop whatever they're holding
            interactor = structureBuilder.GetComponentInParent<StructureController>().selectingInteractor;
            // print("INTERACTOR: " + interactor);
            if (interactor != null) interactor.allowSelect = false;
            // sc.hand1.GetComponent<HandDistanceGrabber>().allowSelect = false;
            // sc.hand2.GetComponent<HandDistanceGrabber>().allowSelect = false;
        
            if (useSolutionSet)
            {
                if (answer.SetEquals(solutionSet))
                {
                    RightAnswerCallback();
                }
                else WrongAnswerCallback(); 

                return;
            }

            if (isMGQ2)
            {
                float correctDistance = 0.08429995f;
                if (answer.Count != 2) // Make sure only 2 atoms are selected
                {
                    WrongAnswerCallback();
                    return;
                }
                List<GameObject> the2atoms = new List<GameObject>();
                foreach (GameObject atomGO in answer)
                {
                    the2atoms.Add(atomGO);
                }
                float localDist = Vector3.Distance(the2atoms[0].transform.localPosition, the2atoms[1].transform.localPosition);
                // float worldDist = Vector3.Distance(the2atoms[0].transform.position, the2atoms[1].transform.position);

                // print("LOCAL DIST: " + localDist);
                // print("WORLD DIST: " + worldDist);
                if (the2atoms[0].transform.position.y != the2atoms[1].transform.position.y && Mathf.Abs(localDist - correctDistance) <= 0.001)
                {
                    RightAnswerCallback();
                }
                else 
                {
                    WrongAnswerCallback();
                }
                return;
            }

            // Correct answer: at least 6 atoms on the same layer (plane)
            if(onSamePlane(answer) && answer.Count >= atomsOnLayer   /*answer.SetEquals(solutionSet)*/)
            {
                RightAnswerCallback();
            }
            else // Wrong answer
            {
                WrongAnswerCallback();
            }
        }

        private void RightAnswerCallback()
        {
            if (interactor != null) interactor.allowSelect = true;
            // structureBuilder.GetComponentInParent<StructureController>().hand2.GetComponent<HandDistanceGrabber>().allowSelect = true;


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
                answer.Clear();
                // pointer.SetActive(false);
                feedbackManager.PlayCorrectAudio();
                StartCoroutine(WaitToCompleteAction());
                //*** StartCoroutine(FadeOutBackdrop());
        }

        private void WrongAnswerCallback()
        {
            if (interactor != null) interactor.allowSelect = true;
            // structureBuilder.GetComponentInParent<StructureController>().hand2.GetComponent<HandDistanceGrabber>().allowSelect = true;

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

        IEnumerator WaitToCompleteAction()
        {
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => feedbackManager.finishedAudio == true);
            yield return new WaitForSeconds(0.2f);
            CompleteAction();
        }


        public override void Play()
        {
            Debug.Log("CALLING AWAIT QUIZ COMPLETE");
            base.Play();
            Debug.Log("after base.play");
            // gets the associated gameobjects for the atoms in solution
            FillSolutionSet(solutionSet, solution);

            feedbackManagerGO.SetActive(true);
            feedbackManager = feedbackManagerGO.GetComponent<AudioFeedbackManager>();
            pointer.SetActive(true);
            Debug.Log("set pointer active");
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

            if (structureBuilder.cellType == CellType.HEX) return;

            // Debug.Log("ATOMS IN CRYSTAL STRUCTURE: " + structureBuilder.crystal.atoms.Count);

            foreach (Vector3 vec in solution)
            {
                solutionSet.Add(structureBuilder.GetAtomAtCoordinate(vec, structureBuilder.cellType).drawnObject);
                // Debug.Log("added vector: " + vec);
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

        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }
    }
}
