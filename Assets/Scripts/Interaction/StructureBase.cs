using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Veridium_Core;
using Veridium_Animation;

/// <summary>
/// StructureBase controls the main structure on the podium 
/// Supports interaction of user and structure
/// </summary>

namespace Veridium_Interaction{
    public class StructureBase : MonoBehaviour
    {
        private bool grabbed;                           // Whether the structure has been grabbed by the user
        public float respawnDistance = 1;               // Distance from the podium at which the structure should teleport home
        public StructureBuilder structureBuilder;       // Reference to the structureBuilder which implements the construction of the structure
        public float sideLength = 1f;                 // Standard side length of a unit cell
        public float sphereRadius = 0.075f;             // Radius of the spheres
        public int planeIndex = 0;                      // Index of the currently visualized plane
        public AnimScriptType spawnAnimation;             // The animation that spawns this structure in
        public bool locked {get; private set;}          // Locked means no interaction
        public ElementLoader elementLoader;             // Element loader associated with this structure
        public StructureController structureController; // Structure controller associated with this 
        public CrystalState currentState, desiredState; // Current and desired state of the crystal

        void Awake(){
            structureController.structureBase = this;
        }

        void OnValidate(){
            spawnAnimation.OnValidate(null);
        }

        public StructureBase(){
            // The default animation for spawning the structure should be a spin up with quadratic easing.
            spawnAnimation = new AnimScriptType();
            spawnAnimation.animationType = AnimationScript.Spin_Up;
            spawnAnimation.OnValidate(null);
            SpinUp defaultAnim = spawnAnimation.animScript as SpinUp;
            defaultAnim.duration = 1.5f;
            defaultAnim.easingType = EasingType.Quadratic;
        }

        void Update(){

            // Fade out spheres near the camera
            if (currentState == CrystalState.INFINITE){

                if(!structureController.structureSelected){
                    structureController.gameObject.transform.position = (structureController.hand1.transform.position + structureController.hand2.transform.position) / 2;
                    structureController.gameObject.transform.rotation = Quaternion.identity;
                    structureController.gameObject.transform.localScale = Vector3.one * 100f;
                } else if (!structureController.scaleGrabberSelected){
                    structureController.gameObject.transform.localScale = Vector3.one * 2f;
                }
            }

        }

        // Prompts the structureBuilder to construct a structure base on an element
        public void ElementAdded(PTElement element){

            int atomicNumber = Coloration.GetNumberByName(element.name);

            structureBuilder.BuildCell(element.type, element.variation, CrystalState.SINGLECELL, sideLength, sphereRadius, atomicNumber);

            planeIndex = 0;

            if (spawnAnimation != null) spawnAnimation.animScript.PlayFromStart();

            currentState = CrystalState.SINGLECELL;

            desiredState = CrystalState.SINGLECELL;
        }

        // Prompts the structureBuilder to destroy the cell
        public void ElementRemoved(){

            structureBuilder.DestroyCell();

        }

        public void SetView(CrystalState state){
            switch (state){
                case CrystalState.INFINITE:
                    InfiniteView();
                break;
                case CrystalState.MULTICELL:
                    MultiCellView();
                break;
                case CrystalState.SINGLECELL:
                    SingleCellView();
                break;
            }
        }

        // Enables infinite view for the crystal lattice
        public void InfiniteView(){

            currentState = CrystalState.INFINITE;

            structureBuilder.Redraw(CrystalState.INFINITE);

            structureBuilder.transform.parent = gameObject.transform.Find("InfiniteViewLocation");

            FindObjectsOfType<Camera>()[0].cullingMask = 1 << LayerMask.NameToLayer("Atoms") | 1 << LayerMask.NameToLayer("InfiniteOnly");

        }

        // Enables multi-cell view for the crystal
        public void MultiCellView(){

            if(currentState == CrystalState.INFINITE){
                structureBuilder.transform.parent = structureController.gameObject.transform;
                structureBuilder.transform.localPosition = Vector3.zero;
                structureBuilder.transform.localRotation = Quaternion.identity;
                structureBuilder.transform.localScale = Vector3.one;
                FindObjectsOfType<Camera>()[0].cullingMask = ~0 ^ 1 << LayerMask.NameToLayer("InfiniteOnly");
            }

            currentState = CrystalState.MULTICELL;

            structureBuilder.Redraw(CrystalState.MULTICELL);

            AnimPlayer animPlayer = structureBuilder.gameObject.AddComponent<AnimPlayer>() as AnimPlayer;
            animPlayer.animationType = AnimationScript.Move_To;
            animPlayer.OnValidate();

            MoveTo anim = animPlayer.animScript as MoveTo;

            anim.updateLocation = false;
            anim.updateRotation = false;
            anim.updateScale = true;

            anim.duration = 1f;
            anim.easingType = EasingType.Elastic;
            structureBuilder.gameObject.transform.localScale = new Vector3(.8f,.8f,.8f);
            anim.selfDestruct = true;
            anim.easeOutOnly = true;

            anim.Play();
            
        }

        // Enables single cell view for the crystal
        public void SingleCellView(){

            if(currentState == CrystalState.INFINITE){
                structureBuilder.transform.parent = structureController.gameObject.transform;
                structureBuilder.transform.localPosition = Vector3.zero;
                structureBuilder.transform.localRotation = Quaternion.identity;
                structureBuilder.transform.localScale = Vector3.one;
                FindObjectsOfType<Camera>()[0].cullingMask = ~0 ^ 1 << LayerMask.NameToLayer("InfiniteOnly");
            }

            currentState = CrystalState.SINGLECELL;

            structureBuilder.Redraw(CrystalState.SINGLECELL);

            AnimPlayer animPlayer = structureBuilder.gameObject.AddComponent<AnimPlayer>() as AnimPlayer;
            animPlayer.animationType = AnimationScript.Move_To;
            animPlayer.OnValidate();

            MoveTo anim = animPlayer.animScript as MoveTo;

            anim.updateLocation = false;
            anim.updateRotation = false;
            anim.updateScale = true;

            anim.duration = 1f;
            anim.easingType = EasingType.Elastic;
            structureBuilder.gameObject.transform.localScale = new Vector3(1.25f,1.25f,1.25f);
            anim.selfDestruct = true;
            anim.easeOutOnly = true;

            anim.Play();

        }

        public void ClosePackedView(){

            foreach(Bond bond in structureBuilder.crystal.bonds.Values){
                Destroy(bond.drawnObject);
            }

            foreach (Atom atom in structureBuilder.crystal.atoms.Values)
            {
                if(atom.drawnObject != null){
                    AnimPlayer animPlayer = structureBuilder.gameObject.AddComponent<AnimPlayer>() as AnimPlayer;
                    animPlayer.animationType = AnimationScript.Move_To;
                    animPlayer.OnValidate();

                    MoveTo anim = animPlayer.animScript as MoveTo;

                    anim.updateLocation = false;
                    anim.updateRotation = false;
                    anim.easingType = EasingType.Elastic;
                    anim.easeOutOnly = true;
                    anim.duration = 3f;
                    anim.selfDestruct = true;
                    anim.endScale = new Vector3(.32f,.32f,.32f);
                    anim.Play();
                }
            }

        }

        public void BallAndStickView(){



        }

        public void Lock(){
            locked = true;

            elementLoader.Lock();

            if (desiredState != currentState) SetView(desiredState);
        }

        public void Unlock(){
            locked = false;

            elementLoader.Unlock();

            desiredState = currentState;
        }
    }
}