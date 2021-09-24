using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode, RequireComponent(typeof(Camera))]
public class MBMaster: MonoBehaviour
{
    // Set camera
    public Camera cam;
    // Set shader and kernel
    public ComputeShader shader;

    // Set groups
    int xGroups;
    int yGroups;

    // Set width and height
    int width;
    int height;

    // Set aspect ratio and minimum zoom
    public float aspectRatio = -1;
    public float minZoom = 5e-5f;

    // Set rendertexture
    RenderTexture tex;

    // Set max iterations
    public int maxIterations = 200;

    // Set the area, the x and y positions and x and y zooms
    public Vector4 area = new Vector4(0, 0, 1, 1);


    void Start()
    {
        Init();
	}

    private void Init()
    {
        // Setting width and height
        Debug.Log("Setting width and height...");
        width = cam.pixelWidth;
        height = cam.pixelHeight;

        // Calculating thread groups
        Debug.Log("Calculating thread groups...");
        // Calculate kernel groups
        xGroups = Mathf.CeilToInt(width / 8f);
        yGroups = Mathf.CeilToInt(height / 8f);

        // Calculate aspect ratio
        if (aspectRatio == -1)
        {
            Debug.Log("Setting aspect ratio...");

            aspectRatio = (float)width / (float)height;

            if (aspectRatio > 0)
            {
                area.z *= aspectRatio;
            }
            else
            {
                area.w *= aspectRatio;
            }
        }

        // Create render texture
        if (tex == null || tex.width != width || tex.height == height)
        {
            Debug.Log("Creating texture...");

            tex = new RenderTexture(width, height, 24);
            tex.enableRandomWrite = true;
            tex.Create();
        }

        // Set shader parameters
        SetShaderParameters();
    }

    void SetShaderParameters()
    {
        Debug.Log("Setting shader parameters...");
        // Set width and height
        shader.SetInt("_Width", width);
        shader.SetInt("_Height", height);

        // Set max iterations
        shader.SetInt("_MaxIterations", maxIterations);

        // Set area
        shader.SetVector("_Area", area);

        // Set result
        shader.SetTexture(0, "Result", tex);
    }

    void HandleInputs()
    {
        // Checking for inputs
        Debug.Log("Checking for inputs...");

        // Zoom in
        if (area.z * 0.99f > minZoom || area.w * 0.99f > minZoom)
        {
            if (Input.GetKey(KeyCode.LeftBracket))
            {
                area.z *= 0.99f;
                area.w *= 0.99f;
            }
        }

        // Zoom out
        if (Input.GetKey(KeyCode.RightBracket))
        {
            area.z *= 1.01f;
            area.w *= 1.01f;
        }

        // Move to the left
        if (Input.GetKey(KeyCode.A)){
            area.x -= 0.01f * area.z;
        }
        // Move to the right
        if (Input.GetKey(KeyCode.D))
        {
            area.x += 0.01f * area.z;
        }
        // Move to the top
        if (Input.GetKey(KeyCode.W))
        {
            area.y += 0.01f * area.z;
        }
        // Move to the bottom
        if (Input.GetKey(KeyCode.S))
        {
            area.y -= 0.01f * area.z;
        }
    }

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Debug.Log("Rendering texture...");
        Init();

        Debug.Log("Dispatching shader...");
        shader.Dispatch(0, xGroups, yGroups, 1);

        // Rendering texture
        Graphics.Blit(tex, destination);

    }

	private void Update()
    {
        // Handle inputs
        HandleInputs();
    }

	private void OnDestroy()
    {
        // Release textures
        tex.Release();
    }
}
