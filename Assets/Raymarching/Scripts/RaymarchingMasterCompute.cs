using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class RaymarchingMasterCompute : MonoBehaviour
{
    public ComputeShader raymarching;

    RenderTexture target;
    Camera cam;

    void Init()
    {
        cam = Camera.current;
    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Init();

        InitRenderTexture();
        SetParameters();

        raymarching.SetTexture(0, "Source", source);
        raymarching.SetTexture(0, "Destination", target);

        int threadGroupsX = Mathf.CeilToInt(cam.pixelWidth / 8.0f);
        int threadGroupsY = Mathf.CeilToInt(cam.pixelHeight / 8.0f);
        raymarching.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        Graphics.Blit(target, destination);
    }

    void SetParameters()
    {
        raymarching.SetMatrix("_CameraToWorld", cam.cameraToWorldMatrix);
        raymarching.SetMatrix("_CameraInverseProjection", cam.projectionMatrix.inverse);
    }

    void InitRenderTexture()
    {
        if (target == null || target.width != cam.pixelWidth || target.height != cam.pixelHeight)
        {
            if (target != null)
            {
                target.Release();
            }
            target = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            target.enableRandomWrite = true;
            target.Create();
        }
    }
}