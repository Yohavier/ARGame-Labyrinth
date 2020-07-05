using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class SpawnItems : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    public List<Tile> itemsPossiblePlaces = new List<Tile>();

    public List<GameObject> powerUps = new List<GameObject>();
    public List<Tile> powerUpsPossiblePlaces = new List<Tile>();

    public void SetItemOnGrid()
    {
        foreach(Tile t in BoardGrid.instance.grid)
        {
            if(t.canMoveVertical || t.canMoveHorizontal)
            {
                if ((t.row > 1 && t.row < 5)|| (t.column > 1 && t.column < 5))
                {
                    itemsPossiblePlaces.Add(t);
                }
            }
        }
        powerUpsPossiblePlaces = itemsPossiblePlaces;
        StartPlacingItemsWithSeed();
        StartPlacingPowerUpsWithSeed();
    }
    private void StartPlacingItemsWithSeed()
    {
        List<string> dicKeys = new List<string>();

        for (int i = 0; i < items.Count; i++)
        {
            int rand = Convert.ToInt32(Math.Max(0, BoardGrid.instance.seedList[i] * itemsPossiblePlaces.Count - 1));
            GameObject newItem = Instantiate(items[i]);
            newItem.transform.SetParent(itemsPossiblePlaces[rand].transform);
            newItem.transform.localPosition = Vector3.zero;
            newItem.GetComponent<MeshRenderer>().enabled = false;
            Generator g = newItem.GetComponent<Generator>();
            Item it = newItem.GetComponent<Item>();
            if (g != null)
            {
                g.position = itemsPossiblePlaces[rand];
            }
            else if (it != null) 
            {
                it.position = itemsPossiblePlaces[rand];
            }
            int col = itemsPossiblePlaces[rand].column;
            int row = itemsPossiblePlaces[rand].row;

            powerUpsPossiblePlaces.Remove(itemsPossiblePlaces[rand]);

            dicKeys.AddMany(
                row.ToString() + col.ToString(),
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

    private void StartPlacingPowerUpsWithSeed()
    {
        for(int i = 0; i < powerUps.Count; i++)
        {
            int rand = Convert.ToInt32(Math.Max(0, BoardGrid.instance.seedList[i] * powerUpsPossiblePlaces.Count - 1));
            GameObject newPowerUp = Instantiate(powerUps[i]);
            newPowerUp.transform.SetParent(powerUpsPossiblePlaces[rand].transform);
            newPowerUp.transform.localPosition = Vector3.zero;
            newPowerUp.GetComponent<MeshRenderer>().enabled = false;

            powerUpsPossiblePlaces.RemoveAt(rand);
        }
    }

    private void HandlePlacesList(string key)
    {
        if (BoardGrid.instance.coordDic.ContainsKey(key))
        {
            Tile n = BoardGrid.instance.coordDic[key];
            if (itemsPossiblePlaces.Contains(n))
                itemsPossiblePlaces.Remove(n);
        }
    }
}

