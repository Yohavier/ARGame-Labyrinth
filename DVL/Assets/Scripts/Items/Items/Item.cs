using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class Item: MonoBehaviour
{
    public string itemName;
    public bool isStored;
    public Vector3 placementVector;

    public Canvas interactionCanvas;
    public Camera arCamera;
    public LayerMask mask;
    public Tile position;
    public bool alreadyRepairedThisTurn;

    public void Awake()
    {
        InstantiateItem();   
    }
    private void OnEnable()
    {
        Eventbroker.instance.onNotifyNextTurn += NewTurn;
    }
    private void NewTurn()
    {
        if (GameManager.instance.GetTurn())
            alreadyRepairedThisTurn = false;
        else
            alreadyRepairedThisTurn = true;
    }
    private void OnDisable()
    {
        Eventbroker.instance.onNotifyNextTurn -= NewTurn;
    }

    public virtual void InstantiateItem()
    {
        arCamera = Camera.main;
        interactionCanvas.worldCamera = arCamera;
    }
    private void Update()
    {
        if (GameManager.instance.activePlayer.GetComponent<Player>().positionTile == position)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = arCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100, mask))
                {
                    if (hit.transform.gameObject == this.gameObject)
                        ToggleGeneratorCanvas();
                }
            }
        }
        else
        {
            interactionCanvas.gameObject.SetActive(false);
        }
    }

    public void ToggleGeneratorCanvas()
    {
        interactionCanvas.gameObject.SetActive(!interactionCanvas.gameObject.activeSelf);
    }

    public virtual void SendInteraction()
    {
        Debug.Log("press");
        if (!alreadyRepairedThisTurn)
        {
            CrewMember player = GameManager.instance.activePlayer.GetComponent<CrewMember>();
            if (player != null)
            {
                player.PickUpItem(this, position);
                interactionCanvas.enabled = false;
            }
        }
    }
}
