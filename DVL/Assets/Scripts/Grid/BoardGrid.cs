using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardGrid : MonoBehaviour
{
	public GameObject trackingManager;

	public List<GameObject> allPossibleMovingTiles = new List<GameObject>();
	public List<GameObject> allPossibleStaticTiles = new List<GameObject>();

	public static int size;
	private int set_size = 7;
	private float gridSpacing = 0.1f;
	public int[] randomRoations;

	public List<Tile> grid = new List<Tile>();

	private void Start()
	{
		size = set_size;
		SetUpGrid();
	}

	private GameObject GetRandomTile(int row, int column)
	{
		if (row % 2 == 0 && column % 2 == 0)
		{
			int index = Random.Range(0, allPossibleStaticTiles.Count);
			return allPossibleStaticTiles[index];
		}
		int index2 = Random.Range(0, allPossibleMovingTiles.Count);
		return allPossibleMovingTiles[index2];
	}

	private void RemoveTileFromList(GameObject tile)
	{
		if (allPossibleMovingTiles.Contains(tile))
		{
			allPossibleMovingTiles.Remove(tile);
		}
		else if (allPossibleStaticTiles.Contains(tile))
		{
			allPossibleStaticTiles.Remove(tile);
		}
	}

	private void SetUpGrid()
	{
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				GameObject randomTile = GetRandomTile(i, j);
				RemoveTileFromList(randomTile);
				GameObject tile = Instantiate(randomTile, new Vector3(i * gridSpacing, 0f, j * gridSpacing), Quaternion.identity, this.transform);
				tile.transform.localEulerAngles = new Vector3(0f, SetRandomRotation(), 0f);
				Tile component = tile.GetComponent<Tile>();
				component.SetTileData(i, j);
				grid.Add(component);
				if ((i == 0 && j == 0) || (i == 0 && j == 6) || (i == 6 && j == 0) || (i == 6 && j == 6))
				{
					this.GetComponent<SpawnPlayer>().SpawnPlayersInCorner(component);
				}
			}
		}
		trackingManager.GetComponent<HandleTrackedImageLib>().ChangeTrackedPrefab(allPossibleMovingTiles[0]);
		RemoveTileFromList(allPossibleMovingTiles[0]);
	}

	private int SetRandomRotation()
	{
		return randomRoations[Random.Range(0, randomRoations.Length)];
	}

	public void InsertNewRoomPushing(Tile entryTile, Tile newRoom)
	{
		GridMovement moveDir = GetMoveDir(entryTile);
		GameObject val = Object.Instantiate<GameObject>(newRoom.gameObject);
		val.transform.SetParent(this.transform);
		int num = SetNewRoomRotation(newRoom);
		val.transform.localEulerAngles = new Vector3(0f, num, 0f);
		val.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		val.transform.localPosition = entryTile.transform.localPosition - moveDir.moveDir;
		Tile component = val.GetComponent<Tile>();
		component.SetTileData(entryTile.row, entryTile.column);
		component.GetComponent<MeshRenderer>().material.color = component.prefabColor;
		component.GetComponent<FindNearestGridSlot>().enabled = false;
		grid.Add(component);
		AdjustColAndRow(component);
		MoveAllTile(entryTile, component);
		Vector3 localEulerAngles = val.transform.localEulerAngles;
	}

	private void AdjustColAndRow(Tile newTile)
	{
		if (newTile.row == 0)
		{
			newTile.row--;
		}
		else if (newTile.row == 6)
		{
			newTile.row++;
		}
		else if (newTile.column == 0)
		{
			newTile.column--;
		}
		else if (newTile.column == 6)
		{
			newTile.column++;
		}
	}

	private void MoveAllTile(Tile entrytile, Tile newtile)
	{
		bool canMoveVertical = entrytile.canMoveVertical;
		bool canMoveHorizontal = entrytile.canMoveHorizontal;
		int column = entrytile.column;
		int row = entrytile.row;
		List<Tile> list = new List<Tile>();
		GridMovement moveDir = GetMoveDir(entrytile);
		foreach (Tile item in grid)
		{
			if (canMoveHorizontal)
			{
				if (item.row == row)
				{
					item.Move(moveDir);
					list.Add(item);
				}
			}
			else if (canMoveVertical && item.column == column)
			{
				item.Move(moveDir);
				list.Add(item);
			}
		}
		foreach (Tile item2 in list)
		{
			if (item2.row < 0 || item2.row > size - 1 || item2.column < 0 || item2.column > size - 1)
			{
				grid.Remove(item2);
				trackingManager.GetComponent<HandleTrackedImageLib>().ChangeTrackedPrefab(item2.gameObject);
				Object.Destroy(item2.gameObject);
				break;
			}
		}
	}

	private GridMovement GetMoveDir(Tile moveTile)
	{
		GridMovement gridMovement = new GridMovement();
		if (moveTile.canMoveVertical)
		{
			if (moveTile.row <= 0)
			{
				gridMovement.moveDir = new Vector3(gridSpacing, 0f, 0f);
				gridMovement.rowChangeDir = 1;
			}
			else if (moveTile.row >= size - 1)
			{
				gridMovement.moveDir = new Vector3(0f - gridSpacing, 0f, 0f);
				gridMovement.rowChangeDir = -1;
			}
		}
		else if (moveTile.canMoveHorizontal)
		{
			if (moveTile.column <= 0)
			{
				gridMovement.moveDir = new Vector3(0f, 0f, gridSpacing);
				gridMovement.colChangeDir = 1;
			}
			else if (moveTile.column >= size - 1)
			{
				gridMovement.moveDir = new Vector3(0f, 0f, 0f - gridSpacing);
				gridMovement.colChangeDir = -1;
			}
		}
		return gridMovement;
	}

	private int SetNewRoomRotation(Tile newRoom)
	{
		Dictionary<float, int> dictionary = new Dictionary<float, int>();
		float num = Mathf.Abs(newRoom.transform.parent.transform.localEulerAngles.y - this.transform.localEulerAngles.y);
		dictionary.Add(Mathf.Abs(num - 0f), 0);
		dictionary.Add(Mathf.Abs(num - 90f), 90);
		dictionary.Add(Mathf.Abs(num - 180f), 180);
		dictionary.Add(Mathf.Abs(num - 270f), 270);
		dictionary.Add(Mathf.Abs(num - 360f), 0);
		float num2 = 400f;
		foreach (float key in dictionary.Keys)
		{
			if (key < num2)
			{
				num2 = key;
			}
		}
		return dictionary[num2];
	}
}
