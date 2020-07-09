using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { None, Lobby, InGame}
public enum MusicIntensity { Low = 0, Medium = 1, High = 2}
public class AudioWwiseManager : MonoBehaviour
{
    public static AudioWwiseManager instance;

    private void Awake()
    {
        instance = this;
        SetMusicGameState(GameState.None);
    }

    public static void PostAudio(string file)
    {
        AkSoundEngine.PostEvent(file, instance.gameObject);
    }

    #region Music Management
    public void SetMusicGameState(GameState gamestate)
    {
        switch (gamestate)
        {
            case GameState.Lobby:
                AkSoundEngine.SetState("GameState", "Lobby");
                break;
            case GameState.InGame:
                AkSoundEngine.SetState("GameState", "InGame");
                break;
            case GameState.None:
                AkSoundEngine.SetState("GameState", "None");
                break;
        }
    }

    public void SetMusicIntensity(MusicIntensity intensity)
    {
        AkSoundEngine.SetRTPCValue("Intensity", (int)intensity * 25);
    }
    #endregion
}
