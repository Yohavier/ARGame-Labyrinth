using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FindNearestGridSlot : MonoBehaviour
{
	public float distance = 0.15f;
	private GameObject target;
	public VR_Tile targetTile;
	public LayerMask mask;

    private void FixedUpdate()
	{
		FindTileWithRays();
	}

	//get the tile that will be pushed away by inserting a new Tile
	int hitCounter = 0;
	private void FindTileWithRays()
	{
		hitCounter = 0;

		hitCounter += SendRay(transform.TransformDirection(Vector3.forward));
		hitCounter += SendRay(-transform.TransformDirection(Vector3.forward));
		hitCounter += SendRay(transform.TransformDirection(Vector3.right));
		hitCounter += SendRay(-transform.TransformDirection(Vector3.right));

		switch (hitCounter)
		{
		    case 1:
			    targetTile = target.GetComponent<VR_Tile>();
			    return;
		    case 3:
			    CallGridToPushRoom();
			    break;
		}

		if (hitCounter == 4)
		{
			HandleMesh(false);
		}
		else
		{
			HandleMesh(true);
		}
		
	}

	private void HandleMesh(bool state)
    {
		GetComponentInChildren<BoxCollider>().enabled = state;
		MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
		SkinnedMeshRenderer[] sMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int i = 0; i < meshes.Length; i++)
        {
			meshes[i].enabled = state;			
        }

		for (int i = 0; i < sMeshes.Length; i++)
		{
			sMeshes[i].enabled = state;
		}
	}
	//Create a raycast root is this gameObject
	private int SendRay(Vector3 rayDirection)
	{
		RaycastHit Hit;
		Vector3 offPos;
		if (targetTile != null)
		{
			offPos = (transform.position - targetTile.transform.position + transform.up).normalized * 0.03f;
		}
		else
		{
			offPos = Vector3.zero;
		}

		if (Physics.Raycast(transform.position + offPos, rayDirection, out Hit, distance, mask))
		{
			Debug.DrawRay(transform.position + offPos, rayDirection * Hit.distance, Color.red);
			target = Hit.collider.gameObject;
			return 1;
		}
		else
		{
			return 0;
		}
	}

	//call and push room into grid
	private void CallGridToPushRoom()
	{
		if (targetTile != null)
		{
			if (targetTile.edgePiece && (targetTile.canMoveVertical || targetTile.canMoveHorizontal))
			{
                Debug.Log("f");
				VR_Grid.instance.InsertNewRoomPushing(targetTile, GetComponent<VR_Tile>());
				targetTile = null;
			}
		}
	}
}
