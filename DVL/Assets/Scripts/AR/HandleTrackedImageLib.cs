using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
public class HandleTrackedImageLib : MonoBehaviour
{
    private ARTrackedImageManager manager;
    public GameObject board;
    public GameObject t1;

    private Dictionary<string, GameObject> trackableDictionary = new Dictionary<string, GameObject>();

    private void Start()
    {
        manager = GetComponent<ARTrackedImageManager>();
        manager.trackedImagesChanged += OnTrackedImagesChanged;
        trackableDictionary.Add("Board", board);
        trackableDictionary.Add("T1", t1); 
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach(var trackedImage in eventArgs.added)
        { 
            trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
        }
        foreach(var trackedImage in eventArgs.updated)
        {
            if (trackableDictionary.ContainsKey(trackedImage.referenceImage.name))
            {          
                trackableDictionary[trackedImage.referenceImage.name].SetActive(true);
                trackableDictionary[trackedImage.referenceImage.name].transform.localPosition = trackedImage.transform.localPosition;
                trackableDictionary[trackedImage.referenceImage.name].transform.localRotation = trackedImage.transform.localRotation;
            }
        }
    }
    public void ChangeTrackedPrefab(Color droppedOutColor)
    {
        t1.GetComponent<Tile>().prefabColor = droppedOutColor;
        t1.GetComponent<MeshRenderer>().material.color = droppedOutColor;
    }
    public void RemoveFromDictionary(string tileName)
    {
        trackableDictionary.Remove(tileName);
    }
    public void AddToDictionary(string tileName, GameObject tile)
    {
        trackableDictionary.Add(tileName, tile);
    }
}
