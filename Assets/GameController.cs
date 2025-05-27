using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Script.ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [HideInInspector] public static GameController Instance { get; private set; }

    public SOAllQuest AllPlayerQuest;
    public SOInventory PlayerInventory;
    public SOPlayerStatsController PlayerStatsController;
    public SOStats Stats;
    public int BaseMoney;

    public bool DebugMode;
    public bool DebugTutroial;
    public bool FillRandomItems;
    public bool DontCleanInventory;
    public bool ResetAllStatsModifiers;
    public bool RemoveAllBaseStatModifiers;

    public ScriptableRendererFeature fullScreenPassFeature; // Przypisz w Inspectorze

    public SaveData saveData = new SaveData();

    private void OnEnable()
    {


        if (DebugMode)
        {
            StartNewGame();
            AllPlayerQuest.CurrentSelectedQuest =
                AllPlayerQuest.randomizedQuests[Random.Range(0, AllPlayerQuest.AllQuestsCount)];

            foreach (var quest in AllPlayerQuest.randomizedQuests)
            {
                Debug.Log(quest.QuestName);
            }
        }

        if (DebugTutroial)
        {
            StartTutorial();
        }

        if (ResetAllStatsModifiers)
        {
            PlayerStatsController.ResetAllModifiers();
        }

        if (RemoveAllBaseStatModifiers)
        {
            PlayerStatsController.RemoveAllBaseModier();
        }

        if (FillRandomItems)
        {
            int randomNumber = Random.Range(0, 4);
            PlayerInventory.ItemsInInventory.Clear();
            for (int i = 0; i < randomNumber; i++)
            {
                ItemData randomItem = PlayerInventory.FindRandomItem().ItemData;
                if (randomItem != null)
                {
                    PlayerInventory.ItemsInInventory.Add(randomItem);
                    if (!PlayerInventory.CollectedItemTypes.Contains(randomItem.ItemType))
                    {
                        PlayerInventory.CollectedItemTypes.Add(randomItem.ItemType);
                    }
                }
            }

            randomNumber = Random.Range(0, 4);
            PlayerInventory.InventoryLobby.Clear();
            for (int i = 0; i < randomNumber; i++)
            {
                ItemData randomItem = PlayerInventory.FindRandomItem().ItemData;
                if (randomItem != null)
                {
                    PlayerInventory.InventoryLobby.Add(randomItem);
                    if (!PlayerInventory.CollectedItemTypes.Contains(randomItem.ItemType))
                    {
                        PlayerInventory.CollectedItemTypes.Add(randomItem.ItemType);
                    }
                }
            }
        }

        // Singleton Init
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        ToggleFullScreenPass(false);

        Instance = this;
        DontDestroyOnLoad(gameObject); // zachowuje między scenami
    }

    public void CleanUpInventory()
    {
        if (!DontCleanInventory)
            PlayerInventory.ClearInventory();
    }

    public void StartNewGame()
    {
        PlayerInventory.ResetCollections();
        AllPlayerQuest.RandomizeAllQuests();
        Stats.lobbyVisit = 0;
        Stats.scoreTotal = BaseMoney;
        Stats.taxPaid = false;
        Stats.scoreTotal = BaseMoney;
    }
    
    public void StartTutorial()
    {
        PlayerInventory.ResetCollections();
        AllPlayerQuest.SetTutorialQuest();
        Stats.lobbyVisit = 0;
        Stats.scoreTotal = BaseMoney;
        Stats.taxPaid = false;
        Stats.scoreTotal = BaseMoney;
    }


    public void RandomizeQuest()
    {
        AllPlayerQuest.CurrentSelectedQuest = AllPlayerQuest.randomizedQuests[0];
        Debug.Log("Randomize quest: " + AllPlayerQuest.CurrentSelectedQuest.Description);
    }

    public void SaveGameState()
    {
        saveData = new SaveData(AllPlayerQuest, PlayerInventory, PlayerStatsController, Stats);
    }

    public void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "savegame.json");
        string json = File.ReadAllText(path);

        saveData = JsonUtility.FromJson<SaveData>(json);
        AllPlayerQuest.LoadAllQuest(saveData.AllQuestsCount, saveData.CurrentSelectedQuestIndex, saveData.AllQuests);

        foreach (var quest in AllPlayerQuest.randomizedQuests)
        {
            Debug.Log(quest.QuestName);
        }

        PlayerInventory.LoadInventory(saveData.ItemsInInventory, saveData.InventoryLobby, saveData.CurrentInvenoryScore
            , saveData.ScoreBackpack, saveData.BaseInventorySize, saveData.CurrInventorySize,
            saveData.CollectedItemsType
            , saveData.CollectedGoblets, saveData.CollectedVases, saveData.CollectedBooks);

        PlayerStatsController.LoadStats(saveData.AllStatsPath, saveData.PlayerStats, saveData.PlayerBaseModifiers);

        Stats.LoadStats(saveData);
    }

    public void ToggleFullScreenPass()
    {
        if (fullScreenPassFeature != null)
        {
            fullScreenPassFeature.SetActive(!fullScreenPassFeature.isActive);
            Debug.Log("FullScreenPassRendererFeature: " + fullScreenPassFeature.isActive);
        }
    }

    public void ToggleFullScreenPass(bool state)
    {
        if (fullScreenPassFeature != null)
        {
            fullScreenPassFeature.SetActive(state);
            Debug.Log("FullScreenPassRendererFeature: " + fullScreenPassFeature.isActive);
        }
    }

    private void OnApplicationQuit()
    {
        ToggleFullScreenPass(false);
    }

    public void ToogleCursorOn(bool toogleMove)
    {
        Debug.Log("ToogleCursorOn");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        if(!toogleMove)
            return;

        PlayerRotation rotation = FindAnyObjectByType<PlayerRotation>();

        if (rotation != null)
        {
            rotation.CanRotation = false;
        }
    }

    public void ToogleCursorOff(bool toogleMove)
    {
        Debug.Log("ToogleCursorOff");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        if(!toogleMove)
            return;

        PlayerRotation rotation = FindAnyObjectByType<PlayerRotation>();

        if (rotation != null)
        {
            rotation.CanRotation = true;
        }
    }

    public void BackToLobby()
    {
        SceneManager.LoadScene(1);
    }
}

[Serializable]
public class SaveData
{
    //SOAllQuest
    public int AllQuestsCount;
    public int CurrentSelectedQuestIndex;
    public List<QuestSaveData> AllQuests = new List<QuestSaveData>();

    //Inventory
    public List<ItemDataSave> ItemsInInventory = new List<ItemDataSave>();
    public List<ItemDataSave> InventoryLobby = new List<ItemDataSave>();
    public float CurrentInvenoryScore;
    public float ScoreBackpack;
    public int BaseInventorySize;
    public int CurrInventorySize;
    public List<int> CollectedItemsType = new List<int>();
    public int CollectedGoblets;
    public int CollectedVases;
    public int CollectedBooks;

    //statsController
    public string AllStatsPath;
    public List<StatsModifierSaveData> PlayerStats = new List<StatsModifierSaveData>();
    public List<StatsModifierSaveData> PlayerBaseModifiers = new List<StatsModifierSaveData>();

    //Stats
    public int InventorySize;
    public float PlayerSpeed;
    public float PlayerRotationSpeed;
    public int ScoreLevel;
    public bool LevelSuccess;
    public int ScoreTotal;
    public int LobbyVisit;
    public bool TaxPaid;

    public SaveData()
    {
    }

    public SaveData(SOAllQuest allQuests, SOInventory playerInventory, SOPlayerStatsController statsController,
        SOStats stats)
    {
        #region Quests

        AllQuestsCount = allQuests.AllQuestsCount;

        if (allQuests.CurrentSelectedQuest != null)
        {
            CurrentSelectedQuestIndex = allQuests.randomizedQuests.IndexOf(allQuests.CurrentSelectedQuest);
        }
        else
        {
            CurrentSelectedQuestIndex = -1;
        }

        AllQuests = new List<QuestSaveData>();

        foreach (var quest in allQuests.randomizedQuests)
        {
            Debug.Log((int)quest.Difficulty);
            Debug.Log(quest.Description);
            Debug.Log(quest.ShortDescription);
            Debug.Log(quest.QuestName);

            QuestSaveData tempSaveData = new QuestSaveData((int)quest.Difficulty, quest.Description,
                quest.ShortDescription, quest.QuestName, quest.RequireItems);
            AllQuests.Add(tempSaveData);
        }

        #endregion

        #region Inventory

        foreach (var item in playerInventory.ItemsInInventory)
        {
            ItemsInInventory.Add(new ItemDataSave((int)item.ItemType, item.ItemName));
        }

        foreach (var item in playerInventory.InventoryLobby)
        {
            InventoryLobby.Add(new ItemDataSave((int)item.ItemType, item.ItemName));
        }

        CurrentInvenoryScore = playerInventory.CurrInvenoryScore;
        ScoreBackpack = playerInventory.ScoreBackup;
        BaseInventorySize = playerInventory.BaseInventorySize;
        CurrInventorySize = playerInventory.CurrInventorySize;

        foreach (var item in playerInventory.CollectedItemTypes)
        {
            CollectedItemsType.Add((int)item);
        }

        CollectedGoblets = playerInventory.CollectedGoblets;
        CollectedVases = playerInventory.CollectedVases;
        CollectedBooks = playerInventory.CollectedBooks;

        #endregion

        #region Stats

        AllStatsPath = statsController.AllStatsPath;

        PlayerStats.Clear();
        foreach (var stat in statsController.PlayerStats)
        {
            StatsModifierSaveData temp = new StatsModifierSaveData((int)stat.ModifiersType, stat.Additive,
                stat.Multiplicative, stat.Icon.name, stat.UpgradeBaseCost, stat.CurrLevel, stat.UpgradeCurrCost,
                stat.statsToUpgrade.Additive, stat.statsToUpgrade.Multiplicative);

            PlayerStats.Add(temp);
        }

        PlayerBaseModifiers.Clear();
        foreach (var stat in statsController.PlayerBaseModifiers)
        {
            StatsModifierSaveData temp = new StatsModifierSaveData((int)stat.ModifiersType, stat.Additive,
                stat.Multiplicative, stat.Icon.name, stat.UpgradeBaseCost, stat.CurrLevel, stat.UpgradeCurrCost,
                stat.statsToUpgrade.Additive, stat.statsToUpgrade.Multiplicative);

            PlayerBaseModifiers.Add(temp);
        }

        #endregion

        #region Stats

        InventorySize = stats.inventorySize;
        PlayerSpeed = stats.playerSpeed;
        PlayerRotationSpeed = stats.playerRotationSpeed;
        ScoreLevel = stats.scoreLevel;
        LevelSuccess = stats.levelSuccess;
        ScoreTotal = stats.scoreTotal;
        LobbyVisit = stats.lobbyVisit;
        TaxPaid = stats.taxPaid;

        #endregion

        string json = JsonUtility.ToJson(this);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "savegame.json"), json);
    }
}

[Serializable]
public class QuestSaveData
{
    public int QuestDifficulty;
    public string Description;
    public string ShortDescription;
    public string QuestName;
    public Dictionary<int, QuestAmountSaveData> RequireItems = new();


    public QuestSaveData(int questDifficulty, string description, string shortDescription, string questName,
        Dictionary<ItemType, QuestAmountData> requireItems)
    {
        QuestDifficulty = questDifficulty;
        Description = description;
        ShortDescription = shortDescription;
        QuestName = questName;

        foreach (var item in requireItems)
        {
            RequireItems.Add((int)item.Key,
                new QuestAmountSaveData(item.Value.CurrentAmount, item.Value.RequiredAmount));
        }
    }
}

[Serializable]
public class QuestAmountSaveData
{
    public int CurrentAmount;
    public int RequiredAmount;

    public QuestAmountSaveData(int x, int y)
    {
        CurrentAmount = x;
        RequiredAmount = y;
    }
}

[Serializable]
public class ItemDataSave
{
    public int ItemType;
    public string ItemName;

    public ItemDataSave(int itemType, string itemName)
    {
        ItemType = itemType;
        ItemName = itemName;
    }
}

[Serializable]
public class StatsModifierSaveData
{
    public StatsModifierSaveData(int modifierType, float additive, float multiplicative, string spriteName,
        int upgradeBaseCost, int currLevel, int upgradeCurrCost, float additiveToAdd, float multiplicativeToAdd)
    {
        ModifierType = modifierType;
        Additive = additive;
        Multiplicative = multiplicative;
        SpriteName = spriteName;
        UpgradeBaseCost = upgradeBaseCost;
        CurrLevel = currLevel;
        UpgradeCurrCost = upgradeCurrCost;
        AdditiveToAdd = additiveToAdd;
        MultiplicativeToAdd = multiplicativeToAdd;
    }

    public int ModifierType;
    public float Additive;
    public float Multiplicative;

    public string SpriteName;
    public int UpgradeBaseCost;
    public int CurrLevel;
    public int UpgradeCurrCost;

    public float AdditiveToAdd;
    public float MultiplicativeToAdd;
}