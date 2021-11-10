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
     * @class Bond
     * Class that describes a linear bond between two Atom objects. Contains
     * functionality for comparing and drawing bonds
     */
    public class Bond {
        // "start" and "end" atoms are inherently arbitrary since direction doesn't really matter
        // in this case start and end are simply used to denote the two Atoms at either ends of the Bonds
        private Atom start;
        private Atom end;

        /**
         * @constructor
         * Instantiates bond with two atoms
         */
        public Bond(Atom start, Atom end) {
            this.start = start;
            this.end = end;
        }

        /**
         * @function GetStart
         * @return Atom start atom
         */
        public Atom GetStart() {
            return this.start;
        }

        /**
         * @function GetEnd
         * @return Atom end atom
         */
        public Atom GetEnd() {
            return this.end;
        }

        /**
         * @function Equals
         * @input other another bond
         * @return bool Whether the Bonds are equivalent
         */
        public bool Equals(Bond other) {
            if (this.start.Equals(other.GetStart()) && this.end.Equals(other.GetEnd())) {
                return true;
            } else if (this.end.Equals(other.GetStart()) && this.start.Equals(other.GetEnd())) {
                return true;
            }
            return false;
        }

        /**
         * @function GetStartPosition
         * @return Vector3 position of start atom
         */
        public Vector3 GetStartPos() {
            return this.start.GetPosition();
        }

        /**
         * @function GetEndpos
         * @return Vector3 position of end atom
         */
        public Vector3 GetEndPos() {
            return this.end.GetPosition();
        }

        /**
         * @function Debug
         * @return string Debug string
         */
        public string Debug() {
            string output = "";
            output += "Bond: ";
            output += "Start : " + start.Debug() + "End : " + end.Debug();
            return output;
        }

        /**
         * @function Draw
         * @input linePrefab the Unity prefab of the Bond
         * @input builder the Unity builder object
         */
        public void Draw(GameObject linePrefab, GameObject builder) {
            Vector3 midpoint = (this.start.GetPosition() + this.end.GetPosition())/2;
            MonoBehaviour.Instantiate(linePrefab, midpoint, 
                Quaternion.LookRotation(end.GetPosition()-start.GetPosition(), Vector3.up)).transform.SetParent(builder.transform);
            // (GameObject.FindWithTag("DebugText").GetComponent<TMPro.TextMeshPro>()).text = midpoint.ToString();

        }

    }
}