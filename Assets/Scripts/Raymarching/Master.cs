using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Simulations.Raymarching
{
	[RequireComponent(typeof(Camera)), ExecuteInEditMode]
	public class Master : SceneViewFilter
	{
		public Shader shader;

		public Material raymarchMaterial
		{
			get
			{
				if (!_raymarchMaterial && shader)
				{
					_raymarchMaterial = new Material(shader);
					_raymarchMaterial.hideFlags = HideFlags.HideAndDontSave;
				}

				return _raymarchMaterial;
			}
		}
		private Material _raymarchMaterial;

		public Camera cam
		{
			get
			{
				if (!_cam)
				{
					_cam = Camera.current;
				}

				return _cam;
			}
		}

		private Camera _cam;

		[Header("Setup")]
		public float maxDistance;
		[Range(1, 1024)]
		public int maxIterations;
		[Range(0.1f, 0.001f)]
		public float accuracy;

		[Header("Directional Light")]
		public Light directionalLight;
		public Color lightColor;
		public float lightIntensity;

		[Header("Shadow")]
		[Range(0, 4)]
		public float shadowIntensity;
		public Vector2 shadowDistance;
		[Range(1, 128)]
		public float shadowPenumbra;

		[Header("Ambient Occlusion")]
		[Range(0.01f, 10.0f)]
		public float aOStepsize;
		[Range(1, 5)]
		public int aOIterations;
		[Range(0, 1)]
		public float aOIntensity;

		[Header("Signed Distance Field")]
		public float colorIntensity;
		public float blendStrength;

		public Vector3 repeatAxis;
		public Vector3 repeatInterval;

		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!raymarchMaterial)
			{
				Graphics.Blit(source, destination);
				return;
			}

			CreateScene();

			_raymarchMaterial.SetMatrix("_CamFrustum", CamFrustum(cam));
			_raymarchMaterial.SetMatrix("_CamToWorld", cam.cameraToWorldMatrix);

			_raymarchMaterial.SetFloat("_MaxDistance", maxDistance);
			_raymarchMaterial.SetInt("_MaxIterations", maxIterations);
			_raymarchMaterial.SetFloat("_Accuracy", accuracy);

			_raymarchMaterial.SetVector("_LightDir", directionalLight ? directionalLight.transform.forward : Vector3.down);
			_raymarchMaterial.SetColor("_LightCol", lightColor);
			_raymarchMaterial.SetFloat("_LightIntensity", lightIntensity);

			_raymarchMaterial.SetVector("_ShadowDistance", shadowDistance);
			_raymarchMaterial.SetFloat("_ShadowIntensity", shadowIntensity);
			_raymarchMaterial.SetFloat("_ShadowPenumbra", shadowPenumbra);

			_raymarchMaterial.SetFloat("_AoStepsize", aOStepsize);
			_raymarchMaterial.SetInt("_AoIterations", aOIterations);
			_raymarchMaterial.SetFloat("_AoIntensity", aOIntensity);

			_raymarchMaterial.SetFloat("_ColorIntensity", colorIntensity);
			_raymarchMaterial.SetFloat("_BlendStrength", blendStrength);

			_raymarchMaterial.SetVector("_RepeatAxis", repeatAxis);
			_raymarchMaterial.SetVector("_RepeatInterval", repeatInterval);

			RenderTexture.active = destination;
			_raymarchMaterial.SetTexture("_MainTex", source);
			GL.PushMatrix();
			GL.LoadOrtho();
			_raymarchMaterial.SetPass(0);
			GL.Begin(GL.QUADS);

			//BL
			GL.MultiTexCoord2(0, 0.0f, 0.0f);
			GL.Vertex3(0.0f, 0.0f, 3.0f);

			//BR
			GL.MultiTexCoord2(0, 1.0f, 0.0f);
			GL.Vertex3(1.0f, 0.0f, 2.0f);

			//TR
			GL.MultiTexCoord2(0, 1.0f, 1.0f);
			GL.Vertex3(1.0f, 1.0f, 1.0f);

			//TL
			GL.MultiTexCoord2(0, 0.0f, 1.0f);
			GL.Vertex3(0.0f, 1.0f, 0.0f);

			GL.End();
			GL.PopMatrix();
		}

		private void CreateScene()
		{
			List<Shape> shapes = new List<Shape>(FindObjectsOfType<Shape>());
			shapes.Sort((a, b) => a.operation.CompareTo(b.operation));

			Vector4[] positions = new Vector4[100];
			Vector4[] scales = new Vector4[100];

			float[] types = new float[100];
			float[] operations = new float[100];

			Color[] colors = new Color[100];

			for (int i = 0; i < shapes.Count; i++)
			{
				var shape = shapes[i];

				positions[i] = shape.position;

				scales[i] = shape.scale;

				types[i] = (float)shape.type;
				operations[i] = (float)shape.operation;

				colors[i] = shape.color;

				if (shape.type == Shape.ShapeType.Plane)
					scales[i] = new Vector3(0, 1, 0);
			}

			_raymarchMaterial.SetVectorArray("shapePositions", positions);
			_raymarchMaterial.SetVectorArray("shapeScales", scales);

			_raymarchMaterial.SetFloatArray("shapeTypes", types);
			_raymarchMaterial.SetFloatArray("shapeOperations", operations);

			_raymarchMaterial.SetColorArray("shapeColors", colors);

			_raymarchMaterial.SetInt("numShapes", shapes.Count);
		}

		private Matrix4x4 CamFrustum(Camera cam)
		{
			Matrix4x4 frustum = Matrix4x4.identity;
			float fov = Mathf.Tan((cam.fieldOfView * 0.5f) * Mathf.Deg2Rad);

			Vector3 goUp = Vector3.up * fov;
			Vector3 goRight = Vector3.right * fov * cam.aspect;

			Vector3 TL = (-Vector3.forward - goRight + goUp);
			Vector3 TR = (-Vector3.forward + goRight + goUp);
			Vector3 BL = (-Vector3.forward - goRight - goUp);
			Vector3 BR = (-Vector3.forward + goRight - goUp);

			frustum.SetRow(0, TL);
			frustum.SetRow(1, TR);
			frustum.SetRow(2, BR);
			frustum.SetRow(3, BL);

			return frustum;
		}
	}
}