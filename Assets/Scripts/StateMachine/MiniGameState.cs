using UnityEngine;
public class MiniGameState : GameState
{
    public override void EnterState()
    {
        Time.timeScale = 0f;
        if (PlayingState.IsStarting) //intro minigame
        {
            //TODO -- play introduction animation
            Loader.LoadStartingMiniGameScene();
        } 
        else //all other minigames
        {
            //TODO -- Play little animation
            Loader.LoadMiniGameScene();
        }
    }

    public override void ExitState()
    {
        
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        throw new System.NotSupportedException();
    }
}

/*
private List<string> minigamesList = new List<string>();

   void Awake()
   {
       PopulateMinigameList();
   }
   
   public override void EnterState()
   {
       StartRandomMiniGame();
       // uiHandler.SwitchToPlayingState();
   }

   public override void ExitState()
   {
       // sceneLoader.LoadScene("SampleScene");
   }

   private void StartRandomMiniGame()
   {
       int i = Random.Range(0, minigamesList.Count);
       // StartCoroutine(sceneLoader.LoadScene(minigamesList[i]));
   }

   private void PopulateMinigameList()
   {
       //start code from chatgpt, prompt: im making a game that has multiple minigames.
       //every minigame is its own scene. how can i make a list of all the minigame scenes while ignoring the scene of the main game?
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

   public override bool IsStateSwitchable(GameStates test)
   {
       return false;
       // return test is GameStates.Game or GameStates.Pause;
   }
*/