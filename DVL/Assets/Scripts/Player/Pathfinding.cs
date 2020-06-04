using System.Collections.Generic;
using UnityEngine;

//A* pathfinding, always chooses shortest path
public class Pathfinding 
{
	private List<Tile> grid;
	private Tile start;
	private Tile target;

	public Pathfinding(List<Tile> boardGrid, Tile c_start, Tile c_target)
	{
		grid = boardGrid;
		start = c_start;
		target = c_target;
	}

	public List<Tile> FindPath()
	{
		Tile a_start = start;
		Tile a_target = target;
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
				return GetFinalPath(a_start, a_target);				
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
		return null;
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

	private List<Tile> GetFinalPath(Tile start, Tile target)
	{
		List<Tile> list = new List<Tile>();
		Tile tile = target;
		while (target != start)
		{
			list.Add(target);
			target = target.Parent;
		}
		list.Reverse();
		return list;
	}

	private int GetManhattenDistance(Tile current, Tile neighbour)
	{
		if(current != null && neighbour != null)
		{
			int num = Mathf.Abs(current.row - neighbour.row);
			int num2 = Mathf.Abs(current.column - neighbour.column);
			return num + num2;
		}
		return 0;
	}
}
