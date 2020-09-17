using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VR_Grid : MonoBehaviour
{
    public static VR_Grid instance;

    public List<VR_Tile> grid = new List<VR_Tile>();
    public Dictionary<string, VR_Tile> coordDic = new Dictionary<string, VR_Tile>();

    public List<GameObject> allPossibleMovingTiles = new List<GameObject>();
    public List<GameObject> allPossibleStaticTiles = new List<GameObject>();

    public int[] randomRoations;
    [HideInInspector] public int size = 7;
    [HideInInspector] public float gridSpacing;
    private bool inMove = false;
    public bool _inMove
    {
        get
        {
            return inMove;
        }
        set
        {
            inMove = value;
            if (!inMove)
                enemyScript.StartMovement();
            else
                enemyScript.StopOnGridMove();
        }
    }

    public GameObject enemy;
    private VR_Enemy enemyScript;
    public VR_Tile trackedTile;
    public VR_Tile lastTrackedTile;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        gridSpacing = 2.5f;
        SetUpGrid();
    }
    public void SetUpGrid()
    {
        List<VR_Tile> cornerTiles = new List<VR_Tile>();
        int tileCount = 0;
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                GameObject randomTile = GetRandomTileFromSeed(row, column);
                RemoveTileFromList(randomTile);
                GameObject tile = Instantiate(randomTile, Vector3.zero, Quaternion.identity, this.transform);
                tile.transform.localPosition = new Vector3(row * gridSpacing, 0f, column * gridSpacing);
                tile.transform.localEulerAngles = new Vector3(0f, SetRandomRotationFromSeed(), 0f);
                VR_Tile component = tile.GetComponent<VR_Tile>();
                component.SetTileData(row, column, true);
                tileCount++;
                component.index = tileCount;
                grid.Add(component);
                coordDic.Add(row.ToString() + column.ToString(), component);
                if ((row == 0 && column == 0) || (row == 0 && column == 6) || (row == 6 && column == 0) || (row == 6 && column == 6))
                {
                    cornerTiles.Add(component);
                }
            }
        }
        SpawnEnemy(grid.Last());
        GameObject leftOverTile = Instantiate(allPossibleMovingTiles[0]);
        VR_Controller.instance.ChangeTrackedPrefab(leftOverTile);
        RemoveTileFromList(allPossibleMovingTiles[0]);
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
    private GameObject GetRandomTileFromSeed(int row, int column)
    {
        if (row % 2 == 0 && column % 2 == 0)
        {
            int index = Random.Range(0, allPossibleStaticTiles.Count -1);
            return allPossibleStaticTiles[index];
        }
        int index2 = Random.Range(0, allPossibleMovingTiles.Count - 1);
        return allPossibleMovingTiles[index2];
    }
    private int SetRandomRotationFromSeed()
    {
        int index = Random.Range(0, 3);
        return randomRoations[index];
    }

    public void RemoveTileFromGrid(VR_Tile removedTile)
    {
        grid.Remove(removedTile);
        UpdateDic();
    }

    private void UpdateDic()
    {
        coordDic.Clear();
        foreach (VR_Tile t in grid)
        {
            coordDic.Add(t.row.ToString() + t.column.ToString(), t);
        }
    }
    private void SpawnEnemy(VR_Tile tile)
    {
        GameObject player = Instantiate(enemy);
        enemyScript = player.GetComponent<VR_Enemy>();
        player.transform.SetParent(tile.transform);
        player.GetComponent<VR_Enemy>().ChangePlayerPosition(tile);
        player.transform.localPosition = Vector3.zero;
    }

    public void InsertNewRoomPushing(VR_Tile entryTile, VR_Tile newRoom)
    {
        _inMove = true;
        MakeSureEverythingIsActive(newRoom);
        GridMovement moveDir = GetMoveDir(entryTile);
        GameObject val = newRoom.gameObject;
        int num = SetNewRoomRotation(newRoom);
        val.transform.SetParent(this.transform);
        val.transform.localEulerAngles = new Vector3(0f, num, 0f);
        val.transform.localPosition = entryTile.transform.localPosition - moveDir.moveDir;
        VR_Tile component = val.GetComponent<VR_Tile>();
        component.SetTileData(entryTile.row, entryTile.column, false);
        component.GetComponent<FindNearestGridSlot>().enabled = false;
        grid.Add(component);
        AdjustColAndRow(component);
        MoveAllTile(entryTile, component);
        lastTrackedTile = component;
        grid.OrderBy(x => x.index);
    }
    private void MakeSureEverythingIsActive(VR_Tile tile)
    {
        tile.GetComponent<BoxCollider>().enabled = true;
        MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = true;
        }
    }

    private void AdjustColAndRow(VR_Tile newTile)
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

    private void MoveAllTile(VR_Tile entrytile, VR_Tile newtile)
    {
        AkSoundEngine.PostEvent("tile_move", gameObject);
        bool canMoveVertical = entrytile.canMoveVertical;
        bool canMoveHorizontal = entrytile.canMoveHorizontal;
        int column = entrytile.column;
        int row = entrytile.row;
        List<VR_Tile> list = new List<VR_Tile>();
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

        foreach (VR_Tile item in grid)
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
    private GridMovement GetMoveDir(VR_Tile moveTile)
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
    private int SetNewRoomRotation(VR_Tile newRoom)
    {
        Dictionary<float, int> dictionary = new Dictionary<float, int>();
        float num = Mathf.Abs(newRoom.transform.localEulerAngles.y - this.transform.localEulerAngles.y);
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
