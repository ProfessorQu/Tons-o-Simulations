using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsHandler : MonoBehaviour
{
	public GameOfLife gameOfLife;

	public InputField gridWidthInput;
	public InputField gridHeightInput;

	public InputField generationSpeedInput;

	private int gridWidth = 8;
	private int gridHeight = 8;

	private float generationSpeed = 0.1f;

	private void Update()
	{
		int width;
		int height;

		float genSpeed;

		bool widthSuccess = int.TryParse(gridWidthInput.text, out width);
		bool heightSuccess = int.TryParse(gridHeightInput.text, out height);

		bool genSpeedSuccess = float.TryParse(generationSpeedInput.text, out genSpeed);

		if (widthSuccess && !(width <= 0) && width != gridWidth)
		{
			gameOfLife.gridWidth = width;
			gridWidth = width;
		}
		else if (widthSuccess)
		{
			gridWidthInput.text = gameOfLife.gridWidth.ToString();
		}

		if (heightSuccess && !(height <= 0) && height != gridHeight)
		{
			gameOfLife.gridHeight = height;
			gridHeight = height;
		}
		else if(heightSuccess)
		{
			gridHeightInput.text = gameOfLife.gridHeight.ToString();
		}

		if (genSpeedSuccess && !(genSpeed <= 0) && !(genSpeed > 1) && genSpeed != generationSpeed)
		{
			gameOfLife.generationSpeed = genSpeed;
			generationSpeed = genSpeed;
		}
		else if (genSpeedSuccess)
		{
			generationSpeedInput.text = gameOfLife.generationSpeed.ToString();
		}

		if (widthSuccess || heightSuccess || genSpeedSuccess)
		{
			gameOfLife.Setup();
			gameOfLife.Randomize();
		}
	}
}
