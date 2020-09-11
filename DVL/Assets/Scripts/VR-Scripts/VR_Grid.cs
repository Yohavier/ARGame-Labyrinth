using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Grid : MonoBehaviour
{
    public static VR_Grid instance;

    public List<Tile> grid = new List<Tile>();
    public Dictionary<string, Tile> coordDic = new Dictionary<string, Tile>();

    public List<GameObject> allPossibleMovingTiles = new List<GameObject>();
    public List<GameObject> allPossibleStaticTiles = new List<GameObject>();

    public int[] randomRoations;
    [HideInInspector] public int size = 7;
    [HideInInspector] public float gridSpacing;
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
        List<Tile> cornerTiles = new List<Tile>();
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
                Tile component = tile.GetComponent<Tile>();
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
        GameObject leftOverTile = Instantiate(allPossibleMovingTiles[0]);
        leftOverTile.GetComponent<Tile>().isInFOW = true;
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
}
