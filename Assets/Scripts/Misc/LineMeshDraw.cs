using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using EasyCurvedLine;
using System;

public class LineMeshDraw: MonoBehaviour
{
    // Note: Need to have a "draw" layer
    #region Variables

    // [SerializeField] Canvas userCanvas; 
    [SerializeField] Material lineMaterial; //the shader we will be applying to the line;
    [SerializeField] private GameObject linePoint;
    [SerializeField] private float lineHoverAmount = 0.0001f; // controls how much the drawn line hovers above the object
    [SerializeField] private float distThreshold = 0.002f; // controls the distance between two line points
    [SerializeField] private Slider toothScaleSlider; // controls the scale of the tooth
    [SerializeField] private Camera mainCamera; // controls whether the -drawpart- line is visible or not
    [SerializeField] private float pencilHapticsStrength = 0.08f;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject pencilTarget;

    /// <summary>
    /// The ray length from the back of the pencil to the target point.
    /// </summary>
    public float pencilRayLength = 0.2f;

    /// <summary>
    /// The ray length from the back of the pencil to snap the pencil to the target
    /// </summary>
    public float markerSnapLength = 0.13f;

    private Vector3 offset = new Vector3(-1.0f, -0.4f, -10.0f); // specific to this pencil to align the XR Ray and the pencil

    // Line Renderer Variables
    private LineRenderer lineRenderer;
    private CurvedLineRenderer curvedLineRenderer;
    private LineRenderer pencilLineRenderer => GetComponent<LineRenderer>();
    private PencilMenu pencilMenu => GetComponent<PencilMenu>();
    private float markerLength; // length of our marker
    private float markerPushDepth = 0.02f; // how far our interactor can go into the drawable object before warning begins to show up
    private float markerWarningRateOffset = 1f; // how "sensitive" the warning for the marker is
    private float lineWidth = 0.002f;

    private GameObject container;
    private Mesh mesh;
    private bool drawing = false;
    private string drawnOnObjectTag;
    private Vector3 drawnOnObjectCenter;
    private bool triggerDown = false;
    private bool grabbed = false;
    private bool showWarning = false; // Whether we should show a warning for the interactor being too into the drawable object
    private bool isMarker = false; // Checking if the object we grabbed is the marker
    private int layerMask = (1 << 13); // can only draw on objects with "drawable" layer

    //Interframe flags and variables
    private float lastTime = 0f;
    private float scale = 1.0f;

    // XR Toolkit variables
    private XRGrabInteractable grabInteractable => GetComponent<XRGrabInteractable>();
    private ActionBasedController controller;
    private Transform grabHandle => grabInteractable.attachTransform;
    private Quaternion originalAttachLocalRotation;
    private Vector3 originalAttachLocalPosition;
    private float originalMaxRaycastDistance;
    private Vector3 lastPoint;
    private Vector3 secondLastPoint;
    private RaycastHit lastRaycast;

    // private GameObject spherePoint;
    // private GameObject spherePoint2;
    private Vector3 firstRayHit;
    // private bool gotLastRaycast = false;
    // private bool finishedInterPoints = true;
    #endregion

    void Start()
    {
        pencilTarget.SetActive(false); // disable the pencil target at the beginning
        markerLength = transform.GetComponent<CapsuleCollider>().height * transform.localScale.z - 0.01f;
    }

    #region ListenerAssignment
    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(Grab);
        grabInteractable.selectExited.AddListener(Drop);
        grabInteractable.activated.AddListener(Activate);
        grabInteractable.deactivated.AddListener(Deactivate);
        pencilLineRenderer.enabled = false;
        playerInput.actions.FindActionMap("PencilActions").Disable();
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(Grab);
        grabInteractable.selectExited.RemoveListener(Drop);
        grabInteractable.activated.RemoveListener(Activate);
        grabInteractable.deactivated.RemoveListener(Deactivate);
    }
    #endregion

    #region Construction
    private void Grab(SelectEnterEventArgs args)
    {
        playerInput.actions.FindActionMap("PencilActions").Enable();
        playerInput.actions.FindActionMap("Default").Disable();

        // Get reference to controller
        controller = args.interactor.GetComponent<ActionBasedController>();

        grabbed = true;

        if (grabInteractable.gameObject == gameObject)
        {
            isMarker = true;
            GetComponent<Outline>().OutlineColor = new Color(0f, 0f, 255f, 0f); // Change outline color to blue from now on, since marker is grabbed
        }

    }

    private void Drop(SelectExitEventArgs args)
    {
        pencilMenu.togglePencilMenu(false);
        playerInput.actions.FindActionMap("PencilActions").Disable();
        playerInput.actions.FindActionMap("Default").Enable();

        grabbed = false;

        if (grabInteractable.gameObject == gameObject)
        {
            isMarker = false;
            GetComponent<Outline>().OutlineColor = new Color(255f, 245f, 0f); // Change outline color back to yellow color
        }
    }
    #endregion

    #region Line Drawing

    private void Activate(ActivateEventArgs args)
    {
        RaycastHit raycastHit;
        triggerDown = true;

        if (Physics.Raycast(GetMarkerStartPos(), transform.TransformDirection(Vector3.forward), out raycastHit, pencilRayLength, layerMask) && grabbed && isMarker)
        {
            drawing = true;
            Debug.Log(raycastHit.collider.gameObject.name);

            firstRayHit = raycastHit.point;
            // spherePoint.transform.position = firstRayHit;

            // if the object we are trying to draw on is already grabbed, don't allow drawing  
            XRBaseInteractable[] interactables = raycastHit.collider.GetComponentsInParent<XRBaseInteractable>(); 
            foreach (XRBaseInteractable interactable in interactables)
            {
                if (interactable.isSelected) drawing = false;
            }

            drawnOnObjectTag = raycastHit.collider.gameObject.tag;
            drawnOnObjectCenter = raycastHit.collider.bounds.center;
            InstantializeDrawPart(raycastHit);
        }
    }

    private void Deactivate(DeactivateEventArgs args = null)
    {
        GenerateDrawPartCollider();
        
        ShiftMarkerAndHandsBack();
        triggerDown = false;
        drawing = false;
        drawnOnObjectTag = "";
    }

    /// <summary>
    /// Creates a container called "DrawPart - " if it doesn't exist yet, which holds the lineRenderer and curvedLineRenderer.
    /// </summary>
    void InstantializeDrawPart(RaycastHit raycastHit)
    {
        scale = toothScaleSlider.value;

        if (!container)
        {
            container = new GameObject("DrawPart - ");
            Debug.Log("Drawing on : " + raycastHit.transform.name);
            container.transform.SetParent(raycastHit.transform);
            container.tag = "DrawPart";

            container.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = container.AddComponent<MeshRenderer>();
            meshRenderer.material = lineMaterial;
            meshRenderer.material.color = pencilMenu.LineColor;

            container.layer = LayerMask.NameToLayer("draw");
            Debug.Log("Container Layer: " + LayerMask.LayerToName(container.layer) + "; layer number; " + container.layer);
            container.AddComponent<MeshCollider>();
            mesh = new Mesh();
            lineWidth = pencilMenu.LineWidth;
            pencilLineRenderer.enabled = true;

            // Making Pencil Line renderer invisible
            pencilLineRenderer.startWidth = 0f;
            pencilLineRenderer.endWidth = 0f;

            pencilLineRenderer.SetPosition(0, GetMarkerStartPos());
        }
    }

    /// <summary>
    /// Takes in the current lineRenderer, bakes the mesh and adds a collider.
    /// </summary>
    void GenerateDrawPartCollider()
    {
         // generate the collider after drawing is done
        if (lineRenderer != null)
        {
            container.GetComponent<MeshCollider>().sharedMesh = mesh;
        }
        lineRenderer = null;
        pencilLineRenderer.enabled = false;
        container = null;
    }

    /// <summary>
    /// The Vector3 position of the top end of the marker.
    /// </summary>
    Vector3 GetMarkerStartPos()
    {
        return transform.position - transform.TransformDirection(new Vector3(0f, 0f, transform.GetComponent<CapsuleCollider>().height / 8f));
    }

    Vector3 GetMarkerTip()
    {
        return transform.position + transform.TransformDirection(new Vector3(0f, 0f, transform.GetComponent<CapsuleCollider>().height / 6f));
    }

    /// <summary>
    /// Shifts the marker and the Oculus hand appropriately.
    /// </summary>
    void ShiftMarkerAndHands(RaycastHit raycastHit)
    {
        float raycastFullDist = raycastHit.distance;
        float offsetDist = raycastFullDist - markerLength;

        controller.GetComponent<XRDirectInteractor>().attachTransform.position += transform.TransformDirection(new Vector3(0f, 0f, lineHoverAmount * 2 + offsetDist));
        controller.transform.GetChild(0).position += transform.TransformDirection(new Vector3(0f, 0f, lineHoverAmount * 2 + offsetDist));
        if (!showWarning) transform.GetComponent<ToggleOutline>().toggleOutline(false);
    }

    void ShiftMarkerAndHandsFromTip(RaycastHit raycastHit)
    {
        float raycastFullDist = raycastHit.distance;
        float offsetDist = raycastFullDist - 0.006f;
        controller.GetComponent<XRDirectInteractor>().attachTransform.position += transform.TransformDirection(new Vector3(0f, 0f, lineHoverAmount * 2 + offsetDist));
        controller.transform.GetChild(0).position += transform.TransformDirection(new Vector3(0f, 0f, lineHoverAmount * 2 + offsetDist));
        if (!showWarning) transform.GetComponent<ToggleOutline>().toggleOutline(false);
    }

    /// <summary>
    /// Shifts the marker and the Oculus hand back to their original location.
    /// </summary>
    void ShiftMarkerAndHandsBack()
    {
        controller.GetComponent<XRDirectInteractor>().attachTransform.localPosition = new Vector3(0f, 0f, 0f);
        controller.transform.GetChild(0).localPosition = new Vector3(0f, 0f, 0f);
        if (!showWarning) transform.GetComponent<ToggleOutline>().toggleOutline(false);
    }

    void Update()
    {
        RaycastHit raycastHit;
        pencilLineRenderer.SetPosition(0, GetMarkerStartPos());
        
        if (controller)
        {
            // For showing warning when user hand/marker is too deep into the drawn-on object
            if (Physics.Raycast(controller.transform.position - transform.TransformDirection(new Vector3(0f, 0f, transform.GetComponent<CapsuleCollider>().height / 8f)), 
                transform.TransformDirection(Vector3.forward), out raycastHit, markerLength - markerPushDepth, layerMask) && isMarker)
            {
                float colorScale = Math.Max(1f - ((raycastHit.distance / (markerLength - markerPushDepth)) - markerWarningRateOffset * 0.1f), 0f);
                GetComponent<Outline>().OutlineColor = new Color(0f, 0f, 255f, colorScale); // Change outline color to blue from now on, since marker is grabbed
                GetComponent<ToggleOutline>().toggleOutline(true);
                showWarning = true;
            }
            else
            {
                showWarning = false;
            }

            // For autosnapping marker when close enough to object
            if (Physics.Raycast(controller.transform.position - transform.TransformDirection(new Vector3(0f, 0f, transform.GetComponent<CapsuleCollider>().height / 8f)), 
                transform.TransformDirection(Vector3.forward), out raycastHit, markerSnapLength, layerMask) && !triggerDown && !drawing && isMarker) // if the marker snap ray hits something
            {

                Physics.Raycast(GetMarkerStartPos(), transform.TransformDirection(Vector3.forward), out raycastHit, pencilRayLength, layerMask);
                ShiftMarkerAndHands(raycastHit);
            }
            else if (grabbed && !triggerDown && !drawing && isMarker)
            {
                ShiftMarkerAndHandsBack();
            }
        
            // For drawing the pencil target when close enough to object
            if (Physics.Raycast(GetMarkerStartPos(), transform.TransformDirection(Vector3.forward), out raycastHit, pencilRayLength, layerMask) && isMarker) // if the pencil ray hits something
            {
                pencilTarget.SetActive(true);
                Vector3 newPos = raycastHit.point;
                pencilTarget.transform.rotation = Quaternion.FromToRotation(transform.forward, raycastHit.normal) * transform.rotation;
                pencilTarget.transform.position = newPos + pencilTarget.transform.TransformDirection(new Vector3(0f, 0f, lineHoverAmount * 2));
            }
            else
            {
                pencilTarget.SetActive(false);
                drawing = false; // User needs to re-click trigger to draw again
            }
        }

        if (triggerDown && isMarker)
        {
            if (drawing)
            {
                Physics.Raycast(GetMarkerStartPos(), transform.TransformDirection(Vector3.forward), out raycastHit, pencilRayLength, layerMask); // lock the pen in the correct place
                ShiftMarkerAndHands(raycastHit); // shift the pen accordingly

                // Send a ray from the marker tip instead of the back end of the marker
                // If the ray hits something and that something is the object that the user initially started drawing on
                if (Physics.Raycast(GetMarkerTip(), transform.TransformDirection(Vector3.forward), out raycastHit, 4f, layerMask) && raycastHit.collider.gameObject.tag == drawnOnObjectTag)
                {
                    pencilTarget.SetActive(true);
                    Vector3 newPos = raycastHit.point;

                    pencilTarget.transform.rotation = Quaternion.FromToRotation(transform.forward, raycastHit.normal) * transform.rotation;
                    pencilTarget.transform.position = newPos + pencilTarget.transform.TransformDirection(new Vector3(0f, 0f, lineHoverAmount * 2));

                    InstantializeDrawPart(raycastHit);

                    if ((Vector3.Distance(lastPoint, raycastHit.point) > distThreshold))
                    {
                        AddNewMeshPoint(ref mesh, raycastHit);

                        container.GetComponent<MeshFilter>().sharedMesh = mesh;
                        if (mesh.vertices.Length >= 3)
                        {
                            // container.GetComponent<MeshCollider>().sharedMesh.Clear(true);
                            container.GetComponent<MeshCollider>().sharedMesh = mesh;
                        }

                        Vector3 newPoint = raycastHit.point + raycastHit.normal * lineHoverAmount;
                        lastPoint = raycastHit.point;
                        pencilLineRenderer.SetPosition(1, newPoint);
                    }
                    
                    // Send haptics
                    controller.SendHapticImpulse(pencilHapticsStrength, Time.fixedDeltaTime);
                }
                else
                {
                    GenerateDrawPartCollider();
                }
            }
            else
            {
                pencilLineRenderer.SetPosition(1, transform.position + transform.TransformDirection(new Vector3(0f, 0f, pencilRayLength))); // Pencil ray in world Z-axis for marker
                GenerateDrawPartCollider();
            }
        }
    }

    private void AddNewMeshPoint(ref Mesh mesh, RaycastHit raycastHit)
    {
        List<Vector3> vertices = new List<Vector3>(mesh.vertices);
        List<Vector3> normals = new List<Vector3>(mesh.normals);
        List<int> triangles = new List<int>(mesh.triangles);

        // get perpendicular to direction we are drawing
        Vector3 tangent = Vector3.Cross(raycastHit.point - lastPoint, raycastHit.normal).normalized * lineWidth * (scale / toothScaleSlider.maxValue) / 2f;
        int lastVertexIndex = mesh.vertexCount - 1;

        Vector3 leftVertexPoint = container.transform.InverseTransformPoint(raycastHit.point + tangent + raycastHit.normal * lineHoverAmount * (0.5f * scale));
        Vector3 rightVertexPoint = container.transform.InverseTransformPoint(raycastHit.point - tangent + raycastHit.normal * lineHoverAmount * (0.5f * scale));

        // check if this is the first point to add
        if (mesh.vertexCount == 0)
        {
            vertices.Add(leftVertexPoint);
            vertices.Add(rightVertexPoint);
            normals.Add(raycastHit.normal);
            normals.Add(raycastHit.normal);
            lastVertexIndex = 1;
        }

        vertices.Add(leftVertexPoint);
        vertices.Add(rightVertexPoint);
        normals.Add(raycastHit.normal);
        normals.Add(raycastHit.normal);
        triangles.AddRange(new int[] { lastVertexIndex - 1, lastVertexIndex, lastVertexIndex + 2 });
        triangles.AddRange(new int[] { lastVertexIndex - 1, lastVertexIndex + 2, lastVertexIndex + 1 });
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
    }
    #endregion

    #region Other Public Methods
    public void HideAllLinesToggle()
    {
        mainCamera.cullingMask = mainCamera.cullingMask ^ (1 << LayerMask.NameToLayer("draw"));
    }
    #endregion
}
