using UnityEngine;

public class FindNearestGridSlot : MonoBehaviour
{
	public float distance = 0.07f;

	private GameObject target;

	private Tile targetTile;

	[SerializeField]
	private RaycastHit Hit;

	private BoardGrid board;

	public LayerMask mask;

	private void Start()
	{
		board = FindObjectOfType<BoardGrid>();
		mask.value = System.Int32.MaxValue;
	}

	private void Update()
	{
		if (LocalGameManager.instance.GetTurn())
			FindTileWithRays();
	}

	//get the tile that will be pushed away by inserting a new Tile
	private void FindTileWithRays()
	{
		int hitCounter = 0;
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out Hit, distance, mask))
		{
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * Hit.distance, Color.yellow);
			target = Hit.collider.gameObject;
			hitCounter++;
		}
		if (Physics.Raycast(transform.position, -transform.TransformDirection(Vector3.forward), out Hit, distance, mask))
		{
			Debug.DrawRay(transform.position, -transform.TransformDirection(Vector3.forward) * Hit.distance, Color.blue);
			target = Hit.collider.gameObject;
			hitCounter++;
		}
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out Hit, distance, mask))
		{
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * Hit.distance, Color.red);
			target = Hit.collider.gameObject;
			hitCounter++;
		}
		if (Physics.Raycast(transform.position, -transform.TransformDirection(Vector3.right), out Hit, distance, mask))
		{
			Debug.DrawRay(transform.position, -transform.TransformDirection(Vector3.right) * Hit.distance, Color.black);
			target = Hit.collider.gameObject;
			hitCounter++;
		}


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

	private void CallGridToPushRoom()
	{
		if (targetTile != null)
		{
			if (targetTile.edgePiece && (targetTile.canMoveVertical || targetTile.canMoveHorizontal))
			{
				NetworkClient.instance.SendGridMove(targetTile, GetComponent<Tile>());
				board.InsertNewRoomPushing(targetTile, GetComponent<Tile>());
				targetTile = null;
			}
			return;
		}
	}
}
