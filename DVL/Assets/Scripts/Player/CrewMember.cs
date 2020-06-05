using System.Collections;
using System.Collections.Generic;
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
		//check for item
		Item item = tile.GetComponentInChildren<Item>();
		if(item != null)
		{
			PickUpItem(item, tile);
		}

		//Check for escape capsule mod
		EscapeCapsule capsule = tile.GetComponent<EscapeCapsule>();
		if (capsule != null)
		{
			DropItemInCapsule(capsule, tile);
		}
	}

	//Pick item up if possible
	private void PickUpItem(Item item, Tile tile)
	{
		if (storedItem == null)
		{
			NetworkClient.instance.SendItemCollected(tile);

			storedItem = item.gameObject;
			item.isStored = true;
			storedItem.transform.SetParent(this.transform);
			InformationPanel.instance.SetItemText(item.itemName);
			storedItem.GetComponent<MeshRenderer>().enabled = false;		
		}
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
}
