using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VR_ItemPickUp : MonoBehaviour
{
    public GameObject storedItem;
    private GameObject tempItem;

    [SerializeField] private SteamVR_Action_Boolean pickUp;
    public SteamVR_Input_Sources handType;

    private void Start()
    {
        pickUp.AddOnStateDownListener(PickUpItem, handType);
        pickUp.AddOnStateUpListener(DropItem, handType);
    }

    public void PickUpItem(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if(tempItem != null)
        {
            storedItem = tempItem;
            storedItem.transform.SetParent(this.gameObject.transform);
        }
    }
    public void DropItem(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (storedItem != null)
        {
            storedItem.transform.SetParent(null);
            storedItem = null;
        }
    }
    public void DropItem()
    {
        if(storedItem != null)
        {
            Destroy(storedItem.gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            tempItem = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Item>() == tempItem)
        {
            tempItem = null;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Item"))
        {
            tempItem = collision.transform.gameObject;
        }
    }
}
