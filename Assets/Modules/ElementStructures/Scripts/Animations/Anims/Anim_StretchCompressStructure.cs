using UnityEngine;
using Veridium.Animation;

namespace Veridium.Modules.ElementStructures
{
    public class Anim_StretchCompressStructure : AnimationBase
    {
        public EasingType easingType = EasingType.Linear;

        private Matrix4x4 startingTransformation;
        private Matrix4x4 endingTransformation;

        [HideInInspector] public bool easeOutOnly = false;

        public Vector3 stretchDirection = Vector3.up;
        public float stretchAmount = 1;

        public StructureBase structureBase;
        
        public override void Play()
        {
            base.Play();

            startingTransformation = structureBase.GetStructureDeformation();
            endingTransformation = Utils.getStretchTransformation(stretchDirection, stretchAmount);

            Debug.Log("Ending Transformation: " + endingTransformation);
        }


        protected override void UpdateAnim()
        {
            base.UpdateAnim();

            float alpha;

            if(!easeOutOnly){
                alpha = Easing.EaseFull(elapsedTimePercent, easingType);
            } else {
                alpha = Easing.EaseOut(elapsedTimePercent, easingType);
            }

            Matrix4x4 structureTransformation = LerpMat(startingTransformation, endingTransformation, alpha);
            structureBase.SetStructureDeformation(structureTransformation);
        }

        public override void Pause()
        {
            base.Pause();
        }

        private Matrix4x4 LerpMat(Matrix4x4 a, Matrix4x4 b, float t)
        {
            Matrix4x4 result = new Matrix4x4();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    result[i, j] = Mathf.Lerp(a[i, j], b[i, j], t);
                }
            }

            return result;
        }

    }
}
