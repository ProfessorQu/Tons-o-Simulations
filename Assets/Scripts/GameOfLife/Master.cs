using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Simulations.Core;

namespace Simulations.GameOfLife{
    public class Master : MasterBase
    {
        public ComputeShader shader;
        int kernel;

        [Min(1)] public int gridWidth = 1;
        [Min(1)] public int gridHeight = 1;

        public float defaultSize = 20;

        public Vector4 [] neigbors = new Vector4[8];

        [Range(0, 1)] public float valueToBeAlive = 0.75f;

        [HideInInspector] public bool playing = false;
        [HideInInspector] public int generation;

        float simulationSpeed = 1;

		public override float SimulationSpeed
        {
            get
            {
                return simulationSpeed;
            }
            set
            {
                simulationSpeed = value;
            }
        }

		int xGroups, yGroups;

        private bool pingpong = true;
        RenderTexture pingTexture, pongTexture;

        private Material mat;

        private void Start()
        {
            Setup();
        }

        public void Setup()
        {
            xGroups = Mathf.CeilToInt(gridWidth / 8f);
            yGroups = Mathf.CeilToInt(gridHeight / 8f);

            if (pingTexture == null || pongTexture == null)
            {
                CreateTextures();
            }

            kernel = shader.FindKernel("CSMain");

            shader.SetInt("_Width", gridWidth);
            shader.SetInt("_Height", gridHeight);

            mat = gameObject.GetComponent<MeshRenderer>().sharedMaterial;

            float aspectRatio = (float)gridWidth / (float)gridHeight;
            transform.localScale = new Vector3(defaultSize * aspectRatio, defaultSize, 1);

            StopSimulation();
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

        public override void Step()
        {
            shader.SetVectorArray("neigbors", neigbors);
            shader.SetInt("numNeigbors", neigbors.Length);

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
            CreateTextures();

            pingpong = true;
            mat.mainTexture = pingTexture;
            generation = 0;
        }

        void CreateTextures()
        {
			pingTexture = new RenderTexture(gridWidth, gridHeight, 24)
			{
				wrapMode = TextureWrapMode.Repeat,
				enableRandomWrite = true,
				filterMode = FilterMode.Point
			};
			pingTexture.Create();

			pongTexture = new RenderTexture(gridWidth, gridHeight, 24)
			{
				wrapMode = TextureWrapMode.Repeat,
				enableRandomWrite = true,
				filterMode = FilterMode.Point
			};
			pongTexture.Create();
        }

        public override void StartSimulation()
        {
            InvokeRepeating("Step", 0.0f, 1 / SimulationSpeed);
            playing = true;
        }

		public override void StopSimulation()
		{
            CancelInvoke();
            playing = false;
		}

        private void OnDestroy()
        {
            if (pingTexture)
                pingTexture.Release();

            if (pongTexture)
                pongTexture.Release();
        }
    }

}
