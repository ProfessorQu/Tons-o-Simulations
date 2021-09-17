using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GOLCameraController : MonoBehaviour
{
	// Set variables to be used as speeds
	public float zoomSpeed = 1;
	public float dragSpeed = 1;

	// Set camera
	private Camera cam;

	// Set dragpos and if dragging
	private Vector2 dragPos;
	private bool dragging = false;

	// Set default variables for resetting
	private float defaultSize;
	private Vector3 defaultPos;

	private void Start()
	{
		// Set the camera to the main camera (this camera)
		cam = Camera.main;

		// Set default size and position
		defaultSize = cam.orthographicSize;
		defaultPos = cam.transform.position;
	}

	private void Update()
	{
		// Get the amount of mouse scroll
		float mouseScroll = Input.mouseScrollDelta.y;

		// If the mouse scroll doesn't make size negative, update size
		if (cam.orthographicSize - mouseScroll > 0)
		{
			cam.orthographicSize -= mouseScroll * Time.deltaTime * zoomSpeed * cam.orthographicSize;
		}

		// If size is negative (lag, etc), set size to 1
		if(cam.orthographicSize < 0)
		{
			cam.orthographicSize = 1;
		}

		// If mouse button is down and not dragging, set drag start position and dragging
		if (Input.GetMouseButtonDown(0) && !dragging)
		{
			dragPos = Input.mousePosition;
			dragging = true;
		}

		// If mouse button is up, set dragging to false
		if (Input.GetMouseButtonUp(0))
		{
			dragging = false;
		}

		// If user is dragging, calculate difference, and update position
		if (dragging)
		{
			Vector2 mousePos = Input.mousePosition;
			Vector2 move = (mousePos - dragPos) * Time.deltaTime * dragSpeed * cam.orthographicSize;

			transform.Translate(-move, Space.World);
		}

		// If the middle mouse button is pressed, reset to default position and size
		if (Input.GetMouseButtonDown(2))
		{
			cam.orthographicSize = defaultSize;
			cam.transform.position = defaultPos;
		}
	}
}