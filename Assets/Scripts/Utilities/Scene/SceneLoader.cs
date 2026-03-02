using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;

public interface ISceneLoader
{
    public void LoadCombatScene(Action sceneLoaded = null);
    public void LoadMenuScene(Action sceneLoaded = null);
    public void LoadStartingMiniGameScene(Action sceneLoaded = null, Action transitionCompleted = null);
    public void LoadMiniGameScene(Action sceneLoaded = null, Action transitionCompleted = null);
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
        Game.IsFrozen = true;
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
        return;
        
        IEnumerator LoadScene(string n, Action completed = null)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(n);
            asyncLoad.allowSceneActivation = false;
            yield return new WaitUntil(() => canLoadScene);
            asyncLoad.allowSceneActivation = true;
            yield return new WaitUntil(() => asyncLoad.isDone);
        
            completed?.Invoke();
        }
    }

    public void LoadCombatScene(Action sceneLoaded = null)
    {
        LoadSceneTransition(_combatScene, sceneLoaded);
    }
    
    public void LoadMenuScene(Action sceneLoaded = null)
    {
        LoadSceneTransition(_menuScene, sceneLoaded);
    }

    public void LoadStartingMiniGameScene(Action sceneLoaded = null, Action transitionCompleted = null)
    {
        LoadSceneTransition(_startingMiniGame, sceneLoaded, transitionCompleted);
    }

    public void LoadMiniGameScene(Action sceneLoaded = null, Action transitionCompleted = null)
    {
        LoadSceneTransition(PickMiniGame(), sceneLoaded, transitionCompleted, transitionType: TransitionType.MiniGame);
    }

    private static void SetSceneSettings()
    {
        Game.IsFrozen = false;
        Game.ActivePlayerTracker.SpawnPlayers();
    }

    private SceneReference PickMiniGame()
    {
        return _miniGameScenes[0];
    }
}

public enum SceneType
{
    Menu,
    Game,
    MiniGame
}
