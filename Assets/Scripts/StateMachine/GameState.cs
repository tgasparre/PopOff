using UnityEngine;

public abstract class GameState
{
    protected static ISceneLoader Loader => Game.Instance.SceneLoader;
    
    public abstract void EnterState();
    public abstract void ExitState();

    /// <summary>
    /// Test to see if the state can be switched into
    /// </summary>
    /// <param name="test">State to test</param>
    /// <returns></returns>
    public abstract bool IsStateSwitchable(GameStates test);
}
