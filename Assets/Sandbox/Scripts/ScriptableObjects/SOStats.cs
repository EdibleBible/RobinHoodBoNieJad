using UnityEngine;

[CreateAssetMenu(fileName = "SOStats", menuName = "Scriptable Objects/SOStats")]
public class SOStats : ScriptableObject
{
    public int inventorySize;
    public float playerSpeed;
    public float playerRotationSpeed;
    public int scoreLevel;
    public bool levelSuccess;
    public int scoreTotal;
    public int lobbyVisit;
    public bool taxPaid;

    public bool VisitSmith;
    public bool VisitWitch;
    public bool VisitLibrary;

    public void LoadStats(SaveData saveData)
    {
        inventorySize = saveData.InventorySize;
        playerSpeed = saveData.PlayerSpeed;
        playerRotationSpeed = saveData.PlayerRotationSpeed;
        scoreLevel = saveData.ScoreLevel;
        levelSuccess = saveData.LevelSuccess;
        scoreTotal = saveData.ScoreTotal;
        lobbyVisit = saveData.LobbyVisit;
        taxPaid = saveData.TaxPaid;
        
        VisitSmith = saveData.VisitSmith;
        VisitWitch = saveData.VisitWitch;
        VisitLibrary = saveData.VisitLibrary;
    }
}
