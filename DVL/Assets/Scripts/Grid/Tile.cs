using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
	public int row;

	public int column;

	public bool edgePiece;

	public bool canMoveHorizontal;

	public bool canMoveVertical;

	public Color prefabColor;

	public Tile Parent;

	[Header("Init Variables")]
	public bool initForward;

	public bool initRight;

	public bool initBackward;

	public bool initLeft;

	[Header("Dynamik Variables")]
	public bool forward;

	public bool right;

	public bool backward;

	public bool left;

	public int gCost;

	public int hCost;

	public int FCost => gCost + hCost;

	public void SetTileData(int rowNum, int colNum)
	{
		this.GetComponent<MeshRenderer>().material.color = prefabColor;
		row = rowNum;
		column = colNum;
		UpdateTileState();
		UpdatePathfindingOptions();
	}

	private void UpdatePathfindingOptions()
	{
		if (transform.localEulerAngles.y == 0f)
		{
			forward = initForward;
			right = initRight;
			backward = initBackward;
			left = initLeft;
		}
		else if (transform.localEulerAngles.y == 90f)
		{
			forward = initLeft;
			right = initForward;
			backward = initRight;
			left = initBackward;
		}
		else if (transform.localEulerAngles.y == 180f)
		{
			forward = initBackward;
			right = initLeft;
			backward = initForward;
			left = initRight;
		}
		else if (transform.localEulerAngles.y == 270f)
		{
			forward = initRight;
			right = initBackward;
			backward = initLeft;
			left = initForward;
		}
	}

	public void Move(GridMovement move)
	{
		transform.localPosition += move.moveDir;
		row += move.rowChangeDir;
		column += move.colChangeDir;
		UpdateTileState();
		UpdatePathfindingOptions();
	}

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
		if (row == 0 || column == 0 || column == BoardGrid.size - 1 || row == BoardGrid.size - 1)
		{
			edgePiece = true;
		}
		else
		{
			edgePiece = false;
		}
	}
}
