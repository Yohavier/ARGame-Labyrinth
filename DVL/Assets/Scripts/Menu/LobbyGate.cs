using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGate : MonoBehaviour
{
    public AnimationClip closeGate;
    public AnimationClip openGate;
    private Animation anim;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        GetComponentInChildren<Canvas>().worldCamera = Camera.main;
    }

    public void InitCharSelection()
    {
        Eventbroker.instance.ToggleGate(true);
        GUIManager.instance.OnPlayerRoleChanged(0);
        HandleButton();
    }

    public void OnToggleGate(bool toggle)
    {
        if (toggle)
        {
            AkSoundEngine.PostEvent("gate_open", gameObject);
            anim.clip = openGate;
        }
        else
        {
            AkSoundEngine.PostEvent("gate_close", gameObject);
            anim.clip = closeGate;
        }
        anim.Play();
    }

    private void HandleButton()
    {
        GetComponentInChildren<Button>().gameObject.SetActive(false);
        GUIManager.instance.readyToggle.interactable = true;
    }
}
