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

    // Set texture input
    public Texture input;

    // Set pingpong bool
    private bool pingpong = true;
    // Set ping and pong RenderTextures
    RenderTexture pingTexture;
    RenderTexture pongTexture;

    // Set material
    private Material mat;

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

        // Set ping texture
        Graphics.Blit(input, pingTexture);

        // Get kernel
        kernel = shader.FindKernel("GameOfLife");

        // Set width and height
        shader.SetInt("Width", gridWidth);
        shader.SetInt("Height", gridHeight);

        // Get material
        mat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
    }

	public void Step()
	{
        if (pingpong)
        {
            // Set textures
            shader.SetTexture(kernel, "Input", pingTexture);
            shader.SetTexture(kernel, "Result", pongTexture);

            // Calculate groups
            int xGroups = Mathf.CeilToInt(gridWidth / 8f);
            int yGroups = Mathf.CeilToInt(gridHeight / 8f);

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

            // Calculate groups
            int xGroups = Mathf.CeilToInt(gridWidth / 8f);
            int yGroups = Mathf.CeilToInt(gridHeight / 8f);

            // Run shader
            shader.Dispatch(kernel, xGroups, yGroups, 1);

            // Set material
            mat.mainTexture = pongTexture;
        }

        // Invert pingpong
        pingpong = !pingpong;
    }

    private void OnDestroy()
    {
        // Release textures
        pingTexture.Release();
        pongTexture.Release();
    }
}
