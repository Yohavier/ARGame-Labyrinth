using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGrid : MonoBehaviour
{
    public GameObject trackingManager;

    //prefablist of tiles that can move
    public List<GameObject> allPossibleMovingTiles = new List<GameObject>();
    //prefablist of tiles that are static
    public List<GameObject> allPossibleStaticTiles = new List<GameObject>();

    //variables for instantiation
    public static int size;
    private int set_size = 7;
    private float gridSpacing = 0.1f;

    //List of all tiles that are currently in the grid 
    public List<Tile> grid = new List<Tile>();


    #region Handle Init
    private void Awake()
    {
        size = set_size;
        SetUpGrid();
    }
    //Get a random tile from the right list
    private GameObject GetRandomTile(int row, int column)
    {
        if (row % 2 == 0 && column % 2 == 0)
        {
            int randomNumber = Random.Range(0, allPossibleStaticTiles.Count);
            return allPossibleStaticTiles[randomNumber];
        }
        else
        {
            int randomNumber = Random.Range(0, allPossibleMovingTiles.Count);
            return allPossibleMovingTiles[randomNumber];
        }
    }
    private void RemoveTileFromList(GameObject tile)
    {
        if(allPossibleMovingTiles.Contains(tile))
        {
            allPossibleMovingTiles.Remove(tile);
        }
        else if (allPossibleStaticTiles.Contains(tile))
        {
            allPossibleStaticTiles.Remove(tile);
        }
    }
    //Creates the Basic Grid depending on the size
    private void SetUpGrid()
    {
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                var randomTile = GetRandomTile(row, column);
                RemoveTileFromList(randomTile);
                var tile = Instantiate(randomTile, new Vector3(row * gridSpacing, 0, column * gridSpacing), Quaternion.identity, this.transform);
                var tilescript = tile.GetComponent<Tile>();             
                tilescript.SetTileData(row, column);
                grid.Add(tilescript);
            }
        }
        trackingManager.GetComponent<HandleTrackedImageLib>().ChangeTrackedPrefab(allPossibleMovingTiles[0].GetComponent<Tile>().prefabColor);
        RemoveTileFromList(allPossibleMovingTiles[0]);
    }
    #endregion

    #region TileMovement
    //Pushes the newRoom into the Grid
    public void InsertNewRoomPushing(Tile entryTile, Tile newRoom)
    {
        GridMovement move = GetMoveDir(entryTile);
        GameObject instNewRoom = Instantiate(newRoom.gameObject);
        instNewRoom.transform.SetParent(this.transform); 
        instNewRoom.transform.localPosition = entryTile.transform.localPosition - move.moveDir;

        Tile instNewTile = instNewRoom.GetComponent<Tile>();
        instNewTile.SetTileData(entryTile.row, entryTile.column);     
        instNewTile.GetComponent<MeshRenderer>().material.color = instNewTile.prefabColor;
        instNewTile.GetComponent<FindNearestGridSlot>().enabled = false;
        
        grid.Add(instNewTile);
        AdjustColAndRow(instNewTile);
        MoveAllTile(entryTile, instNewTile);
    }
    private void AdjustColAndRow(Tile newTile)
    {
        if (newTile.row == 0)
            newTile.row -= 1;
        else if (newTile.row == 6)
            newTile.row += 1;
        else if (newTile.column == 0)
            newTile.column -= 1;
        else if (newTile.column == 6)
            newTile.column += 1;
    }

    //Moves all matching Tiles depending on the GridMovement
    private void MoveAllTile(Tile entrytile, Tile newtile)
    {
        bool vertical = entrytile.canMoveVertical;
        bool horizontal = entrytile.canMoveHorizontal;
        int col = entrytile.column;
        int row = entrytile.row;

        List<Tile> movedTileList = new List<Tile>();
        GridMovement dir = GetMoveDir(entrytile);
        foreach (Tile tile in grid)
        {
            tile.transform.localRotation = Quaternion.identity;
            if (horizontal)
            {
                if (tile.row == row)
                {
                    tile.Move(dir);
                    movedTileList.Add(tile);
                }
            }
            else if (vertical)
            {
                if (tile.column == col)
                {
                    tile.Move(dir);
                    movedTileList.Add(tile);
                }
            }                
        }
        foreach (Tile tile in movedTileList)
        {
            if (tile.row < 0 || tile.row > size - 1 || tile.column < 0 || tile.column > size - 1)
            {
                grid.Remove(tile);
                trackingManager.GetComponent<HandleTrackedImageLib>().ChangeTrackedPrefab(tile.prefabColor);
                Destroy(tile.gameObject);
                return;
            }
        }
    }

    //Creates a Movement class depending on the input Tile
    private GridMovement GetMoveDir(Tile moveTile)
    {
        GridMovement newMove = new GridMovement();
        if (moveTile.canMoveVertical)
        {
            if (moveTile.row <= 0)
            {
                newMove.moveDir = new Vector3(gridSpacing, 0, 0);
                newMove.rowChangeDir = 1;
            }
            else if (moveTile.row >= size - 1)
            {
                newMove.moveDir = new Vector3(-gridSpacing, 0, 0);
                newMove.rowChangeDir = -1;
            }
        }
        else if (moveTile.canMoveHorizontal)
        {
            if (moveTile.column <= 0)
            {
                newMove.moveDir = new Vector3(0, 0, gridSpacing);
                newMove.colChangeDir = 1;
            }
            else if (moveTile.column >= size - 1)
            {
                newMove.moveDir = new Vector3(0, 0, -gridSpacing);
                newMove.colChangeDir = -1;
            }
        }
        return newMove;
    }
    #endregion
}
