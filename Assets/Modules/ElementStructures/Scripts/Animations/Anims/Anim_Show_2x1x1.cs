using UnityEngine;
using Veridium.Animation;
using Veridium.Core;

namespace Veridium.Modules.ElementStructures
{
    public class Anim_Show_2x1x1 : AnimationBase
    {
        ///<summary>
        /// Template for animations. Duplicate this file to create new animations
        /// Consult the animation README for more info on how to use this template
        ///</summary>
        ///

        public StructureBuilder structureBuilder;
        public Vector3[] unitCellHighlightPositions;
        
        // Called when animation is started
        public override void Play()
        {
            draw2x1x1Crystal();
            drawCellHighlight();

            if (structureBuilder.gameObject.GetComponent<Anim_MoveTo>() != null) Destroy(structureBuilder.gameObject.GetComponent<Anim_MoveTo>());
            Anim_MoveTo anim = structureBuilder.gameObject.AddComponent<Anim_MoveTo>() as Anim_MoveTo;

            anim.updateLocation = true;
            anim.updateRotation = false;
            anim.updateScale = true;

            anim.endLocation = new Vector3(-0.175f, 0, 0) + structureBuilder.gameObject.transform.parent.position;
            anim.endScale = new Vector3(.7f, .7f, .7f);

            anim.duration = 1f;
            anim.easingType = EasingType.Exponential;
            //structureBuilder.gameObject.transform.localScale = new Vector3(.8f,.8f,.8f);
            anim.selfDestruct = true;
            anim.easeOutOnly = true;

            anim.Play();

            base.Play();
        }

        // Called when animation ends
        public override void End()
        {
            base.End();
        }

        // Called when animation is paused
        public override void Pause()
        {
            base.Pause();
        }

        // Called when animation restarts
        protected override void ResetChild()
        {
            base.ResetChild();
        }

        // Called every frame while animation is playing
        protected override void UpdateAnim()
        {
            base.UpdateAnim();
        }

        private void draw2x1x1Crystal()
        {
            Crystal crystal = structureBuilder.crystal;

            foreach (Bond bond in crystal.bonds.Values)
            {
                if(bond.drawnObject != null) Destroy(bond.drawnObject);
            }

            foreach (Atom atom in crystal.atoms.Values)
            {
                if(atom.drawnObject != null) Destroy(atom.drawnObject);
            }

            foreach (Transform child in structureBuilder.transform)
            {
                if (child.tag == "cage") MonoBehaviour.Destroy(child.gameObject);
            }

            for ( float k = 0; k < 2; k++){
                Vector3 coord = new Vector3(k,0,0);
                UnitCell unitCell = crystal.GetUnitCellAtCoordinate(coord);
                if (unitCell != null){
                    unitCell.builder = crystal.builder;
                    unitCell.Draw();
                }
            }
        }

        private void drawCellHighlight()
        {
            GameObject cage = new GameObject("CellHighlight");
            cage.transform.parent = structureBuilder.transform;
            cage.transform.localPosition = Vector3.zero;
            cage.transform.localRotation = Quaternion.identity;
            cage.transform.localScale = Vector3.one;
            cage.tag = "cage";
            LineRenderer lr = cage.AddComponent<LineRenderer>();

            LineRenderer original = structureBuilder.GetComponent<LineRenderer>();
            // System.Type type = original.GetType();
            // System.Reflection.FieldInfo[] fields = type.GetFields();
            // foreach (System.Reflection.FieldInfo field in fields) field.SetValue(lr, field.GetValue(original));
            lr.useWorldSpace = original.useWorldSpace;
            lr.material = original.material;
            lr.positionCount = unitCellHighlightPositions.Length;
            lr.startWidth = original.startWidth;
            lr.endWidth = original.endWidth;

            lr.SetPositions(unitCellHighlightPositions);
        }

    }
}
