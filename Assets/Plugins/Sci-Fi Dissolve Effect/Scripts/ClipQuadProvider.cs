using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ClipQuadProvider : MonoBehaviour
{
    [SerializeField] private Renderer clipQuadRenderer;
    [SerializeField] private Renderer[] targetRenderers;
    [SerializeField] private string propertyName = "_ClipQuadMatrix";

    private MaterialPropertyBlock materialPropertyBlock;
    private int propertyID;

    private void OnEnable()
    {
        materialPropertyBlock = new MaterialPropertyBlock();
        propertyID = Shader.PropertyToID(propertyName);
    }

    private void OnValidate()
    {
        propertyID = Shader.PropertyToID(propertyName);
    }

    private void Update()
    {
        SetClipQuad();
    }

    private void LateUpdate()
    {
        SetClipQuad();
    }

    private void SetClipQuad()
    {
        if (clipQuadRenderer == null)
            return;
        
        if (targetRenderers == null)
            return;

        var clipQuadMatrix = clipQuadRenderer.worldToLocalMatrix;


        foreach(var r in targetRenderers)
        {
            if (r == null)
                continue;

            r.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetMatrix(propertyID, clipQuadMatrix);
            r.SetPropertyBlock(materialPropertyBlock);
        }
    }
}
