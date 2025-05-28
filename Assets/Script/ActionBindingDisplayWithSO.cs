using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class ActionBindingDisplayWithSO : MonoBehaviour
{
    public SO_ActionTextMap actionTextMap;

    [Tooltip("Akcja, której tekst ma się wyświetlać.")]
    public InputActionReference inputActionReference;

    [Header("UI Text Components")]
    [Tooltip("Tekst wyświetlający nazwę akcji (z SO)")]
    public TextMeshProUGUI actionNameText;

    [Tooltip("Tekst wyświetlający aktualne przypisanie klawisza/przycisku")]
    public TextMeshProUGUI bindingText;

    private bool isRebinding = false;

    private void OnEnable()
    {
        UpdateTexts();
    }

    // Aktualizuje oba teksty: nazwę akcji i przypisanie
    public void UpdateTexts()
    {
        UpdateActionNameText();
        UpdateBindingText();
    }

    private void UpdateActionNameText()
    {
        if (actionNameText == null)
            return;

        if (inputActionReference == null)
        {
            actionNameText.text = "No action assigned";
            return;
        }

        if (actionTextMap != null && actionTextMap.Inputs.TryGetValue(inputActionReference, out string displayText))
        {
            actionNameText.text = displayText;
        }
        else
        {
            actionNameText.text = inputActionReference.action?.name ?? "No action name";
        }
    }

    private void UpdateBindingText()
    {
        if (bindingText == null)
            return;

        if (inputActionReference == null || inputActionReference.action == null)
        {
            bindingText.text = "-";
            return;
        }

        string bindingDisplay = GetBindingDisplayName();

        if (!string.IsNullOrEmpty(bindingDisplay))
        {
            bindingText.text = bindingDisplay;
        }
        else
        {
            bindingText.text = "-";
        }
    }

    private string GetBindingDisplayName()
    {
        var action = inputActionReference.action;
        if (action == null)
            return null;

        int bindingIndex = 0;

        if (action.bindings.Count > 0)
        {
            return action.GetBindingDisplayString(bindingIndex);
        }

        return null;
    }

    public void StartRebind()
    {
        /*if (isRebinding)
            return;

        if (inputActionReference == null || inputActionReference.action == null)
        {
            Debug.LogWarning("Brak przypisanej akcji do przebindowania.");
            return;
        }

        StartCoroutine(RebindCoroutine());*/
    }

    private IEnumerator RebindCoroutine()
    {
        isRebinding = true;

        var action = inputActionReference.action;
        int bindingIndex = 0;

        // Pokazujemy info o przebindowaniu
        if (bindingText != null)
            bindingText.text = "Press a key...";

        action.Disable();

        var rebind = action.PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")  // możesz usunąć, jeśli chcesz mysz
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                operation.Dispose();
                action.Enable();
                isRebinding = false;
                UpdateBindingText();
            })
            .Start();

        while (isRebinding)
            yield return null;
    }
}
