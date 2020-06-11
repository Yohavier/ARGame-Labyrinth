using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CrewMember : Player
{
	//Cant move to next Tile if there is already a player instance inside
	public override bool CheckForOtherPlayers(Tile nextTile)
	{
		if (nextTile.GetComponentInChildren<Player>() != null)
		{
			return false;
		}
		return true;
	}

	//Checks if there is something on the tile
	public override void CheckTileForOtherMods(Tile tile)
	{
		base.CheckTileForOtherMods(tile);
		//check for item
		Item item = tile.GetComponentInChildren<Item>();
		if(item != null && storedItem == null)
		{
			InformationPanel.instance.SetPickUpItemButton(true);
			InformationPanel.instance.pickUpItemButton.onClick.AddListener(() => PickUpItem(item, tile));
		}
        else
        {
			RemovePickUpButtonListener();
		}

		//Check for Generator
		Generator generator = tile.GetComponentInChildren<Generator>();
		if(generator != null)
        {
			InformationPanel.instance.SetRepairGeneratorButton(true);
			InformationPanel.instance.repairGeneratorButton.onClick.AddListener(() => Repair(generator));
        }
        else
        {
			RemoveRepairButtonListener();
		}

		//Check for escape capsule mod
		EscapeCapsule capsule = tile.GetComponent<EscapeCapsule>();
		if (capsule != null && storedItem != null)
		{
			InformationPanel.instance.SetDropItemButton(true);
			InformationPanel.instance.dropItemButton.onClick.AddListener(() => DropItemInCapsule(capsule, tile));
		}
        else
        {
			RemoveDropItemButtonListener();
        }
	}

	//Pick item up if possible
	private void PickUpItem(Item item, Tile tile)
	{ 
		RemovePickUpButtonListener();

		NetworkClient.instance.SendItemCollected(tile);

		storedItem = item.gameObject;
		item.isStored = true;
		storedItem.transform.SetParent(this.transform);
		InformationPanel.instance.SetItemText(item.itemName);
		storedItem.GetComponent<MeshRenderer>().enabled = false;		
	}

    private void RemovePickUpButtonListener()
    {
		InformationPanel.instance.SetPickUpItemButton(false);
		InformationPanel.instance.pickUpItemButton.onClick.RemoveAllListeners();
	}

	private void RemoveDropItemButtonListener()
    {
		InformationPanel.instance.SetDropItemButton(false);
		InformationPanel.instance.dropItemButton.onClick.RemoveAllListeners();
    }

	private void RemoveRepairButtonListener()
    {
		InformationPanel.instance.SetRepairGeneratorButton(false);
		InformationPanel.instance.repairGeneratorButton.onClick.RemoveAllListeners();
	}

	//if over capsule drop stored item
	private void DropItemInCapsule(EscapeCapsule capsule, Tile tile)
	{
		if (storedItem != null)
		{
			capsule.DisplayProgress();
			storedItem.transform.SetParent(tile.transform);
			storedItem.GetComponent<MeshRenderer>().enabled = true;
			storedItem = null;
			InformationPanel.instance.SetItemText("none");
		}
	}

	private void Repair(Generator generator)
    {
		RemoveRepairButtonListener();
		generator.ActivateGenerator();
    }
}
