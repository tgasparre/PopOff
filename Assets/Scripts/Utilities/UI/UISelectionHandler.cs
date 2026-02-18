using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Util for navigating the UI with a controller, when changing menus this script tells the EventSystem which
/// new button to select 
/// </summary>
public class UISelectionHandler : MonoBehaviour
{
    [SerializeField] private Button _selection;

    public void SelectButton()
    {
        _selection.Select();
    }
}
