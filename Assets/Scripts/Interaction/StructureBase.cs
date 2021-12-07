using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using sib;

/// <summary>
/// StructureBase controls the main structure on the podium 
/// Supports interaction of user and structure
/// </summary>

namespace SIB_Interaction{
    public class StructureBase : MonoBehaviour
    {
        public GameObject structure;                // The structure on the podium
        private bool grabbed;                       // Whether the structure has been grabbed by the user
        public float respawnDistance = 1;           // Distance from the podium at which the structure should teleport home
        public StructureBuilder structureBuilder;   // Reference to the structureBuilder which implements the construction of the structure
        public float sideLength = 0.5f;             // Standard side length of a unit cell
        public float sphereRadius = 0.075f;          // Radius of the spheres


        // Start is called before the first frame update
        void Start()
        {
            
        }
        

        // Update is called once per frame
        void Update()
        {

        }

        // Prompts the structureBuilder to construct a structure base on an element
        public void ElementAdded(PTElement element){

            structureBuilder.BuildCell(element.type, element.variation, CrystalState.SINGLECELL, sideLength, sphereRadius);

        }

        // Prompts the structureBuilder to destroy the cell
        public void ElementRemoved(){

            structureBuilder.DestroyCell();

        }

        public void InfiniteView(){



        }

        public void MultiCellView(){


            
        }
    }
}