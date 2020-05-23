using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile: MonoBehaviour
{
    public int row;
    public int column;
    public bool edgePiece;
    public bool canMoveHorizontal;
    public bool canMoveVertical;

    //Set Tile Data
    public void SetTileData(int rowNum, int colNum)
    {
        row = rowNum;
        column = colNum;
        UpdateTileState();
    }

    //Move the Tile
    public void Move(GridMovement move)
    {
        transform.position += move.moveDir;
        row += move.rowChangeDir;
        column += move.colChangeDir;
        UpdateTileState();
    }

    //Updates all the states of this tile
    private void UpdateTileState()
    {
        if (row % 2 == 0 && column % 2 == 0)
        {
            canMoveHorizontal = false;
            canMoveVertical = false;
        }
        else if (row % 2 != 0 && column % 2 == 0)
        {
            canMoveVertical = false;
            canMoveHorizontal = true;
        }
        else if (row % 2 == 0 && column % 2 != 0)
        {
            canMoveVertical = true;
            canMoveHorizontal = false;
        }
        else
        {
            canMoveVertical = true;
            canMoveHorizontal = true;
        }

        if (row == 0 || column == 0 || column == BoardGrid.size - 1 || row == BoardGrid.size - 1)
        {
            edgePiece = true;
        }
        else
        {
            edgePiece = false;
        }
    }
}
