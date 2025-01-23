/**
 * @author      Siddharth Hathi
 * @title       Unit Cells
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Veridium.Modules.ElementStructures
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
        public GameObject drawnObject {get; private set;}
        public GameObject cylinderChild {get; private set;}
        public GameObject builder;

        /**
         * @constructor
         * Instantiates bond with two atoms
         */
        public Bond(Atom startAtom, Atom endAtom) {
            start = startAtom;
            end = endAtom;
        }

        /**
         * @function GetStart
         * @return Atom start atom
         */
        public Atom GetStart() {
            return start;
        }

        /**
         * @function GetEnd
         * @return Atom end atom
         */
        public Atom GetEnd() {
            return end;
        }

        /**
         * @function Equals
         * @input other another bond
         * @return bool Whether the Bonds are equivalent
         */
        public bool Equals(Bond other) {
            if (start.Equals(other.GetStart()) && end.Equals(other.GetEnd())) {
                return true;
            } else if (end.Equals(other.GetStart()) && start.Equals(other.GetEnd())) {
                return true;
            }
            return false;
        }

        /**
         * @function GetStartPosition
         * @return Vector3 position of start atom
         */
        public Vector3 GetStartPos() {
            return start.GetPosition();
        }

        /**
         * @function GetEndpos
         * @return Vector3 position of end atom
         */
        public Vector3 GetEndPos() {
            return end.GetPosition();
        }

        /**
         * @function Draw
         * @input linePrefab the Unity prefab of the Bond
         * @input builder the Unity builder object
         */
        public void Draw() {
            Vector3 midpoint = (start.GetPosition() + end.GetPosition())/2;
            float distance = Vector3.Distance(start.GetPosition(), end.GetPosition());

            drawnObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Edge"), midpoint, Quaternion.LookRotation(end.GetPosition()-start.GetPosition(), Vector3.up));
            cylinderChild = drawnObject.transform.GetChild(0).gameObject;
            drawnObject.transform.SetParent(builder.transform);
            drawnObject.transform.localScale = new Vector3(1f,1f,distance/0.5f);
            drawnObject.transform.localPosition = midpoint;
            drawnObject.transform.localRotation = Quaternion.LookRotation(end.GetPosition()-start.GetPosition());
            
            if (builder.GetComponent<StructureBuilder>().cellType == CellType.HEX)
            {
                drawnObject.tag = "bond";
                Vector3 hexagonalT = new Vector3(1.5f, 0.75f, Mathf.Sqrt(3)/2f);
                drawnObject.transform.localScale = new Vector3(1f, 1f, distance * 2f / 3f); // wtf is this
                drawnObject.transform.localPosition -= hexagonalT * Constants.hexBaseLength;
                drawnObject.transform.localPosition = Vector3.Scale(drawnObject.transform.localPosition, new Vector3(1/3f, 1/2f, 1/3f));
                Vector3 newCenterPoint = Vector3.Scale((start.GetPosition() - (hexagonalT * Constants.hexBaseLength)), new Vector3(1/3f, 1/2f, 1/3f));
                drawnObject.transform.localScale *= Constants.hexBaseLength;

                drawnObject.transform.localPosition = Vector3.MoveTowards(drawnObject.transform.localPosition, newCenterPoint, Vector3.Distance(newCenterPoint, drawnObject.transform.localPosition) - drawnObject.transform.localScale.z/4f);
            }
        }
    }
}