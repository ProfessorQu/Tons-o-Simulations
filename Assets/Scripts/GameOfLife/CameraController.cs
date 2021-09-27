using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Simulations.GameOfLife
{
	[RequireComponent(typeof(Camera))]
	public class CameraController : MonoBehaviour
	{
		public float zoomSpeed = 1;
		public float dragSpeed = 1;

		private Camera cam;

		private Vector2 dragPos;
		private bool dragging = false;

		private float defaultSize;
		private Vector3 defaultPos;

		private void Start()
		{
			cam = Camera.main;

			defaultSize = cam.orthographicSize;
			defaultPos = cam.transform.position;
		}

		private void Update()
		{
			float mouseScroll = Input.mouseScrollDelta.y;

			if (cam.orthographicSize - mouseScroll > 0)
			{
				cam.orthographicSize -= mouseScroll * Time.deltaTime * zoomSpeed * cam.orthographicSize;
			}

			if (cam.orthographicSize < 0)
			{
				cam.orthographicSize = 1;
			}

			if (Input.GetMouseButtonDown(0) && !dragging)
			{
				dragPos = Input.mousePosition;
				dragging = true;
			}

			if (Input.GetMouseButtonUp(0))
			{
				dragging = false;
			}

			if (dragging)
			{
				Vector2 mousePos = Input.mousePosition;
				Vector2 move = (mousePos - dragPos) * Time.deltaTime * dragSpeed * cam.orthographicSize;

				transform.Translate(-move, Space.World);
			}

			if (Input.GetMouseButtonDown(2))
			{
				cam.orthographicSize = defaultSize;
				cam.transform.position = defaultPos;
			}
		}

		private void OnEnable()
		{
			dragPos = new Vector2(0, 0);
			dragging = false;
		}
	}
}
