using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    public List<Tile> possiblePlaces = new List<Tile>();

    public void SetItemOnGrid()
    {
        foreach(Tile t in BoardGrid.instance.grid)
        {
            if(t.canMoveVertical || t.canMoveHorizontal)
            {
                if ((t.row > 1 && t.row < 5)|| (t.column > 1 && t.column < 5))
                {
                    possiblePlaces.Add(t);
                }
            }
        }
        StartPlacingWithSeed();
    }
    private void StartPlacingWithSeed()
    {
        List<string> dicKeys = new List<string>();

        for (int i = 0; i < items.Count; i++)
        {
            int rand = Convert.ToInt32(Math.Max(0, BoardGrid.instance.seedList[i] * possiblePlaces.Count - 1));
            GameObject newItem = Instantiate(items[i]);
            newItem.transform.SetParent(possiblePlaces[rand].transform);
            newItem.transform.localPosition = Vector3.zero;
            newItem.GetComponent<MeshRenderer>().enabled = false;

            int col = possiblePlaces[rand].column;
            int row = possiblePlaces[rand].row;

            dicKeys.AddMany(
                row.ToString() + (col - 1).ToString(),
                row.ToString() + (col + 1).ToString(),
                (row - 1).ToString() + col.ToString(),
                (row + 1).ToString() + col.ToString(),
                (row + 1).ToString() + (col + 1).ToString(),
                (row - 1).ToString() + (col - 1).ToString()
                );

            foreach (string key in dicKeys)
            {
                HandlePlacesList(key);
            }
            dicKeys.Clear();
        }
    }

    private void HandlePlacesList(string key)
    {
        if (BoardGrid.instance.coordDic.ContainsKey(key))
        {
            Tile n = BoardGrid.instance.coordDic[key];
            if (possiblePlaces.Contains(n))
                possiblePlaces.Remove(n);
        }
    }
}

