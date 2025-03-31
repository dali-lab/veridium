using UnityEngine;

public class MirrorTransform : MonoBehaviour
{
    public Transform A; // Original object
    public Transform M; // Mirror object
    public Transform B; // Mirrored object

    void Update()
    {
        if (A == null || M == null || B == null) return;

        // Mirror Position
        Vector3 toA = A.position - M.position;
        Vector3 mirroredPosition = A.position - 2 * Vector3.Dot(toA, M.right) * M.right;
        B.position = mirroredPosition;

        // Mirror Rotation
        Vector3 mirroredForward = A.forward - 2 * Vector3.Dot(A.forward, M.right) * M.right;
        Vector3 mirroredUp = A.up - 2 * Vector3.Dot(A.up, M.right) * M.right;

        B.rotation = Quaternion.LookRotation(mirroredForward, mirroredUp);
    }
}
