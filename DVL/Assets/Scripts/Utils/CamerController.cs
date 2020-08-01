using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerController : MonoBehaviour
{
    [Header("Camera Positions")]
    public float camerRideDuration = 2;
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
    private IEnumerator CameraRide(Transform targetPos)
    {
        var pos1 = transform.position;
        var pos2 = targetPos.position;
        Vector3 to = targetPos.localEulerAngles;
        bool rotating = true;
        for (float t = 0f; t < camerRideDuration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(pos1, pos2, t / camerRideDuration);

            if (rotating)
            {
                if (Vector3.Distance(transform.eulerAngles, to) > 0.01f)
                {
                    transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, to, t/camerRideDuration);
                }
                else
                {
                    transform.eulerAngles = to;
                    rotating = false;
                }
            }
            yield return null;
        }
        transform.position = pos2;
    }
}
