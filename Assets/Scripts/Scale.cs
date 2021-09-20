using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class Scale : MonoBehaviour
{
    [SerializeField] GameObject part;
    [SerializeField] GameObject tool;
    [SerializeField] TextMeshProUGUI scaleValue;

    private float originalPartScale;
    private float originalToolScale;
    Slider scaleSlider => GetComponent<Slider>();

    public float ScaleValue { get => scaleSlider.value; }

    private void Start()
    {
        originalPartScale = part.transform.localScale.x;
        originalToolScale = tool.transform.localScale.x;
    }

    private void OnEnable()
    {
        scaleSlider.onValueChanged.AddListener(Rescale);
    }

    private void OnDisable()
    {
        scaleSlider.onValueChanged.RemoveListener(Rescale);
    }

    public void Rescale(float value)
    {
        float toolYOffset = tool.GetComponent<MeshFilter>().mesh.bounds.extents.y * (value - tool.transform.localScale.y / originalToolScale);
        tool.transform.Translate(new Vector3(0, toolYOffset, 0), Space.Self);
        part.transform.localScale = Vector3.one * (originalPartScale * value);
        int subMeshCount = part.GetComponentsInChildren<MeshCollider>().Length;
        tool.transform.localScale = Vector3.one * (originalToolScale * value);
        scaleValue.text = string.Format("{0:F0}", value);
        int lineRendererCount = part.GetComponentsInChildren<LineRenderer>().Length;
        for (int i = 0; i < lineRendererCount; i++)
        {
            part.GetComponentsInChildren<LineRenderer>()[i].widthMultiplier = value;
        }
    }
}
