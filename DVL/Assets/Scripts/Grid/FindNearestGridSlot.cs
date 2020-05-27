using UnityEngine;

public class FindNearestGridSlot : MonoBehaviour
{
	public float distance = 0.07f;

	private GameObject target;

	private Tile targetTile;

	[SerializeField]
	private RaycastHit Hit;

	private BoardGrid board;

	private void Start()
	{
		board = Object.FindObjectOfType<BoardGrid>();
	}

	private void Update()
	{
		FindTileWithRays();
	}

	private void FindTileWithRays()
	{
		int hitCounter = 0;
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out Hit, distance))
		{
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * Hit.distance, Color.yellow);
			target = Hit.collider.gameObject;
			hitCounter++;
		}
		if (Physics.Raycast(transform.position, -transform.TransformDirection(Vector3.forward), out Hit, distance))
		{
			Debug.DrawRay(transform.position, -transform.TransformDirection(Vector3.forward) * Hit.distance, Color.blue);
			target = Hit.collider.gameObject;
			hitCounter++;
		}
		if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out Hit, distance))
		{
			Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * Hit.distance, Color.red);
			target = Hit.collider.gameObject;
			hitCounter++;
		}
		if (Physics.Raycast(transform.position, -transform.TransformDirection(Vector3.right), out Hit, distance))
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
			if (targetTile != null)
			{
				if (targetTile.edgePiece && (targetTile.canMoveVertical || targetTile.canMoveHorizontal))
				{
					board.InsertNewRoomPushing(targetTile, this.GetComponent<Tile>());
					targetTile = null;
				}
				return;
			}
			break;
		}
		if (hitCounter == 0)
		{
			targetTile = null;
		}
	}
}
