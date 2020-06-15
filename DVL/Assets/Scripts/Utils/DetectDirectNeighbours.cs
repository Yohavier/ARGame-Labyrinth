using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectDirectNeighbours
{
    private List<Tile> neighbours = new List<Tile>();
    private Dictionary<string, Tile> dic;
    public DetectDirectNeighbours()
    {
        dic = BoardGrid.instance.coordDic;
    }
    public List<Tile> Detect(Tile tile)
    {
        int row = tile.row;
        int column = tile.column;

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

    private void AddToList(string row, string column)
    {
        if (dic.ContainsKey(row + column))
            neighbours.Add(dic[row + column]);
    }
}