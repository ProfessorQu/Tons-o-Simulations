using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameOfLife))]
public class GameOfLifeEditor : Editor
{
	private GameOfLife gameOfLife;

	public override void OnInspectorGUI()
	{
		// Base on in spector GUI
		base.OnInspectorGUI();

		// Set target
		gameOfLife = (GameOfLife)target;

		// Begin horizontal
		GUILayout.BeginHorizontal();

		// Step button
		if (GUILayout.Button("Step"))
		{
			gameOfLife.Step();
		}

		// Reset button
		if (GUILayout.Button("Reset"))
		{
			gameOfLife.Setup();
		}

		// End horizontal
		GUILayout.EndHorizontal();



		// Begin horizontal
		GUILayout.BeginHorizontal();

		// Randomize button
		if (GUILayout.Button("Randomize"))
		{
			gameOfLife.Randomize();
		}

		// End horizontal
		GUILayout.EndHorizontal();
	}
}
