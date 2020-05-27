using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Pathfinding : MonoBehaviour
{
	public Tile TargetPosition;

	private Tile prevTile;

	private List<Tile> grid;

	private List<Tile> CurrentSavedFinalPath;

	public Button startMoveButton;

	private void OnEnable()
	{
		startMoveButton = FindObjectOfType<Button>();
		startMoveButton.onClick.AddListener(SendMovePathToPlayer);
		grid = FindObjectOfType<BoardGrid>().grid;
	}
	private void OnDisable()
	{
		startMoveButton.onClick.RemoveListener(SendMovePathToPlayer);
	}

	private void Update()
	{
		if (TargetPosition != null && prevTile != TargetPosition)
		{
			prevTile = TargetPosition;
			FindPath(this.GetComponent<Player>().positionTile, TargetPosition);
		}
	}

	private void FindPath(Tile a_start, Tile a_target)
	{
		List<Tile> list = new List<Tile>();
		HashSet<Tile> hashSet = new HashSet<Tile>();
		list.Add(a_start);
		while (list.Count > 0)
		{
			Tile tile = list[0];
			for (int i = 1; i < list.Count; i++)
			{
				if (list[i].FCost < tile.FCost || (list[i].FCost == tile.FCost && list[i].hCost < tile.hCost))
				{
					tile = list[i];
				}
			}
			list.Remove(tile);
			hashSet.Add(tile);
			if (tile == a_target)
			{
				GetFinalPath(a_start, a_target);
				return;
			}
			foreach (Tile neighbouringTile in GetNeighbouringTiles(tile))
			{
				if (!hashSet.Contains(neighbouringTile))
				{
					int num = tile.gCost + GetManhattenDistance(tile, neighbouringTile);
					if (num < neighbouringTile.gCost || !list.Contains(neighbouringTile))
					{
						neighbouringTile.gCost = num;
						neighbouringTile.hCost = GetManhattenDistance(neighbouringTile, a_target);
						neighbouringTile.Parent = tile;
						if (!list.Contains(neighbouringTile))
						{
							list.Add(neighbouringTile);
						}
					}
				}
			}
		}
		MonoBehaviour.print((object)"No Path Found");
		if (CurrentSavedFinalPath != null)
		{
			ClearPathMarks(CurrentSavedFinalPath);
		}
	}

	private void ClearPathMarks(List<Tile> path)
	{
		if (path.Count != 0)
		{
			foreach (Tile item in path)
			{
				item.GetComponent<MeshRenderer>().material.color = item.prefabColor;
			}
		}
		path.Clear();
	}

	private List<Tile> GetNeighbouringTiles(Tile a_Tile)
	{
		List<Tile> list = new List<Tile>();
		foreach (Tile item in grid)
		{
			if ((item.column == a_Tile.column && (item.row == a_Tile.row - 1 || item.row == a_Tile.row + 1)) || (item.row == a_Tile.row && (item.column == a_Tile.column - 1 || item.column == a_Tile.column + 1)))
			{
				if (item.column == a_Tile.column - 1 && a_Tile.backward && item.forward)
				{
					list.Add(item);
				}
				else if (item.column == a_Tile.column + 1 && a_Tile.forward && item.backward)
				{
					list.Add(item);
				}
				else if (item.row == a_Tile.row - 1 && a_Tile.left && item.right)
				{
					list.Add(item);
				}
				else if (item.row == a_Tile.row + 1 && a_Tile.right && item.left)
				{
					list.Add(item);
				}
			}
		}
		return list;
	}

	private void GetFinalPath(Tile start, Tile target)
	{
		if (CurrentSavedFinalPath != null)
		{
			ClearPathMarks(CurrentSavedFinalPath);
		}
		List<Tile> list = new List<Tile>();
		Tile tile = target;
		TargetPosition.GetComponent<MeshRenderer>().material.color = Color.red;
		list.Add(TargetPosition);
		while (tile != start)
		{
			list.Add(tile);
			tile.GetComponent<MeshRenderer>().material.color = Color.red;
			tile = tile.Parent;
		}
		list.Reverse();
		CurrentSavedFinalPath = list;
	}

	private int GetManhattenDistance(Tile current, Tile neighbour)
	{
		int num = Mathf.Abs(current.row - neighbour.row);
		int num2 = Mathf.Abs(current.column - neighbour.column);
		return num + num2;
	}

	private void SendMovePathToPlayer()
	{
		if (CurrentSavedFinalPath.Count > 0)
		{
			TargetPosition = null;
			LocalGameManager.local.activePlayer.GetComponent<Player>().MoveToTarget(CurrentSavedFinalPath);
		}
	}
}
