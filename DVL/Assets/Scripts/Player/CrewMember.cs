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

			//check for item
			HandlePickUpItem(tile);

			//Check for Generator
			HandleRepairGenerator(tile);

			//Check for escape capsule mod
			HandleDropItem(tile);

			//Check for PowerUps
			HandlePowerUpCollection(tile);
		}
    }

	#region HandlePowerUps
	//TODO needs refractor
	private void HandlePowerUpCollection(Tile tile)
    {
		ChangePowerUpSlotHandleIcon(IsPowerUpPresent(tile));
		TogglePowerUpUseButton(InformationPanel.instance.powerUpSlot1);
		TogglePowerUpUseButton(InformationPanel.instance.powerUpSlot2);
	}
	private void TogglePowerUpUseButton(Button button)
    {
		if (button.GetComponent<PowerUpSlot>().storedPowerUp != null)
			if(!button.GetComponent<PowerUpSlot>().storedPowerUp.GetComponent<PowerUpBase>().isInUse)
				button.interactable = true;
    }
	private void DisablePowerUpSlots()
    {
		InformationPanel.instance.powerUpSlot1.interactable = false;
		InformationPanel.instance.powerUpSlot2.interactable = false;
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
			if (ui.powerUpSlot1.GetComponent<PowerUpSlot>().storedPowerUp != null)
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.Exchange, ui.slotIcon1);
				AddPickUpListener(ui.slotIcon1, ui.powerUpSlot1, powerUp);
			}
            else
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.PickUp, ui.slotIcon1);
				AddPickUpListener(ui.slotIcon1, ui.powerUpSlot1, powerUp);
			}			

			if (ui.powerUpSlot2.GetComponent<PowerUpSlot>().storedPowerUp != null)
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.Exchange, ui.slotIcon2);
				AddPickUpListener(ui.slotIcon2, ui.powerUpSlot2, powerUp);
			}
            else
            {
				ui.ChangeSlotIcon(PowerUpSlotIcon.PickUp, ui.slotIcon2);
				AddPickUpListener(ui.slotIcon2, ui.powerUpSlot2, powerUp);
			}
		}
        else
        {
			ui.ChangeSlotIcon(PowerUpSlotIcon.None, ui.slotIcon1);
			ui.ChangeSlotIcon(PowerUpSlotIcon.None, ui.slotIcon2);
			RemoveAllIconListeners();
		}
    }
	private void AddPickUpListener(Button icon, Button slot, PowerUpBase powerUp)
    {
		icon.onClick.AddListener(() => ExchangePowerUp(slot, powerUp));
	}
	private void RemoveAllIconListeners()
    {
		InformationPanel.instance.slotIcon1.onClick.RemoveAllListeners();
		InformationPanel.instance.slotIcon2.onClick.RemoveAllListeners();
	}
	private void StorePowerUp(Button slot, PowerUpBase powerUp) 
	{
		powerUp.pickedUp = true;
		slot.image.sprite = powerUp.powerUpImage;
		slot.GetComponent<PowerUpSlot>().storedPowerUp = powerUp.powerUpPrefab;
		slot.interactable = true;
		Destroy(powerUp.GetComponent<MeshRenderer>());
		HandlePowerUpCollection(positionTile);
		NetworkClient.instance.SendPowerUpCollected(positionTile);
	}
	private void ExchangePowerUp(Button slot, PowerUpBase powerUp) 
	{
		slot.GetComponent<PowerUpSlot>().DropEverythingInSlot();
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
		RemovePickUpButtonListener();
		RemoveDropItemButtonListener();
		RemoveRepairGeneratorButtonListener();
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
    public void PickUpItem(Item item, Tile tile)
	{
		if(IsLocalPlayer())
			NetworkClient.instance.SendItemCollected(tile);

		RemovePickUpButtonListener();

		storedItem = item.gameObject;
		item.isStored = true;
		storedItem.transform.SetParent(this.transform);
		InformationPanel.instance.SetItemText(item.itemName);
		storedItem.gameObject.SetActive(false);
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
	public void DropItem(EscapeCapsule capsule, Tile tile)
	{
		if (storedItem != null)
		{
			if (capsule != null)
				capsule.DisplayProgress();

			if (IsLocalPlayer())
				NetworkClient.instance.SendItemDropped(tile);

			storedItem.gameObject.SetActive(true);
			storedItem.transform.SetParent(tile.transform);
			storedItem.layer = 8;
			storedItem = null;
			InformationPanel.instance.SetItemText("none");
			RemoveAllEventListeners();
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
            if (!generator.isFinished)
            {
				InformationPanel.instance.SetRepairGeneratorButton(true);
				InformationPanel.instance.repairGeneratorButton.onClick.AddListener(() => RepairGenerator(generator));
			}
		}
		else
		{
			RemoveRepairGeneratorButtonListener();
		}
	}
	private void RepairGenerator(Generator generator)
	{
		RemoveRepairGeneratorButtonListener();
		generator.RepairGenerator(repairSpeed);
		NetworkClient.instance.SendGeneratorRepaired(repairSpeed);
	}
	private void RemoveRepairGeneratorButtonListener()
	{
		InformationPanel.instance.SetRepairGeneratorButton(false);
		InformationPanel.instance.repairGeneratorButton.onClick.RemoveAllListeners();
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
