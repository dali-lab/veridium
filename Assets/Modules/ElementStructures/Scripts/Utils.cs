using UnityEngine;

namespace Veridium.Modules.ElementStructures {
    public class Utils {
        public static Matrix4x4 getStretchTransformation(Vector3 stretchDirection, float stretchAmount) {
             Vector3 axis = Vector3.Cross(stretchDirection, Vector3.up);
            float angle = Vector3.Angle(stretchDirection, Vector3.up);
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, axis));

            // 2. Scale structure in the direction of stretchDirection
            return rotationMatrix.inverse * Matrix4x4.Scale(new Vector3(1, stretchAmount, 1)) * rotationMatrix;
        }
    }
}