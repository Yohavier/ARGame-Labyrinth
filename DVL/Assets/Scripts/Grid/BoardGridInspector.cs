using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if Unity_Editor
using UnityEditor;un

[CustomEditor(typeof(BoardGrid))]
public class BoardGridInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BoardGrid myBoardGrid = (BoardGrid)target;
        if(GUILayout.Button("Shuffle Board"))
        {
            myBoardGrid.ShuffleBoard();
        }
    }
}
#endif