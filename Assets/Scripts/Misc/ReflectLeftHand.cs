using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[DefaultExecutionOrder(-1)] 
public class ReflectLeftHand : MonoBehaviour
{
    private XRGrabInteractable grabInteractable => GetComponent<XRGrabInteractable>();
    private Transform grabHandle => grabInteractable.attachTransform;
    private Quaternion originalGrabHandleRotation;
    private Vector3 originalGrabHandlePosition;

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(Grab);
        grabInteractable.selectExited.AddListener(Drop);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(Grab);
        grabInteractable.selectExited.RemoveListener(Drop);
    }

    private void Awake()
    {
        originalGrabHandleRotation = grabHandle.localRotation;
        originalGrabHandlePosition = grabHandle.localPosition;
    }

    private void Grab(SelectEnterEventArgs args)
    {  // if (Left hand Grabs the tool, mirror the transform about the yz-plane
        if (args.interactor.name.Substring(0, 4) == "Left")
        {
            grabHandle.localPosition = new Vector3(-1 * grabHandle.localPosition.x, grabHandle.localPosition.y, grabHandle.localPosition.z);
            grabHandle.localRotation = Quaternion.Euler(grabHandle.localRotation.eulerAngles.x, -1 * grabHandle.localRotation.eulerAngles.y, -1 * grabHandle.localRotation.eulerAngles.z);

            // TODO: Mark -- this is specificially for rotating the eraser -- not working yet
            // if (args.interactor.selectTarget.gameObject.tag == "Eraser")
            // {
            //     Debug.Log("reflecting eraser");
            //     grabHandle.localRotation = Quaternion.Euler(grabHandle.localRotation.eulerAngles.x, -1 * grabHandle.localRotation.eulerAngles.y, -1 * grabHandle.localRotation.eulerAngles.z);
            // }
        }
    }

    private void Drop(SelectExitEventArgs args)
    {
        grabHandle.localPosition = originalGrabHandlePosition;
        grabHandle.localRotation = originalGrabHandleRotation;
    }
}
