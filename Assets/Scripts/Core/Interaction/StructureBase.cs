using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using sib;

/// <summary>
/// Structure.cs controls the main structure on the podium 
/// Supports interaction of user and structure
/// 
/// </summary>

namespace SIB_Interaction{
    public class StructureBase : MonoBehaviour
    {
        public GameObject structure;            // The structure on the podium
        private bool grabbed;                   // Whether the structure has been grabbed by the user
        public GameObject headSet;              // The eye position of the player
        public float respawnDistance = 1;
        public StructureBuilder structureBuilder;
        public float sideLength = 0.5f;
        public float sphereRadius = 0.05f;


        // Start is called before the first frame update
        void Start()
        {
            
        }
        

        // Update is called once per frame
        void Update()
        {

        }

        public void ElementAdded(PTElement element){
            structureBuilder.BuildCell(element.type, element.variation, CrystalState.SINGLECELL, sideLength, sphereRadius);
        }

        public void ElementRemoved(){
            
        }
    }
}