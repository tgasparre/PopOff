using System;
using UnityEngine;

public class StartButton : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        //TODO 
        //countdown before start

        Game.currentState = GameStates.Playing;
    }
}
