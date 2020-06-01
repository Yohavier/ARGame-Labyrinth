using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FogOfWar : MonoBehaviour
{
    public static FogOfWar fow;
	public List<GameObject> activeFogOfWarItems = new List<GameObject>();
	public int fogReach;
	public List<Tile> finalFogPath = new List<Tile>();

	private void Awake()
    {
        fow = this;
    }

	//call if player moves, to update fog of war
    public void OnChangePlayerPosition(Tile newPosition)
    {
		ClearCurrentActiveFOWItems();
		if (BoardGrid.GridInstance.grid.Contains(newPosition))
		{
			foreach (Tile neighborTile in GetScalableNeighbouringTiles(newPosition))
			{			
				neighborTile.isInFOW = false;
				neighborTile.PrefabColor();
				var childTrans = neighborTile.GetComponentsInChildren<Transform>();
				foreach (Transform trans in childTrans)
				{
					if (trans.gameObject.layer != 8)
					{
						trans.GetComponent<MeshRenderer>().enabled = true;
						activeFogOfWarItems.Add(trans.gameObject);
					}
				}
			}
		}
		else
		{
			finalFogPath.Add(newPosition);
			newPosition.isInFOW = false;
			newPosition.PrefabColor();
			var childTrans = newPosition.GetComponentsInChildren<Transform>();
			foreach (Transform trans in childTrans)
			{
				if (trans.gameObject.layer != 8)
				{
					trans.GetComponent<MeshRenderer>().enabled = true;
					activeFogOfWarItems.Add(trans.gameObject);
				}
			}
		}
    }
	
	//clear and hide current list of active gameObjects 
	private void ClearCurrentActiveFOWItems()
	{
		foreach(Tile t in BoardGrid.GridInstance.grid)
		{
			t.isInFOW = true;
			t.PrefabColor();
		}
		foreach (GameObject item in activeFogOfWarItems) 
		{
			if(item != null)
			{
				item.GetComponent<MeshRenderer>().enabled = false;
			} 
		}
		activeFogOfWarItems.Clear();
		finalFogPath.Clear();
	}

	private List<Tile> GetScalableNeighbouringTiles(Tile a_tile)
	{
		List<Tile> toCheck = new List<Tile>();
		finalFogPath.Add(a_tile);
		toCheck.Add(a_tile);

		for (int i = 0; i < fogReach; i++)
		{
			if (toCheck.Count > 0)
			{
				List<Tile> neighbour = new List<Tile>();
				foreach (Tile t in toCheck)
				{
					neighbour = neighbour.Union(AllNeighboursOfTile(t)).ToList();
				}
				foreach (Tile t in neighbour)
				{
					if (!finalFogPath.Contains(t))
					{
						finalFogPath.Add(t);
						toCheck.Add(t);
					}
				}
			}
		}
		return finalFogPath;
	}

	//forward + col
	//right +row
	private List<Tile> AllNeighboursOfTile(Tile tile)
	{
		List<Tile> allNeighbors = new List<Tile>();
		
		if(tile.column - 1 >= 0)
		{
			Tile check = BoardGrid.GridInstance.coordDic[tile.row.ToString() + (tile.column - 1).ToString()];
			if(check.forward && tile.backward)
				allNeighbors.Add(check);
		}	

		if(tile.column + 1 <= 6)
		{
			Tile check = BoardGrid.GridInstance.coordDic[tile.row.ToString() + (tile.column + 1).ToString()];
			if (check.backward && tile.forward)
				allNeighbors.Add(check);
		}

		if(tile.row - 1 >= 0)
		{
			Tile check = BoardGrid.GridInstance.coordDic[(tile.row - 1).ToString() + tile.column.ToString()];
			if (check.right && tile.left)
				allNeighbors.Add(check);
		}

		if(tile.row + 1 <= 6)
		{
			Tile check = BoardGrid.GridInstance.coordDic[(tile.row + 1).ToString() + tile.column.ToString()];
			if (check.left && tile.right)
				allNeighbors.Add(check);
		}
		return allNeighbors;
	}
}
