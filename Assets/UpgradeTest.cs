using System;
using Script.ScriptableObjects;
using UnityEngine;

public class UpgradeTest : MonoBehaviour
{
    [SerializeField] private GameObject UpgradePanel;
    
    [SerializeField] private SOPlayerStatsController playerStatsController;
    [SerializeField] private StatParameters speedModifier;
    [SerializeField] private StatParameters staminaModifier;
    [SerializeField] private StatParameters accelerationModifier;
    [SerializeField] private StatParameters inventorySizeModifier;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (UpgradePanel.activeSelf)
            {
                HideUpgradePanel();
            }
            else
            {
                ShowUpgradePanel();
            }
        }
    }

    public void ShowUpgradePanel()
    {
        UpgradePanel.SetActive(true);
    }

    public void HideUpgradePanel()
    {
        UpgradePanel.SetActive(false);
    }

    public void ResetPlayerStats()
    {
        playerStatsController.ClearPlayerBaseModifiers();
    }
}