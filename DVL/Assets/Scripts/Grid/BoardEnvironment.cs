using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEnvironment : MonoBehaviour
{
    public static BoardEnvironment instance;
    private Transform board;
    public List<GameObject> signalPost = new List<GameObject>();

    public LobbyRocket[] rockets;
    public GameObject gameEnv;


    private void Start()
    { 
        instance = this;
        Eventbroker.instance.onChangeGameState += ChangeEnv;
        board = BoardGrid.instance.transform;
    }
    private void OnDisable()
    {
        Eventbroker.instance.onChangeGameState -= ChangeEnv;
    }

    private void ChangeEnv(GameFlowState state)
    {
        if (state == GameFlowState.LOBBY)
        {
            foreach(LobbyRocket rocket in rockets)
            {
                rocket.gameObject.SetActive(true);
                rocket.SetUpRocket(state);
            }
        }
        else if(state == GameFlowState.GAME)
        {
            gameEnv.SetActive(true);
        }
    }

    void Update()
    {
        if(board != null)
        {
            this.transform.position = board.position;
            this.transform.rotation = board.rotation;
            this.transform.localScale = board.localScale;
        }
    }

    public void ActivateNextSignal(int signal)
    {
        signalPost[signal - 1].GetComponent<MeshRenderer>().material.color = Color.green;
        signalPost[signal - 1].GetComponentInChildren<ParticleSystem>().Play();
    }
}
