/**
 * @author      Siddharth Hathi
 * @title       Unit Cells
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Veridium_Interaction;

namespace Veridium_Core
{
    /**
    * @class Atom
    * Object class used to store positioning data and distinguishing characteristics for an atom
    */
    public class Atom
    {
        // The Atom's position in global-space
        private Vector3 position;
        
        // The number of protons in the atom
        private int atomicNumber;

        public GameObject drawnObject { get; private set; }
        private bool metallic = true;
        public GameObject builder;                         // The structure builder that created this

        /**
         * Constructor - creates a new Atom object
         */
        public Atom(int number, Vector3 pos) 
        {
            atomicNumber = number;
            position = pos;
        }

        /**
         * @function compare
         * @input   otherAtom   the atom being compared
         * @return  bool        it the atom equivalent to this
         * Compares another atom to itself. Returns true if it has the same
         * position and atomic number.
         */
        public bool Equals(Atom otherAtom) 
        {
            if (otherAtom.GetAtomicNumber() == atomicNumber && 
                otherAtom.GetPosition() == position ) {
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

            return atomicNumber;
        }

        /**
         * @function getPosition
         * @return  Vector3     The Atom's  position
         * Returns the position of the Atom
         */
        public Vector3 GetPosition() 
        {
            return position;
        }
        
        /**
         * @function Draw
         * @input atomPrefab GameObject containing the prefab for the atom
         * @input builder GameObject reference to the StructureBuilder MonoBehavior
         * Draws the atom by instantiating a prefab at the correct position and attatching it to the builder
         */
        public void Draw() 
        {
            drawnObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Atom_Prefab"), Vector3.zero, Quaternion.identity);
            drawnObject.transform.SetParent(builder.transform);
            drawnObject.transform.localPosition = position;
            drawnObject.transform.localScale = Vector3.one * 0.15f;
            drawnObject.GetComponent<Renderer>().material.color = Coloration.GetColorByNumber(atomicNumber);

            if (metallic)
            {
                drawnObject.GetComponent<Renderer>().material.SetFloat("_Metallic", 1.0f);
                drawnObject.GetComponent<Renderer>().material.SetFloat("_Glossiness", 0.65f);
            }

        }

        public void Highlight()
        {
            drawnObject.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Coloration.GetColorByNumber(atomicNumber));
            drawnObject.GetComponentInChildren<Renderer>().material.EnableKeyword("_EMISSION");
        }

        public void Unhighlight()
        {
            drawnObject.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.black);
            drawnObject.GetComponentInChildren<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }
}