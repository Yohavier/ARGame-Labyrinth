using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public class BoardGrid : MonoBehaviour
{
	public static BoardGrid instance;

	public GameObject trackingManager;
	public HandleTrackedImageLib trackingInstance;
	public Dictionary<string, Tile> coordDic = new Dictionary<string, Tile>();
	//Prefab Lists of Tiles
	public List<GameObject> allPossibleMovingTiles = new List<GameObject>();
	public List<GameObject> allPossibleStaticTiles = new List<GameObject>();

	public List<float> seedList = new List<float>();
	public int seedCount = -1;
	public bool readyToSetup = false;
	public bool inMove = false;

	public int tileCount;

	public int size;
	private int set_size = 7;
	private float gridSpacing = 0.1f;
	public int[] randomRoations;

	//List of the current Grid
	public List<Tile> grid = new List<Tile>();
	public Tile trackedTile;
	public Tile lastTrackedTile;

	public GameObject fog;

	#region Initialization 
	private void Awake()
	{
		instance = this;
		trackingInstance = trackingManager.GetComponent<HandleTrackedImageLib>();
	}
	private void Start()
	{
		size = set_size;
	}

	private void OnGUI()
	{
		if (NetworkManager.instance.isDebug)
		{
			foreach (Tile tile in grid)
			{
				Vector2 pos = Camera.main.WorldToScreenPoint(tile.transform.position);
				pos.y = Screen.height - pos.y;
				string label = tile.index.ToString() + System.Environment.NewLine + tile.row.ToString() + System.Environment.NewLine + tile.column.ToString();
				Vector2 size = GUI.skin.label.CalcSize(new GUIContent(label));
				pos -= size / 2;
				GUI.Label(new Rect(pos, new Vector2(200, 200)), tile.index.ToString());
			}
		}
	}

	private void Update()
	{
		if (readyToSetup)
		{
			readyToSetup = false;
			SetUpGrid();
		}
		if (trackedTile == null)
			trackedTile = trackingInstance.tilePrefabParent.GetComponent<Tile>();
		else if (!inMove)
			trackedTile.index = 0;
	}

	//Remove a tile from the 2 lists
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

	//Select a random Tile, out of the 2 lists
	private GameObject GetRandomTileFromSeed(int row, int column)
	{
		seedCount++;
		if (row % 2 == 0 && column % 2 == 0)
		{
			int index = Convert.ToInt32(Math.Max(0, seedList[seedCount] * allPossibleStaticTiles.Count - 1));
			return allPossibleStaticTiles[index];
		}
		int index2 = Convert.ToInt32(Math.Max(0, seedList[seedCount] * allPossibleMovingTiles.Count - 1));
		return allPossibleMovingTiles[index2];
	}

	//Create Random Rotation for the Grid Tiles
	private int SetRandomRotationFromSeed()
	{
		int index = Convert.ToInt32(Math.Max(0, seedList[seedCount] * randomRoations.Length - 1));
		return randomRoations[index];
	}

	//Set up grid and calls to spawn Players 
	public void SetUpGrid()
	{
		List<Tile> cornerTiles = new List<Tile>();
		for (int row = 0; row < size; row++)
		{
			for (int column = 0; column < size; column++)
			{
				GameObject randomTile = GetRandomTileFromSeed(row, column);
				RemoveTileFromList(randomTile);
				GameObject tile = Instantiate(randomTile, new Vector3(row * gridSpacing, 0f, column * gridSpacing), Quaternion.identity, this.transform);
				tile.transform.localEulerAngles = new Vector3(0f, SetRandomRotationFromSeed(), 0f);
				Tile component = tile.GetComponent<Tile>();
				component.SetTileData(row, column);
				tileCount++;
				component.index = tileCount;
				grid.Add(component);
				CreateFogForTile(component);
				coordDic.Add(row.ToString() + column.ToString(), component);
				if ((row == 0 && column == 0) || (row == 0 && column == 6) || (row == 6 && column == 0) || (row == 6 && column == 6))
				{
					cornerTiles.Add(component);
				}
			}
		}
		GameObject leftOverTile = Instantiate(allPossibleMovingTiles[0]);
		CreateFogForTile(leftOverTile.GetComponent<Tile>());
		trackingManager.GetComponent<HandleTrackedImageLib>().ChangeTrackedPrefab(leftOverTile);
		RemoveTileFromList(allPossibleMovingTiles[0]);
		GetComponent<SpawnPlayer>().SpawnPlayersInCorner(cornerTiles);
		GetComponent<SpawnItems>().SetItemOnGrid();
	}

	public void CreateFogForTile(Tile tile)
	{
		if (NetworkManager.instance.isDebug)
			return;
		GameObject f = Instantiate(fog);
		f.transform.SetParent(tile.transform);
		f.transform.localPosition = new Vector3(0, 0.05f, 0);
	}
	#endregion

	#region new Room
	//inserts the Room into the grid and moves all depending tiles
	public void InsertNewRoomPushing(Tile entryTile, Tile newRoom)
	{
		inMove = true;
		GridMovement moveDir = GetMoveDir(entryTile);
		GameObject val = newRoom.gameObject;
		int num = SetNewRoomRotation(newRoom);
		val.transform.SetParent(this.transform);
		val.transform.localEulerAngles = new Vector3(0f, num, 0f);
		val.transform.localPosition = entryTile.transform.localPosition - moveDir.moveDir;
		Tile component = val.GetComponent<Tile>();
		component.SetTileData(entryTile.row, entryTile.column);
		component.GetComponent<FindNearestGridSlot>().enabled = false;
		grid.Add(component);
		AdjustColAndRow(component);
		MoveAllTile(entryTile, component);
		lastTrackedTile = component;
		grid.OrderBy(x => x.index);
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
		int dir = 0;

		if (moveDir.colChangeDir != 0)
		{
			dir = moveDir.colChangeDir;
		}

		else
		{
			dir = moveDir.rowChangeDir;
		}

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
	}

	private void UpdateDic()
	{
		coordDic.Clear();
		foreach(Tile t in grid)
		{
			coordDic.Add(t.row.ToString() + t.column.ToString(), t);
		}
	}

	public void RemoveTileFromGrid(Tile removedTile)
	{
		lastTrackedTile.index = removedTile.index;
		grid.Remove(removedTile);
		UpdateDic();
		FogOfWar.fow.OnChangePlayerPosition(LocalGameManager.instance.activePlayer.GetComponent<Player>().positionTile);
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
    #endregion
}
