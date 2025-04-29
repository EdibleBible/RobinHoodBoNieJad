using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public PlayerControll PlayerInputActions { get; private set; }

    private void Awake()
    {
        // Singleton Init
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // zachowuje między scenami

        PlayerInputActions = new PlayerControll();
        PlayerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        PlayerInputActions?.Dispose();
    }

    /// <summary>
    /// Zwraca nazwę przycisku przypisanego do konkretnej akcji (np. "F" dla Torch).
    /// </summary>
    /// <param name="action">InputAction (np. PlayerInputActions.Player.ToogleTorch)</param>
    /// <param name="bindingIndex">Opcjonalny indeks bindu (domyślnie 0)</param>
    /// <returns>Wyświetlana nazwa przycisku (np. "F", "E", "Button South")</returns>
    public string GetBindingDisplayString(InputAction action, int bindingIndex = 0)
    {
        if (action == null || bindingIndex < 0 || bindingIndex >= action.bindings.Count)
        {
            Debug.LogWarning("Binding not found.");
            return "";
        }

        return action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);
    }

    /// <summary>
    /// Alternatywnie możesz uzyskać nazwę po nazwie akcji, np. "ToogleTorch"
    /// </summary>
    public string GetBindingDisplayString(string actionName)
    {
        var action = PlayerInputActions.asset.FindAction(actionName);
        if (action != null)
            return action.GetBindingDisplayString(0, InputBinding.DisplayStringOptions.DontUseShortDisplayNames);

        Debug.LogWarning($"Action '{actionName}' not found in input actions.");
        return "";
    }


    public string CompereTextWithInput(string actionName, string textToCompare)
    {
        var action = GetBindingDisplayString(actionName); 
        var comperedText = $"`{action}`: {textToCompare}`";
        return comperedText;
    }
}