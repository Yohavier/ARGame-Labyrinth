using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class DebugConsole : MonoBehaviour
{
    public static DebugConsole instance;
    List<string> log = new List<string>();
    float lastLogTime = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        enabled = GUIManager.instance.isDebug;
    }

    void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        log.Add(logString);
        lastLogTime = 0;
    }

    private void OnGUI()
    {
        lastLogTime += Time.deltaTime;
        if (lastLogTime >= 3)
            log.Clear();

        if (log.Count == 0)
            return;

        for (int i = 0; i < log.Count; i++)
        {
            GUILayout.Label(log[i]);
        }
    }
}
