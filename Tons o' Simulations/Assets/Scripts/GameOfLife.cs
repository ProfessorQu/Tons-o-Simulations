using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public ComputeShader shader;

    int kernel;

    RenderTexture cellTexture;
    RenderTexture cellRender;

    [Min(1)] public int cellAmount = 1;
    [Min(1)] public int cellSize = 1;

    private void Awake()
    {
        // Get kernel hand;e
        kernel = shader.FindKernel("GameOfLife");

        // Set cell amount and cell size
        shader.SetInt("_CellAmount", cellAmount);
        shader.SetInt("_CellSize", cellSize);

        // Init cells
        cellTexture = new RenderTexture(cellAmount, cellAmount, 24);
        cellTexture.enableRandomWrite = true;
        cellTexture.Create();

        // Set cells
        shader.SetTexture(kernel, "_CellTexture", cellTexture);

        // Calculate thread groups
        int groups = Mathf.CeilToInt(cellAmount / 8f);

        // Run shader
        shader.Dispatch(kernel, groups, groups, 1);
    }

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Set cell amount and cell size
        shader.SetInt("_CellAmount", cellAmount);
        shader.SetInt("_CellSize", cellSize);

        // Set cells
        shader.SetTexture(kernel, "_CellTexture", cellTexture);

        // Calculate thread groups
        int groups = Mathf.CeilToInt(cellAmount / 8f);

        // Run shader
        shader.Dispatch(kernel, groups, groups, 1);

        // Display texture
        Graphics.Blit(cellTexture, destination);
    }
}
