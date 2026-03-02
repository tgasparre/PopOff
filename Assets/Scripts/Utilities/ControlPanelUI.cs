using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

//claude generated file to auto populate control panel with currently bound controls
public class ControlPanelUI : MonoBehaviour
{
    public InputActionAsset inputActions;
    public TextMeshProUGUI controlsText;

    void Start()
    {
        string display = "";
        foreach (var map in inputActions.actionMaps)
        {
            display += $"<b>{map.name}</b>\n";
            foreach (var action in map.actions)
            {
                bool inComposite = false;

                for (int i = 0; i < action.bindings.Count; i++)
                {
                    var binding = action.bindings[i];

                    if (binding.isComposite)
                    {
                        // This is the composite header (e.g. "2DVector")
                        display += $"  {action.name}:\n";
                        inComposite = true;
                    }
                    else if (binding.isPartOfComposite)
                    {
                        // Each named part, e.g. "up", "down"
                        string key = action.GetBindingDisplayString(i);
                        display += $"    {binding.name}: {key}\n";
                    }
                    else
                    {
                        // Normal non-composite binding
                        inComposite = false;
                        string key = action.GetBindingDisplayString(i);
                        display += $"  {action.name}: {key}\n";
                    }
                }
            }
        }
    }
}