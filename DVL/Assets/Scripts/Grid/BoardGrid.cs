using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGrid : MonoBehaviour
{
    public static int size;
    public int set_size = 7;
    public GameObject TilePrefab;
    public float gridSpacing;

    public List<Tile> grid = new List<Tile>();

    //Visual helper
    public Material even;
    public Material odd;

    private void Awake()
    {
        size = set_size;
        SetUpGrid();
    }

    //Creates the Basic Grid depending on the size
    private void SetUpGrid()
    {
        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                var tile = Instantiate(TilePrefab, new Vector3(row * gridSpacing, 0, column * gridSpacing), Quaternion.identity, this.transform);
                var tilescript = tile.GetComponent<Tile>();             
                tilescript.SetTileData(row, column);
                grid.Add(tilescript);
                AssignMaterial(tilescript);
            }
        }
    }

    //Pushes the newRoom into the Grid
    public void InsertNewRoomPushing(Tile entryTile, Tile newRoom)
    {
        GridMovement move = GetMoveDir(entryTile);
        newRoom.transform.position = entryTile.transform.position - move.moveDir;
        newRoom.SetTileData(entryTile.row, entryTile.column);
        grid.Add(newRoom);
        MoveAllTile(entryTile, newRoom);
    }

    //Move all Tiles depending on the GridMovement
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
            if (horizontal)
            {
                if (tile.row == row && tile != newtile)
                {
                    tile.Move(dir);
                    movedTileList.Add(tile);
                }
            }
            else if (vertical)
            {
                if (tile.column == col && tile != newtile)
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
                tile.GetComponent<FindNearestGridSlot>().enabled = true;
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

    //Just for visual help
    private void AssignMaterial(Tile tile)
    {
        if((tile.row + tile.column)%2 == 0)
        {
            tile.GetComponent<MeshRenderer>().material = even;
        }
        else
        {
            tile.GetComponent<MeshRenderer>().material = odd;
        }
    }
}
