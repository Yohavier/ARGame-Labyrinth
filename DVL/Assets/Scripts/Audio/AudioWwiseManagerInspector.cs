using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AudioWwiseManager))]
public class AudioWwiseManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AudioWwiseManager myAudioWwiseManger = (AudioWwiseManager)target;
        if (GUILayout.Button("Low Intensity"))
        {
            myAudioWwiseManger.SetMusicIntensity(MusicIntensity.Low);
        }
        if (GUILayout.Button("Medium Intensity"))
        {
            myAudioWwiseManger.SetMusicIntensity(MusicIntensity.Medium);
        }
        if (GUILayout.Button("High Intensity"))
        {
            myAudioWwiseManger.SetMusicIntensity(MusicIntensity.High);
        }
    }
}
#endif
