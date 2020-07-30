using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FindNearestGridSlot : MonoBehaviour
{
	public float distance = 0.15f;
	private GameObject target;
	private Tile targetTile;
	public LayerMask mask;

    private void FixedUpdate()
	{
        if (LocalGameManager.instance.activePlayer)
        {
			if(LocalGameManager.instance.activePlayer.GetComponent<Player>().playerState == PlayerState.ALIVE)
            {
				if (LocalGameManager.instance._moveTileToken && !LocalGameManager.instance.activePlayer.GetComponent<Player>().isWalking)
                {
					FindTileWithRays();
				}
			}
		}
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
			targetTile = target.GetComponent<Tile>();
			return;
		case 3:
			CallGridToPushRoom();
			break;
		}

		if (hitCounter == 0)
		{
			targetTile = null;
		}
	}

	//Create a raycast root is this gameObject
	private int SendRay(Vector3 rayDirection)
	{
		RaycastHit Hit;
		Vector3 offPos;
		if (targetTile != null)
		{
			offPos = (transform.position - targetTile.transform.position).normalized * 0.03f;
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
				NetworkClient.instance.SendGridMove(targetTile, GetComponent<Tile>());
				BoardGrid.instance.InsertNewRoomPushing(targetTile, GetComponent<Tile>());
				LocalGameManager.instance._moveTileToken = false;
				LocalGameManager.instance.canMove = false;
				targetTile = null;
			}
		}
	}
}
