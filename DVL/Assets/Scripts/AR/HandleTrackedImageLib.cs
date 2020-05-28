using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class HandleTrackedImageLib : MonoBehaviour
{
	private ARTrackedImageManager manager;
	public GameObject board;
	public GameObject tilePrefabParent;
	public static HandleTrackedImageLib CustomTrackingManagerInstance;
	private Dictionary<string, GameObject> trackableDictionary = new Dictionary<string, GameObject>();

	private void Awake()
	{
		CustomTrackingManagerInstance = this;
	}
	private void Start()
	{
		manager = this.GetComponent<ARTrackedImageManager>();
		manager.trackedImagesChanged += OnTrackedImagesChanged;
		trackableDictionary.Add("Board", board);
		trackableDictionary.Add("T1", tilePrefabParent);
	}

	//Update function for the Image Tracker to get the right pos/rot
	private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
	{
		foreach (var trackedImage in eventArgs.added)
		{
			trackedImage.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
		}
		foreach (var trackedImage in eventArgs.updated)
		{
			if (trackableDictionary.ContainsKey(trackedImage.referenceImage.name))
			{
				trackableDictionary[trackedImage.referenceImage.name].SetActive(true);
				trackableDictionary[trackedImage.referenceImage.name].transform.localPosition = trackedImage.transform.localPosition;
				trackableDictionary[trackedImage.referenceImage.name].transform.localRotation = trackedImage.transform.localRotation;
			}
		}
	}

	//sets up the new Prefab that dropped out of the Grid 
	public void ChangeTrackedPrefab(GameObject droppedOutPrefab)
	{
		tilePrefabParent.SetActive(false);

		droppedOutPrefab.transform.SetParent(tilePrefabParent.transform);
		droppedOutPrefab.transform.localPosition = Vector3.zero;
		droppedOutPrefab.transform.localRotation = Quaternion.identity;

		droppedOutPrefab.GetComponent<FindNearestGridSlot>().enabled = true;
		droppedOutPrefab.GetComponent<MeshRenderer>().material.color = droppedOutPrefab.GetComponent<Tile>().prefabColor;

		Invoke("ToggleBackOn", 2);
	}

	//Reactivates the Prefab after time 
	private void ToggleBackOn()
	{
		tilePrefabParent.SetActive(true);
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
