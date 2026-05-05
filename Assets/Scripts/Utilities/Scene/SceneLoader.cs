using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Tymski;
using UnityEngine.InputSystem;
using Object = System.Object;
using Random = UnityEngine.Random;

public interface ISceneLoader
{
    public void LoadCombatScene(Action sceneLoaded = null);
    public void LoadMenuScene(Action sceneLoaded = null);
    public void LoadStartingMiniGameScene(Action sceneLoaded = null, Action transitionCompleted = null);
    public void LoadMiniGameScene(Action sceneLoaded = null, Action transitionCompleted = null);
}
public class SceneLoader : MonoBehaviour, ISceneLoader 
{
    public static readonly bool IEEE_BUILD = false;
    private int IEEE_index = 0;

    private void Awake()
    {
        IEEE_index = 0;
        ObjectPlacer.IsFrozen = false;
    }

    [Header("Scenes")]
    [SerializeField] private SceneReference _menuScene;
    [SerializeField] private SceneReference _combatScene;
    [SerializeField] private SceneReference _startingMiniGame;
    [SerializeField] private SceneReference[] _miniGameScenes;
    
    private List<int> _unplayedMiniGames = new List<int>();

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
        ObjectPlacer.IsFrozen = false;
        
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
        Game.ActivePlayerTracker.SpawnAllPlayers();
    }

    private SceneReference PickMiniGame()
    {
        if (IEEE_BUILD)
        {
            if (IEEE_index == 3) ObjectPlacer.IsFrozen = true;
            return _miniGameScenes[(IEEE_index++)%_miniGameScenes.Length];
        }
        
        if (_unplayedMiniGames.Count == 0) { _unplayedMiniGames = Enumerable.Range(0, _miniGameScenes.Length).ToList(); }
        int minigameToPlay = _unplayedMiniGames[Random.Range(0, _unplayedMiniGames.Count)];
        _unplayedMiniGames.Remove(minigameToPlay);
        
        return _miniGameScenes[minigameToPlay];
    }

    // private void Update()
    // {
    //     if (Keyboard.current.leftShiftKey.isPressed && Keyboard.current.pKey.wasPressedThisFrame)
    //     {
    //         if (PlayingState.CurrentGameplayState == GameplayStates.MiniGame)
    //         {
    //             MiniGameInfo.Instance.TriggerEndMiniGame(0);
    //         }
    //     }
    // }
}

public enum SceneType
{
    Menu,
    Game,
    MiniGame
}
