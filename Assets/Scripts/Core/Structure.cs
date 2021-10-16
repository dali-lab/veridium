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
    public class Structure : MonoBehaviour
    {
        public GameObject structure;            // The structure on the podium
        private bool grabbed;                   // Whether the structure has been grabbed by the user
        public GameObject headSet;              // The eye position of the player
        public float respawnDistance = 1;


        // Start is called before the first frame update
        void Start()
        {
            if(structure == null){
                InitStructure();
            }
            
        }
        

        // Update is called once per frame
        void Update()
        {

            if(!grabbed){
                // Check if rotating
                //if (structure.GetComponent<Rigidbody>().angularVelocity.magnitude > 0.5){
                
                //} else {

                    // Snap the object to closest rotation
                //    Quaternion closestSide = GetClosestSide(structure.transform.rotation);
                //    structure.transform.rotation = Quaternion.Slerp(structure.transform.rotation, closestSide, Time.deltaTime*5);
                //}
            }

            // If the structure is too far from the player and not on the pedestal, put it on the pedestal
            if ((structure.transform.position - headSet.transform.position).magnitude > respawnDistance){
                structure.transform.localPosition = Vector3.zero;
                structure.transform.rotation = Quaternion.identity; 
            }
        }

        private Quaternion GetClosestSide(Quaternion r){
            
            List<float> angles = new List<float>();

            Quaternion[] directions = new Quaternion[] {new Quaternion(1,0,0,0), new Quaternion(-1,0,0,0), new Quaternion(0,1,0,0), new Quaternion(0,-1,0,0), new Quaternion(0,0,1,0), new Quaternion(0,0,-1,0)};

            foreach (Quaternion direction in directions){
                angles.Add(Quaternion.Angle(direction, r));
            }

            float min = angles[0];
            int idx = 0;
            for (int i = 1; i < 5; i ++){
                if (angles[i] < min){
                    idx = i;
                    min = angles[i];
                }
            }

            return(directions[idx]);
        }

        // Constructs the structure that will be displayed
        private GameObject InitStructure(){

            // Clear any existing structure
            if(structure != null){
                Destroy(structure);
            }

            // Create a new structure
            // return structureBuilder.BuildStructure();
            return null;
        }
    }
}