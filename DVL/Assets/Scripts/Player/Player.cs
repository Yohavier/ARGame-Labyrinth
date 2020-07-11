using Assets.Scripts.GameManagement;
using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState { ALIVE, DEAD, DYING}
public class Player : MonoBehaviour
{
	[Header("Player Stats")]
	public PlayerIndex playerIndex;
	public SO_PlayerClass playerRole;

	public int diceModificator = 0;
	public int fogOfWarRadius = 0;
	public int footstepDetectionRadius =  0;
	public int repairSpeed = 0;
	public int maxDeathTurnCounter = 0;
	private Animator anim;

	//PowerUps
	private CommunicatorPowerUp _communicatorPowerUp;
	public CommunicatorPowerUp communicatorPowerUp
    {
        get { return _communicatorPowerUp; }
        set 
		{ 
			_communicatorPowerUp = value;
			ChangePlayerPosition(positionTile);	
		}
    }

	[HideInInspector] public FogOfWar playerFOW;
	[HideInInspector] public Tile positionTile;
	[HideInInspector] public GameObject storedItem;
	[HideInInspector] private WalkingTraces trace;
	[HideInInspector] public bool isWalking;


	//Health Status of player
	private PlayerState _playerState;
	public PlayerState playerState
	{
		get
		{
			return _playerState;
		}
		set
		{
			_playerState = value;

			if (IsLocalPlayer())
			{
				InformationPanel.instance.SetStateText(_playerState.ToString());
			}

			if (_playerState == PlayerState.DYING)
			{
				Dying();
			}
			else if (_playerState == PlayerState.DEAD)
			{
				Dead();
			}
		}
	}

	public bool IsLocalPlayer()
	{
		return playerIndex == LocalGameManager.instance.localPlayerIndex;
	}

    private void Start()
    {
		//anim = GetComponent<Animator>();
    }
    #region Initialization
    public void SetUpPlayer(int count)
	{
		playerIndex = ChooseRightIndex(count);
		trace = GetComponent<WalkingTraces>();

		if (NetworkClient.instance.networkPlayers[count - 1].roleIndex > RoleIndex.Invalid)
		{
			playerRole = InformationPanel.instance.playerRoles[(int)NetworkClient.instance.networkPlayers[count - 1].roleIndex];
			playerRole.SetPlayerStats(this);
		}

		if (!IsLocalPlayer())
		{
			MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
			SkinnedMeshRenderer sr = GetComponentInChildren<SkinnedMeshRenderer>();
			if (mr != null)
				mr.enabled = false;
			else if (sr != null)
				sr.enabled = false;

			Destroy(GetComponent<FogOfWar>());
		}
		else
		{
			LocalGameManager.instance.activePlayer = gameObject;
			playerState = PlayerState.ALIVE;
			playerFOW = GetComponent<FogOfWar>();
			SetInformations();
		}
	}
	private PlayerIndex ChooseRightIndex(int count)
    {
		switch (count)
		{
			case 1:
				return PlayerIndex.Player1;
			case 2:
				return PlayerIndex.Player2;
			case 3:
				return PlayerIndex.Player3;
			case 4:
				return PlayerIndex.Enemy;
			default:
				Debug.LogWarning("Player Initializaion went wrong!");
				return PlayerIndex.Invalid;
		}
	}
	private void SetInformations()
    {
		InformationPanel.instance.SetPlayerText(playerIndex.ToString());
		InformationPanel.instance.SetItemText("None");
	}
    #endregion

    #region Player Movement
    public void ChangePlayerPosition(Tile newPos)
	{
		positionTile = newPos;

		if (playerFOW != null)
		{
			//if active player call his FOW
			playerFOW.OnChangePlayerPosition(positionTile);
		}
		else if(LocalGameManager.instance.activePlayer != null)
		{
			//if not active Player, call active players FOW
			LocalGameManager.instance.activePlayer.GetComponent<Player>().playerFOW.OnChangePlayerPosition(LocalGameManager.instance.activePlayer.GetComponent<Player>().positionTile);	
		}

        if (!positionTile.isInFOW)
        {
			if(playerIndex == PlayerIndex.Enemy)
            {
				AkSoundEngine.SetSwitch("character", "enemy", gameObject);
            }
            else
            {
				AkSoundEngine.SetSwitch("character", "crew", gameObject);
			}
			AkSoundEngine.PostEvent("character_footstep", gameObject);
        }
			
		CreatePositionIndicatorTrace();
	}

	private void CreatePositionIndicatorTrace()
    {
		if (trace != null)
		{
			trace.SpawnParticlesystem(positionTile);
		}
	}

	//Move along a List of Tiles
	public void MoveToTarget(List<Tile> path)
	{
		if (path.Count > 0)
			StartCoroutine(Moving(path, 1));
	}

	//Moving Player Along Path
	private IEnumerator Moving(List<Tile> path, float time)
	{
		isWalking = true;
		//anim.SetBool("Walk Forward", true);
		foreach(Tile tile in path)
		{
			if (CheckForOtherPlayers(tile))
			{
				AdjustRotation(tile);
				float i = 0.0f;
				float rate = 1.0f / time;
				while (i < 1.0f)
				{
					i += Time.deltaTime * rate;
					var movementVector = Vector3.Lerp(new Vector3(positionTile.transform.position.x, transform.position.y, positionTile.transform.position.z),
													  new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z), i);
					transform.position = movementVector;				
					yield return null;
				}
				transform.SetParent(tile.transform);
				transform.localPosition = Vector3.zero;
				ChangePlayerPosition(tile);
				tile.UpdateTileFOW();
				LocalGameManager.instance._stepsLeft--;
			}
			else
			{
				StopAllCoroutines();
				isWalking = false;
				//anim.SetBool("Walk Forward", false);
			}
			yield return null;
		}

		if (LocalGameManager.instance.localPlayerIndex == LocalGameManager.instance.currentTurnPlayer)
			CheckTileForOtherMods(path[path.Count - 1]);
		isWalking = false;
		//anim.SetBool("Walk Forward", false);
	}

	//Rotate player in move direction 
	private void AdjustRotation(Tile lookTarget)
	{
		Vector3 relativePos = lookTarget.transform.position - transform.position;

		// the second argument, upwards, defaults to Vector3.up
		Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
		transform.rotation = rotation;
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
	}
    #endregion


    //Check for other Players
    public virtual bool CheckForOtherPlayers(Tile nextTile)
	{
		if (nextTile.GetComponentInChildren<Player>() != null)
		{
			return false;
		}
		return true;
	}
	
	public virtual void CheckTileForOtherMods(Tile tile) {}
	protected virtual void Dying() { }
	protected virtual void Dead() { }
	protected virtual void CheckDeathCounter() { }
	public virtual void NotifyNextTurn(bool toggle) { }
}
