using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public ComputeShader shader;
    private int kernel;

    [Min(1)] public int gridWidth = 1;
    [Min(1)] public int gridHeight = 1;

    public Texture input;

    private bool pingpong = true;
    RenderTexture pingTexture;
    RenderTexture pongTexture;

    private Material mat;

    private void Start()
    {
        Setup();
    }

    public void Setup()
    {
        pingTexture = null;
        pongTexture = null;

        mat = null;

        SetupTextures();
        SetupShader();
        SetupQuad();
    }

	public void Step()
	{
        if (pingpong)
        {
            shader.SetTexture(kernel, "Input", pingTexture);
            shader.SetTexture(kernel, "Result", pongTexture);

            int xGroups = Mathf.CeilToInt(gridWidth / 8f);
            int yGroups = Mathf.CeilToInt(gridHeight / 8f);

            shader.Dispatch(kernel, xGroups, yGroups, 1);

            // Graphics.Blit(pongTexture, destination);
            mat.mainTexture = pingTexture;
        }
        else
        {
            shader.SetTexture(kernel, "Input", pongTexture);
            shader.SetTexture(kernel, "Result", pingTexture);

            int xGroups = Mathf.CeilToInt(gridWidth / 8f);
            int yGroups = Mathf.CeilToInt(gridHeight / 8f);

            shader.Dispatch(kernel, xGroups, yGroups, 1);

            // Graphics.Blit(pingTexture, destination);
            mat.mainTexture = pongTexture;
        }

        pingpong = !pingpong;
    }

    private void SetupTextures()
    {
        pingTexture = new RenderTexture(gridWidth, gridHeight, 24);
        pingTexture.wrapMode = TextureWrapMode.Repeat;
        pingTexture.enableRandomWrite = true;
        pingTexture.filterMode = FilterMode.Point;
        pingTexture.Create();

        pongTexture = new RenderTexture(gridWidth, gridHeight, 24);
        pongTexture.wrapMode = TextureWrapMode.Repeat;
        pongTexture.enableRandomWrite = true;
        pongTexture.filterMode = FilterMode.Point;
        pongTexture.Create();
    }

    private void SetupShader()
    {
        kernel = shader.FindKernel("GameOfLife");

        Graphics.Blit(input, pingTexture);

        shader.SetInt("Width", gridWidth);
        shader.SetInt("Height", gridHeight);
    }

    private void SetupQuad()
    {
        mat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;

        transform.localScale = new Vector3(20, 20, 1);
    }

    private void OnDestroy()
    {
        // Release textures
        pingTexture.Release();
        pongTexture.Release();
    }
}
