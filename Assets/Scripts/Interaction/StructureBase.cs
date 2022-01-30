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



        }

        // Enables multi-cell view for the crystal
        public void MultiCellView(){


            
        }

        // Enables single cell view for the crystal
        public void SingleCellView(){



        }

        public void ClosePackedView(){

            foreach(Bond bond in structureBuilder.crystal.bonds.Values){
                Destroy(bond.drawnObject);
            }

            foreach (Atom atom in structureBuilder.crystal.atoms.Values)
            {
                if(atom.drawnObject != null){
                    Anim_MoveTo anim = atom.drawnObject.transform.Find("Sphere").gameObject.AddComponent<Anim_MoveTo>() as Anim_MoveTo;
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