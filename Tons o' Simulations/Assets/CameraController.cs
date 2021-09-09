using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public int zoomSpeed = 1;
	public int dragSpeed = 1;

	public GameOfLife gameOfLife;

	private Camera cam;
	private Vector2 screenMiddle;

	private float defaultSize;
	private Vector3 defaultPos;

	private void Start()
	{
		cam = Camera.main;

		screenMiddle.x = Screen.width - Screen.width / 2;
		screenMiddle.y = Screen.height - Screen.height / 2;

		defaultSize = cam.orthographicSize;
		defaultPos = cam.transform.position;
	}

	private void Update()
	{
		float mouseScroll = Input.mouseScrollDelta.y;

		if (cam.orthographicSize - mouseScroll > 0)
		{
			cam.orthographicSize -= mouseScroll * Time.deltaTime * zoomSpeed;
		}

		if (Input.GetMouseButton(0))
		{
			Vector2 pos = Camera.main.ScreenToViewportPoint((Vector2)Input.mousePosition - screenMiddle);
			Vector2 move = pos * Time.deltaTime * dragSpeed * cam.orthographicSize;

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
