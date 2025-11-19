using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader 
{
    private bool gameHasStarted = false;

    public bool HasGameStarted()
    {
        return gameHasStarted;
    }

    public void SetGameStarted(bool hasStarted)
    {
        gameHasStarted = hasStarted;
    }
    
    //start code from chatgpt, prompt: how do you switch between scenes in unity using code?
    public IEnumerator LoadScene(string scene)
    {
        Debug.Log("Loading scene " + scene);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    //end code from chatgpt
    
    //checks if the active scene is a minigame
    public bool IsInMinigameScene()
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Minigame_"))
        {
            return true; 
        }
        return false;
    }

    public void InstantLoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
