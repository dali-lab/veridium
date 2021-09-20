using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class HandSlider : MonoBehaviour
{

    [SerializeField] private float sliderLength = 0.3f;
    [SerializeField] private Slider UISlider;
    [SerializeField] private GameObject parent;
    private float maxX, minX;

    private XRBaseInteractor interactor;
    private bool shouldGetHandPos = false;
    private XRGrabInteractable grabInteractor => GetComponent<XRGrabInteractable>();
    // Start is called before the first frame update
    void Start()
    {
        minX = transform.localPosition.x;
        maxX = minX + sliderLength;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldGetHandPos)
        {
            // Update the position of the 3D slider
            float newXPos = Mathf.Clamp(GetLocalXPosition(interactor.GetComponent<Transform>().position), minX, maxX);

            Vector3 newPos = new Vector3(newXPos, transform.localPosition.y, transform.localPosition.z);
            transform.localPosition = newPos;

            // Update the UI slider value, based on if using whole numbers or not
            if (UISlider.wholeNumbers)
            {
                UISlider.value = Mathf.Floor(((newXPos - minX) / sliderLength) * UISlider.maxValue);
            }
            else
            {
                UISlider.value = ((newXPos - minX) / sliderLength) * UISlider.maxValue;
            }

        }
    }

    private void OnEnable()
    {
        grabInteractor.selectEntered.AddListener(GrabbedBy);
        grabInteractor.selectExited.AddListener(GrabEnd);
    }

    private void OnDisable()
    {
        grabInteractor.selectEntered.RemoveListener(GrabbedBy);
        grabInteractor.selectExited.RemoveListener(GrabEnd);
    }

    private void GrabEnd(SelectExitEventArgs args)
    {
        shouldGetHandPos = false;
        transform.SetParent(parent.transform);
    }

    private void GrabbedBy(SelectEnterEventArgs args)
    {
        interactor = GetComponent<XRGrabInteractable>().selectingInteractor;
        interactor.gameObject.GetComponent<XRDirectInteractor>().hideControllerOnSelect = true;
        transform.SetParent(parent.transform);

        shouldGetHandPos = true;
    }

    public Vector3 GetInteractorPosition() => interactor.GetComponent<Transform>().position;

    private float GetLocalXPosition(Vector3 position)
    {
        Vector3 localPos = parent.transform.InverseTransformPoint(position);
        return localPos.x;
    }
}
