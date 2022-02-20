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
        public GameObject structure;                    // The structure on the podium
        private bool grabbed;                           // Whether the structure has been grabbed by the user
        public float respawnDistance = 1;               // Distance from the podium at which the structure should teleport home
        public StructureBuilder structureBuilder;       // Reference to the structureBuilder which implements the construction of the structure
        public float sideLength = 1f;                 // Standard side length of a unit cell
        public float sphereRadius = 0.075f;             // Radius of the spheres
        public int planeIndex = 0;                      // Index of the currently visualized plane
        public Anim_SpinUp spinUpAnimation;             // The animation that spawns this structure in
        public bool locked {get; private set;}          // Locked means no interaction
        public ElementLoader elementLoader;             // Element loader associated with this structure
        public StructureController structureController; // Structure controller associated with this 
        public CrystalState currentState;

        void Awake(){
            structureController.structureBase = this;
        }

        // Prompts the structureBuilder to construct a structure base on an element
        public void ElementAdded(PTElement element){

            int atomicNumber = Coloration.GetNumberByName(element.name);

            structureBuilder.BuildCell(element.type, element.variation, CrystalState.SINGLECELL, sideLength, sphereRadius, atomicNumber);

            planeIndex = 0;

            if (spinUpAnimation != null) spinUpAnimation.PlayFromStart();
        }

        // Prompts the structureBuilder to destroy the cell
        public void ElementRemoved(){

            structureBuilder.DestroyCell();

        }

        // Enables infinite view for the crystal lattice
        public void InfiniteView(){

            currentState = CrystalState.INFINITE;

            structureBuilder.Redraw(CrystalState.INFINITE);

            structureBuilder.transform.parent = gameObject.transform;

            if (structureBuilder.gameObject.GetComponent<Anim_MoveTo>() != null) Destroy(structureBuilder.gameObject.GetComponent<Anim_MoveTo>());
            Anim_MoveTo anim = structureBuilder.gameObject.AddComponent<Anim_MoveTo>() as Anim_MoveTo;

            anim.updateLocation = true;
            anim.updateRotation = true;
            anim.updateScale = true;
            anim.useTransform = true;
            anim.endTransform = gameObject.transform;

            anim.duration = .75f;
            anim.easingType = EasingType.Quadratic;
            anim.selfDestruct = true;

            anim.Play();

            FindObjectsOfType<Camera>()[0].cullingMask = 1 << LayerMask.NameToLayer("Atoms");

        }

        // Enables multi-cell view for the crystal
        public void MultiCellView(){

            if(currentState == CrystalState.INFINITE){
                structureBuilder.transform.parent = structureController.gameObject.transform;
                structureBuilder.transform.localPosition = Vector3.zero;
                structureBuilder.transform.localRotation = Quaternion.identity;
                structureBuilder.transform.localScale = Vector3.one;
                FindObjectsOfType<Camera>()[0].cullingMask = ~0;
            }

            currentState = CrystalState.MULTICELL;

            structureBuilder.Redraw(CrystalState.MULTICELL);

            if (structureBuilder.gameObject.GetComponent<Anim_MoveTo>() != null) Destroy(structureBuilder.gameObject.GetComponent<Anim_MoveTo>());
            Anim_MoveTo anim = structureBuilder.gameObject.AddComponent<Anim_MoveTo>() as Anim_MoveTo;

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
                FindObjectsOfType<Camera>()[0].cullingMask = ~0;
            }

            currentState = CrystalState.SINGLECELL;

            structureBuilder.Redraw(CrystalState.SINGLECELL);

            if (structureBuilder.gameObject.GetComponent<Anim_MoveTo>() != null) Destroy(structureBuilder.gameObject.GetComponent<Anim_MoveTo>());
            Anim_MoveTo anim = structureBuilder.gameObject.AddComponent<Anim_MoveTo>() as Anim_MoveTo;

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
                    Anim_MoveTo anim = atom.drawnObject.AddComponent<Anim_MoveTo>() as Anim_MoveTo;
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

        // Called by joystick switch, switches the plane index up or down
        public void Switch(bool right){

            planeIndex += right ? 1 : -1;
            if(planeIndex > structureBuilder.numPlanes - 1) planeIndex -= structureBuilder.numPlanes;
            if(planeIndex < 0) planeIndex += structureBuilder.numPlanes;

            structureBuilder.HighlightPlaneAtIndex(planeIndex);

        }

        public void Lock(){
            locked = true;

            elementLoader.Lock();
        }

        public void Unlock(){
            locked = false;

            elementLoader.Unlock();
        }
    }
}