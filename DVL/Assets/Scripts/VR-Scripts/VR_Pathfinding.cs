using System.Collections.Generic;
using UnityEngine;

//A* pathfinding, always chooses shortest path
public class VR_Pathfinding
{
	public static List<VR_Tile> FindPath(int stepsLeft, VR_Tile a_start, VR_Tile a_target)
	{
		List<VR_Tile> list = new List<VR_Tile>();
		HashSet<VR_Tile> hashSet = new HashSet<VR_Tile>();
		list.Add(a_start);
		while (list.Count > 0)
		{
			VR_Tile tile = list[0];
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

			foreach (VR_Tile neighbouringTile in VR_DetectNeighbours.DetectMoveOptionsFromSingleTile(tile))
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

	private static List<VR_Tile> GetFinalPath(VR_Tile start, VR_Tile target, int steps)
	{
		List<VR_Tile> list = new List<VR_Tile>();
		VR_Tile tile = target;
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

	private static int GetManhattenDistance(VR_Tile current, VR_Tile neighbour)
	{
		if (current != null && neighbour != null)
		{
			int num = Mathf.Abs(current.row - neighbour.row);
			int num2 = Mathf.Abs(current.column - neighbour.column);
			return num + num2;
		}
		return 0;
	}
}
