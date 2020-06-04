using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewMember : Player
{
	public override bool checkForOtherPlayers(Tile nextTile)
	{
		if (nextTile.GetComponentInChildren<Player>() != null)
		{
			return false;
		}
		return true;
	}

	public override void CheckTileForOtherMods(Tile tile)
	{
		if (storedItem == null)
		{
			var item = tile.GetComponentInChildren<Item>();
			if (item != null)
			{
				storedItem = item.gameObject;
<<<<<<< HEAD
=======
				item.isStored = true;
>>>>>>> Jonas
				storedItem.transform.SetParent(this.transform);
				InformationPanel.playerPanel.SetItemText(item.itemName);
				storedItem.GetComponent<MeshRenderer>().enabled = false;
				NetworkClient.instance.SendItemCollected(tile);
			}
		}

		EscapeCapsule capsule = tile.GetComponent<EscapeCapsule>();
		if (capsule != null)
		{
			if(storedItem != null)
			{
				capsule.DisplayProgress();
				storedItem.transform.SetParent(tile.transform);
				storedItem.GetComponent<MeshRenderer>().enabled = true;
				storedItem = null;
				InformationPanel.playerPanel.SetItemText("none");
			}
		}
	}
}
