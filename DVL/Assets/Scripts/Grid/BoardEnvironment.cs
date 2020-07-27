using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEnvironment : MonoBehaviour
{
    public static BoardEnvironment instance;
    private Transform board;
    public List<GameObject> signalPost = new List<GameObject>();

    private void OnEnable()
    { 
        instance = this;
        if(BoardGrid.instance)
            board = BoardGrid.instance.transform;
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
