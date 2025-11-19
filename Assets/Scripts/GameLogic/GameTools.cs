using UnityEngine;

public class GameTools 
{
    public bool gameHasStarted = false;

    public bool HasGameStarted()
    {
        return gameHasStarted;
    }

    public void SetGameStarted(bool gameStarted)
    {
        gameHasStarted = gameStarted;
    }
    
}
