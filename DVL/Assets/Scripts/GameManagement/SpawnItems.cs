using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>();
    public List<Tile> possiblePlaces = new List<Tile>();

    public void SetItemOnGrid()
    {
        int itemCount = items.Count;

        foreach(Tile t in BoardGrid.GridInstance.grid)
        {
            if(t.canMoveVertical || t.canMoveHorizontal)
            {
                if ((t.row > 1 && t.row < 5)|| (t.column > 1 && t.column < 5))
                {
                    possiblePlaces.Add(t);
                }
            }
        }
        StartPlacing();
    }
    private void StartPlacing()
    {
        for (int i = 0; i < items.Count; i++)
        {
            int rand = Random.Range(0, possiblePlaces.Count);
            GameObject newItem = Instantiate(items[i]);
            newItem.transform.SetParent(possiblePlaces[rand].transform);
            newItem.transform.localPosition = Vector3.zero;
            possiblePlaces.Remove(possiblePlaces[rand]);
            newItem.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}

