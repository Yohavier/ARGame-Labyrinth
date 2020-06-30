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
			HandlePowerUps(tile);
		}
    }

	#region HandlePowerUps
	private void HandlePowerUps(Tile tile)
    {
        if (tile)
        {
			PowerUpBase powerUP = tile.GetComponentInChildren<PowerUpBase>();
			if (powerUP)
			{
                if (!powerUP.pickedUp)
                {
					NetworkClient.instance.SendPowerUpCollected(tile);
					var freeSlot = CanCollectPowerUp();
					if (freeSlot != null)
					{
						StorePowerUp(freeSlot, powerUP);
					}
				}
			}
		}
    }
	private Button CanCollectPowerUp()
    {
		var ui = InformationPanel.instance;
		if(ui.powerUpSlot1.GetComponent<PowerUpSlot>().storedPowerUp == null)
        {
			return ui.powerUpSlot1;
        }
		else if (ui.powerUpSlot2.GetComponent<PowerUpSlot>().storedPowerUp == null)
        {
			return ui.powerUpSlot2;
		}
		else
        {
			return null;
        }
    }

	private void StorePowerUp(Button freeSlot, PowerUpBase powerUp)
    {
        if (freeSlot.GetComponent<PowerUpSlot>().storedPowerUp != null)
        {
			freeSlot.GetComponent<PowerUpSlot>().DropEverythingInSlot();
        }

		powerUp.pickedUp = true;
		freeSlot.image.sprite = powerUp.powerUpImage;
		freeSlot.GetComponent<PowerUpSlot>().storedPowerUp = powerUp.powerUpPrefab;
		freeSlot.interactable = true;
		Destroy(powerUp.GetComponent<MeshRenderer>());
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
        }
    }
	private void RemoveAllEventListeners()
    {
		RemovePickUpButtonListener();
		RemoveDropItemButtonListener();
		RemoveRepairGeneratorButtonListener();
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

	public void GetHealedByOtherPlayer()
    {
		deathTurnCounter = 0;
		playerState = PlayerState.ALIVE;
		GetComponent<MeshRenderer>().material.color = Color.yellow;
		Eventbroker.instance.onNotifyNextTurn -= CheckDeathCounter;
	}
    #endregion
}
