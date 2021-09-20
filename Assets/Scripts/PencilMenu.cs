using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PencilMenu : MonoBehaviour
{
    [SerializeField] private GameObject pencilMenuObject;
    [SerializeField] private GameObject pencilMenuColors;
    [SerializeField] private GameObject pencilMenuWidths;
    [SerializeField] private GameObject markerTip;
    [SerializeField] private GameObject pencilTarget;
    [SerializeField] private GameObject markerCap;
    // public Slider UISlider; // needs to be public for access on lines 120-121 in "LineDraw.cs"

    private XRBaseInteractor interactor;
    private XRGrabInteractable grabInteractor => GetComponent<XRGrabInteractable>();
    // private UnityEngine.XR.InputDevice device;
    private bool isMenuOn = false;
    public bool IsMenuOn { get => isMenuOn; set { isMenuOn = value; } }

    // Pencil line color
    private Color lineColor = Color.black;
    public Color LineColor { get => lineColor; set { lineColor = value; } }
    private float lineWidth = 0.001f;
    public float LineWidth { get => lineWidth; set { lineWidth = value; } }

    // Pencil Menu info
    private Dictionary<string, float> presetLineWidths;
    // private Dictionary<string, List<Vector3>> presetGraphiteSizes;
    private Dictionary<string, List<Vector3>> presetMarkerTipSizes;
    private Dictionary<string, float> presetPencilRayLengths;
    private Dictionary<string, Vector3> presetPencilTargetSizes;
    private List<GameObject> pencilColors;
    private List<GameObject> pencilWidths;


    // Start is called before the first frame update
    void Start()
    {
        presetLineWidths = new Dictionary<string, float>() {
            {"Thin", 0.002f},
            {"Normal", 0.005f},
            {"Thick", 0.008f}
        };

        presetMarkerTipSizes = new Dictionary<string, List<Vector3>>() {
            {"Thin", new List<Vector3>() {
                new Vector3(0f, 0f, 0.1666f), // position
                new Vector3(0.5f, 0.5f, 2f) // scale
                }
            },
            {"Normal", new List<Vector3>() {
                new Vector3(0f, 0f, 0.1924f), // position
                new Vector3(0.7f, 0.7f, 1f) // scale
                }
            },
            {"Thick", new List<Vector3>() {
                new Vector3(0f, 0f, 0.1882f), // position
                new Vector3(1f, 1f, 1f) // scale
                }
            },
        };

        // presetGraphiteSizes = new Dictionary<string, List<Vector3>>() {
        //     {"Thin", new List<Vector3>() {
        //         new Vector3(0f, 0.7f, 0f), // position
        //         new Vector3(0.5f, 0.5f, 0.5f) // scale
        //         }
        //     },
        //     {"Normal", new List<Vector3>() {
        //         new Vector3(0f, 1.008f, 0f), // position
        //         new Vector3(0.5f, 0.1692894f, 0.5f) // scale
        //         }
        //     },
        //     {"Thick", new List<Vector3>() {
        //         new Vector3(0f, 1.11f, 0f), // position
        //         new Vector3(0.5f, 0.05454505f, 0.5f) // scale
        //         }
        //     },
        // };

        presetPencilRayLengths = new Dictionary<string, float>() {
            {"Thin", 0.2f},
            {"Normal", 0.2f},
            {"Thick", 0.2f}
        };

        presetPencilTargetSizes = new Dictionary<string, Vector3>() {
            {"Thin", new Vector3(1f, 1f, 1f)},
            {"Normal", new Vector3(2f, 2f, 2f)},
            {"Thick", new Vector3(3.25f, 3.25f, 3.25f)}
        };


        // Set up references to each pencil color pad gameobject
        pencilColors = new List<GameObject>();
        for (int i = 0; i < pencilMenuColors.transform.childCount; i++)
        {
            GameObject child = pencilMenuColors.transform.GetChild(i).gameObject;
            pencilColors.Add(child);
        }

        // Set up references to each pencil width pad gameobject
        pencilWidths = new List<GameObject>();
        for (int i = 0; i < pencilMenuWidths.transform.childCount; i++)
        {
            GameObject child = pencilMenuWidths.transform.GetChild(i).gameObject;
            pencilWidths.Add(child);
        }

        pencilMenuObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Toggles the Marker Menu on and off
    /// </summary>
    public void togglePencilMenu()
    {
        isMenuOn = !isMenuOn;
        if (isMenuOn)
        {
            pencilMenuObject.SetActive(true);
        }
        else
        {
            pencilMenuObject.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles the Marker Menu on and off based on the passed in boolean.
    /// </summary>
    /// <param name="val">Boolean for whether the Marker Menu is on.</param>
    public void togglePencilMenu(bool val)
    {
        isMenuOn = val;
        pencilMenuObject.SetActive(val);
    }

    // Checking for triggering color change when pencil touches color
    IEnumerator OnTriggerEnter(Collider other)
    {
        Debug.Log("hitting object: " + other.gameObject.name);
        if (other.tag == "PencilColor")
        {
            Material lineColorMat = other.GetComponent<MeshRenderer>().material; // new Material color
            lineColor = lineColorMat.color;

            // Change pencil outer color            
            Material[] matCopy = transform.GetChild(0).GetComponent<MeshRenderer>().materials; // change the marker body's color around the tip
            matCopy[1] = other.GetComponent<MeshRenderer>().material;
            transform.GetChild(0).GetComponent<MeshRenderer>().materials = matCopy;

            markerTip.GetComponent<MeshRenderer>().material = lineColorMat; // change the marker tip's color
            pencilTarget.transform.GetChild(0).GetComponent<SpriteRenderer>().color = lineColor; // change pencil target color
            markerCap.GetComponent<MeshRenderer>().material = lineColorMat; // change the marker cap's color

            updateOutline(other);

        }
        else if (other.tag == "PencilWidth")
        {
            string name = other.gameObject.name;
            if (presetLineWidths.ContainsKey(name))
            {
                lineWidth = presetLineWidths[name]; // change pencil line width

                markerTip.transform.localPosition = presetMarkerTipSizes[name][0]; // change marker tip size
                markerTip.transform.localScale = presetMarkerTipSizes[name][1];

                this.gameObject.GetComponent<LineMeshDraw>().pencilRayLength = presetPencilRayLengths[name]; // change pencil ray length
                pencilTarget.transform.localScale = presetPencilTargetSizes[name]; // change pencil target size
            }
            updateOutline(other);
        }
        else if (other.tag == "PencilLineToggle")
        {
            gameObject.GetComponent<LineMeshDraw>().HideAllLinesToggle();
            other.gameObject.GetComponent<ToggleMenu>().toggleMenu();
            // other.isTrigger = false;
            yield return new WaitForSeconds(0.5f);
            // other.isTrigger = true;
        }
    } 

    /// <summary>
    /// Function to outline the selected option on the Pencil Menu.
    /// </summary>
    void updateOutline(Collider other)
    {
        if (other.tag == "PencilColor")
        {
            foreach (GameObject colorPad in pencilColors)
            {
                // colorPad.GetComponent<Outline>().enabled = false; // disable outline for all color pads
                colorPad.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            }
            // other.gameObject.GetComponent<Outline>().enabled = true; // enable outline for the touched color pad
            other.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        }
        else if (other.tag == "PencilWidth")
        {
            foreach (GameObject widthPad in pencilWidths)
            {
                // widthPad.GetComponent<Outline>().enabled = false; // disable outline for all width pads
                widthPad.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
            }
            // other.gameObject.GetComponent<Outline>().enabled = true; // enable outline for the touched width pad
            other.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
        }
    }

    /// <summary>
    /// Function to hide the marker cap.
    /// </summary>
    public void hideMarkerCap(bool value)
    {
        if (value)
        {
            markerCap.SetActive(false);
        }
        else 
        {
            markerCap.SetActive(true);
        }
    }
}
