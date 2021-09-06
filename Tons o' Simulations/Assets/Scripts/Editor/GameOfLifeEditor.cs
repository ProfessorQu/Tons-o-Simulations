using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameOfLife))]
public class GameOfLifeEditor : Editor
{
	private GameOfLife gameOfLife;

	private void OnEnable()
	{
		gameOfLife = (GameOfLife)target;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

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
	}
}
