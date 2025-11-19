using UnityEngine;

public static class PluginsSpriteTools 
{
    public static bool IsOffScreen(SpriteRenderer spriteRenderer)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(spriteRenderer.transform.position);
        if (screenPosition.x > Screen.width || screenPosition.x < 0 || screenPosition.y > Screen.height ||
            screenPosition.y < 0)
        {
            return true;
        }
        return false;
    }
}
