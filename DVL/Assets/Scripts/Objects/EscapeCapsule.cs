using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeCapsule : MonoBehaviour
{
    MeshRenderer[] meshes;
    private void Start()
    {
        meshes = GetComponentsInChildren<MeshRenderer>();
    }
    public void DisplayProgress()
    {
        GameManager.instance.CheckWinConditionCrew();
    }

    private void Update()
    {
        foreach(MeshRenderer mesh in meshes)
        {
            mesh.material.color = Color.green;
        }
    }
}
