using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;
using Veridium.Interaction;

namespace Veridium.Modules.ElementStructures
{
    public class StructureController : HandDistanceGrabbable
    {

        /// <summary>
        /// Structure Controller handles the input from the grab handles 
        /// in the structure to solve for the appropriate motion of the
        /// structure. This handles one hand grab and two hand grab, the
        /// latter for scaling. 
        /// </summary>
        
        public GameObject structure;                // GameObject References
        private float initialLineWidth;
        [HideInInspector] public StructureBase structureBase;

        public StructureController(){
            drawRay = false;
        }


        // extends OnSelectExiting from XRGrabInteractabe. Resets the structure to its state before grabbing
        protected override void OnSelectExiting(SelectExitEventArgs args) {

            base.OnSelectExiting(args); // Run this method in parent

            // Reset the structure to its original state
            if (structureBase.currentState == CrystalState.INFINITE && interactorsSelecting.Count == 0) 
            {
                //structureBase.elementLoader.ResetStructure();
                LineRenderer lr = structure.GetComponent<LineRenderer>();
                lr.startWidth = initialLineWidth;
                lr.endWidth = initialLineWidth;
                structureBase.SingleCellView();
                gameObject.transform.localScale = Vector3.one;
                gameObject.transform.localPosition = Vector3.zero;
            } 
        }

        void Start()
        {
            if (structure.TryGetComponent<LineRenderer>(out LineRenderer lr))
            {
                initialLineWidth = lr.startWidth;
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Cell Type HEX is different than other cell views
            if (structureBase?.elementLoader?.heldElement?.type == CellType.HEX)
            {
                if (gameObject.transform.localScale.x >= 2.4)
                {
                    if (structureBase.currentState != CrystalState.INFINITE) structureBase.InfiniteView();
                }
                else if (gameObject.transform.localScale.x >= 1.8)
                {
                    if (structureBase.currentState != CrystalState.MULTICELLHEX2)
                    {
                        if (structure.TryGetComponent<LineRenderer>(out LineRenderer lr))
                        {
                            lr.startWidth = initialLineWidth;
                            lr.endWidth = initialLineWidth;
                        }
                        structureBase.MultiCellView(CellType.HEX, 2); 
                    }
                }
                else if (gameObject.transform.localScale.x >= 1.2)
                {
                    if (structureBase.currentState != CrystalState.MULTICELLHEX1) 
                    {
                        if (structure.TryGetComponent<LineRenderer>(out LineRenderer lr))
                        {
                            lr.startWidth = initialLineWidth;
                            lr.endWidth = initialLineWidth;
                        }
                        structureBase.MultiCellView(CellType.HEX, 1);
                    }
                } 
                else if (gameObject.transform.localScale.x <= 0.9)
                {
                    if (structureBase.currentState != CrystalState.SINGLECELL)
                    {
                        if (structure.TryGetComponent<LineRenderer>(out LineRenderer lr))
                        {
                            lr.startWidth = initialLineWidth;
                            lr.endWidth = initialLineWidth;
                        }
                        structureBase.SingleCellView();
                    }
                }
            }
            else
            {
                if (gameObject.transform.localScale.x >= 2){

                    if (structureBase.currentState != CrystalState.INFINITE) structureBase.InfiniteView();

                } else if (gameObject.transform.localScale.x >= 1.2 && structureBase?.elementLoader?.heldElement?.type != CellType.HEX){

                    if (structureBase.currentState != CrystalState.MULTICELL) structureBase.MultiCellView();

                } else if (gameObject.transform.localScale.x <= 0.9){

                    if (structureBase.currentState != CrystalState.SINGLECELL) structureBase.SingleCellView();
                }
            }
        }
    }
}
