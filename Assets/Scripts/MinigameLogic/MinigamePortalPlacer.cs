using System;
using UnityEngine;

public class MiniGamePortalPlacer : ObjectPlacer
{
   public Game portalGame;
   public void Start()
   {
      TagToClean = "Portal";
      minimumSecondsToCreate = 10f;
      maximumSecondsToCreate = 30f;
   }

   public override void Place()
   {
      Debug.Log("placed object");
      Vector3 position = SpriteTools.RandomTopOfScreenLocationWorldSpace();
      Instantiate(ObjectPrefab, position, Quaternion.identity);
      ObjectPrefab.GetComponent<MinigamePortal>().game = portalGame;
   }
}
