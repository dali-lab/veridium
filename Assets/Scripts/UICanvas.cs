using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

public class UICanvas : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Color lineColor = Color.black;
    private Color[] lineColors = { Color.black, Color.red, Color.green, Color.blue };

    public Color LineColor { get => lineColor; set { lineColor = value; } }
    public void ColorPick(int colorIndex)
    {
        lineColor = lineColors[colorIndex];
    }
}
