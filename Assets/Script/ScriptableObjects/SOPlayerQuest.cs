using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "SOPlayerQuest", menuName = "Scriptable Objects/SOPlayerQuest")]
public class SOPlayerQuest : ScriptableObject
{
    public QuestDifficulty Difficulty;
    public string Description;
    public string ShortDescription;
    public string QuestName;

    public SerializedDictionary<ItemType, QuestAmountData> RequireItems =
        new SerializedDictionary<ItemType, QuestAmountData>();

    public SerializedDictionary<ItemType, int> SpawnedItems = new SerializedDictionary<ItemType, int>();
    
    public ItemsTranslator ItemsTranslator;

    public void Reset()
    {
        foreach (var item in RequireItems)
        {
            item.Value.CurrentAmount = 0;
        }
    }

    public bool IsQuestComplete()
    {
        foreach (var item in RequireItems)
        {
            if (item.Value.CurrentAmount < item.Value.RequiredAmount)
            {
                return false;
            }
        }

        return true;
    }

    public string GetDifficultyAsText(QuestDifficulty difficulty)
    {
        switch (difficulty)
        {
            case QuestDifficulty.easy:
                return "Easy";
            case QuestDifficulty.medium:
                return "Medium";
            case QuestDifficulty.Hard:
                return "Hard";
            case QuestDifficulty.VeryHard:
                return "Very Hard";
            default:
                return difficulty.ToString();
        }
    }


    public void RandomizeQuest(QuestDifficulty difficulty)
    {
        Difficulty = difficulty;
        RequireItems.Clear();
        QuestName = GenerateRandomQuestName();
        ShortDescription = $"New Quest {Difficulty}";

        int itemAmount = 0;
        int itemRequiredAmountMin = 0;
        int itemRequiredAmountMax = 0;
        List<ItemType> avaibleCollection = ItemTypeHelper.GetCollectibles();

        switch (Difficulty)
        {
            case QuestDifficulty.easy:
                itemAmount = Random.Range(1, 2);
                itemRequiredAmountMax = 3;
                itemRequiredAmountMin = 1;
                break;

            case QuestDifficulty.medium:
                itemAmount = Random.Range(2, 4);
                itemRequiredAmountMax = 5;
                itemRequiredAmountMin = 1;
                break;

            case QuestDifficulty.Hard:
                itemAmount = Random.Range(4, 5);
                itemRequiredAmountMax = 5;
                itemRequiredAmountMin = 3;
                break;

            case QuestDifficulty.VeryHard:
                itemAmount = Random.Range(6, 9);
                itemRequiredAmountMax = 7;
                itemRequiredAmountMin = 3;
                break;
        }

        for (int i = 0; i < itemAmount; i++)
        {
            if (avaibleCollection.Count <= 0)
                break;

            ItemType itemType = avaibleCollection[Random.Range(0, avaibleCollection.Count)];
            avaibleCollection.Remove(itemType);

            RequireItems.Add(itemType,
                new QuestAmountData(0, Random.Range(itemRequiredAmountMin, itemRequiredAmountMax)));
        }

        ItemsTranslator = Resources.Load<ItemsTranslator>("Data/ItemsTranslator");
        
        Description = $"{QuestName} ({Difficulty})";
        Description += "\n\n" + GenerateRandomDescription() + "\n";

        foreach (var item in RequireItems)
        {
            Description += $"\n{ItemsTranslator.itemTypeDictionary.Where(x => x.Key == item.Key).FirstOrDefault().Value} Amount: {item.Value.RequiredAmount}\n";
        }
    }
    
    public void ResetSpawnedItem()
    {
        SpawnedItems.Clear();
    }

    private static readonly string[] PossibleQuestNames = new[]
    {
        "Whispers of the Forgotten Vault",
        "Echoes Beneath the Tomb",
        "Relics of the Hollow Depths",
        "The Collector's Crypt",
        "Shadows of the Buried Shrine",
        "Secrets of the Sunken Chapel",
        "The Warden of Withered Bones",
        "Lament of the Lost Pilgrims",
        "Tithes to the Silent Watcher",
        "The Hollow King's Legacy",
        "Curses Woven in Dust and Stone",
        "Veins of the Undying Roots",
        "Hymns from the Black Reliquary",
        "Vault of the Broken Sigil",
        "Chains Beneath the Crimson Seal",
        "The Scribe’s Last Candle",
        "Echoes of the Nameless Rite",
        "A Pact Signed in Shallow Graves",
        "The Tombbound Testament",
        "Beneath the Grinning Idol",
        "The Silence Between the Bones",
        "Fragments of a Withered Oath",
        "The Ghoul-Warden’s Bounty",
        "The Prayer That Opened the Grave",
        "Feast of the Bone Monks",
        "The Inheritance of Ashes",
        "Footsteps in the Dust-Choked Hall",
        "The Grasp of the Unentombed",
        "Where No Light Ever Reaches",
        "Thorns of the Grieving Garden",
        "The Archivist’s Forbidden Shelf",
        "The Key That Screams",
        "Sins Etched in Black Marble",
        "The Pilgrimage of the Hollow Saint",
        "Return to the Catacombs Below",
        "Offerings to the Blind Oracle",
        "The Sigil That Should Not Be Broken",
        "The Sepulcher's Final Guardian",
        "A Choir Beneath the Stones",
        "When the Soil Remembers",
        "The Candle Burns Backward",
        "The Debt of the Bonewright",
        "The Whispering Vault Opens",
        "Shadows Cling to the Gravetender"
    };

    private static readonly string[] PossibleQuestDescriptions = new[]
    {
        "Ancient voices echo from a sealed vault deep beneath the hills - Rumors say it holds treasures kept by forgotten kings.",
        "Locals speak of footsteps in the tomb at night. Whatever walks there guards something of great worth.",
        "Lost relics lie buried in the sunken catacombs.",
        "A noble hoarder of rare artifacts was buried with his collection. It’s time someone redistributed his wealth.",
        "This shrine was meant to stay buried. Where priests see heresy, we see opportunity.",
        "There are secrets worth gold to the right buyer.",
        "Pilgrims once vanished on their way to a sacred site. It's time you've ended this cycle.",
        "Here lies a chapel meant to silence the dead. Something inside never stopped screaming.",
        "Chains etched with forbidden runes bind a seal deep underground.",
        "A candle left burning in a forgotten library suggests someone - or something — still seeks forbidden truths.",
        "A pact once made in desperation lies carved into shallow graves. The spirits bound by it demand revenge… or closure.",
        "Beneath us, a hidden chamber pulses with arcane light and forgotten wealth.",
        "At dusk, the crypt bell tolls once for the living and twice for the trespasser. It hasn’t rung in decades… until now.",
        "The hidden trove may still aid the people sworn to protect.",
        "The catacombs breathe in silence, but scattered notes tell of someone who made it far deeper than most.",
        "Dig too deep, and the ground may answer in kind."
    };


    private string GenerateRandomQuestName()
    {
        return PossibleQuestNames[Random.Range(0, PossibleQuestNames.Length)];
    }

    private string GenerateRandomDescription()
    {
        return PossibleQuestDescriptions[Random.Range(0, PossibleQuestDescriptions.Length)];
    }
}

[Serializable]
public class QuestAmountData
{
    public int CurrentAmount;
    public int RequiredAmount;

    public QuestAmountData(int currentAmount, int requiredAmount)
    {
        CurrentAmount = currentAmount;
        RequiredAmount = requiredAmount;
    }
}

public enum QuestDifficulty
{
    easy = 1,
    medium = 2,
    Hard = 3,
    VeryHard = 4
}