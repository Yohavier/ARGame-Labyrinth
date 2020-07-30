using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpSlot : MonoBehaviour
{
    public GameObject _storedPowerUp;
    public GameObject storedPowerUp
    {
        get
        {
            return _storedPowerUp;
        }
        set
        {
            _storedPowerUp = value;
            if(_storedPowerUp == null) 
            {
                powerUpIcon.image.color = new Color(1, 1, 1, 0);
            }
            else
            {
                powerUpIcon.image.color = new Color(1, 1, 1, 1);
            }
        }
    }
    public Button powerUpIcon;
    public Button powerUpHandleIcon;

    public void UsePowerUp()
    {
        if (storedPowerUp != null) 
            storedPowerUp.GetComponent<PowerUpBase>().OnUse(LocalGameManager.instance.activePlayer.GetComponent<Player>(), this);
    }

    public void DropEverythingInSlot()
    {
        if(storedPowerUp!= null)
        {
            storedPowerUp.GetComponent<PowerUpBase>().ReverseOnDrop(LocalGameManager.instance.activePlayer.GetComponent<Player>());
            Destroy(storedPowerUp.gameObject);
            powerUpIcon.image.sprite = null;
            powerUpIcon.interactable = false;
            storedPowerUp = null;
        }
    }
}
