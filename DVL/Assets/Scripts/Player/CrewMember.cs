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
				storedItem.transform.SetParent(this.transform);
				InformationPanel.playerPanel.SetItemText(item.itemName);
			}
		}
		EscapeCapsule capsule = tile.GetComponent<EscapeCapsule>();
		if (capsule != null)
		{
			capsule.DisplayProgress();
			storedItem.transform.SetParent(tile.transform);
			storedItem = null;
			InformationPanel.playerPanel.SetItemText("none");
		}
	}
}
