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

    private Vector2 scale;

    public int step;
    private int _step = 0;

    private void Start()
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

        kernel = shader.FindKernel("GameOfLife");

        Graphics.Blit(input, pingTexture);

        shader.SetInt("Width", gridWidth);
        shader.SetInt("Height", gridHeight);

        Camera camera = GetComponent<Camera>();

        scale.x = camera.pixelWidth / 500;
        scale.y = camera.pixelHeight / 500;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (step > _step)
        {
            if (pingpong)
            {
                shader.SetTexture(kernel, "Input", pingTexture);
                shader.SetTexture(kernel, "Result", pongTexture);

                int xGroups = Mathf.CeilToInt(gridWidth / 8f);
                int yGroups = Mathf.CeilToInt(gridHeight / 8f);

                shader.Dispatch(kernel, xGroups, yGroups, 1);

                Graphics.Blit(pingTexture, destination, scale, new Vector2(0, 0));
            }
            else
            {

                shader.SetTexture(kernel, "Input", pongTexture);
                shader.SetTexture(kernel, "Result", pingTexture);

                int xGroups = Mathf.CeilToInt(gridWidth / 8f);
                int yGroups = Mathf.CeilToInt(gridHeight / 8f);

                shader.Dispatch(kernel, xGroups, yGroups, 1);

                Graphics.Blit(pongTexture, destination, scale, new Vector2(0, 0));
            }

            pingpong = !pingpong;
            _step++;
        }

        //Graphics.Blit(cellPrevious, destination);
    }

    private void OnDestroy()
    {
        pingTexture.Release();
        pongTexture.Release();
    }
}
