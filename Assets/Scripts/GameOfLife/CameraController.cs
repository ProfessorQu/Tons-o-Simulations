using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
	public float zoomSpeed = 1;
	public float dragSpeed = 1;

	public GameOfLife gameOfLife;

	private Camera cam;

	private Vector2 dragPos;
	private bool dragging;

	private float defaultSize;
	private Vector3 defaultPos;

	private void Start()
	{
		cam = Camera.main;

		defaultSize = cam.orthographicSize;
		defaultPos = cam.transform.position;

		dragging = false;
	}

	private void Update()
	{
		float mouseScroll = Input.mouseScrollDelta.y;

		if (cam.orthographicSize - mouseScroll > 0)
		{
			cam.orthographicSize -= mouseScroll * Time.deltaTime * zoomSpeed * cam.orthographicSize;
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

		if (Input.GetMouseButtonDown(1))
		{
			gameOfLife.Step();
		}

		if (Input.GetMouseButtonDown(2))
		{
			cam.orthographicSize = defaultSize;
			cam.transform.position = defaultPos;
		}
	}
}