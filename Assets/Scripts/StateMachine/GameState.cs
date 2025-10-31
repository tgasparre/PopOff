using UnityEngine;

public class GameState : MonoBehaviour
{
    [SerializeField]
    protected UIHandler uiHandler;
    protected SceneLoader sceneLoader = new SceneLoader();
    
    public virtual void EnterState()
    {
        
    }

    public virtual void EndState()
    {
        return;
    }
    
}
