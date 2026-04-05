using System;
using System.Collections.Generic;
using UnityEngine;

//chatGPT script prompt: 
//I am working in unity to build a game, and I am encountering a bug that only
//exists on builds of the game and not in editor. I have a feeling that it is throwing some
//sort of error that I can't see because builds don't have a log output. Make a quick script
//using Application.logMessageReceivedThreaded and the OnGUI method to listen for logs during
//the build and display the on the screen in the bottom right corner
public class BuildLogger : MonoBehaviour
{
    private readonly Queue<string> logQueue = new Queue<string>();
    private const int maxLogs = 20;

    public static BuildLogger Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        Application.logMessageReceivedThreaded += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceivedThreaded -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string formattedLog = $"[{type}] {logString}";

        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            formattedLog += $"\n{stackTrace}";
        }

        lock (logQueue)
        {
            logQueue.Enqueue(formattedLog);

            if (logQueue.Count > maxLogs)
            {
                logQueue.Dequeue();
            }
        }
    }

    private void OnGUI()
    {
        const int width = 500;
        const int height = 300;
        const int padding = 10;

        Rect rect = new Rect(
            Screen.width - width - padding,
            Screen.height - height - padding,
            width,
            height
        );

        GUILayout.BeginArea(rect, GUI.skin.box);

        lock (logQueue)
        {
            foreach (var log in logQueue)
            {
                GUILayout.Label(log);
            }
        }

        GUILayout.EndArea();
    }
}
