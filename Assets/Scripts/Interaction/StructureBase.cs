using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using sib;
using SIB_Animation;

/// <summary>
/// StructureBase controls the main structure on the podium 
/// Supports interaction of user and structure
/// </summary>

namespace SIB_Interaction{
    public class StructureBase : MonoBehaviour
    {
        public GameObject structure;                    // The structure on the podium
        private bool grabbed;                           // Whether the structure has been grabbed by the user
        public float respawnDistance = 1;               // Distance from the podium at which the structure should teleport home
        public StructureBuilder structureBuilder;       // Reference to the structureBuilder which implements the construction of the structure
        public float sideLength = 0.5f;                 // Standard side length of a unit cell
        public float sphereRadius = 0.075f;             // Radius of the spheres
        public int planeIndex = 0;                      // Index of the currently visualized plane
        public Anim_SpinUp spinUpAnimation;             // The animation that spawns this structure in


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

        // Called by joystick switch, switches the plane index up or down
        public void Switch(bool right){

            planeIndex += right ? 1 : -1;
            if(planeIndex > structureBuilder.numPlanes - 1) planeIndex -= structureBuilder.numPlanes;
            if(planeIndex < 0) planeIndex += structureBuilder.numPlanes;

            structureBuilder.HighlightPlaneAtIndex(planeIndex);

        }
    }
}