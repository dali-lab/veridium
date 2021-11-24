using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class HandButton : XRBaseInteractable
{
    public UnityEvent OnPressed = null;
    private float yMin = 0.0f;
    private float yMax = 0.0f;
    private bool previousPress = false;

    [SerializeField] private float buttonPressDepth = 0.5f;

    private float previousHandHeight = 0.0f;
    private XRBaseInteractor hoverInteractor = null;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        StartPress(args.interactor);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        EndPress(args.interactor);
    }

    private void StartPress(XRBaseInteractor interactor)
    {
        hoverInteractor = interactor;
        previousHandHeight = GetLocalYPosition(hoverInteractor.transform.position);
    }

    private void EndPress(XRBaseInteractor interactor)
    {
        hoverInteractor = null;
        previousHandHeight = 0.0f;

        previousPress = false;
        SetYPosition(yMax);
    }

    private void Start()
    {
        SetMinMax();
    }

    private void SetMinMax()
    {
        Collider collider = GetComponent<Collider>();
        yMin = transform.localPosition.y - (collider.bounds.size.y * buttonPressDepth);
        yMax = transform.localPosition.y;
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if (hoverInteractor)
        {
            float newHandHeight = GetLocalYPosition(hoverInteractor.transform.position);
            float handDiff = previousHandHeight - newHandHeight;
            previousHandHeight = newHandHeight;

            float newPos = transform.localPosition.y - handDiff;
            SetYPosition(newPos);

            CheckPress();
        }

    }

    private float GetLocalYPosition(Vector3 position)
    {
        Vector3 localPos = transform.parent.InverseTransformPoint(position);
        // Vector3 localPos = transform.root.InverseTransformPoint(position);      
        return localPos.y;
    }

    private void SetYPosition(float position)
    {
        Vector3 newPos = transform.localPosition;
        newPos.y = Mathf.Clamp(position, yMin, yMax);
        transform.localPosition = newPos;
    }

    private void CheckPress()
    {
        bool inPos = InPosition();

        if (inPos && inPos != previousPress)
        {
            OnPressed.Invoke();
            Debug.Log("Button Pressed");
        }

        previousPress = inPos;
    }

    // Is the button in the correct position to invoke the "click"
    private bool InPosition()
    {
        float inRange = Mathf.Clamp(transform.localPosition.y, yMin, yMin + 0.01f);
        return transform.localPosition.y == inRange;
    }
}
