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
#if UNITY_STANDALONE
		MouseRay();
#endif
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
	private void MouseRay()
	{
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 100, mask))
			{
				ManagePath(hit.transform.GetComponent<Tile>(), LocalGameManager.instance.localPlayerIndex);
				NetworkClient.instance.SendPlayerMove(hit.transform.GetComponent<Tile>());
			}
		}
	}


	public void ManagePath(Tile targetTile, playingPlayer playerIndex)
	{
		Player playerObject = GameManager.instance.allPlayers[(int)playerIndex].GetComponent<Player>();

		if (targetTile == null)
		{
			currentSelectedTarget = targetTile;
			HandlePreviousPath();

			if (path != null)
				path.Clear();
		}
		else if (targetTile != currentSelectedTarget)
		{
			currentSelectedTarget = targetTile;
			HandlePreviousPath();

			if (path != null) 
				path.Clear();

			Pathfinding p = new Pathfinding(BoardGrid.instance.grid, playerObject.positionTile, currentSelectedTarget);
			path = p.FindPath();
			if (path != null)
			{
				if (NetworkManager.instance.isDebug || playerObject.gameObject == LocalGameManager.instance.activePlayer.gameObject)
				{
					foreach (Tile t in path)
					{
						MeshRenderer[] meshes = t.GetComponentsInChildren<MeshRenderer>();
						foreach (MeshRenderer mesh in meshes)
						{
							mesh.material.color = Color.red;
						}
					}
				}
			}
		}
		else if (path != null && targetTile == currentSelectedTarget)
		{
			Debug.Log(path.Count);
			HandlePreviousPath();
			foreach(Tile t in path)
			{
				t.PrefabColor();
			}
			playerObject.MoveToTarget(path);
			currentSelectedTarget = null;
		}
		else
			Debug.LogWarning("Something went wrong with path");
	}

	private void HandlePreviousPath()
	{
		if (path != null)
		{
			foreach (Tile t in path)
			{
				t.PrefabColor();
			}
		}
	}
}
