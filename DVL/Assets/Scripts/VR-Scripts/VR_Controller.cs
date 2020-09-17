using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class VR_Controller : MonoBehaviour
{
	public static VR_Controller instance;
	[Header("Controller")]
	public ControllerType controllerType;
	public Camera tileCam;

	[Header("Environment")]
	public GameObject environment;
	public GameObject planePrefab;
	public GameObject boardPrefab;

	[Header("AR Tracking")]
	public GameObject tilePrefabParent;
	public LayerMask trackingCheckMask;
	private bool lockBoard = false;

	[Header("Movement")]
	public List<Tile> path;
	private Tile currentSelectedTarget;

	#region Getter Setter
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
	#endregion

	#region Unity Methods

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		VirtualControlSetup();
		controllerType = ControllerType.PC;
	}
	private void Update()
	{
			PCController();
	}
	#endregion

	#region Controller
	private void PCController()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			RaycastHit hit;
			Ray mousePos = tileCam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(mousePos.origin, mousePos.direction, out hit, 100, trackingCheckMask))
			{
				if (hit.transform.name == "DebugPlane")
				{
					tilePrefabParent.transform.position = hit.point;
				}
			}
		}

		if (Input.GetKey(KeyCode.A))
		{
			tilePrefabParent.transform.GetChild(0).transform.localEulerAngles += new Vector3(0, 90, 0) * Time.deltaTime;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			tilePrefabParent.transform.GetChild(0).transform.localEulerAngles += new Vector3(0, -90, 0) * Time.deltaTime;
		}
	}
	#endregion

	#region MovementPath
	public void ManagePath(Tile targetTile, PlayerIndex playerIndex, int steps)
	{
		Player playerObject = GameManager.instance.allPlayers[(int)playerIndex].GetComponent<Player>();

		if (targetTile == null)
		{
			currentSelectedTarget = targetTile;
			HandlePreviousPath();

			if (path != null)
				path.Clear();
		}
		else if (targetTile != currentSelectedTarget)
		{
			currentSelectedTarget = targetTile;
			HandlePreviousPath();

			if (path != null)
				path.Clear();

			path = Pathfinding.FindPath(steps, playerObject.positionTile, currentSelectedTarget);

			//Color path red
			if (path != null)
			{
				if (GUIManager.instance.isDebug || playerObject.gameObject == GameManager.instance.activePlayer.gameObject)
				{
					foreach (Tile t in path)
					{
						MeshRenderer[] meshes = t.GetComponentsInChildren<MeshRenderer>();
						foreach (MeshRenderer mesh in meshes)
						{
							if (mesh.CompareTag("Tile"))
								mesh.material.color = Color.red;
						}
					}
				}
			}
		}
		else if (path != null && targetTile == currentSelectedTarget)
		{
			HandlePreviousPath();
			foreach (Tile t in path)
			{
				t.PrefabColor();
			}
			playerObject.MoveToTarget(path);
			currentSelectedTarget = null;
		}
		else
			Debug.LogWarning("Something went wrong with path");
	}
	private void HandlePreviousPath()
	{
		if (path != null)
		{
			foreach (Tile t in path)
			{
				t.PrefabColor();
			}
		}
	}
	#endregion

	#region Controller Setup
	private void VirtualControlSetup()
	{
		GameObject plane = Instantiate(planePrefab);
		plane.transform.SetParent(environment.transform);
		plane.transform.localPosition = new Vector3(0, 0, 0);
		plane.transform.rotation = Quaternion.identity;
		plane.name = "DebugPlane";
	}

	public void ChangeTrackedPrefab(GameObject droppedOutPrefab)
	{
		var meshes = droppedOutPrefab.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer mesh in meshes)
		{
			mesh.enabled = true;
			mesh.material.color = Color.white;
		}

		droppedOutPrefab.transform.SetParent(tilePrefabParent.transform);
		droppedOutPrefab.transform.localPosition = Vector3.zero;
		droppedOutPrefab.transform.localRotation = Quaternion.identity;

		droppedOutPrefab.GetComponent<FindNearestGridSlot>().enabled = true;
		VR_Grid.instance.trackedTile = droppedOutPrefab.GetComponent<VR_Tile>();
		tilePrefabParent.SetActive(false);
		Invoke("ToggleBackOn", 2);
	}


	private void ToggleBackOn()
	{
		tilePrefabParent.SetActive(true);
	}
	public void LockBoard()
	{
		lockBoard = !lockBoard;
	}
	#endregion

	#region calculate the right position and rotation
	private Vector3 GetOffset(ARTrackedImage image)
	{
		switch (image.referenceImage.name)
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
}

