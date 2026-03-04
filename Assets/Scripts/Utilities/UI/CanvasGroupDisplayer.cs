using UnityEngine;

public static class CanvasGroupDisplayer
{
    public static void Show(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public static void Hide(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Toggles the canvas group based on the alpha
    /// 0 alpha = show, 1 alpha = hide
    /// </summary>
    /// <param name="canvasGroup"></param>
    public static void Toggle(CanvasGroup canvasGroup)
    {
        if (canvasGroup.alpha == 0) Show(canvasGroup);
        else Hide(canvasGroup);
    }
}
