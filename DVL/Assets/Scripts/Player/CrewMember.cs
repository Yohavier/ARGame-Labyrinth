using UnityEngine;
using UnityEngine.UI;

public class CrewMember : Player
{
	private int deathTurnCounter = 0;
	//Cant move to next Tile if there is already a player instance inside
	public override bool CheckForOtherPlayers(Tile nextTile)
	{
		Player player = nextTile.GetComponentInChildren<Player>();
		if ( player != null)
		{
			if (player.playerState == PlayerState.ALIVE)
            {
				return false;
			}
		}
		return true;
	}
	public override void CheckTileForOtherMods(Tile tile)
	{
		if(playerState == PlayerState.ALIVE)
        {
			//Check for doors
			base.CheckTileForOtherMods(tile);

			//Check for escape capsule mod
			HandleDropItem(tile);		
		}
		if(playerState != PlayerState.DEAD)
        {
			//Check for PowerUps
			HandlePowerUpCollection(tile);
		}
    }

	//TODO Rework PowerUp Handling
	#region HandlePowerUps
	private void HandlePowerUpCollection(Tile tile)
    {
		ChangePowerUpSlotHandleIcon(IsPowerUpPresent(tile));
		TogglePowerUpUseButton(GUIManager.instance.slot1);
		TogglePowerUpUseButton(GUIManager.instance.slot2);
	}
	private void TogglePowerUpUseButton(PowerUpSlot slot)
    {
		if (slot.storedPowerUp != null)
			if(!slot.storedPowerUp.GetComponent<PowerUpBase>().isInUse)
				slot.powerUpIcon.interactable = true;
    }
	private void DisablePowerUpSlots()
    {
		GUIManager.instance.slot1.powerUpIcon.interactable = false;
		GUIManager.instance.slot2.powerUpIcon.interactable = false;
	}
	private PowerUpBase IsPowerUpPresent(Tile tile)
    {
		PowerUpBase powerUp = tile.GetComponentInChildren<PowerUpBase>();
		if (powerUp && !powerUp.pickedUp)
			return powerUp;
		else
			return null;
    }
	private void ChangePowerUpSlotHandleIcon(PowerUpBase powerUp)
	{
		GUIManager ui = GUIManager.instance;
		if (powerUp != null)
        {
			if (ui.slot1.storedPowerUp != null)
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.Exchange, ui.slot1.powerUpHandleIcon);
				AddPickUpListener(ui.slot1.powerUpHandleIcon, ui.slot1, powerUp);
			}
            else
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.PickUp, ui.slot1.powerUpHandleIcon);
				AddPickUpListener(ui.slot1.powerUpHandleIcon, ui.slot1, powerUp);
			}			

			if (ui.slot2.storedPowerUp != null)
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.Exchange, ui.slot2.powerUpHandleIcon);
				AddPickUpListener(ui.slot2.powerUpHandleIcon, ui.slot2, powerUp);
			}
            else
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.PickUp, ui.slot2.powerUpHandleIcon);
				AddPickUpListener(ui.slot2.powerUpHandleIcon, ui.slot2, powerUp);
			}
		}
        else
        {
			ui.ChangeSlotIcon(PowerUpSlotIcon.None, ui.slot1.powerUpHandleIcon);
			ui.ChangeSlotIcon(PowerUpSlotIcon.None, ui.slot2.powerUpHandleIcon);
			RemoveAllIconListeners();
		}
    }
	private void AddPickUpListener(Button icon, PowerUpSlot slot, PowerUpBase powerUp)
    {
		icon.onClick.AddListener(() => ExchangePowerUp(slot, powerUp));
	}
	private void RemoveAllIconListeners()
    {
		GUIManager.instance.slot1.powerUpHandleIcon.onClick.RemoveAllListeners();
		GUIManager.instance.slot2.powerUpHandleIcon.onClick.RemoveAllListeners();
	}
	private void StorePowerUp(PowerUpSlot slot, PowerUpBase powerUp) 
	{
		AkSoundEngine.PostEvent("powerUp_pickUp", this.gameObject);
		powerUp.pickedUp = true;
		slot.powerUpIcon.image.sprite = powerUp.powerUpImage;
		slot.storedPowerUp = powerUp.powerUpPrefab;
		slot.powerUpIcon.interactable = true;

		MeshRenderer[] meshes = powerUp.GetComponentsInChildren<MeshRenderer>();
		foreach(MeshRenderer mesh in meshes)
        {
			Destroy(mesh);
        }
		HandlePowerUpCollection(positionTile);
		NetworkClient.instance.SendPowerUpCollected(positionTile);
	}
	private void ExchangePowerUp(PowerUpSlot slot, PowerUpBase powerUp) 
	{
		slot.DropEverythingInSlot();
		StorePowerUp(slot, powerUp);
	}
	#endregion

	#region HandleNextTurn
	public override void NotifyNextTurn(bool toggle)
    {
        if(toggle)
        {
			CheckTileForOtherMods(positionTile);
        }
        else
        {
			RemoveAllEventListeners();
			DisablePowerUpSlots();
        }
    }
	private void RemoveAllEventListeners()
    {
		RemoveAllIconListeners();
    }
    #endregion

    #region Handle Dying
    protected override void Dead()
    {
		Debug.Log(playerIndex + " died");
		GameManager.instance.CheckWinConditionMonster();
		GetComponent<MeshRenderer>().material.color = Color.black;
		Eventbroker.instance.onNotifyNextTurn -= CheckDeathCounter;
		if (storedItem != null)
			DropItem(null, positionTile);
    }
    protected override void Dying()
    {
		Debug.Log(playerIndex + " is dying");
		Eventbroker.instance.onNotifyNextTurn += CheckDeathCounter;
    }
	protected override void CheckDeathCounter()
    {
		deathTurnCounter++;
		if(deathTurnCounter >= maxDeathTurnCounter)
        {
			playerState = PlayerState.DEAD;
        }
		else
			Debug.Log(playerIndex + " is dying " + deathTurnCounter + "/" + maxDeathTurnCounter);
	}
    #endregion

    #region Handle Pickup Item extension
    public void PickUpItem(Item item, Tile tile)
	{
		if(IsLocalPlayer())
			NetworkClient.instance.SendItemCollected(tile);

		if (!tile.isInFOW)
			AkSoundEngine.PostEvent("powerUp_pickUp", gameObject);

		storedItem = item.gameObject;
		item.isStored = true;
		storedItem.transform.SetParent(this.transform);
		GUIManager.instance.SetItemText(item.itemName);
		storedItem.gameObject.SetActive(false);
	}
    #endregion

    #region Handle Drop Item extension
	private void HandleDropItem(Tile tile)
    {
		EscapeCapsule capsule = tile.GetComponent<EscapeCapsule>();
		if (capsule != null && storedItem != null)
		{
			DropItem(capsule, tile);
		}
	}
	public void DropItem(EscapeCapsule capsule, Tile tile)
	{
		if (storedItem != null)
		{
			if (capsule != null)
				capsule.DisplayProgress();

			if (IsLocalPlayer())
            {
				NetworkClient.instance.SendItemDropped(tile);
			}

			if(!tile.isInFOW)
				AkSoundEngine.PostEvent("item_drop", gameObject);

			storedItem.gameObject.SetActive(true);
			storedItem.transform.SetParent(tile.transform);
			storedItem.layer = 8;
			storedItem = null;
			GUIManager.instance.SetItemText("none");
		}
	}
	#endregion

	#region Handle Repair Generator extension
	public void RepairGenerator(Generator generator)
	{
		generator.RepairGenerator(repairSpeed);
		NetworkClient.instance.SendGeneratorRepaired(repairSpeed);
	}
    #endregion

    #region Handle Heal Player extension

	public void GetHealed()
    {
		playerState = PlayerState.ALIVE;
		anim.SetTrigger("heal");
		deathTurnCounter = 0;
		GetComponent<MeshRenderer>().material.color = Color.yellow;
		Eventbroker.instance.onNotifyNextTurn -= CheckDeathCounter;
	}
    #endregion
}
