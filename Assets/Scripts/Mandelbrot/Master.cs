using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Simulations.Core;

namespace Simulations.Mandelbrot
{
    [ExecuteInEditMode, RequireComponent(typeof(Camera))]
    public class Master : Singleton
    {
        public Camera cam;
        public ComputeShader shader;

        int xGroups;
        int yGroups;

        int width;
        int height;

        public float aspectRatio = -1;
        public float minZoom = 5e-5f;

        RenderTexture tex;

        public int maxIterations = 200;

        public Vector2 pos = new Vector2(0, 0);
        public Vector2 zoom = new Vector2(1, 1);


        void Start()
        {
            Init();
        }

        private void Init()
        {
            width = cam.pixelWidth;
            height = cam.pixelHeight;

            xGroups = Mathf.CeilToInt(width / 8f);
            yGroups = Mathf.CeilToInt(height / 8f);

            if (aspectRatio == -1)
            {
                Debug.Log("Setting aspect ratio...");

                aspectRatio = (float)width / (float)height;

                if (aspectRatio > 0)
                {
                    zoom.x *= aspectRatio;
                }
                else
                {
                    zoom.y *= aspectRatio;
                }
            }

            if (tex == null || tex.width != width || tex.height == height)
            {
                Debug.Log("Creating texture...");

                tex = new RenderTexture(width, height, 24);
                tex.enableRandomWrite = true;
                tex.Create();
            }

            SetShaderParameters();
        }

        void SetShaderParameters()
        {
            shader.SetInt("_Width", width);
            shader.SetInt("_Height", height);

            shader.SetInt("_MaxIterations", maxIterations);

            shader.SetVector("_Area", new Vector4(pos.x, pos.y, zoom.x, zoom.y));

            shader.SetTexture(0, "Result", tex);
        }

        void HandleInputs()
        {
            if (zoom.x * 0.99f > minZoom || zoom.y * 0.99f > minZoom)
            {
                if (Input.GetKey(KeyCode.LeftBracket))
                {
                    zoom *= 0.99f;
                }
            }

            if (Input.GetKey(KeyCode.RightBracket))
            {
                zoom *= 1.01f;
            }

            if (Input.GetKey(KeyCode.A))
            {
                pos.x -= 0.01f * zoom.magnitude;
            }
            if (Input.GetKey(KeyCode.D))
            {
                pos.x += 0.01f * zoom.magnitude;
            }
            if (Input.GetKey(KeyCode.W))
            {
                pos.y += 0.01f * zoom.magnitude;
            }
            if (Input.GetKey(KeyCode.S))
            {
                pos.y -= 0.01f * zoom.magnitude;
            }
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Init();

            shader.Dispatch(0, xGroups, yGroups, 1);

            Graphics.Blit(tex, destination);

        }

        private void Update()
        {
            HandleInputs();
        }

        private void OnDestroy()
        {
            tex.Release();
        }
    }
}
