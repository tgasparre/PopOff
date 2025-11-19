using UnityEngine;
using UnityEngine.InputSystem;

public class MinigameEnder : MonoBehaviour
{
    public Game game;
    private bool isPlayerInRange;
    
    public void Update()
    {
        if (isPlayerInRange && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            game.EndMinigame();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collided with minigame ender");
        isPlayerInRange = true;
    }
    
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
