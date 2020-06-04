using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;

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

	private void Start()
	{
		if(NetworkManager.instance.isDebug)
			DebugFog();	
	}

	//call if player moves, to update fog of war
	public void OnChangePlayerPosition(Tile newPosition)
    {
		if (NetworkManager.instance.isDebug)
		{
			DebugFog();
			return;
		}

		HideEverythingInFOW();
		if (BoardGrid.instance.grid.Contains(newPosition))
		{
			List<Tile> neighbours = GetScalableNeighbouringTiles(newPosition);
			ToggleVisiblePartOn(neighbours);
		}
		else
		{
			finalFogPath.Add(newPosition);
			List<Tile> neighbours = new List<Tile>();
			neighbours.Add(newPosition);
			ToggleVisiblePartOn(neighbours);			
		}
    }
	
	//clear and hide complete grid
	private void HideEverythingInFOW()
	{
		foreach(Tile t in BoardGrid.instance.grid)
		{
			t.isInFOW = true;
			t.PrefabColor();
			var trans = t.GetComponentsInChildren<Transform>();
			foreach(Transform tr in trans)
			{
				if (tr.CompareTag("Fog"))
				{				
					tr.GetComponent<MeshRenderer>().enabled = true;
				}
			}
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

	//Toogle everything on that is in FOW(passed list) range 
	private void ToggleVisiblePartOn(List<Tile> neighbours)
	{
		foreach (Tile neighborTile in neighbours)
		{
			neighborTile.isInFOW = false;
			neighborTile.PrefabColor();
			var childMeshes = neighborTile.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer mesh in childMeshes)
			{
				if (mesh.CompareTag("Fog"))
				{
					mesh.GetComponent<MeshRenderer>().enabled = false;
				}
				else if (mesh.gameObject.layer != 8)
				{
					if (mesh.GetComponent<Item>())
					{
						if (mesh.GetComponent<Item>().isStored)
						{
							break;
						}
					}
					mesh.GetComponent<MeshRenderer>().enabled = true;
					activeFogOfWarItems.Add(mesh.gameObject);
				}
			}
		}
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
			Tile check = BoardGrid.instance.coordDic[tile.row.ToString() + (tile.column - 1).ToString()];
			if(check.forward && tile.backward)
				allNeighbors.Add(check);
		}	

		if(tile.column + 1 <= 6)
		{
			Tile check = BoardGrid.instance.coordDic[tile.row.ToString() + (tile.column + 1).ToString()];
			if (check.backward && tile.forward)
				allNeighbors.Add(check);
		}

		if(tile.row - 1 >= 0)
		{
			Tile check = BoardGrid.instance.coordDic[(tile.row - 1).ToString() + tile.column.ToString()];
			if (check.right && tile.left)
				allNeighbors.Add(check);
		}

		if(tile.row + 1 <= 6)
		{
			Tile check = BoardGrid.instance.coordDic[(tile.row + 1).ToString() + tile.column.ToString()];
			if (check.left && tile.right)
				allNeighbors.Add(check);
		}
		return allNeighbors;
	}

	public void DebugFog()
	{
		foreach (Tile t in BoardGrid.instance.grid)
		{
			t.isInFOW = false;
			t.PrefabColor();
			List<MeshRenderer> meshes = t.GetComponentsInChildren<MeshRenderer>().ToList();
			foreach(MeshRenderer mesh in meshes)
			{
				if (mesh.CompareTag("Fog"))
				{
					mesh.enabled = false;
				}
				else
				{
					mesh.enabled = true;
					mesh.material.color = Color.white;
				}				
			}
		}
	}
}
