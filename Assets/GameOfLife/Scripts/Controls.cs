using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
	// Get Game of Life
	public GameOfLife gameOfLife;

	// Get input fields for grid width and grid height
	public InputField gridWidthInput;
	public InputField gridHeightInput;

	// Get input field for generation speed
	public InputField generationSpeedInput;

	[Space]

	// Get text of the play/stop button
	public Text play;

	[Space]

	// Get game menu and options menu
	public GameObject game;
	public GameObject options;

	// Store private variables to check for change
	private int gridWidth;
	private int gridHeight;

	private float generationSpeed;


	// Look if option menu is open or closed
	private bool optionsOpen = false;

	private void Start()
	{
		// Set text
		gridWidthInput.text = gameOfLife.gridWidth.ToString();
		gridHeightInput.text = gameOfLife.gridHeight.ToString();

		generationSpeedInput.text = gameOfLife.generationSpeed.ToString();


		// Set game and option menu active
		game.SetActive(!optionsOpen);
		options.SetActive(optionsOpen);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			// Toggle option menu open
			optionsOpen = !optionsOpen;

			// Set game and option menu active
			game.SetActive(!optionsOpen);
			options.SetActive(optionsOpen);
		}


		// Set play text
		play.text = gameOfLife.playing ? "Stop" : "Play";
	}

	public void Submit()
	{
		// Set width, height, generation speed
		int width;
		int height;

		float genSpeed;


		// Test for successes for width and height conversion
		bool widthSuccess = int.TryParse(gridWidthInput.text, out width) && width > 0 && width != gridWidth;
		bool heightSuccess = int.TryParse(gridHeightInput.text, out height) && height > 0 && height != gridHeight;

		// Replace "." with ","
		string genSpeedString = generationSpeedInput.text.Replace(".", ",");

		// Test for success for generation speed conversion
		bool genSpeedSuccess = float.TryParse(genSpeedString, out genSpeed) && genSpeed > 0.00001 && genSpeed <= 1 && genSpeed != generationSpeed;

		// If width success, set gridwidth
		if (widthSuccess)
		{
			gameOfLife.gridWidth = width;
			gridWidth = width;
		}
		// If no success, reset the text
		else
		{
			gridWidthInput.text = gameOfLife.gridWidth.ToString();
		}

		// If height success, set height
		if (heightSuccess)
		{
			gameOfLife.gridHeight = height;
			gridHeight = height;
		}
		// If no success, reset the text
		else
		{
			gridHeightInput.text = gameOfLife.gridHeight.ToString();
		}

		// If generation speed success, set generation speed
		if (genSpeedSuccess)
		{
			gameOfLife.generationSpeed = genSpeed;
			generationSpeed = genSpeed;
		}
		// If no success, reset the text
		else
		{
			generationSpeedInput.text = gameOfLife.generationSpeed.ToString();
		}

		// If width or height is a success reset entire board
		if (widthSuccess || heightSuccess)
		{
			gameOfLife.Clear();
			gameOfLife.Setup();
		}

		// If gen speed is a success, just cancel invoke
		if (genSpeedSuccess)
		{
			gameOfLife.CancelInvoke();
		}
	}
}
