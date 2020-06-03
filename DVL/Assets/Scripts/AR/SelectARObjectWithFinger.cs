using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class SelectARObjectWithFinger : MonoBehaviour
{
	private Vector2 touchPosition;
	private Camera arCamera;
	
	//Lets the raycast only collide with certain things
	public LayerMask mask;
	public Tile DebugTile;
	private Tile currentSelectedTarget;
	public List<Tile> path;
	private void Awake()
	{
		arCamera = FindObjectOfType<Camera>();
	}

	private void Update()
	{		
		RayCastOnTouch();
		if (Input.GetKeyDown(KeyCode.E))
		{
			ManagePath(DebugTile);
		}
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
				Tile hitTile = hitObject.transform.GetComponent<Tile>();
				if (hitObject.transform.CompareTag("Tile"))
				{
					ManagePath(hitTile);
				}
			}
		}
	}

	private void ManagePath(Tile targetTile)
	{	
		if(targetTile != currentSelectedTarget)
		{
			currentSelectedTarget = targetTile;
			foreach (Tile t in path)
			{
				t.PrefabColor();
			}
			Pathfinding p = new Pathfinding(BoardGrid.GridInstance.grid, LocalGameManager.local.activePlayer.GetComponent<Player>().positionTile, targetTile);
			path = p.FindPath();
			foreach (Tile wayElement in path)
			{
				MeshRenderer[] meshes = wayElement.GetComponentsInChildren<MeshRenderer>();
				foreach(MeshRenderer mesh in meshes)
				{
					mesh.material.color = Color.red;
				}
			} 
		}
		else if(targetTile == currentSelectedTarget)
		{
			foreach (Tile wayElement in path)
			{
				wayElement.PrefabColor();
			}
			LocalGameManager.local.activePlayer.GetComponent<Player>().MoveToTarget(path);
			currentSelectedTarget = null;
		}
	}
}
