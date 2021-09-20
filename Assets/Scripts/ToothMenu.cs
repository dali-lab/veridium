using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToothMenu : MonoBehaviour
{
    [SerializeField] private GameObject toothObject;
    public GameObject ToothObject { get => toothObject; set { toothObject = value; } }

    [SerializeField] private GameObject toothCanvas;
    [SerializeField] private GameObject toggleButtonPrefab;

    private Dictionary<GameObject, List<GameObject>> features;
    // private List<GameObject> features; // Cusps, Fossa, Grooves, Pits, Ridges
    private List<GameObject> parts; // Dentin, Enamel, Pulp

    // Canvas variables
    private Dictionary<string, GameObject> canvasParts;
    private float canvasWidth;
    private float canvasHeight;

    // Start is called before the first frame update
    void Start()
    {
        canvasParts = new Dictionary<string, GameObject>();
        canvasWidth = toothCanvas.GetComponent<RectTransform>().rect.width;
        canvasHeight = toothCanvas.GetComponent<RectTransform>().rect.height;

        // Grab all features
        // features = new List<GameObject>();
        features = new Dictionary<GameObject, List<GameObject>>();
        foreach (Transform child in toothObject.transform.GetChild(0).GetChild(0)) // toothObject -> features -> Landmarks -> list
        {
            List<GameObject> featureList = new List<GameObject>();
            features.Add(child.gameObject, featureList);

            foreach (Transform featurePart in child)
            {
                featureList.Add(featurePart.gameObject);
            }
            // GameObject go = new GameObject(child.gameObject.name);
            // go.transform.parent = toothCanvas.transform;
            // go.transform.localPosition = toothCanvas.transform.localPosition;
            // go.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            // canvasParts.Add(child.gameObject.name, go);
        }

        // Grab all parts
        parts =  new List<GameObject>();
        foreach (Transform child in toothObject.transform.GetChild(1)) // toothObject -> parts -> list
        {
            parts.Add(child.gameObject);
        }

        InstantiateFeatureButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Function to instantiate the feature UI buttons
    void InstantiateFeatureButtons()
    {
        float canvasYInterval = (canvasHeight - 0.3f) / features.Count;
        float dY = 0f;
        foreach (GameObject feature in features.Keys)
        {
            GameObject go = Instantiate(toggleButtonPrefab, toothCanvas.transform.position, toothCanvas.transform.rotation);
            go.name = feature.name;
            go.transform.parent = toothCanvas.transform;
            go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = feature.name;
            canvasParts.Add(feature.name, go);

            // Position the object correctly
            go.transform.localPosition -= new Vector3(0.75f, -0.4f, 0f);
            go.transform.Rotate(new Vector3(-90f, 0f, 0f)); // Rotate it facing forward instead of up
            go.transform.localScale *= 0.25f; // Scale the button down

            go.transform.localPosition += new Vector3(0f, dY, 0f);
            dY -= canvasYInterval;
        }

        // canvasXInterval = (canvasWidth - 0.6f) / parts.Count;
        // dX = 0f;
        // foreach (GameObject part in parts)
        // {
        //     GameObject go = Instantiate(toggleButtonPrefab, toothCanvas.transform.position, toothCanvas.transform.rotation);
        //     go.name = part.name;
        //     go.transform.parent = toothCanvas.transform;
        //     go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = part.name;
        //     canvasParts.Add(part.name, go);

        //     // Position the object correctly
        //     go.transform.localPosition -= new Vector3(0.4f, -0.3f, 0f);
        //     go.transform.Rotate(new Vector3(-90f, 0f, 0f));
        //     go.transform.localScale *= 0.25f;
        //     go.transform.localPosition += new Vector3(dX, 0f, 0f);
        //     dX += canvasXInterval;
        // }
        InstantiateFeatureParts();
    }

    // Function to instantiate each feature subcomponents UI
    void InstantiateFeatureParts()
    {
        foreach (GameObject featureKey in features.Keys)
        {
            int numFeatureParts = features[featureKey].Count;
            float canvasYInterval, canvasXInterval, dX, dY = 0f;
            if (numFeatureParts < 6)
            {
                canvasYInterval = (canvasHeight) / 5;
                canvasXInterval = canvasWidth / 4;
            }
            else
            {
                canvasYInterval = (canvasHeight) / features[featureKey].Count;
            }

            foreach (GameObject featurePart in features[featureKey])
            {
                GameObject go = Instantiate(toggleButtonPrefab, canvasParts[featureKey.name].transform.position, canvasParts[featureKey.name].transform.rotation);
                go.name = featurePart.name;
                go.transform.parent = canvasParts[featureKey.name].transform;
                go.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = featurePart.name;
                canvasParts.Add(featurePart.name, go);

                // Position the object correctly
                go.transform.localPosition -= new Vector3(0f, 0f, 0f);
                go.transform.Rotate(new Vector3(90f, 0f, 0f)); // Rotate it facing forward instead of up
                go.transform.localScale *= 0.25f; // Scale the button down
                // Debug.Log(dY);

                go.transform.localPosition += new Vector3(0f, dY, 0f);
                dY -= canvasYInterval;
            }
        }
    }

}
