using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;

public interface ISceneLoader
{
    public void LoadCombatScene(Action sceneLoaded = null);
    public void LoadMenuScene(Action sceneLoaded = null);
    public void LoadStartingMiniGameScene(Action sceneLoaded = null);
    public void LoadMiniGameScene(Action sceneLoaded = null);
}
public class SceneLoader : MonoBehaviour, ISceneLoader 
{
    [Header("Scenes")]
    [SerializeField] private SceneReference _menuScene;
    [SerializeField] private SceneReference _combatScene;
    [SerializeField] private SceneReference _startingMiniGame;
    [SerializeField] private SceneReference[] _miniGameScenes;

    public bool canLoadScene { get; private set; }  = true;

    private void LoadSceneTransition(SceneReference scene, Action sceneLoaded = null, Action transitionCompleted = null, TransitionType transitionType = TransitionType.Scene)
    {
        canLoadScene = false;
        StartCoroutine(LoadScene(scene, () =>
        {
            SetSceneSettings();
            sceneLoaded?.Invoke();
        }));
        
        GameCanvas.Instance.Transition(transitionType, () =>
        {
            canLoadScene = true;
            transitionCompleted?.Invoke();
        });
    }
    private IEnumerator LoadScene(string n, Action completed = null)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(n);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitUntil(() => canLoadScene);
        asyncLoad.allowSceneActivation = true;
        yield return new WaitUntil(() => asyncLoad.isDone);
        
        completed?.Invoke();
    }

    public void LoadCombatScene(Action sceneLoaded = null)
    {
        LoadSceneTransition(_combatScene, sceneLoaded);
    }
    
    public void LoadMenuScene(Action sceneLoaded = null)
    {
        LoadSceneTransition(_menuScene, sceneLoaded);
    }

    public void LoadStartingMiniGameScene(Action sceneLoaded = null)
    {
        LoadSceneTransition(_startingMiniGame, sceneLoaded);
    }

    public void LoadMiniGameScene(Action sceneLoaded = null)
    {
        LoadSceneTransition(PickMiniGame(), sceneLoaded, transitionType: TransitionType.MiniGame);
    }

    private static void SetSceneSettings()
    {
        Time.timeScale = 1f;
        Game.Instance.SpawnPlayers();
    }

    private SceneReference PickMiniGame()
    {
        return _miniGameScenes[0];
    }
    
    private void InstantLoadScene(string n)
    {
        SceneManager.LoadScene(n);
    }
    
    // private bool gameHasStarted = false;
    //
    // public bool HasGameStarted()
    // {
    //     return gameHasStarted;
    // }
    //
    // public void SetGameStarted(bool hasStarted)
    // {
    //     gameHasStarted = hasStarted;
    // }
    //
    // //start code from chatgpt, prompt: how do you switch between scenes in unity using code?
    // public IEnumerator LoadScene(string scene)
    // {
    //     Debug.Log("Loading scene " + scene);
    //     AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);
    //     while (!asyncLoad.isDone)
    //     {
    //         yield return null;
    //     }
    // }
    // //end code from chatgpt
    //
    // //checks if the active scene is a minigame
    // public bool IsInMinigameScene()
    // {
    //     if (SceneManager.GetActiveScene().name.StartsWith("Minigame_"))
    //     {
    //         return true; 
    //     }
    //     return false;
    // }
    //
    // public void InstantLoadScene(string scene)
    // {
    //     SceneManager.LoadScene(scene);
    // }
}

public enum SceneType
{
    Menu,
    Game,
    MiniGame
}
