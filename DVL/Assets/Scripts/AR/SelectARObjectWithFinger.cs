using Assets.Scripts.GameManagement;
using System.Collections.Generic;
using UnityEngine;

public class SelectARObjectWithFinger : MonoBehaviour
{
	public static SelectARObjectWithFinger instance;
	private Vector2 touchPosition;
	public Camera arCamera;
	
	//Lets the raycast only collide with certain things
	public LayerMask mask;
	private Tile currentSelectedTarget;
	public List<Tile> path;
	private void Awake()
	{
		instance = this;
	}
    private void OnEnable()
    {
		Eventbroker.instance.onNotifyNextTurn += NewTurn;
    }
	private void NewTurn()
    {
		HandlePreviousPath();
    }
    private void OnDisable()
    {
		Eventbroker.instance.onNotifyNextTurn -= NewTurn;
    }

    private void Update()
	{
        if (LocalGameManager.instance.activePlayer)
        {
			if(LocalGameManager.instance.activePlayer.GetComponent<Player>().playerState == PlayerState.ALIVE)
            {
				if (!LocalGameManager.instance.activePlayer.GetComponent<Player>().isWalking)
				{
					if (LocalGameManager.instance.GetTurn())
                    {
						if (LocalGameManager.instance._stepsLeft > 0)
						{
                            if (LocalGameManager.instance.canMove)
                            {
								RayCastOnTouch();
								MouseRay();
							}
						}
					}
				}
			}
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
					ManagePath(hitTile, LocalGameManager.instance.localPlayerIndex, LocalGameManager.instance._stepsLeft);
				}
			}
		}
	}
	private void MouseRay()
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 100, mask))
			{
				ManagePath(hit.transform.GetComponent<Tile>(), LocalGameManager.instance.localPlayerIndex, LocalGameManager.instance._stepsLeft);
				NetworkClient.instance.SendPlayerMove(hit.transform.GetComponent<Tile>());
			}
		}
#endif
	}

	public void ManagePath(Tile targetTile, PlayerIndex playerIndex, int steps)
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
			path = p.FindPath(steps);
			
			//Color path red
			if (path != null)
			{
				if (GUIManager.instance.isDebug || playerObject.gameObject == LocalGameManager.instance.activePlayer.gameObject)
				{
					foreach (Tile t in path)
					{
						MeshRenderer[] meshes = t.GetComponentsInChildren<MeshRenderer>();
						foreach (MeshRenderer mesh in meshes)
						{
							if(mesh.CompareTag("Tile"))
								mesh.material.color = Color.red;
						}
					}
				}
			}
		}
		else if (path != null && targetTile == currentSelectedTarget)
		{
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
