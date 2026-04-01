using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EventSystemController : MonoBehaviour
{
    private static EventSystem reference;
    private void Awake()
    {
        if (reference != null) RemoveEventSystem();
        
        // SceneManager.sceneLoaded += SceneLoaded;
        reference = GetComponent<EventSystem>();
        DontDestroyOnLoad(gameObject);
    }

    private static void SceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
    {
        RemoveEventSystem();
    }

    private static void RemoveEventSystem()
    {
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        foreach (EventSystem system in eventSystems)
        {
            if (system == reference) continue;
            Destroy(system.gameObject);
        }
    }

    private void OnDestroy()
    {
        // SceneManager.sceneLoaded -= SceneLoaded;
    }
}
