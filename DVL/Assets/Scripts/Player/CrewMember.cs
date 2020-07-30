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
		TogglePowerUpUseButton(InformationPanel.instance.powerUpSlot1);
		TogglePowerUpUseButton(InformationPanel.instance.powerUpSlot2);
	}
	private void TogglePowerUpUseButton(PowerUpSlot slot)
    {
		if (slot.storedPowerUp != null)
			if(!slot.storedPowerUp.GetComponent<PowerUpBase>().isInUse)
				slot.powerUpIcon.interactable = true;
    }
	private void DisablePowerUpSlots()
    {
		InformationPanel.instance.powerUpSlot1.powerUpIcon.interactable = false;
		InformationPanel.instance.powerUpSlot2.powerUpIcon.interactable = false;
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
		InformationPanel ui = InformationPanel.instance;
		if (powerUp != null)
        {
			if (ui.powerUpSlot1.storedPowerUp != null)
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.Exchange, ui.powerUpSlot1.powerUpHandleIcon);
				AddPickUpListener(ui.powerUpSlot1.powerUpHandleIcon, ui.powerUpSlot1, powerUp);
			}
            else
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.PickUp, ui.powerUpSlot1.powerUpHandleIcon);
				AddPickUpListener(ui.powerUpSlot1.powerUpHandleIcon, ui.powerUpSlot1, powerUp);
			}			

			if (ui.powerUpSlot2.storedPowerUp != null)
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.Exchange, ui.powerUpSlot2.powerUpHandleIcon);
				AddPickUpListener(ui.powerUpSlot2.powerUpHandleIcon, ui.powerUpSlot2, powerUp);
			}
            else
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.PickUp, ui.powerUpSlot2.powerUpHandleIcon);
				AddPickUpListener(ui.powerUpSlot2.powerUpHandleIcon, ui.powerUpSlot2, powerUp);
			}
		}
        else
        {
			ui.ChangeSlotIcon(PowerUpSlotIcon.None, ui.powerUpSlot1.powerUpHandleIcon);
			ui.ChangeSlotIcon(PowerUpSlotIcon.None, ui.powerUpSlot2.powerUpHandleIcon);
			RemoveAllIconListeners();
		}
    }
	private void AddPickUpListener(Button icon, PowerUpSlot slot, PowerUpBase powerUp)
    {
		icon.onClick.AddListener(() => ExchangePowerUp(slot, powerUp));
	}
	private void RemoveAllIconListeners()
    {
		InformationPanel.instance.powerUpSlot1.powerUpHandleIcon.onClick.RemoveAllListeners();
		InformationPanel.instance.powerUpSlot2.powerUpHandleIcon.onClick.RemoveAllListeners();
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
		InformationPanel.instance.SetItemText(item.itemName);
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
			InformationPanel.instance.SetItemText("none");
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
		deathTurnCounter = 0;
		GetComponent<MeshRenderer>().material.color = Color.yellow;
		Eventbroker.instance.onNotifyNextTurn -= CheckDeathCounter;
	}
    #endregion
}
