using System.Collections.Generic;
using UnityEngine;

//A* pathfinding, always chooses shortest path
public class Pathfinding 
{
	public static List<Tile> FindPath(int stepsLeft, Tile a_start, Tile a_target)
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
				return GetFinalPath(a_start, a_target, stepsLeft);				
			}

			foreach (Tile neighbouringTile in DetectDirectNeighbours.DetectMoveOptionsFromSingleTile(tile))
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
		Debug.LogWarning("No Path Found!");
		return null;
	}

	private static List<Tile> GetFinalPath(Tile start, Tile target, int steps)
	{
		List<Tile> list = new List<Tile>();
		Tile tile = target;
		while (target != start)
		{
			list.Add(target);
			target = target.Parent;
		}
		list.Reverse();
		if (list.Count > steps)
        {
			Debug.LogWarning("Chosen Path is to long!");
			return null;
		}
		return list;
	}

	private static int GetManhattenDistance(Tile current, Tile neighbour)
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
