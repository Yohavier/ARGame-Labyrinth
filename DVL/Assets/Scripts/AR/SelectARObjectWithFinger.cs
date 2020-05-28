using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SelectARObjectWithFinger : MonoBehaviour
{
	private Vector2 touchPosition;
	private Camera arCamera;
	
	//Lets the raycast only collide with certain things
	public LayerMask mask;

	private void Awake()
	{
		arCamera = FindObjectOfType<Camera>();
	}

	private void Update()
	{
		RayCastOnTouch();
	}

	//Sends Ray from touch position with the camera rot to select a path
	private void RayCastOnTouch()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			touchPosition = touch.position;
			RaycastHit hitObject;
			Ray ray = arCamera.ScreenPointToRay(touchPosition);

			if (touch.phase == TouchPhase.Began && Physics.Raycast(ray, out hitObject, mask))
			{
				Tile component = hitObject.transform.GetComponent<Tile>();
				if (hitObject.transform.CompareTag("Tile"))
				{
					LocalGameManager.local.activePlayer.GetComponent<Pathfinding>().TargetPosition = component;
				}
			}
		}
	}
}
