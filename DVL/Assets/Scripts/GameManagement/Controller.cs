using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public enum ControllerType {None, Mobile_Haptik, Mobile_Virtual, PC }
public class Controller : MonoBehaviour
{
	public static Controller instance;
	[Header("Controller")]
	public ControllerType controllerType = ControllerType.None;
	public CamerController cameraController;

	[Header("Environment")]
	public GameObject environment;
	public GameObject planePrefab;
	public GameObject boardPrefab;

	[Header("AR Tracking")]
	public GameObject tilePrefabParent;
	public LayerMask trackingCheckMask;
	private ARTrackedImageManager manager;
	private List<string> BoardTrackers = new List<string>();
	private bool lockBoard = false;

	[Header("Movement")]
	public List<Tile> path;
	private Tile currentSelectedTarget;
	
	[Header("Mobile Input")]
	public LayerMask virtualLayerMask;
	private Vector2 touchPosition;
	private bool isTouching;
	private float tapTimer;
	private Vector2 prevTouch;
	private float deltaPos;

	private bool characterSelection = false;
	private bool canSwipe = false;

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
	//Set Up a plane for PC play
	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		VirtualControlSetup();
		SetUpBoardTracker();
		if (controllerType == ControllerType.PC)
		{	
			VirtualControlSetup();
		}
		else
		{
			manager = GetComponent<ARTrackedImageManager>();
			manager.trackedImagesChanged += OnTrackedImagesChanged;
		}
		ChooseController(false);
		cameraController.SetUp();
	}
	private void Update()
	{
        if (characterSelection)
        {
            if (!GUIManager.instance.readyToggle.isOn)
            {
				if(Input.GetMouseButton(0) || Input.touches.Length > 0)
					HandleSwipe();
            }
			return;
        }

		if (controllerType == ControllerType.PC)
		{
			PCController();

			if (CanControl())
				PCTileSelectionController();
		}
		else if (controllerType == ControllerType.Mobile_Virtual)
		{
			VirtualController();
		}
		else if(controllerType == ControllerType.Mobile_Haptik)
        {
			if (CanControl())
				MobileTileSelectionController();
		}
	}

	private void OnEnable()
    {
		Eventbroker.instance.onNotifyNextTurn += NewTurn;
		Eventbroker.instance.onChangeGameState += ChangeGameState;
	}
    private void OnDisable()
    {
		Eventbroker.instance.onNotifyNextTurn -= NewTurn;
		Eventbroker.instance.onChangeGameState -= ChangeGameState;
	}
    #endregion

    #region Choose Controller
    public void MobileHaptikTileControl(bool value)
	{
		ChooseController(value);
		if (!value)
			Controller.instance.tilePrefabParent.transform.rotation = Quaternion.identity;
	}
	private void ChooseController(bool value)
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		controllerType = ControllerType.PC;
#elif UNITY_IOS || UNITY_ANDROID
		if (value)
			controllerType = ControllerType.Mobile_Haptik;
		else
			controllerType = ControllerType.Mobile_Virtual;
#else
		controllerType = ControllerType.None;
#endif
	}
    #endregion

    #region Controller
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
			tilePrefabParent.transform.GetChild(0).transform.localEulerAngles += new Vector3(0, 90, 0) * Time.deltaTime;
		}
		else if (Input.GetKey(KeyCode.D))
		{
			tilePrefabParent.transform.GetChild(0).transform.localEulerAngles += new Vector3(0, -90, 0) * Time.deltaTime;
		}
	}
	private void VirtualController()
	{
		if (Input.touchCount > 0)
		{
			if (Input.touchCount == 2)
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

			if(touch.phase == TouchPhase.Began)
            {
				MobileTileSelectionController();
            }

			if (isTouching)
			{
				tapTimer += Time.deltaTime;
				if (tapTimer > 0.2f)
				{
					if (Physics.Raycast(ray.origin, ray.direction, out hit, 2, 1 << 9))
					{
						if (hit.transform.name == "DebugPlane")
						{
							tilePrefabParent.transform.position = hit.point;
						}
					}
				}
			}
		}
	}
	private void PCTileSelectionController()
	{
		if (Input.GetMouseButtonDown(0) && !GameManager.instance.activePlayer.GetComponent<Player>().isWalking)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 100, 1 << 8))
			{
				ManagePath(hit.transform.GetComponent<Tile>(), GameManager.instance.localPlayerIndex, GameManager.instance._stepsLeft);
				NetworkClient.instance.SendPlayerMove(hit.transform.GetComponent<Tile>());
			}
		}
	}
	private void MobileTileSelectionController()
	{
		if (Input.touchCount > 0 && !GameManager.instance.activePlayer.GetComponent<Player>().isWalking)
		{
			Touch touch = Input.GetTouch(0);
			touchPosition = touch.position;
			RaycastHit hitTile;
			Ray ray = Camera.main.ScreenPointToRay(touchPosition);

			if (touch.phase == TouchPhase.Began && Physics.Raycast(ray, out hitTile, 1 << 8))
			{
				Tile tile = hitTile.transform.GetComponent<Tile>();
				if (hitTile.transform.CompareTag("Tile"))
				{
					NetworkClient.instance.SendPlayerMove(tile);
					ManagePath(tile, GameManager.instance.localPlayerIndex, GameManager.instance._stepsLeft);
				}
			}
		}
	}
	private void HandleTouchRotation(Touch touch)
	{
		if (touch.phase == TouchPhase.Began)
		{
			prevTouch = touch.position;
		}

		deltaPos = prevTouch.x - touch.position.x;
		tilePrefabParent.transform.GetChild(0).transform.localEulerAngles += new Vector3(0, deltaPos * 4, 0) * Time.deltaTime;
		prevTouch = touch.position;
	}
	private bool CanControl()
	{
		if (GameManager.instance.activePlayer != null)
		{
			if (GameManager.instance.activePlayer.GetComponent<Player>().playerState == PlayerState.ALIVE)
			{
				if (!GameManager.instance.activePlayer.GetComponent<Player>().isWalking)
				{
					if (GameManager.instance.GetTurn())
					{
						if (GameManager.instance._stepsLeft > 0)
						{
							if (GameManager.instance.canMove)
							{
								return true;
							}
						}
					}
				}
			}
		}
		return false;
    }
	#endregion

	private void NewTurn()
	{
		HandlePreviousPath();
	}
	private void ChangeGameState(GameState gameState)
	{
		if (gameState == GameState.LOBBY)
		{
			characterSelection = true;
		}
		else
		{
			characterSelection = false;
		}
	}

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
		plane.transform.position = new Vector3(0, 0, 0);
		plane.transform.rotation = Quaternion.identity;
		plane.name = "DebugPlane";
	}
	private void SetUpBoardTracker()
	{
		BoardTrackers.AddMany("BottomLeft", "BottomRight", "TopLeft", "TopRight");
	}
    #endregion

    #region Tile Tracking
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
            if (_activeLooseTile && controllerType != ControllerType.Mobile_Virtual)
            {
				tilePrefabParent.SetActive(true);
				tilePrefabParent.transform.GetChild(0).transform.localPosition = trackedImage.transform.localPosition;
				tilePrefabParent.transform.GetChild(0).transform.localRotation = trackedImage.transform.localRotation;
			}
		}
	}
	/*
	private bool isTrackable(ARTrackedImage image)
	{
		if (image.trackingState == TrackingState.Tracking)
		{
			RaycastHit hit;
			Vector3 dir = (image.transform.position - Camera.main.transform.position).normalized;
			float distance = (image.transform.position - Camera.main.transform.position).magnitude;
			MeshRenderer[] mesh = tilePrefabParent.GetComponentsInChildren<MeshRenderer>();
			if (Physics.Raycast(Camera.main.transform.position, dir, out hit, distance, trackingCheckMask))
			{
				foreach (MeshRenderer m in mesh)
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
	}*/
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
				if (lockBoard == false)
				{
					if (trackedImage.trackingState == TrackingState.Tracking)
					{
						multiTrackList.Add(trackedImage);
					}
				}
			}
			else
			{
				if (true)
				{
					HandleSingleTracker(trackedImage);
				}
			}
		}

		if (multiTrackList.Count > 0)
			HandleMultiTracker(multiTrackList[0]);
	}
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
		droppedOutPrefab.transform.SetParent(tilePrefabParent.transform);
		droppedOutPrefab.transform.localPosition = Vector3.zero;
		droppedOutPrefab.transform.localRotation = Quaternion.identity;

		droppedOutPrefab.GetComponent<FindNearestGridSlot>().enabled = true;
		BoardGrid.instance.trackedTile = droppedOutPrefab.GetComponent<Tile>();
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

	#region Handle Swipe
	Vector2 swipeDelta;
	Vector2 startSwipe;

	private void HandleSwipe()
    {
		swipeDelta = Vector2.zero;
		SetStart(false);
        if (canSwipe)
        {
			if (swipeDelta.magnitude > 100)
			{
				//Which direction?
				float x = swipeDelta.x;
				float y = swipeDelta.y;
				if (Mathf.Abs(x) > Mathf.Abs(y))
				{
					//Left or Right
					if (x < 0)
					{
						canSwipe = false;
						GUIManager.instance.OnPlayerRoleChanged(-1);
					}
					else
					{
						canSwipe = false;
						GUIManager.instance.OnPlayerRoleChanged(1);
					}
				}
				SetStart(true);
			}
		}
	}
	private void SetStart(bool forceReset)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
		if (Input.GetMouseButtonDown(0) || forceReset)
        {
            if (Input.GetMouseButtonDown(0))
            {
				canSwipe = true;
            }
			startSwipe = Input.mousePosition;
		}
		swipeDelta = (Vector2)Input.mousePosition - startSwipe;
#elif UNITY_ANDROID || UNITY_IOS
		if(Input.GetTouch(0).phase == TouchPhase.Began || forceReset)	
		{
			if(Input.GetTouch(0).phase == TouchPhase.Began)
			{
				canSwipe = true;
			}
			startSwipe = Input.touches[0].position;	
		}
		swipeDelta = Input.touches[0].position - startSwipe;
#endif
	}
	#endregion
}

