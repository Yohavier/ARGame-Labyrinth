using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCalls : MonoBehaviour
{
    public static AudioCalls instance;
    private void Awake()
    {
        instance = this;
    }

    public static void PostAudio(string file)
    {
        AkSoundEngine.PostEvent(file, instance.gameObject);
    }
}
