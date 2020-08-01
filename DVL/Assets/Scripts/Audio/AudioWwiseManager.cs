using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MusicIntensity { Low = 0, Medium = 1, High = 2}
public class AudioWwiseManager : MonoBehaviour
{
    public static AudioWwiseManager instance;

    private void Awake()
    {
        instance = this;
    }
    private void OnEnable()
    {
        Eventbroker.instance.onChangeGameState += SetGameState;
    }
    private void OnDisable()
    {
        
    }
    public static void PostAudio(string file)
    {
        AkSoundEngine.PostEvent(file, instance.gameObject);
    }

    #region Music Management
    public void SetGameState(GameState state)
    {
        switch (state)
        {
            case GameState.JOIN:
                AkSoundEngine.SetState("GameState", "None");
                break;
            case GameState.LOBBY:
                AkSoundEngine.SetState("GameState", "Lobby");
                break;
            case GameState.GAME:
                AkSoundEngine.SetState("GameState", "InGame");
                break;
        }
    }

    public void SetMusicIntensity(MusicIntensity intensity)
    {
        AkSoundEngine.SetRTPCValue("Intensity", (int)intensity * 25);
    }
    #endregion
}
