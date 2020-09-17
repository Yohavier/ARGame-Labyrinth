using System.Collections.Generic;
using System.Linq;

public class VR_DetectNeighbours
{
    public static List<VR_Tile> DetectSingleRadiusTile(VR_Tile tile)
    {
        if (tile == null)
            return null;
        List<VR_Tile> neighbours = new List<VR_Tile>();
        List<string> addKeyList = new List<string>();

        addKeyList.AddMany(
            tile.row.ToString() + (tile.column + 1).ToString(),
            tile.row.ToString() + (tile.column - 1).ToString(),
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

    public static List<VR_Tile> DetectCross(VR_Tile tile)
    {
        if (tile == null)
            return null;

        List<VR_Tile> neighbours = new List<VR_Tile>();
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

    public static List<VR_Tile> DetectTileRadius(VR_Tile tile, int radius, bool beMovable)
    {
        if (tile == null)
            return null;

        List<VR_Tile> neighbours = new List<VR_Tile>();
        List<VR_Tile> toCheck = new List<VR_Tile>();
        List<VR_Tile> allneighbours = new List<VR_Tile>();

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

    public static List<VR_Tile> DetectMoveOptionsFromSingleTile(VR_Tile tile)
    {
        if (tile == null)
            return null;

        List<VR_Tile> allNeighbors = new List<VR_Tile>();

        if (tile.column - 1 >= 0)
        {
            VR_Tile check = VR_Grid.instance.coordDic[tile.row.ToString() + (tile.column - 1).ToString()];
            if ((check.ingameForwardModule == TileDirectionModule.NONE || (check.ingameForwardModule == TileDirectionModule.DOOR && check.doorOpen)) && (tile.ingameBackwardModule == TileDirectionModule.NONE || (tile.ingameBackwardModule == TileDirectionModule.DOOR && tile.doorOpen)))
                allNeighbors.Add(check);
        }

        if (tile.column + 1 <= 6)
        {
            VR_Tile check = VR_Grid.instance.coordDic[tile.row.ToString() + (tile.column + 1).ToString()];
            if ((check.ingameBackwardModule == TileDirectionModule.NONE || (check.ingameBackwardModule == TileDirectionModule.DOOR && check.doorOpen)) && (tile.ingameForwardModule == TileDirectionModule.NONE || (tile.ingameForwardModule == TileDirectionModule.DOOR && tile.doorOpen)))
                allNeighbors.Add(check);
        }

        if (tile.row - 1 >= 0)
        {
            VR_Tile check = VR_Grid.instance.coordDic[(tile.row - 1).ToString() + tile.column.ToString()];
            if ((check.ingameRightModule == TileDirectionModule.NONE || (check.ingameRightModule == TileDirectionModule.DOOR && check.doorOpen)) && (tile.ingameLeftModule == TileDirectionModule.NONE || (tile.ingameLeftModule == TileDirectionModule.DOOR && tile.doorOpen)))
                allNeighbors.Add(check);
        }

        if (tile.row + 1 <= 6)
        {
            VR_Tile check = VR_Grid.instance.coordDic[(tile.row + 1).ToString() + tile.column.ToString()];
            if ((check.ingameLeftModule == TileDirectionModule.NONE || (check.ingameLeftModule == TileDirectionModule.DOOR && check.doorOpen)) && (tile.ingameRightModule == TileDirectionModule.NONE || (tile.ingameRightModule == TileDirectionModule.DOOR && tile.doorOpen)))
                allNeighbors.Add(check);
        }
        return allNeighbors;
    }

    private static List<VR_Tile> AddToList(List<string> keyAddList, List<VR_Tile> actualList)
    {
        var dic = VR_Grid.instance.coordDic;

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