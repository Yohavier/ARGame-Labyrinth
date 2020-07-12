using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharInfo : MonoBehaviour
{
    public Text infoText;
    public Text roleText;
    public Text movementText;
    public Text tracetadiusText;
    public Text visiontadiusText;
    public Text repairSpeedText;
    public Text healthText;

    public void DisplayNewInfo(SO_PlayerClass role)
    {
        SetInfoText(role.roleInfoText);
        SetRoleText(role.roleName);
        SetMovementText(role.diceModificator.ToString());
        SetTraceradiusText(role.footstepDetectionRadius.ToString());
        SetVisionradiusText(role.fogOfWarRadius.ToString());
        SetRepairSpeedText(role.repairSpeed);
        SetHealthText(role.maxDeathTurnCounter);
    }

    private void SetInfoText(string txt)
    {
        infoText.text = txt;
    }
    private void SetRoleText(string txt)
    {
        roleText.text = txt;
    }
    private void SetMovementText(string txt)
    {
        movementText.text = "Move Buff: " + txt;
    }
    private void SetTraceradiusText(string txt)
    {
        tracetadiusText.text = "Traceradius: " + txt;
    }
    private void SetVisionradiusText(string txt)
    {
        visiontadiusText.text = "Visionradius: " + txt;
    }
    private void SetRepairSpeedText(int txt)
    {
        if (txt == 0)
            repairSpeedText.text = "";
        else
            repairSpeedText.text = "Repairspeed: " + txt.ToString();
    }
    private void SetHealthText(int txt)
    {
        if (txt > 50)
            healthText.text = "Health: Immortal";
        else
            healthText.text = "Health: " + txt.ToString();
    }
}
