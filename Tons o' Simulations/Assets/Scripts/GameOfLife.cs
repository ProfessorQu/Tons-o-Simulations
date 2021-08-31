using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public ComputeShader shader;
    int kernel;

    Renderer rend;
    RenderTexture texture;

    private void Start()
    {
        // Get kernel hand;e
        kernel = shader.FindKernel("GameOfLife");

        // Create texture
        texture = new RenderTexture(256, 256, 24);
        texture.enableRandomWrite = true;
        texture.Create();

        // Pass the width and height to shader
        shader.SetFloat("Width", texture.width);
        shader.SetFloat("Height", texture.height);

        // Set texture
        shader.SetTexture(kernel, "Result", texture);
        // Run shader
        shader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);


        // Get renderer
        rend = GetComponent<Renderer>();

        // Scale quad to camera size
        var quadHeight = Camera.main.orthographicSize * 2.0f;
        var quadWidth = quadHeight * Screen.width / Screen.height;

        transform.localScale = new Vector3(quadWidth, quadHeight, 1);
    }

	private void Update()
    {
        // Create texture
        if (texture == null)
        {
            texture = new RenderTexture(256, 256, 24);
            texture.enableRandomWrite = true;
            texture.Create();
        }

        // Set texture
        shader.SetTexture(kernel, "Result", texture);
        // Run shader
        shader.Dispatch(kernel, texture.width / 8, texture.height / 8, 1);

        // Set texture to material texture
        rend.material.mainTexture = texture;
    }
}
