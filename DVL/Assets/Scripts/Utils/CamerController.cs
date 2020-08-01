using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class CamerController : MonoBehaviour
{
    [Header("Camera Positions")]
    public Transform gameView;
    public Transform lobbyPlayer1;
    public Transform lobbyPlayer2;
    public Transform lobbyPlayer3;
    public Transform lobbyPlayer4;

    #region Unity Methods
    private void OnDisable()
    {
        Eventbroker.instance.onChangeGameState -= PCHandleCamera;
    }
    #endregion
    public void SetUp()
    {
        if (Controller.instance.controllerType == ControllerType.PC)
        {
            Eventbroker.instance.onChangeGameState += PCHandleCamera;
        }
    }
    private void PCHandleCamera(GameFlowState state)
    {
        if (state == GameFlowState.LOBBY)
        {
            Transform target = null;
            switch(LocalGameManager.instance.localPlayerIndex)
            {
                case PlayerIndex.Player1:
                    target = lobbyPlayer1;
                    break;
                case PlayerIndex.Player2:
                    target = lobbyPlayer2;
                    break;
                case PlayerIndex.Player3:
                    target = lobbyPlayer3;
                    break;
                case PlayerIndex.Enemy:
                    target = lobbyPlayer4;
                    break;
                default:
                    Debug.LogError("Lobby Camera Error");
                    break;
            }
            StartCoroutine(CameraRide(target));
        }      
        else if (state == GameFlowState.GAME)
            StartCoroutine(CameraRide(gameView));
    }
    float duration = 4;
    private IEnumerator CameraRide(Transform targetPos)
    {
        var pos1 = transform.position;
        var pos2 = targetPos.position;

        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            transform.rotation = targetPos.rotation;
            yield return null;
        }
        transform.position = pos2;
    }
}
