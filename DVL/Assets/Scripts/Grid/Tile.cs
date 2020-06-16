using System;
using UnityEngine;
using System.Collections;
using System.Threading;

public enum TileDirectionModule { NONE, WALL, DOOR, WINDOW}
public class Tile : MonoBehaviour
{
	public int row;	//right - left
	public int column; //forward - backward

	[HideInInspector] public bool edgePiece;
	[HideInInspector] public bool canMoveHorizontal;
	[HideInInspector] public bool canMoveVertical;

	[Header("Pathfinding Costs")]
	[HideInInspector] public int gCost;
	[HideInInspector] public int hCost;
	[HideInInspector] public int FCost => gCost + hCost;
	[HideInInspector] public Tile Parent;

	[Header("Modules")]
	public TileDirectionModule initForwardModule;
	public TileDirectionModule initBackwardModule;
	public TileDirectionModule initRightModule;
	public TileDirectionModule initLeftModule;


	[HideInInspector] public TileDirectionModule ingameForwardModule;
	[HideInInspector] public TileDirectionModule ingameBackwardModule;
	[HideInInspector] public TileDirectionModule ingameRightModule;
	[HideInInspector] public TileDirectionModule ingameLeftModule;

	public bool CheckModulation(TileDirectionModule modulation)
    {
        switch (modulation)
        {
			case TileDirectionModule.NONE:
				return true;
			case TileDirectionModule.WALL:
				return false;
			case TileDirectionModule.WINDOW:
				return false;
			case TileDirectionModule.DOOR:
				if (doorOpen)
					return true;
				else
					return false;
			default:
				Debug.LogError("Invalid Movement on Tile " + gameObject.name);
				return false;
        }			
    }

	private bool doorOpen = true;
	public bool isInFOW;

	public int index = -1; //Identifier of tile, -1 invalid index

	//Set the Data on Init or if newly pushed into the grid (Called by BoardGrid)
	public void SetTileData(int rowNum, int colNum)
	{
		isInFOW = true;
		row = rowNum;
		column = colNum;
		PrefabColor();
		UpdateTileState();
		UpdateTileMoveOptions();
	}

	//call to move the Tile on the grid
	public void Move(GridMovement move)
	{
		var targetPos = transform.localPosition + move.moveDir;
		StartCoroutine(MoveInterpolate(transform.localPosition, targetPos, 1));
		row += move.rowChangeDir;
		column += move.colChangeDir;
		UpdateTileState();
		UpdateTileMoveOptions();
	}

	//Lerp between 2 Tile position over Time
	private IEnumerator MoveInterpolate(Vector3 startPos, Vector3 targetPos, float time)
	{
		float i = 0.0f;
		float rate = 1.0f / time;
		while (i < 1.0f)
		{
			BoardGrid.instance.inMove = true;
			i += Time.deltaTime * rate;
			transform.localPosition = Vector3.Lerp(startPos, targetPos, i);
			yield return null;
		}

		if (row < 0 || row > BoardGrid.instance.size - 1 || column < 0 || column > BoardGrid.instance.size - 1)
		{
			BoardGrid.instance.inMove = false;
			BoardGrid.instance.RemoveTileFromGrid(this);
			HandleTrackedImageLib.CustomTrackingManagerInstance.ChangeTrackedPrefab(this.gameObject);
		}
	}

	//Update the possible Movestates of the Tile
	private void UpdateTileState()
	{
		if (row % 2 == 0 && column % 2 == 0)
		{
			canMoveHorizontal = false;
			canMoveVertical = false;
		}
		else if (row % 2 != 0 && column % 2 == 0)
		{
			canMoveVertical = false;
			canMoveHorizontal = true;
		}
		else if (row % 2 == 0 && column % 2 != 0)
		{
			canMoveVertical = true;
			canMoveHorizontal = false;
		}
		else
		{
			canMoveVertical = true;
			canMoveHorizontal = true;
		}

		
		if (row == 0 || column == 0 || column == BoardGrid.instance.size - 1 || row == BoardGrid.instance.size - 1)
		{
			edgePiece = true;
		}
		else
		{
			edgePiece = false;
		}
	}

	//changes the move bools dependign on the rotation of the Tile in the Grid
	private void UpdateTileMoveOptions()
	{
		if (transform.localEulerAngles.y == 0f)
		{
			ingameForwardModule = initForwardModule;
			ingameBackwardModule = initBackwardModule;
			ingameRightModule = initRightModule;
			ingameLeftModule = initLeftModule;
		}
		else if (transform.localEulerAngles.y == 90f)
		{
			ingameForwardModule = initLeftModule;
			ingameBackwardModule = initRightModule;
			ingameRightModule = initForwardModule;
			ingameLeftModule = initBackwardModule;
		}
		else if (transform.localEulerAngles.y == 180f)
		{
			ingameForwardModule = initBackwardModule;
			ingameBackwardModule = initForwardModule;
			ingameRightModule = initLeftModule;
			ingameLeftModule = initRightModule;
		}
		else if (transform.localEulerAngles.y == 270f)
		{
			ingameForwardModule = initRightModule;
			ingameBackwardModule = initLeftModule;
			ingameRightModule = initBackwardModule;
			ingameLeftModule = initForwardModule;
		}
	}

	public void PrefabColor()
	{
		MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
		if (isInFOW && !NetworkManager.instance.isDebug)
		{			
			foreach(MeshRenderer mesh in meshes)
			{
				mesh.material.color = Color.black;
			}
		}
		else
		{
			foreach(MeshRenderer mesh in meshes)
			{
				mesh.material.color = Color.white;
			}
		}
	}

	public void ToggleDoors()
    {
		doorOpen = !doorOpen;
		if (doorOpen)
			OpenTileDoors();
		else
			CloseTileDoors();

		LocalGameManager.instance.activePlayer.GetComponent<FogOfWar>().OnChangePlayerPosition(this);
    }

	private void OpenTileDoors()
    {
		//Play Animation
    }

	private void CloseTileDoors()
    {
		//Play Animation
    }
}
