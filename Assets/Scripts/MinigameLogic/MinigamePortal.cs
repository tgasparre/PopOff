using UnityEngine;
using UnityEngine.InputSystem;

public class MinigamePortal : MonoBehaviour
{
    public Game game;
    private bool isPlayerInRange;

    public void Update()
    {
        if (isPlayerInRange && Keyboard.current.enterKey.wasPressedThisFrame)
        {
            game.TriggerMinigame();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("collision with portal entered");
        isPlayerInRange = true;
    }
    
    //start code from chatgpt: i want some action to occur when the player is close enough
    //to an object and presses the enter button. How would i write a method to do that?
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
    //end code from chatgpt
}
