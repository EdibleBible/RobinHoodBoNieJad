using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuCheats : MonoBehaviour
{
    /*bool showConsole;
    string[] input;
    public InputActionAsset globalInputActions;
    private InputAction cheatsAction;
    public TMP_InputField inputField;
    public Image inputBG;
    public delegate PlayerBase GetPlayerEvent();
    public static event GetPlayerEvent GetPlayerBase;
    private PlayerBase player;
    private bool hasPlayer;

    private void Start()
    {
        cheatsAction = globalInputActions.FindAction("CheatConsole");
        cheatsAction.Enable();
        cheatsAction.performed += ToggleCheats;
    }

    private void OnDisable()
    {
        cheatsAction.performed -= ToggleCheats;
        cheatsAction.Disable();
    }

    private void ToggleCheats(InputAction.CallbackContext context)
    {
        if (player == null)
        {
            try { player = GetPlayerBase(); }
            catch (NullReferenceException) { hasPlayer = false; }
            finally
            {
                if (player != null)
                {
                    hasPlayer = true;
                }
            }
        }
        showConsole = !showConsole;
        if (showConsole)
        {
            inputBG.enabled = true;
            inputField.ActivateInputField();
        }
        else { inputBG.enabled = false; inputField.text = "";}
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
        switch (input[0].ToLower())
        {
            case "scene":
                CheatScene(input);
                break;
            case "playerinventory":
                if (!hasPlayer) { break; }
                CheatInventory(input);
                break;
            case "level":
                CheatLevel();
                break;
        }
    }

    public void CheatScene(string[] input)
    {
        if (int.TryParse(input[1], out int result))
        {
            SceneManager.LoadScene(result);
        }
        switch (input[1].ToLower())
        {
            case "mainmenu":
                LockCursor(false);
                SceneManager.LoadScene(0);
                break;
            case "lobby":
                LockCursor(false);
                SceneManager.LoadScene(1);
                break;
            case "build":
            case "level":
                LockCursor(true);
                SceneManager.LoadScene(2); 
                break;
            case "edible":
                SceneManager.LoadScene(3);
                break;
            case "3dprala":
                SceneManager.LoadScene(4);
                break;
            case "sample":
                LockCursor(true);
                SceneManager.LoadScene(5);
                break;
            case "animation":
                LockCursor(true);
                SceneManager.LoadScene(6);
                break;
            case "3dkosmo":
                SceneManager.LoadScene(7);
                break;
        }
    }

    public void LockCursor(bool toLock)
    {
        Cursor.visible = !toLock;
        if (toLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        } else
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }

    public void CheatInventory(string[] input)
    {
        switch (input[1].ToLower())
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

    public void CheatLevel()
    {
        switch (input[1].ToLower())
        {
            case "play":
                LockCursor(true);
                SceneManager.LoadScene(2);
                break;
            case "exit":
                LockCursor(false);
                player.hotbar.SaveToInventory(player.inventory);
                SceneManager.LoadScene(1);
                break;
        }
    }*/
}
