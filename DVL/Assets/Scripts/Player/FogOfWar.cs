using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Scripts.GameManagement;

public class FogOfWar : MonoBehaviour
{
	public LayerMask fogMask;
	public List<Tile> finalFogPath = new List<Tile>();

	//call if player moves, to update fog of war
	public void OnChangePlayerPosition(Tile newPosition)
    {
		List<Tile> neighbours = new List<Tile>();
		if (BoardGrid.instance.grid.Contains(newPosition))
		{
			//DetectDirectNeighbours n = new DetectDirectNeighbours();
			neighbours = DetectDirectNeighbours.DetectTileRadius(newPosition, LocalGameManager.instance.activePlayer.GetComponent<Player>().fogOfWarRadius, true);
		}
		else
		{
			neighbours.Add(newPosition);					
		}
		neighbours.AddRange(GetWindowsInTile(newPosition));

		if (GetComponent<Player>().communicatorPowerUp)
			neighbours.AddRange(GetCommunicatorTile(GetComponent<Player>().communicatorPowerUp));


		
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

		if (GUIManager.instance.isDebug)
		{
			tempNewVis = DebugFog();
		}

		ToggleNonVisibleTiles(tempNonVis);
		ToggleVisibleTilesOn(tempNewVis);
	}

	private List<Tile> GetCommunicatorTile(CommunicatorPowerUp powerUp) 
	{
		Player targetPlayer = powerUp.targetForCommunication.GetComponent<Player>();
		return DetectDirectNeighbours.DetectTileRadius(targetPlayer.positionTile, targetPlayer.fogOfWarRadius, true);
	}
	private List<Tile> GetWindowsInTile(Tile tile)
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
			finalFogPath.Remove(nonVisibleList[i]);
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
        for (int i = 0; i < visibleList.Count; i++)
        {
			finalFogPath.Add(visibleList[i]);
			visibleList[i].isInFOW = false;
			visibleList[i].PrefabColor();

			MeshRenderer[] tileMeshes = visibleList[i].GetComponentsInChildren<MeshRenderer>();

			foreach (MeshRenderer mesh in tileMeshes)
			{
				if (CheckIfIsInFogMask(mesh.gameObject))
				{
					mesh.GetComponent<MeshRenderer>().enabled = true;
				}
			}
		}
	}
	private bool CheckIfIsInFogMask(GameObject toCheck)
    {
		return fogMask == (fogMask | (1 << toCheck.layer));
    }

	public List<Tile> DebugFog()
	{
		return BoardGrid.instance.grid;
	}
}
