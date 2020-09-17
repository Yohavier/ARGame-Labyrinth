using Assets.Scripts.Player;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class VR_Enemy : MonoBehaviour
{
	public Animator anim;

	public bool pauseMovement;
	public bool stopMovement;
	private IEnumerator walkCoroutine;
	private IEnumerator walkCycle;

	[HideInInspector] public FogOfWar playerFOW;
	[HideInInspector] public VR_Tile positionTile;
	private VR_Tile playerTargetTile;

	private bool _isWalking;
	public bool isWalking
	{
		get { return _isWalking; }
		set
		{
			_isWalking = value;
			if (anim != null)
			{
				anim.SetBool("Walk Forward", value);
			}
		}
	}

	private void Start()
	{
		anim = GetComponent<Animator>();
		StartMovement();
	}

	#region Player Movement
	public void ChangePlayerPosition(VR_Tile newPos)
	{
		positionTile = newPos;
		playerTargetTile = CheckNewSurrounding(positionTile);
	}

	//Moving Player Along Path
	private IEnumerator MoveToTarget(List<VR_Tile> path, float time)
	{
		isWalking = true;
		anim.SetBool("Walk Forward", true);
		foreach (VR_Tile tile in path)
		{
			if (CheckForOtherPlayers(tile) && !stopMovement)
			{
				SmoothRotation(tile);
				float i = 0.0f;
				float rate = 1.0f / time;
				while (i < 1.0f)
				{
					if (!pauseMovement)
					{
						i += Time.deltaTime * rate;
						var movementVector = Vector3.Lerp(new Vector3(positionTile.transform.position.x, transform.position.y, positionTile.transform.position.z),
														  new Vector3(tile.transform.position.x, transform.position.y, tile.transform.position.z), i);
						transform.position = movementVector;
					}
					yield return null;
				}
				transform.SetParent(tile.transform);
				transform.localPosition = Vector3.zero;
				ChangePlayerPosition(tile);
			}
			else
			{
				StopAllCoroutines();
				isWalking = false;
				stopMovement = false;
			}
			yield return null;
		}
		isWalking = false;
	}

	//Rotate player in move direction 
	private void SmoothRotation(VR_Tile tile)
	{
		Vector3 relativePos = tile.transform.position - transform.position;
		Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
		Quaternion finrot = Quaternion.AngleAxis(rotation.eulerAngles.y, Vector3.up);
		StartCoroutine(Rotate(Vector3.up, CalcShortestRot(transform.eulerAngles.y, finrot.eulerAngles.y), .3f));
	}

	IEnumerator Rotate(Vector3 axis, float angle, float duration = 1.0f)
	{
		Quaternion from = transform.rotation;
		Quaternion to = transform.rotation;
		to *= Quaternion.Euler(axis * angle);

		float elapsed = 0.0f;
		while (elapsed < duration)
		{
			transform.rotation = Quaternion.Slerp(from, to, elapsed / duration);
			elapsed += Time.deltaTime;
			yield return null;
		}
		transform.rotation = to;
	}

	float CalcShortestRot(float from, float to)
	{
		// If from or to is a negative, we have to recalculate them.
		// For an example, if from = -45 then from(-45) + 360 = 315.
		if (from < 0)
		{
			from += 360;
		}

		if (to < 0)
		{
			to += 360;
		}

		// Do not rotate if from == to.
		if (from == to ||
		   from == 0 && to == 360 ||
		   from == 360 && to == 0)
		{
			return 0;
		}

		// Pre-calculate left and right.
		float left = (360 - from) + to;
		float right = from - to;
		// If from < to, re-calculate left and right.
		if (from < to)
		{
			if (to > 0)
			{
				left = to - from;
				right = (360 - to) + from;
			}
			else
			{
				left = (360 - to) + from;
				right = to - from;
			}
		}

		// Determine the shortest direction.
		return ((left <= right) ? left : (right * -1));
	}
	#endregion


	//Check for other Players
	public virtual bool CheckForOtherPlayers(VR_Tile nextTile)
	{
		if (nextTile.GetComponentInChildren<Player>() != null)
		{
			return false;
		}
		return true;
	}

	public virtual void CheckTileForOtherMods(VR_Tile tile) { }
	protected virtual void Dying() { }
	protected virtual void Dead() { }
	protected virtual void CheckDeathCounter() { }
	public virtual void NotifyNextTurn(bool toggle) { }
	protected virtual VR_Tile CheckNewSurrounding(VR_Tile tile)
	{
		List<VR_Tile> tiles = VR_DetectNeighbours.DetectTileRadius(tile, 5, true);
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].playerPos != null)
            {
				if (tiles[i] != playerTargetTile)
                {
					return tiles[i];
				}
            }
        }
		return null;
	}

	private IEnumerator AutoMove()
    {
		VR_Tile target = null;
		float timer = 0;
        while (true)
        {
			if(playerTargetTile != null && playerTargetTile != positionTile)
            {
				if (playerTargetTile != target)
                {
					target = playerTargetTile;
					var path = VR_Pathfinding.FindPath(100, positionTile, playerTargetTile);

					if (path != null)
					{
						if (walkCoroutine != null)
						{
							StopCoroutine(walkCoroutine);
						}
						walkCoroutine = MoveToTarget(path, 3);
						StartCoroutine(walkCoroutine);
					}
				}			
			}
            else
            {
				timer += Time.deltaTime;
				if (timer > 1)
				{
					timer = 0;
					var path = VR_Pathfinding.FindPath(100, positionTile, VR_Grid.instance.grid[Random.Range(0, VR_Grid.instance.grid.Count - 1)]);
					if (path != null && isWalking == false)
					{
						if (walkCoroutine != null)
						{
							StopCoroutine(walkCoroutine);
						}
						walkCoroutine = MoveToTarget(path, 1);
						StartCoroutine(walkCoroutine);
					}
				}
			}
			yield return null;
        }
    }

	public void StopOnGridMove()
    {
		if (walkCoroutine != null) 
			StopCoroutine(walkCoroutine);
		if (walkCycle != null)
			StopCoroutine(walkCycle);

		isWalking = false;
		stopMovement = false;
		transform.SetParent(positionTile.transform);
		transform.localPosition = Vector3.zero;
	}
	public void StartMovement()
    {
        if (VR_Grid.instance.grid.Contains(positionTile))
        {
			walkCycle = AutoMove();
			StartCoroutine(walkCycle);
		}
    }
}
