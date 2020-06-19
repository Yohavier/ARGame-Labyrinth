using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class FogOfWar : MonoBehaviour
{
	public LayerMask fogMask;

	public List<Tile> finalFogPath = new List<Tile>();
	private int fogReach
    {
        get { return FogReach + GetComponent<Player>().fogOfWarModificator; }
    }
	private int FogReach = 2;

	//call if player moves, to update fog of war
	public void OnChangePlayerPosition(Tile newPosition, bool communication)
    {

		if (NetworkManager.instance.isDebug)
		{
			DebugFog();
			return;
		}

		if(communication == false)
        {
            if (finalFogPath.Contains(newPosition))
			{
				finalFogPath.Remove(newPosition);
            }
		}


		List<Tile> neighbours = new List<Tile>();
		if (BoardGrid.instance.grid.Contains(newPosition))
		{
			neighbours = GetScalableNeighbouringTiles(newPosition);
		}
		else
		{
			neighbours.Add(newPosition);					
		}
		neighbours.AddRange(CheckWindow(newPosition));

		
		List<Tile> tempNonVis = new List<Tile>();
		List<Tile> tempNewVis = new List<Tile>();

		for (int i = 0; i < finalFogPath.Count; i++)
        {
            if (!neighbours.Contains(finalFogPath[i]))
            {
				tempNonVis.Add(finalFogPath[i]);
            }
        }
		
        for (int j = 0; j < neighbours.Count; j++)
        {
            if (!finalFogPath.Contains(neighbours[j]))
            {
				tempNewVis.Add(neighbours[j]);
            }
        }

		ToggleNonVisibleTiles(tempNonVis);
		ToggleVisibleTilesOn(tempNewVis);

		finalFogPath = neighbours;
	}

	private List<Tile> CheckWindow(Tile tile)
	{
		List<Tile> windowTiles = new List<Tile>();
		string key = "";
		if (tile.ingameForwardModule == TileDirectionModule.WINDOW)
		{
			key = tile.row.ToString() + (tile.column + 1).ToString();
			if (BoardGrid.instance.coordDic.ContainsKey(key))
				windowTiles.Add(BoardGrid.instance.coordDic[key]);
		}
		if (tile.ingameBackwardModule == TileDirectionModule.WINDOW)
		{
			key = tile.row.ToString() + (tile.column - 1).ToString();
			if (BoardGrid.instance.coordDic.ContainsKey(key))
				windowTiles.Add(BoardGrid.instance.coordDic[key]);
		}
		if (tile.ingameRightModule == TileDirectionModule.WINDOW)
		{
			key = (tile.row + 1).ToString() + tile.column.ToString();
			if (BoardGrid.instance.coordDic.ContainsKey(key))
				windowTiles.Add(BoardGrid.instance.coordDic[key]);
		}
		if (tile.ingameLeftModule == TileDirectionModule.WINDOW)
		{
			key = (tile.row - 1).ToString() + tile.column.ToString();
			if (BoardGrid.instance.coordDic.ContainsKey(key))
				windowTiles.Add(BoardGrid.instance.coordDic[key]);
		}
		return windowTiles;
	}
	private void ToggleNonVisibleTiles(List<Tile> nonVisibleList)
    {
        for (int i = 0; i < nonVisibleList.Count; i++)
        {
			nonVisibleList[i].isInFOW = true;
			nonVisibleList[i].PrefabColor();
			MeshRenderer[] tileMeshes = nonVisibleList[i].GetComponentsInChildren<MeshRenderer>();

			foreach (MeshRenderer mesh in tileMeshes)
			{
				if (CheckIfIsInFogMask(mesh.gameObject))
				{
					mesh.GetComponent<MeshRenderer>().enabled = false;
				}
			}
		}	
    }
	private void ToggleVisibleTilesOn(List<Tile> visibleList)
	{
		foreach (Tile tile in visibleList)
		{	
			tile.isInFOW = false;
			tile.PrefabColor();

			MeshRenderer[] tileMeshes = tile.GetComponentsInChildren<MeshRenderer>();

			foreach (MeshRenderer mesh in tileMeshes)
			{
				if (CheckIfIsInFogMask(mesh.gameObject))
				{
					mesh.GetComponent<MeshRenderer>().enabled = true;
				}
			}
		}
	}

	private List<Tile> GetScalableNeighbouringTiles(Tile a_tile)
	{
		List<Tile> neighbours = new List<Tile>();
		List<Tile> toCheck = new List<Tile>();
		List<Tile> allneighbours = new List<Tile>();

		allneighbours.Add(a_tile);
		toCheck.Add(a_tile);


		for (int i = 0; i < fogReach; i++)
		{
			//for every Tile in to check get the neighbours
			for (int j = 0; j < toCheck.Count; j++)
            {
				allneighbours = allneighbours.Union(AllTilesInWay(toCheck[j])).ToList();
			}

			//for every neighbour check if already in path and in to check
			for (int k = 0; k < allneighbours.Count; k++)
			{
                if (!neighbours.Contains(allneighbours[k]))
                {
					if (!toCheck.Contains(allneighbours[k]))
						toCheck.Add(allneighbours[k]);

					neighbours.Add(allneighbours[k]);
				}
			}
		}
		return neighbours;
	}

	private List<Tile> AllTilesInWay(Tile tile)
	{	
		List<Tile> allNeighbors = new List<Tile>();
		
		if(tile.column - 1 >= 0)
		{
			Tile check = BoardGrid.instance.coordDic[tile.row.ToString() + (tile.column - 1).ToString()];
			if(check.ingameForwardModule == TileDirectionModule.NONE && tile.ingameBackwardModule == TileDirectionModule.NONE)
				allNeighbors.Add(check);
		}	

		if(tile.column + 1 <= 6)
		{
			Tile check = BoardGrid.instance.coordDic[tile.row.ToString() + (tile.column + 1).ToString()];
			if (check.ingameBackwardModule == TileDirectionModule.NONE && tile.ingameForwardModule == TileDirectionModule.NONE) 
				allNeighbors.Add(check);
		}

		if(tile.row - 1 >= 0)
		{
			Tile check = BoardGrid.instance.coordDic[(tile.row - 1).ToString() + tile.column.ToString()];
			if (check.ingameRightModule == TileDirectionModule.NONE && tile.ingameLeftModule == TileDirectionModule.NONE) 
				allNeighbors.Add(check);
		}

		if(tile.row + 1 <= 6)
		{
			Tile check = BoardGrid.instance.coordDic[(tile.row + 1).ToString() + tile.column.ToString()];
			if (check.ingameLeftModule == TileDirectionModule.NONE && tile.ingameRightModule == TileDirectionModule.NONE)
				allNeighbors.Add(check);
		}
		return allNeighbors;
	}
	
	private bool CheckIfIsInFogMask(GameObject toCheck)
    {
		return fogMask == (fogMask | (1 << toCheck.layer));
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
				}				
			}
		}
	}
}
