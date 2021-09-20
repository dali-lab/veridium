using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


public class PlierGrab : MonoBehaviour
{

    [SerializeField] Collider PlierCollider;
    [SerializeField] GameObject PlierFillingSpot;
    [SerializeField] GameObject OriginalFillingParent;

    private bool selected = false;

    private List<GameObject> triggerList;
    private List<GameObject> fillingsList;

    // XR Toolkit variables
    private XRGrabInteractable grabInteractable => GetComponent<XRGrabInteractable>();


    // Start is called before the first frame update
    void Start()
    {
        triggerList = new List<GameObject>();
        fillingsList = new List<GameObject>();

        foreach (Transform child in OriginalFillingParent.transform)
        {
            fillingsList.Add(child.gameObject);
        }

    }

    #region ListenerAssignment
    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(Grab);
        grabInteractable.selectExited.AddListener(Drop);
        grabInteractable.activated.AddListener(Activate);
        grabInteractable.deactivated.AddListener(Deactivate);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(Grab);
        grabInteractable.selectExited.RemoveListener(Drop);
        grabInteractable.activated.RemoveListener(Activate);
        grabInteractable.deactivated.RemoveListener(Deactivate);
    }
    #endregion

    #region ControllerAssignment

    // User presses middle finger trigger
    private void Grab(SelectEnterEventArgs args)
    {
        this.GetComponent<Collider>().enabled = false;
    }

    // User releases middle finger trigger
    private void Drop(SelectExitEventArgs args)
    {
        this.GetComponent<Collider>().enabled = true;
    }

    // User presses index finger trigger
    private void Activate(ActivateEventArgs args)
    {
        // TODO: run animation for hand squeezing pliers and pliers coming together
        Debug.Log("activated!");

        foreach(GameObject filling in triggerList)
        {
            // Move each filling to the filling position at the tip of the plier
            filling.GetComponent<Rigidbody>().useGravity = false;
            filling.GetComponent<Rigidbody>().isKinematic = true;
            filling.transform.SetParent(PlierFillingSpot.transform);
            filling.transform.localPosition = new Vector3(0f, 0f, 0f);
            Debug.Log("toggling false in activate");
            filling.GetComponent<ToggleOutline>().toggleOutline(false);
            selected = true;
        }
    }

    // User releases index finger trigger
    private void Deactivate(DeactivateEventArgs args = null)
    {
        // TODO: run animation for hand unsqueezing pliers and pliers coming apart
        foreach (GameObject filling in triggerList)
        {
            filling.GetComponent<Rigidbody>().useGravity = true;
            filling.GetComponent<Rigidbody>().isKinematic = false;
            filling.transform.SetParent(OriginalFillingParent.transform);
            selected = false;
        }
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggered: " + other.gameObject.name);
        if (other.tag == "Filling")
        {
            triggerList.Add(other.gameObject);
            Debug.Log("toggling true in triggerEnter");
            if (!selected)
            {
                other.GetComponent<ToggleOutline>().toggleOutline(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerList.Contains(other.gameObject))
        {
            triggerList.Remove(other.gameObject);
            Debug.Log("toggling false in triggerexit");
            other.GetComponent<ToggleOutline>().toggleOutline(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}