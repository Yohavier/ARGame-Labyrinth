using Assets.Scripts.GameManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGate : MonoBehaviour
{
    public void StartAnimation()
    {
        GetComponent<Animation>().Play();
        InformationPanel.instance.OnPlayerRoleChanged(0);
        HandleButton();
    }

    private void HandleButton()
    {
        GetComponentInChildren<Button>().gameObject.SetActive(false);
        SwipeManager.instance.canSwipe = true;
        GUIManager.instance.readyToggle.interactable = true;
        GUIManager.instance.startMatchButton.interactable = true;
    }
}
