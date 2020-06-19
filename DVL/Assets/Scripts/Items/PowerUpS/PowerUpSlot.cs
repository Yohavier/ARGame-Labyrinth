using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpSlot : MonoBehaviour
{
    public GameObject storedPowerUp;

    public void UsePowerUp()
    {
        var a = Instantiate(storedPowerUp);
        a.GetComponent<PowerUpBase>().OnUse();

        ResetPowerUpSlot();
    }

    private void ResetPowerUpSlot()
    {
        storedPowerUp = null;
        GetComponent<Button>().interactable = false;
        GetComponent<Image>().sprite = null;
    }
}
