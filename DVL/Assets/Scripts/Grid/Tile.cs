using System;
using UnityEngine;
using System.Collections;
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
		var targetPos = transform.localPosition + move.moveDir;
		StartCoroutine(MoveInterpolate(transform.localPosition, targetPos, 1));
		row += move.rowChangeDir;
		column += move.colChangeDir;
		UpdateTileState();
		UpdatePathfindingOptions();
		MessagePlayer();
	}
	private void MessagePlayer()
	{
		var player = transform.GetComponentInChildren<Player>();
		if (player == LocalGameManager.local.activePlayer)
		{
			player.ChangePlayerPosition(this);
		}
	}
	private IEnumerator MoveInterpolate(Vector3 startPos, Vector3 targetPos, float time)
	{
		float i = 0.0f;
		float rate = 1.0f / time;
		while (i < 1.0f)
		{
			i += Time.deltaTime * rate;
			transform.position = Vector3.Lerp(startPos, targetPos, i);
			yield return null;
		}

		if (row < 0 || row > BoardGrid.GridInstance.size - 1 || column < 0 || column > BoardGrid.GridInstance.size - 1)
		{
			BoardGrid.GridInstance.RemoveTileFromGrid(this);
			HandleTrackedImageLib.CustomTrackingManagerInstance.ChangeTrackedPrefab(this.gameObject);
		}
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
		if (row == 0 || column == 0 || column == BoardGrid.GridInstance.size - 1 || row == BoardGrid.GridInstance.size - 1)
		{
			edgePiece = true;
		}
		else
		{
			edgePiece = false;
		}
	}
}
