using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
	// Get Game of Life
	public GOLMaster gameOfLife;

	// Get input fields for grid width and grid height
	public InputField gridWidthInput;
	public InputField gridHeightInput;

	// Get input field for generation speed
	public Slider generationSpeedInput;
	public Text generationSpeedText;

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

		generationSpeedInput.value = gameOfLife.generationSpeed;
		generationSpeedText.text = gameOfLife.generationSpeed.ToString();

		// Set game and option menu active
		game.SetActive(!optionsOpen);
		options.SetActive(optionsOpen);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ToggleOptions();
		}

		if(Input.GetKeyDown(KeyCode.F1)){
			if(game.activeInHierarchy){
				game.SetActive(false);
			}
			else if (!options.activeInHierarchy)
			{
				game.SetActive(true);
			}
		}

		// Set play text
		play.text = gameOfLife.playing ? "Stop" : "Play";
	}

	public void ToggleOptions()
	{
		// Toggle option menu open
		optionsOpen = !optionsOpen;

		// Set game and option menu active
		game.SetActive(!optionsOpen);
		options.SetActive(optionsOpen);
	}

	public void Submit()
	{
		// Set width, height, generation speed
		int width;
		int height;

		// Test for successes for width and height conversion
		bool widthSuccess = int.TryParse(gridWidthInput.text, out width) && width > 0 && width != gridWidth;
		bool heightSuccess = int.TryParse(gridHeightInput.text, out height) && height > 0 && height != gridHeight;

		float genSpeed = generationSpeedInput.value;

		// Test for success for generation speed conversion
		bool genSpeedSuccess = genSpeed > 0.00001 && genSpeed <= 1 && genSpeed != generationSpeed;

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

			generationSpeedText.text = ((int)genSpeed).ToString();
		}
		// If no success, reset the text
		else
		{
			generationSpeedInput.value = gameOfLife.generationSpeed;
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
