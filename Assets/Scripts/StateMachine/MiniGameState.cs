using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MiniGameState : GameState
{
    private List<string> minigamesList = new List<string>();
    private SceneLoader sceneLoader = new SceneLoader();

    void Awake()
    {
        PopulateMinigameList();
    }
    
    public override void EnterState()
    {
        StartRandomMiniGame();
    }

    private void StartRandomMiniGame()
    {
        int i = Random.Range(0, minigamesList.Count);
        StartCoroutine(sceneLoader.LoadScene(minigamesList[i]));
    }

    private void PopulateMinigameList()
    {
        //start code from chatgpt, prompt: im making a game that has multiple minigames. every minigame is its own scene. how can i make a list of all the minigame scenes while ignoring the scene of the main game?
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneName.StartsWith("Minigame_"))
                minigamesList.Add(sceneName);
        }
        //end code from chatgpt
    }
    
    
    
    
}
