using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Simulations.GameOfLife
{
	[CustomEditor(typeof(Master))]
	public class GameOfLifeEditor : Editor
	{
		private Master gameOfLife;

		bool[] neighbors = new bool[24] {
			false,
			false,
			false,
			false,
			false,
			false,
			true,
			true,
			true,
			false,
			false,
			true,
			true,
			false,
			false,
			true,
			true,
			true,
			false,
			false,
			false,
			false,
			false,
			false,
		};

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			gameOfLife = (Master)target;

			GUILayout.BeginHorizontal();
			for (int i = 0; i < 25; i++)
			{
				if (i == 12)
				{
					GUILayout.Space(80);
					continue;
				}

				if (i % 5 == 0)
				{
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
				}

				int x = i % 5 - 2;
				int y = -(i / 5 - 2);

				int idx = i > 12 ? i - 1 : i;

				gameOfLife.neighborOffsets[idx] = new Vector2(0, 0);

				if (neighbors[idx])
				{
					gameOfLife.neighborOffsets[idx] = new Vector2(x, y);
				}

				if (GUILayout.Button(neighbors[idx] ? "@" : "...."))
				{
					neighbors[idx] = !neighbors[idx];
				}
			}

			GUILayout.EndHorizontal();
			GUILayout.Space(20);
			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Step"))
			{
				gameOfLife.Step();
			}

			if (GUILayout.Button("Reset"))
			{
				gameOfLife.Setup();
			}

			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Randomize"))
			{
				gameOfLife.Randomize();
			}

			GUILayout.EndHorizontal();
		}
	}
}
