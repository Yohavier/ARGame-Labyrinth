using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectDirectNeighbours
{
    private List<Tile> neighbours = new List<Tile>();
    private Dictionary<string, Tile> dic;

    int row;
    int column;

    public DetectDirectNeighbours()
    {
        dic = BoardGrid.instance.coordDic;
    }

    public List<Tile> DetectCircle(Tile tile)
    {
        row = tile.row;
        column = tile.column;

        AddToList(tile.row.ToString(), (tile.column + 1).ToString());
        AddToList(tile.row.ToString(), (tile.column - 1).ToString());
        AddToList((tile.row + 1).ToString(), tile.column.ToString());
        AddToList((tile.row - 1).ToString(), tile.column.ToString());
        AddToList((tile.row - 1).ToString(), (tile.column - 1).ToString());
        AddToList((tile.row + 1).ToString(), (tile.column - 1).ToString());
        AddToList((tile.row - 1).ToString(), (tile.column + 1).ToString());
        AddToList((tile.row + 1).ToString(), (tile.column + 1).ToString());

        return neighbours;
    }

    
    public List<Tile> DetectCross(Tile tile)
    {
        row = tile.row;
        column = tile.column;

        AddToList(tile.row.ToString(), (tile.column + 1).ToString());
        AddToList(tile.row.ToString(), (tile.column - 1).ToString());
        AddToList((tile.row + 1).ToString(), tile.column.ToString());
        AddToList((tile.row - 1).ToString(), tile.column.ToString());

        return neighbours;
    }

    private void AddToList(string row, string column)
    {
        if (dic.ContainsKey(row + column))
            neighbours.Add(dic[row + column]);
    }
}