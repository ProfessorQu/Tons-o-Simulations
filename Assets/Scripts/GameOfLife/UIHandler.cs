using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Simulations.GameOfLife
{
	public class UIHandler : MonoBehaviour
	{
		public Master gameOfLife;

		public InputField widthInput;
		public InputField heightInput;

		public Slider genSpeedInput;
		public Text genSpeedInputText;

		public Text currGenText;

		[Space]

		public Image play;
		public Sprite playSprite;
		public Sprite pauseSprite;

		[Space]

		public GameObject game;
		public GameObject options;

		private int prevWidth;
		private int prevHeight;

		private float prevGenSpeed;

		private bool menuOpen = false;

		private void Start()
		{
			widthInput.text = gameOfLife.gridWidth.ToString();
			heightInput.text = gameOfLife.gridHeight.ToString();

			genSpeedInput.value = gameOfLife.generationSpeed;
			genSpeedInputText.text = gameOfLife.generationSpeed.ToString();

			game.SetActive(true);
			options.SetActive(false);
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (menuOpen)
				{
					CloseMenu();
				}
				else
				{
					OpenMenu();
				}
			}

			if (Input.GetKeyDown(KeyCode.F1))
			{
				if (game.activeInHierarchy)
				{
					game.SetActive(false);
				}
				else if (!options.activeInHierarchy)
				{
					game.SetActive(true);
				}
			}

			currGenText.text = "Generation: " + gameOfLife.generation;

			play.sprite = gameOfLife.playing ? pauseSprite : playSprite;
		}

		public void CloseMenu()
		{
			game.SetActive(true);
			options.SetActive(false);

			menuOpen = false;
		}

		public void OpenMenu()
		{
			game.SetActive(false);
			options.SetActive(true);

			menuOpen = true;
		}

		public void Submit()
		{
			// Set width, height, generation speed
			int width;
			int height;

			// Test for successes for width and height conversion
			bool widthSuccess = int.TryParse(widthInput.text, out width) && width > 0 && width != prevWidth;
			bool heightSuccess = int.TryParse(heightInput.text, out height) && height > 0 && height != prevHeight;

			float genSpeed = genSpeedInput.value;

			// Test for success for generation speed conversion
			bool genSpeedSuccess = genSpeed > 1e-05 && genSpeed <= 1 && genSpeed != prevGenSpeed;

			// If width success, set gridwidth
			if (widthSuccess)
			{
				gameOfLife.gridWidth = width;
				prevWidth = width;
			}
			// If no success, reset the text
			else
			{
				widthInput.text = gameOfLife.gridWidth.ToString();
			}

			// If height success, set height
			if (heightSuccess)
			{
				gameOfLife.gridHeight = height;
				prevHeight = height;
			}
			// If no success, reset the text
			else
			{
				heightInput.text = gameOfLife.gridHeight.ToString();
			}

			// If generation speed success, set generation speed
			if (genSpeedSuccess)
			{
				gameOfLife.generationSpeed = genSpeed;
				prevGenSpeed = genSpeed;

				genSpeedInputText.text = ((int)genSpeed).ToString();
			}
			// If no success, reset the text
			else
			{
				genSpeedInput.value = gameOfLife.generationSpeed;
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
}
