using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
public class HandleTrackedImageLib : MonoBehaviour
{
	private ARTrackedImageManager manager;
	private GameObject boardPrefab;
	public GameObject tilePrefabParent;
	public static HandleTrackedImageLib instance;
	public LayerMask trackingCheckMask;
	public GameObject boardEnv;

	private List<string> BoardTrackers = new List<string>();
	private bool lockBoard = false;
	private bool activeLooseTile = true;
	private bool _activeLooseTile
    {
        get { return activeLooseTile; }
        set 
		{ 
			activeLooseTile = value;
            if (!activeLooseTile)
            {
				tilePrefabParent.SetActive(false);
			}
		}
    }
	public void LockBoard()
    {
		lockBoard = !lockBoard;
    }
	private void Awake()
	{
		instance = this;
	}
	private void Start()
	{
		boardPrefab = FindObjectOfType<BoardGrid>().gameObject;
		VirtualControlSetup();
		SetUpBoardTracker();
		if(SelectARObjectWithFinger.instance.controllerType == ControllerType.PC)
        {
			VirtualControlSetup();
		}
		else
        {
			manager = GetComponent<ARTrackedImageManager>();
			manager.trackedImagesChanged += OnTrackedImagesChanged;	
		}
	}

	//Set Up a plane for PC play
	public GameObject planePrefab;
	private void VirtualControlSetup()
    {
		GameObject plane = Instantiate(planePrefab);
		plane.transform.SetParent(boardEnv.transform);
		plane.transform.position = new Vector3(0, 0, 0);
		plane.transform.rotation = Quaternion.identity;
		plane.name = "DebugPlane";
	}

    private void Update()
    {
		if(SelectARObjectWithFinger.instance.controllerType == ControllerType.PC)
        {
			PCController();
		}
		else if(SelectARObjectWithFinger.instance.controllerType == ControllerType.Mobile_Virtual)
        {
			VirtualController();
        }
	}

	//Cast a ray to handle the tilePrefabParent with mouse
	private void PCController()
    {
		if (Input.GetKey(KeyCode.LeftShift))
		{
			RaycastHit hit;
			Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(mousePos.origin, mousePos.direction, out hit, 2, 1 << 9))
			{
				if (hit.transform.name == "DebugPlane")
				{
					tilePrefabParent.transform.position = hit.point;
				}	
			}
		}

        if (Input.GetKey(KeyCode.A))
        {
			tilePrefabParent.transform.localEulerAngles += new Vector3(0, 90, 0) * Time.deltaTime;
        }
		else if (Input.GetKey(KeyCode.D))
        {
			tilePrefabParent.transform.localEulerAngles += new Vector3(0, -90, 0) * Time.deltaTime;
		}
	}

	private Vector2 touchPosition;
	private bool isTouching;
	private float tapTimer;
	private void VirtualController()
    {
		if (Input.touchCount > 0)
		{
			if(Input.touchCount == 2)
            {
				HandleTouchRotation(Input.GetTouch(1));
				return;
            }
			Touch touch = Input.GetTouch(0);
			touchPosition = touch.position;
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(touchPosition);

			if (touch.phase == TouchPhase.Began)
            {
				isTouching = true;
				tapTimer = 0;
			}
			else if (touch.phase == TouchPhase.Ended)
            {
				isTouching = false;
			}

			if (isTouching && Physics.Raycast(ray.origin, ray.direction, out hit, 2, 1 << 9))
			{
				tapTimer += Time.deltaTime;
                if (tapTimer > 0.2f)
                {
					if (hit.transform.name == "DebugPlane")
					{
						tilePrefabParent.transform.position = hit.point;
					}
				}
			}
		}
	}

	private Vector2 prevTouch;
	private float deltaPos;
	private void HandleTouchRotation(Touch touch)
    {
		if (touch.phase == TouchPhase.Began)
        {
			prevTouch = touch.position;
		}

		deltaPos = prevTouch.x - touch.position.x;

		tilePrefabParent.transform.localEulerAngles += new Vector3(0, deltaPos * 4, 0) * Time.deltaTime;

		prevTouch = touch.position;
    }
	//Includes all Multitrackernames to List
    private void SetUpBoardTracker()
    {
		BoardTrackers.AddMany("BottomLeft", "BottomRight", "TopLeft", "TopRight");
	}

	//Update function for the Image Tracker to get the right pos/rot
	private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
	{
		foreach (var trackedImage in eventArgs.added)
		{
			trackedImage.transform.localScale = new Vector3(1f, 1f, 1f);
		}

		List<ARTrackedImage> multiTrackList = new List<ARTrackedImage>();
		foreach (var trackedImage in eventArgs.updated)
		{
			if (BoardTrackers.Contains(trackedImage.referenceImage.name))
			{
				if(lockBoard == false)
                {
					if (trackedImage.trackingState == TrackingState.Tracking)
					{
						multiTrackList.Add(trackedImage);
					}
				}
			}
			else
			{
                if (isTrackable(trackedImage)) 
				{ 
					HandleSingleTracker(trackedImage);
                }				
			}
		}

		if(multiTrackList.Count>0)
			HandleMultiTracker(multiTrackList[0]);
	}

	//sets up the new Prefab that dropped out of the Grid 
	public void ChangeTrackedPrefab(GameObject droppedOutPrefab)
	{
		if (GUIManager.instance.isDebug)
		{
			droppedOutPrefab.GetComponent<Tile>().isInFOW = false;
			droppedOutPrefab.GetComponent<Tile>().PrefabColor();
			var meshes = droppedOutPrefab.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer mesh in meshes)
			{
				mesh.enabled = true;
				mesh.material.color = Color.white;
			}
		}
		//_activeLooseTile = false;
		tilePrefabParent.SetActive(true);

		droppedOutPrefab.transform.SetParent(tilePrefabParent.transform);
		droppedOutPrefab.transform.localPosition = Vector3.zero;
		droppedOutPrefab.transform.localRotation = Quaternion.identity;

		droppedOutPrefab.GetComponent<FindNearestGridSlot>().enabled = true;
		BoardGrid.instance.trackedTile = droppedOutPrefab.GetComponent<Tile>();

		Invoke("ToggleBackOn", 2);
	}

	//Reactivates the Prefab after time 
	private void ToggleBackOn()
	{
		//_activeLooseTile = true;
		tilePrefabParent.SetActive(true);
	}
	private void HandleMultiTracker(ARTrackedImage trackedImage)
    {
		if (trackedImage.trackingState != TrackingState.Tracking)
			return;

		boardPrefab.transform.localPosition = trackedImage.transform.localPosition - GetOffset(trackedImage);
		boardPrefab.transform.localEulerAngles = GetRotation(trackedImage);
	}
	private void HandleSingleTracker(ARTrackedImage trackedImage)
    {
		if(trackedImage.referenceImage.name == "Tile")
        {
            if (_activeLooseTile && SelectARObjectWithFinger.instance.controllerType != ControllerType.Mobile_Virtual)
            {
				tilePrefabParent.SetActive(true);
				tilePrefabParent.transform.localPosition = trackedImage.transform.localPosition;
				tilePrefabParent.transform.localRotation = trackedImage.transform.localRotation;
			}
		}
	}
	#region calculate the right position and rotation
    private Vector3 GetOffset(ARTrackedImage image)
    {
        switch(image.referenceImage.name)
        {
			case "BottomLeft":
				return Vector3.zero;
			case "BottomRight":
				return boardPrefab.transform.right * 0.6f;
			case "TopLeft":
				return (boardPrefab.transform.right + boardPrefab.transform.forward) * 0.6f;
			case "TopRight":
				return boardPrefab.transform.forward * 0.6f;
			default:
				return Vector3.zero;
		}
    }
	private Vector3 GetRotation(ARTrackedImage image)
    {
		Vector3 gy = GyroModifyCamera().eulerAngles;
		Vector3 a = new Vector3(gy.x, image.transform.localEulerAngles.y, gy.z);
		return a;
	}
	Quaternion GyroModifyCamera()
	{
		return GyroToUnity(Input.gyro.attitude);
	}
	private static Quaternion GyroToUnity(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}
	#endregion
    private bool isTrackable(ARTrackedImage image)
    {
		if(image.trackingState == TrackingState.Tracking)
        {
			RaycastHit hit;
			Vector3 dir = (image.transform.position - Camera.main.transform.position).normalized;
			float distance = (image.transform.position - Camera.main.transform.position).magnitude;
			MeshRenderer[] mesh = tilePrefabParent.GetComponentsInChildren<MeshRenderer>();
			if (Physics.Raycast(Camera.main.transform.position, dir, out hit, distance, trackingCheckMask))
			{
				foreach(MeshRenderer m in mesh)
                {
					m.material.color = Color.red;
					tilePrefabParent.SetActive(false);
                }
				return false;
			}
			foreach (MeshRenderer m in mesh)
			{
				m.material.color = Color.white;
				tilePrefabParent.SetActive(true);
			}
			return true;
		}
		return false;
    }
}
