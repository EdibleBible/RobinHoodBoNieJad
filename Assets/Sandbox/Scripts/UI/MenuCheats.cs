using System;
using System.Collections.Generic;
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
    private bool hasPlayer;
    public SOInventory playerInventory;
    public SOStats playerStats;
    public List<CheatsItems> itemList = new();
    private Dictionary<string, GameObject> itemDictionary = new();
    public GameObject helpMenu;

    private void Start()
    {
        cheatsAction = globalInputActions.FindAction("CheatConsole");
        cheatsAction.Enable();
        cheatsAction.performed += ToggleCheats;
    }

    private void OnEnable()
    {
        foreach (var pair in itemList)
        {
            if (!itemDictionary.ContainsKey(pair.key))
            {
                itemDictionary.Add(pair.key, pair.prefab);
            }
        }
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
        else { inputBG.enabled = false; inputField.text = ""; helpMenu.SetActive(false); }
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
            case "help":
                helpMenu.SetActive(true);
                inputField.ActivateInputField();
                break;
            case "scene":
                CheatScene(input);
                break;
            case "level":
                //CheatLevel();
                break;
            case "additem":
                AddItem(input[1].ToLower());
                break;
            case "money":
                playerInventory.CurrInvenoryScore = ParseInput(input[1]);
                playerStats.scoreTotal = ParseInput(input[1]);
                break;
            case "inventorysize":
                playerInventory.BaseInventorySize = ParseInput(input[1]);
                playerStats.inventorySize = ParseInput(input[1]);
                break;
            case "visit":
                playerStats.lobbyVisit = (ParseInput(input[1]) - 1);
                break;
            case "paytax":
                PayTax(input);
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

    public void CheatLevel()
    {
        switch (input[1].ToLower())
        {
            case "play":
                LockCursor(true);
                SceneManager.LoadScene(2);
                break;

        }
    }

    public void AddItem(string inputItem)
    {
        if (itemDictionary.ContainsKey(inputItem))
        {
            GameObject addedItem = Instantiate(itemDictionary[inputItem]);
            ItemData addedItemData = addedItem.GetComponent<ItemBase>().ItemData;
            playerInventory.ItemsInInventory.Add(addedItemData);
            Destroy(addedItem);
        }
    }

    public int ParseInput(string input)
    {
        if (int.TryParse(input, out int parsedInt))
        {
            return parsedInt;
        }
        return -1;
    }

    public void PayTax(string[] input)
    {
        if (input.Length == 0 || input[1] == "true") { playerStats.taxPaid = true; }
        else { playerStats.taxPaid = false; };
    }
}

[Serializable] public class CheatsItems
{
    public string key;
    public GameObject prefab;
}
