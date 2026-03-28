using UnityEngine;

public class SlipperyMiniGame : MiniGameInfo
{
    public TiltingPlatform MainPlatform;
    [Space]
    [SerializeField] private GameObject startingPlatformParent;
    
    protected override void StartMiniGame()
    {
        startingPlatformParent.SetActive(false);
        MainPlatform.StartTiltingPlatform(MiniGameTime);
    }

    protected override void OnEndMiniGame()
    {
        MainPlatform.EndGame();
    }
}
