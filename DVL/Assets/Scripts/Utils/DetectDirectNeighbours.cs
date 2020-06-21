﻿using System.Collections.Generic;
using System.Linq;

public class DetectDirectNeighbours
{
    private List<Tile> neighbours = new List<Tile>();
    private Dictionary<string, Tile> dic;

    public DetectDirectNeighbours()
    {
        dic = BoardGrid.instance.coordDic;
    }

    public static List<Tile> DetectSingleRadiusTile(Tile tile)
    {
        if (tile == null)
            return null;
        List<Tile> neighbours = new List<Tile>();
        List<string> addKeyList = new List<string>();

        addKeyList.AddMany(
            tile.row.ToString() + (tile.column + 1).ToString(),
            tile.row.ToString()+ (tile.column - 1).ToString(),
            (tile.row + 1).ToString() + tile.column.ToString(),
            (tile.row - 1).ToString(), tile.column.ToString(),
            (tile.row - 1).ToString(), (tile.column - 1).ToString(),
            (tile.row + 1).ToString(), (tile.column - 1).ToString(), 
            (tile.row - 1).ToString(), (tile.column + 1).ToString(),
            (tile.row + 1).ToString(), (tile.column + 1).ToString()
            );

        neighbours = AddToList(addKeyList, neighbours);
        return neighbours;
    }

    public static List<Tile> DetectCross(Tile tile)
    {
        if (tile == null)
            return null;

        List<Tile> neighbours = new List<Tile>();
        List<string> addKeyList = new List<string>();

        addKeyList.AddMany(
            tile.row.ToString() + (tile.column + 1).ToString(),
            tile.row.ToString() + (tile.column - 1).ToString(),
            (tile.row + 1).ToString(), tile.column.ToString(),
            (tile.row - 1).ToString(), tile.column.ToString()
            );

        neighbours = AddToList(addKeyList, neighbours);
        return neighbours;
    }

    public static List<Tile> DetectTileRadius(Tile tile, int radius, bool beMovable)
    {
        if (tile == null)
            return null;

        List<Tile> neighbours = new List<Tile>();
        List<Tile> toCheck = new List<Tile>();
        List<Tile> allneighbours = new List<Tile>();

        allneighbours.Add(tile);
        toCheck.Add(tile);


        for (int i = 0; i < radius; i++)
        {
            //for every Tile in to check get the neighbours
            for (int j = 0; j < toCheck.Count; j++)
            {
                if (beMovable)
                    allneighbours = allneighbours.Union(DetectMoveOptionsFromSingleTile(toCheck[j])).ToList();
                else
                    allneighbours = allneighbours.Union(DetectSingleRadiusTile(toCheck[j])).ToList();
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

    public static List<Tile> DetectMoveOptionsFromSingleTile(Tile tile)
    {
        if (tile == null)
            return null;

        List<Tile> allNeighbors = new List<Tile>();

        if (tile.column - 1 >= 0)
        {
            Tile check = BoardGrid.instance.coordDic[tile.row.ToString() + (tile.column - 1).ToString()];
            if (check.ingameForwardModule == TileDirectionModule.NONE && tile.ingameBackwardModule == TileDirectionModule.NONE)
                allNeighbors.Add(check);
        }

        if (tile.column + 1 <= 6)
        {
            Tile check = BoardGrid.instance.coordDic[tile.row.ToString() + (tile.column + 1).ToString()];
            if (check.ingameBackwardModule == TileDirectionModule.NONE && tile.ingameForwardModule == TileDirectionModule.NONE)
                allNeighbors.Add(check);
        }

        if (tile.row - 1 >= 0)
        {
            Tile check = BoardGrid.instance.coordDic[(tile.row - 1).ToString() + tile.column.ToString()];
            if (check.ingameRightModule == TileDirectionModule.NONE && tile.ingameLeftModule == TileDirectionModule.NONE)
                allNeighbors.Add(check);
        }

        if (tile.row + 1 <= 6)
        {
            Tile check = BoardGrid.instance.coordDic[(tile.row + 1).ToString() + tile.column.ToString()];
            if (check.ingameLeftModule == TileDirectionModule.NONE && tile.ingameRightModule == TileDirectionModule.NONE)
                allNeighbors.Add(check);
        }
        return allNeighbors;
    }

    private static List<Tile> AddToList(List<string> keyAddList, List<Tile> actualList)
    {
        var dic = BoardGrid.instance.coordDic;

        foreach (string key in keyAddList)
        {
            if (dic.ContainsKey(key))
            {
                if (!actualList.Contains(dic[key]))
                {
                    actualList.Add(dic[key]);
                }
            }
        }

        return actualList;
    }
}