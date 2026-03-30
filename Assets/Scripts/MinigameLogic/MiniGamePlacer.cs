using System;
using UnityEngine;

public class MiniGamePlacer : ObjectPlacer
{
    protected override void StartPlacing()
    {
        throw new NotImplementedException();
    }
}

// public Game portalGame;
// public void Start()
// {
//     TagToClean = "Portal";
//     minimumSecondsToCreate = 10f;
//     maximumSecondsToCreate = 30f;
// }
//
// public override void Place()
// {
//     Debug.Log("placed object");
//     Vector3 position = SpriteTools.RandomTopOfScreenLocationWorldSpace();
//     Instantiate(ObjectPrefab, position, Quaternion.identity);
//     ObjectPrefab.GetComponent<MinigamePortal>().game = portalGame;
// }