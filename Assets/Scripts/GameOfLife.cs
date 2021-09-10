using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    // Set shader and kernel
    public ComputeShader shader;
    private int kernel;

    // Set grid width and height
    [Min(1)] public int gridSize = 1;

    [Range(0, 1)] public float valueToBeAlive = 0.75f;

    [HideInInspector] public bool playing = false;

    public float generationSpeed;

    // Calculate groups
    int groups;

    // Set pingpong bool
    private bool pingpong = true;
    // Set ping and pong RenderTextures
    RenderTexture pingTexture;
    RenderTexture pongTexture;

    // public Texture2D tex2D;

    // Set material
    private Material mat;

    private void Start()
    {
        Setup();
        Randomize();
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
        groups = Mathf.CeilToInt(gridSize / 8f);

        // Create ping texture
        pingTexture = new RenderTexture(gridSize, gridSize, 24);
        pingTexture.wrapMode = TextureWrapMode.Repeat;
        pingTexture.enableRandomWrite = true;
        pingTexture.filterMode = FilterMode.Point;
        pingTexture.Create();

        // Create pong texture
        pongTexture = new RenderTexture(gridSize, gridSize, 24);
        pongTexture.wrapMode = TextureWrapMode.Repeat;
        pongTexture.enableRandomWrite = true;
        pongTexture.filterMode = FilterMode.Point;
        pongTexture.Create();

        // Get kernel
        kernel = shader.FindKernel("CSMain");

        // Set width and height
        shader.SetInt("_Width", gridSize);
        shader.SetInt("_Height", gridSize);

        // Get material
        mat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;

        CancelInvoke();

        InvokeRepeating("Step", 0.0f, generationSpeed);
    }

    public void Randomize()
    {
        // Init simulation
        int initKernel = shader.FindKernel("CSInit");

        RenderTexture tex = pingpong ? pingTexture : pongTexture;

        // Set texture
        shader.SetTexture(initKernel, "Result", tex);

        // Pass Random
        shader.SetFloat("_Random", Random.Range(-10, 10));
        // Pass value to be alive
        shader.SetFloat("_ValueToBeAlive", valueToBeAlive);
        // Run init shader
        shader.Dispatch(initKernel, groups, groups, 1);
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
            shader.Dispatch(kernel, groups, groups, 1);

            // Set material
            mat.mainTexture = pingTexture;
        }
        else
        {
            // Set textures
            shader.SetTexture(kernel, "Input", pongTexture);
            shader.SetTexture(kernel, "Result", pingTexture);

            // Run shader
            shader.Dispatch(kernel, groups, groups, 1);

            // Set material
            mat.mainTexture = pongTexture;
        }

        // Invert pingpong
        pingpong = !pingpong;
    }

	private void Update()
	{
		if (playing && !IsInvoking())
        {
            InvokeRepeating("Step", 0.0f, generationSpeed);
        }
        else if (!playing && IsInvoking())
        {
            CancelInvoke();
        }
	}

	private void OnDestroy()
    {
        // Release textures
        pingTexture.Release();
        pongTexture.Release();
    }
}
