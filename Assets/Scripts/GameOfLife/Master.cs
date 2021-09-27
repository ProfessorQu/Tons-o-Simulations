using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Simulations.Core;

namespace Simulations.GameOfLife{
    public class Master : Singleton
    {
        public ComputeShader shader;
        private int kernel;

        [Min(1)] public int gridWidth = 1;
        [Min(1)] public int gridHeight = 1;

        public float defaultSize = 20;

        [Range(0, 1)] public float valueToBeAlive = 0.75f;

        [HideInInspector] public bool playing = false;
        [HideInInspector] public int generation;

        public float generationSpeed;

        int xGroups;
        int yGroups;

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
            xGroups = Mathf.CeilToInt(gridWidth / 8f);
            yGroups = Mathf.CeilToInt(gridHeight / 8f);

            if (pingTexture == null)
            {
                pingTexture = new RenderTexture(gridWidth, gridHeight, 24);
                pingTexture.wrapMode = TextureWrapMode.Repeat;
                pingTexture.enableRandomWrite = true;
                pingTexture.filterMode = FilterMode.Point;
                pingTexture.Create();
            }

            if (pongTexture == null)
            {
                pongTexture = new RenderTexture(gridWidth, gridHeight, 24);
                pongTexture.wrapMode = TextureWrapMode.Repeat;
                pongTexture.enableRandomWrite = true;
                pongTexture.filterMode = FilterMode.Point;
                pongTexture.Create();
            }

            kernel = shader.FindKernel("CSMain");

            shader.SetInt("_Width", gridWidth);
            shader.SetInt("_Height", gridHeight);

            mat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;

            float aspectRatio = (float)gridWidth / (float)gridHeight;
            transform.localScale = new Vector3(defaultSize * aspectRatio, defaultSize, 1);

            Pause();
        }

        public void Randomize()
        {
            Clear();

            int initKernel = shader.FindKernel("CSInit");

            RenderTexture tex = pingpong ? pingTexture : pongTexture;

            shader.SetTexture(initKernel, "Result", tex);

            shader.SetFloat("_Random", Random.Range(-10, 10));
            shader.SetFloat("_ValueToBeAlive", valueToBeAlive);

            shader.Dispatch(initKernel, xGroups, yGroups, 1);
        }

        public void Step()
        {
            if (pingpong)
            {
                shader.SetTexture(kernel, "Input", pingTexture);
                shader.SetTexture(kernel, "Result", pongTexture);

                shader.Dispatch(kernel, xGroups, yGroups, 1);

                mat.mainTexture = pingTexture;
            }
            else
            {
                shader.SetTexture(kernel, "Input", pongTexture);
                shader.SetTexture(kernel, "Result", pingTexture);

                shader.Dispatch(kernel, xGroups, yGroups, 1);

                mat.mainTexture = pongTexture;
            }

            pingpong = !pingpong;

            generation++;
        }

        public void Clear()
        {
            pingpong = true;

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

            mat.mainTexture = pingTexture;

            generation = 0;
        }

        public void Play()
        {
            InvokeRepeating("Step", 0.0f, generationSpeed);

            playing = true;
        }

		public void Pause()
		{
            CancelInvoke();

            playing = false;
		}

        private void OnDestroy()
        {
            if (pingTexture)
            {
                pingTexture.Release();
            }

            if (pongTexture)
            {
                pongTexture.Release();
            }
        }
    }

}
