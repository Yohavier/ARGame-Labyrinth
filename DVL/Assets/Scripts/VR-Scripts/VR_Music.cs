using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Music : MonoBehaviour
{
    private void Start()
    {
        AkSoundEngine.SetState("GameState", "InGame");
        AkSoundEngine.SetRTPCValue("intensity", 75);
    }
}
