using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindNearestGridSlot : MonoBehaviour
{
    public float distance = 0.07f;
    GameObject target;
    Tile targetTile;
    [SerializeField]
    RaycastHit Hit;
    private BoardGrid board;
    private void Start()
    {
        board = FindObjectOfType<BoardGrid>();
    }
    private void Update()
    {
        FindTileWithRays();
    }

    //shots ray in 4 directions and tries to find the right Tile
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

        if (hitCounter == 1)
        {
            targetTile = target.GetComponent<Tile>();
        }
        else if (hitCounter == 3 && targetTile != null)
        {
            if (targetTile.edgePiece && (targetTile.canMoveVertical || targetTile.canMoveHorizontal))
            {
                board.InsertNewRoomPushing(targetTile, GetComponent<Tile>());
                Debug.Log(targetTile.row.ToString() + targetTile.column.ToString());
                targetTile = null;
                GetComponent<FindNearestGridSlot>().enabled = false;
            }
        }
        else if (hitCounter == 0)
        {
            targetTile = null;
        }
    }
}
