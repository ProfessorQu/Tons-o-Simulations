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

    public float defaultSize = 20;

    [Range(0, 1)] public float valueToBeAlive = 0.75f;

    [HideInInspector] public bool playing = false;

    public float generationSpeed;

    // Set groups
    int xGroups;
    int yGroups;

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
        // Calculate kernel groups
        xGroups = Mathf.CeilToInt(gridWidth / 8f);
        yGroups = Mathf.CeilToInt(gridHeight / 8f);

        if (pingTexture == null)
        {
            // Create ping texture
            pingTexture = new RenderTexture(gridWidth, gridHeight, 24);
            pingTexture.wrapMode = TextureWrapMode.Repeat;
            pingTexture.enableRandomWrite = true;
            pingTexture.filterMode = FilterMode.Point;
            pingTexture.Create();
        }

        if (pongTexture == null)
        {
            // Create pong texture
            pongTexture = new RenderTexture(gridWidth, gridHeight, 24);
            pongTexture.wrapMode = TextureWrapMode.Repeat;
            pongTexture.enableRandomWrite = true;
            pongTexture.filterMode = FilterMode.Point;
            pongTexture.Create();
        }

        // Get kernel
        kernel = shader.FindKernel("CSMain");

        // Set width and height
        shader.SetInt("_Width", gridWidth);
        shader.SetInt("_Height", gridHeight);

        // Get material
        mat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;

        float aspectRatio = (float)gridWidth / (float)gridHeight;

        transform.localScale = new Vector3(defaultSize * aspectRatio, defaultSize, 1);

        CancelInvoke();
    }

    public void Randomize()
    {
        Clear();

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
        shader.Dispatch(initKernel, xGroups, yGroups, 1);
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

    public void Clear()
    {
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

        mat.mainTexture = pingTexture;
    }

    public void Play()
    {
        playing = !playing;
    }

    private void Update()
	{
		if (playing && !IsInvoking())
        {
            InvokeRepeating("Step", 0.0f, generationSpeed);
        }
        else if (!playing)
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
