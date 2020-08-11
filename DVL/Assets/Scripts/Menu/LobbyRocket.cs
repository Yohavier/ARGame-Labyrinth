using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class LobbyRocket : MonoBehaviour
{
    public ParticleSystem rocketBlaster;
    public PlayerIndex index;

    private LobbyGate gate;
    private LobbyCharacter character;
    public GameObject mark;
    private void OnDisable()
    {
        StopAllCoroutines();
        Eventbroker.instance.onChangeGameState -= StartBooster;
        Eventbroker.instance.onChangeCharacter -= character.ChangeSelectedCharacter;
        Eventbroker.instance.onToggleGate -= gate.OnToggleGate;
    }

    public void SetUpRocket(GameState state)
    {
        gate = GetComponentInChildren<LobbyGate>();
        character = GetComponentInChildren<LobbyCharacter>();
        Eventbroker.instance.onChangeGameState += StartBooster;
        if (state == GameState.LOBBY)
        {
            if(index == GameManager.instance.localPlayerIndex)
            {
                Eventbroker.instance.onChangeCharacter += character.ChangeSelectedCharacter;
                Eventbroker.instance.onToggleGate += gate.OnToggleGate;
                mark.SetActive(true);
            }
            else
            {
                gate.GetComponentInChildren<Canvas>().gameObject.SetActive(false);         
            }
        }
    }
    public void StartBooster(GameState state)
    {
        if(state == GameState.GAME)
        {         
            StartCoroutine(Boost());
        }

    }
    private IEnumerator Boost()
    {
        mark.SetActive(false);
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f,6f));
        AkSoundEngine.PostEvent("lobby_rocket", gameObject);
        rocketBlaster.Play();
        float a;
        float t;
        float distance;
        bool loop = true;
        a = 0;
        t = 0;

        while (loop)
        {
            a += t;

            transform.position += rocketBlaster.transform.up * a * 0.01f;
            t += Time.deltaTime * 0.01f;
            distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            if (distance > 50)
            {
                loop = false;
            }
            yield return null;
        }
        this.gameObject.SetActive(false);
    }
}
