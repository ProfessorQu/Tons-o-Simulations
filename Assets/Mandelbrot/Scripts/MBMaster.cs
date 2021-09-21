using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class MBMaster: MonoBehaviour
{
    // Set camera
    Camera cam;

    // Set shader and kernel
    public ComputeShader shader;

    // Set groups
    int xGroups;
    int yGroups;

    RenderTexture set;

    // Set material
    private Material mat;

    public int maxIterations;

    public Vector4 area = new Vector4(0, 0, 1, 1);

    private void Setup()
    {
        cam = Camera.current;

        int width = cam.pixelWidth;
        int height = cam.pixelHeight;

        // Calculate kernel groups
        xGroups = Mathf.CeilToInt(width / 8f);
        yGroups = Mathf.CeilToInt(height / 8f);

        set = new RenderTexture(width, height, 24);
        set.enableRandomWrite = true;
        set.Create();

        // Set width and height
        shader.SetInt("_Width", width);
        shader.SetInt("_Height", height);

        shader.SetInt("_MaxIterations", maxIterations);

        shader.SetVector("_Area", area);
    }

    void HandleInputs(){
        if (Input.GetKey(KeyCode.LeftBracket))
        {
            area.z *= 0.99f;
            area.w *= 0.99f;
        }
        else if (Input.GetKey(KeyCode.RightBracket)){
            area.z *= 1.01f;
            area.w *= 1.01f;
        }

        if(Input.GetKey(KeyCode.A)){
            area.x -= 0.01f * area.z;
        }
        if (Input.GetKey(KeyCode.D))
        {
            area.x += 0.01f * area.z;
        }
        if (Input.GetKey(KeyCode.W))
        {
            area.y += 0.01f * area.z;
        }
        if (Input.GetKey(KeyCode.S))
        {
            area.y -= 0.01f * area.z;
        }
    }

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Setup();

        shader.SetTexture(0, "Result", set);
        shader.Dispatch(0, xGroups, yGroups, 1);

        Graphics.Blit(set, destination);

    }

	private void Update()
    {
        HandleInputs();
    }

	private void OnDestroy()
    {
        // Release textures
        set.Release();
    }
}
