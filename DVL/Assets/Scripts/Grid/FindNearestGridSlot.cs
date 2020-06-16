using UnityEngine;
using UnityEngine.SocialPlatforms;

public class FindNearestGridSlot : MonoBehaviour
{
	public float distance = 0.1f;
	private GameObject target;
	private Tile targetTile;
	public LayerMask mask;

	private void Update()
	{
        if (LocalGameManager.instance.activePlayer)
        {
			if(LocalGameManager.instance.activePlayer.GetComponent<Player>().playerState == PlayerState.ALIVE)
            {
				if (LocalGameManager.instance.GetTurn() && !LocalGameManager.instance.activePlayer.GetComponent<Player>().isWalking)
                {
					FindTileWithRays();
				}
			}
		}
	}

	//get the tile that will be pushed away by inserting a new Tile
	private void FindTileWithRays()
	{
		int hitCounter = 0;

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
		if (Physics.Raycast(transform.position, rayDirection, out Hit, distance, mask))
		{
			Debug.DrawRay(transform.position, rayDirection * Hit.distance, Color.red);
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
				targetTile = null;
			}
		}
	}
}
