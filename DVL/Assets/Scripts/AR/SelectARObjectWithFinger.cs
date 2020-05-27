using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SelectARObjectWithFinger : MonoBehaviour
{
	private ARRaycastManager arRaycastManager;

	private Vector2 touchPosition;

	private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

	private Camera arCamera;

	public LayerMask mask;

	private void Awake()
	{
		arRaycastManager = GetComponent<ARRaycastManager>();
		arCamera = FindObjectOfType<Camera>();
	}

	private void Update()
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
