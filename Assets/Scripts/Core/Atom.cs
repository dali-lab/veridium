/**
 * @author      Siddharth Hathi
 * @title       Unit Cells
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace sib
{
    /**
    * @class Atom
    * Object class used to store positioning data and distinguishing characteristics for an atom
    */
    public class Atom {
        // The Atom's position in global-space
        private Vector3 position;
        
        // The number of protons in the atom
        private int atomicNumber;

        private GameObject drawnObject;
        private bool metallic = true;

        /**
         * Constructor - creates a new Atom object
         */
        public Atom(int atomicNumber, Vector3 position) {
            this.atomicNumber = atomicNumber;
            this.position = position;
            this.drawnObject = null;
        }

        /**
         * @function compare
         * @input   otherAtom   the atom being compared
         * @return  bool        it the atom equivalent to this
         * Compares another atom to itself. Returns true if it has the same
         * position and atomic number.
         */
        public bool Equals(Atom otherAtom) {
            if (otherAtom.GetAtomicNumber() == this.atomicNumber && 
                otherAtom.GetPosition() == this.position ) {
                    return true;
            }
            return false;
        }

        /**
         * @function getAtomicNumber
         * @return  int     The Atom's atomic number
         * Returns the atomic number for the Atom
         */
        public int GetAtomicNumber() {
            return this.atomicNumber;
        }

        /**
         * @function getPosition
         * @return  Vector3     The Atom's  position
         * Returns the position of the Atom
         */
        public Vector3 GetPosition() {
            return this.position;
        }

        /**
         * @function Debug
         * @return debugging string
         */
        public string Debug() {
            string output = "";
            output += "Atom w atomic Number: " + this.atomicNumber.ToString() + " Position: (" + this.position.x.ToString() + ", " + this.position.y.ToString() + ", " + this.position.z.ToString() + ")\n";
            return output;
        }
        

        /**
         * @function Draw
         * @input atomPrefab GameObject containing the prefab for the atom
         * @input builder GameObject reference to the StructureBuilder MonoBehavior
         * Draws the atom by instantiating a prefab at the correct position and attatching it to the builder
         */
        public void Draw(GameObject atomPrefab, GameObject builder) {
            this.drawnObject = MonoBehaviour.Instantiate(atomPrefab, this.position, Quaternion.identity);
            drawnObject.transform.SetParent(builder.transform);
            drawnObject.GetComponentInChildren<Renderer>().material.color = Coloration.GetColorByNumber(atomicNumber);

            if(metallic){
                drawnObject.GetComponentInChildren<Renderer>().material.SetFloat("_Metallic", 1.0f);
                drawnObject.GetComponentInChildren<Renderer>().material.SetFloat("_Glossiness", 0.65f);
            }
        }

        /**
         * @function GetDrawnObject
         * @return GameObject representing Atom in the scene. Null if Atom 
         * hasn't been instantiated in current context.
         * Returns the GameObject corresponding to the Atom.
         */
        public GameObject GetDrawnObject() {
            return this.drawnObject;
        }

        public void Highlight(){
            GetDrawnObject().GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Coloration.GetColorByNumber(atomicNumber));
            GetDrawnObject().GetComponentInChildren<Renderer>().material.EnableKeyword("_EMISSION");
        }

        public void Unhighlight(){
            GetDrawnObject().GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.black);
            GetDrawnObject().GetComponentInChildren<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}