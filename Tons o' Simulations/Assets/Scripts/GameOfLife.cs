using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    // Set shader and kernel
    public ComputeShader shader;
    private int kernel;

    // Set grid width and height
    [Min(1)] public int gridWidth = 1;
    [Min(1)] public int gridHeight = 1;

    // Calculate groups
    int xGroups;
    int yGroups;

    // Set texture input
    // public Texture input;

    // Set pingpong bool
    private bool pingpong = true;
    // Set ping and pong RenderTextures
    RenderTexture pingTexture;
    RenderTexture pongTexture;

    // Set 2D texture
    public Texture2D tex2D;

    // Set material
    private Material mat;

    // Set divisions
    float widthDivide;
    float heightDivide;

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        // Set textures to null
        pingTexture = null;
        pongTexture = null;

        // Set material to null
        mat = null;

        // Reset pingpong
        pingpong = true;

        // Calculate kernel groups
        xGroups = Mathf.CeilToInt(gridWidth / 8f);
        yGroups = Mathf.CeilToInt(gridHeight / 8f);

        // Create ping texture
        pingTexture = new RenderTexture(gridWidth, gridHeight, 24);
        pingTexture.wrapMode = TextureWrapMode.Repeat;
        pingTexture.enableRandomWrite = true;
        pingTexture.filterMode = FilterMode.Point;
        pingTexture.Create();

        // Create pong texture
        pongTexture = new RenderTexture(gridWidth, gridHeight, 24);
        pongTexture.wrapMode = TextureWrapMode.Repeat;
        pongTexture.enableRandomWrite = true;
        pongTexture.filterMode = FilterMode.Point;
        pongTexture.Create();

        // Create texture 2D
        tex2D = new Texture2D(gridWidth, gridHeight, TextureFormat.ARGB32, false);

        // Get kernel
        kernel = shader.FindKernel("CSMain");

        // Set width and height
        shader.SetInt("Width", gridWidth);
        shader.SetInt("Height", gridHeight);

        // Get material
        mat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;

        // Calculate divisions
        widthDivide = (gridWidth - 1) / 20.0f;
        heightDivide = (gridHeight - 1) / 20.0f;


        // Init simulation
        int initKernel = shader.FindKernel("CSInit");

        // Set texture
        shader.SetTexture(initKernel, "Result", pingTexture);

        // Pass Random
        shader.SetFloat("Random", Random.Range(-10, 10));
        // Run init shader
        shader.Dispatch(initKernel, xGroups, yGroups, 1);
        // Render image
        Step();
    }

	public void Step()
	{

        if (pingpong)
        {
            // Set textures
            shader.SetTexture(kernel, "Input", pingTexture);
            shader.SetTexture(kernel, "Result", pongTexture);

            // Run shader
            shader.Dispatch(kernel, xGroups, yGroups, 1);

            // Set material
            mat.mainTexture = pingTexture;
        }
        else
        {
            // Set textures
            shader.SetTexture(kernel, "Input", pongTexture);
            shader.SetTexture(kernel, "Result", pingTexture);

            // Run shader
            shader.Dispatch(kernel, xGroups, yGroups, 1);

            // Set material
            mat.mainTexture = pongTexture;
        }

        // Invert pingpong
        pingpong = !pingpong;
    }

	private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Get mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Instantiate X and Y variables
            int cellX, cellY;

            // Scale 0 - 20  -->  0 - 7   (multiply by 0.35)
            // Scale 0 - 20  -->  0 - 15  (multiply by 0.75)
            //      15 / 20 = 0.75
            //      7  / 20 = 0.35
            cellX = Mathf.RoundToInt((mousePos.x + 10) * widthDivide);
            cellY = Mathf.RoundToInt((mousePos.y + 10) * heightDivide);

            // Test if the click is within bounds
            if (!(cellX > gridWidth - 1 || cellX < 0 || cellY > gridHeight - 1 || cellY < 0))
            {
                RenderTexture tex = (pingpong) ? pingTexture : pongTexture;
                // Set the active rendertexture
                RenderTexture.active = tex;

                // Read the pixels
                tex2D.ReadPixels(new Rect(0, 0, gridWidth, gridHeight), 0, 0);
                // Set pixel
                tex2D.SetPixel(cellX, cellY, new Color(1, 1, 1));
                // Apply texture
                tex2D.Apply();

                // Copy texture
                Graphics.Blit(tex2D, tex);

                // Unset the active rendertexture
                RenderTexture.active = null;

                Debug.Log(string.Format("Changed texture at ({0}, {1})", cellX, cellY));
            }
            else
            {
                Debug.Log(string.Format("({0}, {1}) is not in the grid", cellX, cellY));
			}
        }
    }

	private void OnDestroy()
    {
        // Release textures
        pingTexture.Release();
        pongTexture.Release();
    }
}
