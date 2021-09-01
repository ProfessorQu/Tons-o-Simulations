using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public ComputeShader shader;
    ComputeBuffer buffer;

    int kernel;
    int[] output;

    RenderTexture texture;

    [Min(1)] public int width;
    [Min(1)] public int height;


    private void Awake()
    {
        // Get kernel hand;e
        kernel = shader.FindKernel("GameOfLife");

        // Init buffer
        buffer = new ComputeBuffer(width * height, sizeof(int));

        // Set buffer
        shader.SetBuffer(kernel, "Result", buffer);

        // Set width and height
        shader.SetInt("Width", width);
        shader.SetInt("Height", height);

        // Calculate thread groups
        int xGroups = Mathf.CeilToInt(width / 8f);
        int yGroups = Mathf.CeilToInt(height / 8f);

        // Run shader
        shader.Dispatch(kernel, xGroups, yGroups, 1);

        // Init output
        output = new int[width * height];

    }

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
        // Set buffer
        shader.SetBuffer(kernel, "Result", buffer);

        // Calculate thread groups
        int xGroups = Mathf.CeilToInt(width / 8f);
        int yGroups = Mathf.CeilToInt(height / 8f);

        // Run shader
        shader.Dispatch(kernel, xGroups, yGroups, 1);

        // Get data to output
        buffer.GetData(output);

        // Display texture
        // Graphics.Blit(texture, destination);
    }

	private void OnDisable()
	{
        if (buffer != null)
        {
            buffer.Release();
            buffer = null;
        }
	}
}
