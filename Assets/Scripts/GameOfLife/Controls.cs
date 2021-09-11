using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controls : MonoBehaviour
{
	public GameOfLife gameOfLife;

	public InputField gridWidthInput;
	public InputField gridHeightInput;

	public InputField generationSpeedInput;

	[Space]

	public Text play;

	[Space]

	public GameObject game;
	public GameObject options;

	private int gridWidth;
	private int gridHeight;

	private float generationSpeed;

	private bool optionsOpen = false;

	private void Start()
	{
		gridWidthInput.text = gameOfLife.gridWidth.ToString();
		gridHeightInput.text = gameOfLife.gridHeight.ToString();

		generationSpeedInput.text = gameOfLife.generationSpeed.ToString();

		game.SetActive(true);
		options.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (optionsOpen)
			{
				game.SetActive(true);
				options.SetActive(false);
			}
			else
			{
				game.SetActive(false);
				options.SetActive(true);
			}

			optionsOpen = !optionsOpen;
		}

		play.text = gameOfLife.playing ? "Stop" : "Play";
	}

	public void Submit()
	{
		int width;
		int height;

		float genSpeed;

		bool widthSuccess = int.TryParse(gridWidthInput.text, out width) && !(width <= 0) && width != gridWidth;
		bool heightSuccess = int.TryParse(gridHeightInput.text, out height) && !(height <= 0) && height != gridHeight;

		string genSpeedString = generationSpeedInput.text.Replace(".", ",");

		bool genSpeedSuccess = float.TryParse(genSpeedString, out genSpeed) && !(genSpeed <= 0) && !(genSpeed > 1) && genSpeed != generationSpeed;

		if (widthSuccess)
		{
			gameOfLife.gridWidth = width;
			gridWidth = width;
		}
		else
		{
			gridWidthInput.text = gameOfLife.gridWidth.ToString();
		}

		if (heightSuccess)
		{
			gameOfLife.gridHeight = height;
			gridHeight = height;
		}
		else
		{
			gridHeightInput.text = gameOfLife.gridHeight.ToString();
		}

		if (genSpeedSuccess)
		{
			gameOfLife.generationSpeed = genSpeed;
			generationSpeed = genSpeed;
		}
		else
		{
			generationSpeedInput.text = gameOfLife.generationSpeed.ToString();
		}

		if (widthSuccess || heightSuccess)
		{
			gameOfLife.Clear();
			gameOfLife.Setup();
		}

		if (genSpeedSuccess)
		{
			gameOfLife.CancelInvoke();
		}
	}
}
