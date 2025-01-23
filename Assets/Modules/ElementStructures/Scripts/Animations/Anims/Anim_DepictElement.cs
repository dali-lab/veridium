using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Veridium.Core;
using Veridium.Interaction;

namespace Veridium.Animation{
    public class Anim_DepictElement : AnimationBase
    {
        public PTElement element;
        public StructureBase structureBase;
        
        // Called when animation is started
        public override void Play()
        {
            /*StructureBuilder structureBuilder = structureBase.structureBuilder;

            int atomicNumber = Coloration.GetNumberByName(element.name);
            float sideLength = structureBase.sideLength;
            float sphereRadius = structureBase.sphereRadius;

            structureBuilder.BuildCell(element.type, element.variation, CrystalState.SINGLECELL, sideLength, sphereRadius, atomicNumber);
*/

            structureBase.ElementRemoved();
            structureBase.ElementAdded(element);


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

    }
}
