using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SelectARObjectWithFinger : MonoBehaviour
{
	public static SelectARObjectWithFinger instance;
	private Vector2 touchPosition;
	private Camera arCamera;
	
	//Lets the raycast only collide with certain things
	public LayerMask mask;
	public Tile DebugTile;
	private Tile currentSelectedTarget;
	public List<Tile> path;
	private void Awake()
	{
		instance = this;
		arCamera = FindObjectOfType<Camera>();
	}

	private void Update()
	{
		if (!LocalGameManager.instance.GetTurn())
			return;

		RayCastOnTouch();
		if (Input.GetKeyDown(KeyCode.E))
		{
			#if UNITY_EDITOR
			NetworkClient.instance.SendPlayerMove(Selection.activeGameObject.GetComponent<Tile>());
			ManagePath(Selection.activeGameObject.GetComponent<Tile>(), LocalGameManager.instance.localPlayerIndex);
			#endif
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
					NetworkClient.instance.SendPlayerMove(hitTile);
					ManagePath(hitTile, LocalGameManager.instance.localPlayerIndex);
				}
			}
		}
	}

	//TODO: Hide prefab colors if not local player
	public void ManagePath(Tile targetTile, playingPlayer playerIndex)
	{

		Player playerObject = GameManager.instance.allPlayers[(int)playerIndex].GetComponent<Player>();
		if(targetTile != currentSelectedTarget)
		{
			currentSelectedTarget = targetTile;
			foreach (Tile t in path)
			{
				t.PrefabColor();
			}
			Pathfinding p = new Pathfinding(BoardGrid.instance.grid, playerObject.positionTile, targetTile);
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
			//LocalGameManager.instance.activePlayer.GetComponent<Player>().MoveToTarget(path);
			playerObject.MoveToTarget(path);
			currentSelectedTarget = null;
		}
	}
}
