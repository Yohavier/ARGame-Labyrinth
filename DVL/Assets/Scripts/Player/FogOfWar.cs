using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public static FogOfWar fow;
	private List<GameObject> activeFogOfWarItems = new List<GameObject>();
	private void Awake()
    {
        fow = this;
    }

    public void ChangePosition(Tile newPosition)
    {
		ClearCurrentActiveFOWItems();
		foreach(Tile neighborTile in GetNeighbouringTiles(newPosition))
		{
			var childTrans = neighborTile.GetComponentsInChildren<Transform>();
			foreach(Transform trans in childTrans)
			{
				if (trans.gameObject.layer != 8)
				{
					trans.GetComponent<MeshRenderer>().enabled = true;
					activeFogOfWarItems.Add(trans.gameObject);
				}
			}
		}
    }
	
	private void ClearCurrentActiveFOWItems()
	{
		foreach (GameObject item in activeFogOfWarItems) 
		{
			item.GetComponent<MeshRenderer>().enabled = false;
		}
		activeFogOfWarItems.Clear();
	}

	private List<Tile> GetNeighbouringTiles(Tile a_Tile)
	{
		List<Tile> list = new List<Tile>();
		foreach (Tile item in BoardGrid.GridInstance.grid)
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
}
