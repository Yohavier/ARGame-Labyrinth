using System;
using UnityEngine;
using System.Collections;
using System.Threading;
using Assets.Scripts.GameManagement;
using System.Collections.Generic;

public enum TileDirectionModule { NONE, WALL, DOOR, WINDOW}
public class Tile : MonoBehaviour
{
	public int row;	//right - left
	public int column; //forward - backward

	//Door animation
	private Animation anim;
	public AnimationClip DoorOpenClip;
	public AnimationClip DoorCloseClip;

	[HideInInspector] public bool edgePiece;
	[HideInInspector] public bool canMoveHorizontal;
	[HideInInspector] public bool canMoveVertical;

	[Header("Pathfinding Costs")]
	[HideInInspector] public int gCost;
	[HideInInspector] public int hCost;
	[HideInInspector] public int FCost => gCost + hCost;
	[HideInInspector] public Tile Parent;

	[Header("Modules")]
	public TileDirectionModule initForwardModule;
	public TileDirectionModule initBackwardModule;
	public TileDirectionModule initRightModule;
	public TileDirectionModule initLeftModule;


	[HideInInspector] public TileDirectionModule ingameForwardModule;
	[HideInInspector] public TileDirectionModule ingameBackwardModule;
	[HideInInspector] public TileDirectionModule ingameRightModule;
	[HideInInspector] public TileDirectionModule ingameLeftModule;

	public bool CheckModulation(TileDirectionModule modulation)
    {
        switch (modulation)
        {
			case TileDirectionModule.NONE:
				return true;
			case TileDirectionModule.WALL:
				return false;
			case TileDirectionModule.WINDOW:
				return false;
			case TileDirectionModule.DOOR:
				if (doorOpen)
					return true;
				else
					return false;
			default:
				Debug.LogError("Invalid Movement on Tile " + gameObject.name);
				return false;
        }			
    }

	public bool doorOpen = true;

	private bool IsInFOW;
	public bool isInFOW
    {
        get
        {
			return IsInFOW;
        }
        set
        {
			IsInFOW= value;
			ToggleFOW(value);
        }
    }

	private void ToggleFOW(bool toggle)
    {
		MeshRenderer[] tileMeshes = GetComponentsInChildren<MeshRenderer>();
		SkinnedMeshRenderer[] sTileMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();

		foreach (MeshRenderer mesh in tileMeshes)
		{
			if (!mesh.gameObject.CompareTag("Tile"))
				mesh.enabled = !toggle;
			else if (mesh.gameObject.CompareTag("Tile"))
				PrefabColor(mesh);
		}

		foreach(SkinnedMeshRenderer sMesh in sTileMeshes)
        {
			if (!sMesh.gameObject.CompareTag("Tile"))
				sMesh.enabled = !toggle;
        }
	}

	public void UpdateTileFOW()
    {
		ToggleFOW(isInFOW);
    }

    private void Awake()
    {
		anim = GetComponent<Animation>();
    }

    public int index = -1; //Identifier of tile, -1 invalid index

	//Set the Data on Init or if newly pushed into the grid (Called by BoardGrid)
	public void SetTileData(int rowNum, int colNum, bool hide)
	{
		if (hide)
			isInFOW = true;

		row = rowNum;
		column = colNum;
		PrefabColor();
		UpdateTileState();
		UpdateTileMoveOptions();
	}

	//call to move the Tile on the grid
	public void Move(GridMovement move)
	{
		var targetPos = transform.localPosition + move.moveDir;
		StartCoroutine(MoveInterpolate(transform.localPosition, targetPos, 1));
		row += move.rowChangeDir;
		column += move.colChangeDir;
		UpdateTileState();
		UpdateTileMoveOptions();
	}

	//Lerp between 2 Tile position over Time
	private IEnumerator MoveInterpolate(Vector3 startPos, Vector3 targetPos, float time)
	{
		float i = 0.0f;
		float rate = 1.0f / time;
		while (i < 1.0f)
		{
			BoardGrid.instance.inMove = true;
			i += Time.deltaTime * rate;
			transform.localPosition = Vector3.Lerp(startPos, targetPos, i);
			yield return null;
		}

		if (row < 0 || row > 6 || column < 0 || column > 6)
		{
			StartCoroutine(ExplosionOrder(FindRightCorners()));
		}
	}

	//Update the possible Movestates of the Tile
	private void UpdateTileState()
	{
		if (row % 2 == 0 && column % 2 == 0)
		{
			canMoveHorizontal = false;
			canMoveVertical = false;
		}
		else if (row % 2 != 0 && column % 2 == 0)
		{
			canMoveVertical = false;
			canMoveHorizontal = true;
		}
		else if (row % 2 == 0 && column % 2 != 0)
		{
			canMoveVertical = true;
			canMoveHorizontal = false;
		}
		else
		{
			canMoveVertical = true;
			canMoveHorizontal = true;
		}

		
		if (row == 0 || column == 0 || column == BoardGrid.instance.size - 1 || row == BoardGrid.instance.size - 1)
		{
			edgePiece = true;
		}
		else
		{
			edgePiece = false;
		}
	}

	//changes the move bools dependign on the rotation of the Tile in the Grid
	private void UpdateTileMoveOptions()
	{
		if (transform.localEulerAngles.y == 0f)
		{
			ingameForwardModule = initForwardModule;
			ingameBackwardModule = initBackwardModule;
			ingameRightModule = initRightModule;
			ingameLeftModule = initLeftModule;
		}
		else if (transform.localEulerAngles.y == 90f)
		{
			ingameForwardModule = initLeftModule;
			ingameBackwardModule = initRightModule;
			ingameRightModule = initForwardModule;
			ingameLeftModule = initBackwardModule;
		}
		else if (transform.localEulerAngles.y == 180f)
		{
			ingameForwardModule = initBackwardModule;
			ingameBackwardModule = initForwardModule;
			ingameRightModule = initLeftModule;
			ingameLeftModule = initRightModule;
		}
		else if (transform.localEulerAngles.y == 270f)
		{
			ingameForwardModule = initRightModule;
			ingameBackwardModule = initLeftModule;
			ingameRightModule = initBackwardModule;
			ingameLeftModule = initForwardModule;
		}
	}

	public void PrefabColor()
	{
		MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
		if (isInFOW && !GUIManager.instance.isDebug)
		{			
			foreach(MeshRenderer mesh in meshes)
			{
				if(mesh.CompareTag("Tile"))
					mesh.material.color = Color.black;
			}
		}
		else
		{
			foreach(MeshRenderer mesh in meshes)
			{
				if (mesh.CompareTag("Tile"))
					mesh.material.color = Color.white;
			}
		}
	}

	public void PrefabColor(MeshRenderer mesh)
	{
		if (isInFOW && !GUIManager.instance.isDebug)
		{
			mesh.material.color = Color.black;
		}
		else
		{
			mesh.material.color = Color.white;
		}
	}

	public void ToggleDoors()
    {
		doorOpen = !doorOpen;
		if (doorOpen)
			OpenTileDoors();
		else
			CloseTileDoors();

		LocalGameManager.instance.activePlayer.GetComponent<FogOfWar>().OnChangePlayerPosition(this);
    }
	public void ToggleDoors(bool toggle)
    {
		doorOpen = toggle;
		if (doorOpen)
			OpenTileDoors();
		else
			CloseTileDoors();

		LocalGameManager.instance.activePlayer.GetComponent<FogOfWar>().OnChangePlayerPosition(this);
	}

	private void OpenTileDoors()
    {
        if (DoorOpenClip != null)
        {
			anim.clip = DoorOpenClip;
			anim.Play();
		}
	}

	private void CloseTileDoors()
    {
		if(DoorCloseClip != null)
        {
			anim.clip = DoorCloseClip;
			anim.Play();
		}
    }

	public bool TileContainsDoor()
    {
		if(ingameForwardModule== TileDirectionModule.DOOR || ingameBackwardModule == TileDirectionModule.DOOR || ingameLeftModule == TileDirectionModule.DOOR || ingameRightModule == TileDirectionModule.DOOR)
        {
			return true;
        }
        else
        {
			return false;
        }
    }

	//sprengplätze mit colliderecken
	//sprengen mit kurzem Zeitversatz
	//shader der das Tile auflösen lässt
	public ParticleSystem explosion;
	public int explosionNumber = 4;
	private List<Vector3> FindRightCorners()
    {
		List<Vector3> corners = new List<Vector3>();
		if (row < 0)
		{
			corners.Add(new Vector3(transform.position.x + 0.05f, 0.07f, transform.position.z + 0.05f));
			corners.Add(new Vector3(transform.position.x + 0.05f, 0.07f, transform.position.z - 0.05f));
		}
		else if (row > 6)
        {
			corners.Add(new Vector3(transform.position.x - 0.05f, 0.07f, transform.position.z + 0.05f));
			corners.Add(new Vector3(transform.position.x - 0.05f, 0.07f, transform.position.z - 0.05f));
		}
		else if(column < 0)
        {
			corners.Add(new Vector3(transform.position.x - 0.05f, 0.07f, transform.position.z + 0.05f));
			corners.Add(new Vector3(transform.position.x + 0.05f, 0.07f, transform.position.z + 0.05f));
		}
		else if (column > 6)
        {
			corners.Add(new Vector3(transform.position.x - 0.05f, 0.07f, transform.position.z - 0.05f));
			corners.Add(new Vector3(transform.position.x + 0.05f, 0.07f, transform.position.z - 0.05f));
		}
		return corners;
    }
	private IEnumerator ExplosionOrder(List<Vector3> pos)
    {
		Vector3 start = pos[0];
		Vector3 end = pos[1];
		Vector3 dir = (start - end).normalized * 0.1f;

        for (int i = 0; i < explosionNumber; i++)
        {
			float factor = i / (float)(explosionNumber - 1);
			Instantiate(explosion, start - dir * factor, Quaternion.identity);
			AkSoundEngine.PostEvent("tile_expell", gameObject);
			yield return new WaitForSeconds(0.2f);
		}

		BoardGrid.instance.inMove = false;
		BoardGrid.instance.RemoveTileFromGrid(this);
		HandleTrackedImageLib.instance.ChangeTrackedPrefab(this.gameObject);
		LocalGameManager.instance.canMove = true;
	}
}
