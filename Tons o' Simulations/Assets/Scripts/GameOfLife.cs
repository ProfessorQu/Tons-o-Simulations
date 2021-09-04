using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public ComputeShader shader;

    int kernel;

    public RenderTexture cellPrevious;
    RenderTexture cellNew;

    RenderTexture cellTexture;

    [Min(1)] public int cellAmount = 1;
    [Min(1)] public int cellSize = 1;

    private void SetupTextures()
    {
        // Init cells
        cellPrevious = new RenderTexture(cellAmount, cellAmount, 24);
        cellPrevious.enableRandomWrite = true;
        cellPrevious.Create();

        // Init cells
        cellNew = new RenderTexture(cellAmount, cellAmount, 24);
        cellNew.enableRandomWrite = true;
        cellNew.Create();


        // Init cells
        cellTexture = new RenderTexture(cellAmount * cellSize, cellAmount * cellSize, 24);
        cellTexture.enableRandomWrite = true;
        cellTexture.Create();
    }

    private void Awake()
    {
        // Get kernel hand;e
        kernel = shader.FindKernel("GameOfLife");

        // Set cell amount and cell size
        shader.SetInt("_CellSize", cellSize);

        SetupTextures();

        // Set cells
        shader.SetTexture(kernel, "_CellPrevious", cellPrevious);
        shader.SetTexture(kernel, "_CellNew", cellNew);

        shader.SetTexture(kernel, "_CellTexture", cellTexture);

        // Calculate thread groups
        int groups = Mathf.CeilToInt(cellAmount / 8f);

        // Run shader
        shader.Dispatch(kernel, groups, groups, 1);
    }

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Set cell amount and cell size
        shader.SetInt("_CellSize", cellSize);

        // Set cells
        shader.SetTexture(kernel, "_CellPrevious", cellPrevious);
        shader.SetTexture(kernel, "_CellNew", cellNew);

        shader.SetTexture(kernel, "_CellTexture", cellTexture);

        // Calculate thread groups
        int groups = Mathf.CeilToInt(cellAmount / 8f);

        // Run shader
        shader.Dispatch(kernel, groups, groups, 1);

        // Display texture
        Graphics.Blit(cellTexture, destination);

        cellPrevious = cellNew;
    }
}
