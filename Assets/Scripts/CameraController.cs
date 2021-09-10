using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour//, IDragHandler, IBeginDragHandler, IEndDragHandler
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
