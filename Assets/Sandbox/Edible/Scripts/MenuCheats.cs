using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuCheats : MonoBehaviour
{
    bool showConsole;
    string[] input;
    public InputActionAsset globalInputActions;
    private InputAction cheatsAction;
    public TMP_InputField inputField;
    public Image inputBG;
    public delegate PlayerBase GetPlayerEvent();
    public static event GetPlayerEvent GetPlayerBase;
    private PlayerBase player;

    private void Start()
    {
        cheatsAction = globalInputActions.FindAction("CheatConsole");
        cheatsAction.Enable();
        cheatsAction.performed += ToggleCheats;
        player = GetPlayerBase();
    }

    private void OnDisable()
    {
        cheatsAction.performed -= ToggleCheats;
        cheatsAction.Disable();
    }

    private void ToggleCheats(InputAction.CallbackContext context)
    {
        showConsole = !showConsole;
        if (showConsole)
        {
            inputBG.enabled = true;
            inputField.Select();
        }
        else { inputBG.enabled = false; inputField.text = ""; }
    }

    private void Update()
    {
        if (showConsole && Input.GetKeyDown(KeyCode.Return))
        {
            input = inputField.text.Split(' ');
            inputField.text = "";
            Cheat();
        }
    }

    public void Cheat()
    {
        Debug.Log(input);
        switch (input[0])
        {
            case "scene":
                CheatScene(input);
                break;
            case "playerInventory":
                CheatInventory(input);
                break;
        }
    }

    public void CheatScene(string[] input)
    {
        if (int.TryParse(input[1], out int result))
        {
            SceneManager.LoadScene(result);
        }
        switch (input[1])
        {
            case "Lobby":
                SceneManager.LoadScene(0);
                break;
            case "Build":
                SceneManager.LoadScene(1);
                break;
            case "Edible":
                SceneManager.LoadScene(2); 
                break;
            case "Prala":
                SceneManager.LoadScene(3);
                break;
        }
    }

    public void CheatInventory(string[] input)
    {
        switch (input[1])
        {
            case "save":
                player.hotbar.SaveToInventory(player.inventory);
                break;
            case "load":
                player.hotbar.Clear();
                player.hotbar.LoadFromInventory(player.inventory);
                break;
            case "clear":
                player.hotbar.Clear();
                break;
        }
    }
}
