using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoaderListener : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Awake");
    }

    // called first
    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        if (scene.name == "SampleScene")
        {
            FindFirstObjectByType<UIHandler>().SwitchToPlayingState();
        }
    }

    // called third
    void Start()
    {
        
    }

    // called when the game is terminated
   void OnDisable()
   {
      Debug.Log("OnDisable");
      SceneManager.sceneLoaded -= OnSceneLoaded;
   }
}
    

