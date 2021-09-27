using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Simulations.GameOfLife
{
	public class UIHandler : MonoBehaviour
	{
		[Header("Game Of Life")]
		public Master gameOfLife;
		public CameraController camControl;

		[Header("Grid Size")]
		public TMP_InputField widthInput;
		public TMP_InputField heightInput;

		[Header("Generation Speed")]
		public Slider genSpeedInput;
		public TMP_Text genSpeedValueText;

		public TMP_Text currGenText;

		[Header("Start/Stop Button")]
		public Image playIcon;
		public Sprite playSprite;
		public Sprite pauseSprite;

		public Image playBackground;
		public Color playColor;
		public Color pauseColor;

		[Header("Menus")]
		public GameObject game;
		public GameObject menu;

		private int prevWidth;
		private int prevHeight;

		private float prevGenSpeed;

		private bool menuOpen = false;

		private void Start()
		{
			widthInput.text = gameOfLife.gridWidth.ToString();
			heightInput.text = gameOfLife.gridHeight.ToString();

			genSpeedInput.value = gameOfLife.SimulationSpeed;
			genSpeedValueText.text = gameOfLife.SimulationSpeed.ToString();

			game.SetActive(true);
			menu.SetActive(false);
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
				if (menuOpen)
				{
					if (menu.activeInHierarchy)
					{
						menu.SetActive(false);
					}
					else
					{
						menu.SetActive(true);
					}
				}
				else
				{
					if (game.activeInHierarchy)
					{
						game.SetActive(false);
					}
					else
					{
						game.SetActive(true);
					}
				}
			}

			UpdateSpeed();
			UpdateStartButton();
		}

		public void CloseMenu()
		{
			game.SetActive(true);
			menu.SetActive(false);

			camControl.enabled = true;

			menuOpen = false;
		}

		public void OpenMenu()
		{
			game.SetActive(false);
			menu.SetActive(true);

			camControl.enabled = false;

			menuOpen = true;
		}

		void UpdateStartButton()
		{
			if (gameOfLife.playing)
			{
				playIcon.sprite = pauseSprite;
				playBackground.color = pauseColor;
			}
			else
			{
				playIcon.sprite = playSprite;
				playBackground.color = playColor;
			}
		}

		public void UpdateGrid()
		{
			bool widthSuccess = int.TryParse(widthInput.text, out int width) && width > 0 && width != prevWidth;
			bool heightSuccess = int.TryParse(heightInput.text, out int height) && height > 0 && height != prevHeight;


			if (widthSuccess)
			{
				gameOfLife.gridWidth = width;
				prevWidth = width;
			}
			else
			{
				widthInput.text = gameOfLife.gridWidth.ToString();
			}

			if (heightSuccess)
			{
				gameOfLife.gridHeight = height;
				prevHeight = height;
			}
			else
			{
				heightInput.text = gameOfLife.gridHeight.ToString();
			}

			if (widthSuccess || heightSuccess)
			{
				gameOfLife.Clear();
				gameOfLife.Setup();
			}
		}

		void UpdateSpeed()
		{
			float genSpeed = genSpeedInput.value;

			bool genSpeedSuccess = genSpeed != prevGenSpeed;

			if (genSpeedSuccess)
			{
				gameOfLife.SimulationSpeed = genSpeed;
				prevGenSpeed = genSpeed;

				genSpeedValueText.text = (Mathf.Round(genSpeed * 100) / 100).ToString();

				if (gameOfLife.playing)
				{
					gameOfLife.StopSimulation();
					gameOfLife.StartSimulation();
				}
			}
			else
			{
				genSpeedInput.value = gameOfLife.SimulationSpeed;
			}

			currGenText.text = "Generation: " + gameOfLife.generation;
		}

		public void StartStop()
		{
			if (gameOfLife.playing){
				gameOfLife.StopSimulation();
			}
			else
			{
				gameOfLife.StartSimulation();
			}
		}
	}
}
