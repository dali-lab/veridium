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
        public Anim_SpinUp spinUpAnimation;             // The animation that spawns this structure in
        public bool locked {get; private set;}          // Locked means no interaction
        public ElementLoader elementLoader;             // Element loader associated with this structure
        public StructureController structureController; // Structure controller associated with this 
        public CrystalState currentState, desiredState;

        void Awake(){
            structureController.structureBase = this;
        }

        void Update(){

            // Fade out spheres near the camera
            if (currentState == CrystalState.INFINITE){

                /*
                foreach (Atom atom in structureBuilder.crystal.atoms.Values)
                {
                    if ((FindObjectsOfType<Camera>()[0].transform.position - atom.drawnObject.transform.position).magnitude - atom.drawnObject.transform.lossyScale.x < .5){
                        atom.drawnObject.GetComponent<Renderer>().materials[0].EnableKeyword("_ALPHABLEND_ON");
                        Color color = gameObject.GetComponent<Renderer>().materials[0].color;
                        float distance = (FindObjectsOfType<Camera>()[0].transform.position - atom.drawnObject.transform.position).magnitude;
                        float distancePercent = (distance - atom.drawnObject.transform.lossyScale.x * 0.5f) / 0.5f;
                        color.a = distancePercent;
                        gameObject.GetComponent<Renderer>().materials[0].color = color;
                    } else {
                        atom.drawnObject.GetComponent<Renderer>().materials[0].DisableKeyword("_ALPHABLEND_ON");
                    }
                }*/

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

            if (spinUpAnimation != null) spinUpAnimation.PlayFromStart();

            currentState = CrystalState.SINGLECELL;

            desiredState = CrystalState.SINGLECELL;
        }

        // Prompts the structureBuilder to destroy the cell
        public void ElementRemoved(){
            structureController.Unlock(); // unlock structure when removed in case it was locked during lecture and removed mid lecture
            structureBuilder.DestroyCell();
            VeridiumButton.Instance.Disable();
        }

        public void SetView(CrystalState state){
            switch (state){
                case CrystalState.INFINITE:
                    InfiniteView();
                break;
                case CrystalState.MULTICELL:
                    MultiCellView();
                break;
                case CrystalState.MULTICELLHEX1:
                    MultiCellView(CellType.HEX, 1);
                break;
                case CrystalState.MULTICELLHEX2:
                    MultiCellView(CellType.HEX, 2);
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

        public void MultiCellView(CellType type, int version)
        {
            if(currentState == CrystalState.INFINITE){
                structureBuilder.transform.parent = structureController.gameObject.transform;
                structureBuilder.transform.localPosition = Vector3.zero;
                structureBuilder.transform.localRotation = Quaternion.identity;
                structureBuilder.transform.localScale = Vector3.one;
                FindObjectsOfType<Camera>()[0].cullingMask = ~0 ^ 1 << LayerMask.NameToLayer("InfiniteOnly");
            }
            
            if (version == 1) currentState = CrystalState.MULTICELLHEX1;
            else if (version == 2) currentState = CrystalState.MULTICELLHEX2;

            structureBuilder.Redraw(currentState);

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
                FindObjectsOfType<Camera>()[0].cullingMask = ~0 ^ 1 << LayerMask.NameToLayer("InfiniteOnly");
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
            structureBuilder.gameObject.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
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
                if (atom.drawnObject != null)
                {
                    Anim_MoveTo anim = atom.drawnObject.AddComponent<Anim_MoveTo>() as Anim_MoveTo;
                    anim.updateLocation = false;
                    anim.updateRotation = false;
                    anim.easingType = EasingType.Elastic;
                    anim.easeOutOnly = true;
                    anim.duration = 3f;
                    anim.selfDestruct = true;
                    anim.endScale = Vector3.one * .25f;
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

            if (desiredState != currentState) SetView(desiredState);
        }

        public void Unlock(){
            locked = false;

            elementLoader.Unlock();

            desiredState = currentState;
        }
    }
}