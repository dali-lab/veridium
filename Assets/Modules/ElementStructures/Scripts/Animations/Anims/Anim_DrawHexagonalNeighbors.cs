using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Animation;

namespace Veridium.Modules.ElementStructures
{
    public class Anim_DrawHexagonalNeighbors : AnimationBase
    {

        ///<summary>
        /// Draws existing atoms at all of the existing coordinates
        /// If more than one step is filled in, the steps will be evenly 
        /// spread across the duration of the animation.
        ///</summary>

        public List<ListWrapper<Vector3>> steps;        // A list of steps. Each step contains a list of coordinates of atoms
        public StructureBuilder structureBuilder;       // The structureBuilder of the atoms to add
        private int currentStep = -1;                   // Current step of the animation. Steps will progress for the length of the steps list
        private Vector3[] neighborRelativeLocations = new Vector3[] {
                Vector3.left,
                Vector3.right,
                new Vector3(0.5f, 0, Mathf.Sqrt(3)/2f), // forward right
                new Vector3(-0.5f, 0, Mathf.Sqrt(3)/2f), // forward left
                new Vector3(0.5f, 0, -Mathf.Sqrt(3)/2f), // back right
                new Vector3(-0.5f, 0, -Mathf.Sqrt(3)/2f) // back left
            };

        // A list wrapper class that allows nested lists to be edited in the inspector
        [System.Serializable]
        public class ListWrapper<T>
        {
            public List<T> list;
        }
        
        // Called when started
        public override void Play()
        {
            base.Play();
            currentStep = -1;
            foreach (Vector3 centerUnitCell in Constants.cell2MultiUnitCellCenterPositions)
            {
                UnitCell uc = structureBuilder.crystal.GetHexUnitCellAtCoordinate(centerUnitCell);
                Atom[] unitCellAtoms = uc.GetVertices();



                // Draw a bond from this center Atom to all 6 of its neighbors
                foreach (Vector3 localNeighbor in neighborRelativeLocations)
                {
                    Bond bond0 = new Bond(unitCellAtoms[0], new Atom(69, unitCellAtoms[0].GetPosition() + localNeighbor)); // layer 0
                    Bond bond1 = new Bond(unitCellAtoms[1], new Atom(69, unitCellAtoms[1].GetPosition() + localNeighbor)); // layer 1

                    bond0.builder = structureBuilder.gameObject;
                    bond1.builder = structureBuilder.gameObject;

                    bond0.Draw();
                    bond1.Draw();
                }
            }
        }

        // Called when restarted 
        protected override void ResetChild()
        {
            base.ResetChild();
            currentStep = -1;
        }

        // Called when paused or ended
        public override void Pause()
        {
            base.Pause();
        }
        
        // Called each frame the animation is playing
        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }
    }
}
