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
	public override void CheckTileForOtherMods(Tile tile)
	{
		//Check for doors
		base.CheckTileForOtherMods(tile);

		//check for item
		HandlePickUpItem(tile);

		//Check for Generator
		HandleRepairGenerator(tile);

		//Check for escape capsule mod
		HandleDropItem(tile);
	}

    #region Handle Pickup Item extension
	private void HandlePickUpItem(Tile tile) 
	{
		Item item = tile.GetComponentInChildren<Item>();
		if (item != null && storedItem == null)
		{
			InformationPanel.instance.SetPickUpItemButton(true);
			InformationPanel.instance.pickUpItemButton.onClick.AddListener(() => PickUpItem(item, tile));
		}
		else
		{
			RemovePickUpButtonListener();
		}
	}
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
    #endregion

    #region Handle Drop Item extension
	private void HandleDropItem(Tile tile)
    {
		EscapeCapsule capsule = tile.GetComponent<EscapeCapsule>();
		if (capsule != null && storedItem != null)
		{
			InformationPanel.instance.SetDropItemButton(true);
			InformationPanel.instance.dropItemButton.onClick.AddListener(() => DropItem(capsule, tile));
		}
		else
		{
			RemoveDropItemButtonListener();
		}
	}
	private void DropItem(EscapeCapsule capsule, Tile tile)
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
	private void RemoveDropItemButtonListener()
    {
		InformationPanel.instance.SetDropItemButton(false);
		InformationPanel.instance.dropItemButton.onClick.RemoveAllListeners();
    }
	#endregion

	#region Handle Repair Generator extension
	private void HandleRepairGenerator(Tile tile)
    {
		Generator generator = tile.GetComponentInChildren<Generator>();
		if (generator != null)
		{
			InformationPanel.instance.SetRepairGeneratorButton(true);
			InformationPanel.instance.repairGeneratorButton.onClick.AddListener(() => RepairGenerator(generator));
		}
		else
		{
			RemoveRepairGeneratorButtonListener();
		}
	}
	private void RepairGenerator(Generator generator)
	{
		RemoveRepairGeneratorButtonListener();
		generator.RepairGenerator();
	}
	private void RemoveRepairGeneratorButtonListener()
	{
		InformationPanel.instance.SetRepairGeneratorButton(false);
		InformationPanel.instance.repairGeneratorButton.onClick.RemoveAllListeners();
	}
	#endregion
}
