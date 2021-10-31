using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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


        // Start is called before the first frame update
        void Start()
        {
            if(structure == null) {
                InitStructure();
            }
            
        }
        

        // Update is called once per frame
        void Update()
        {

        }

        // Constructs the structure that will be displayed
        private GameObject InitStructure() {

            // Clear any existing structure
            if(structure != null) {
                Destroy(structure);
            }

            // Create a new structure
            // return structureBuilder.BuildStructure();
            return null;
        }

        public void ElementAdded(PTElement element){
            
        }
    }
}