using UnityEngine;

public class GameState : MonoBehaviour
{
    //makes it show up in the unity editor, from chatGPT
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
