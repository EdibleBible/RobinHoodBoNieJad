using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuLobbyTaxes : MonoBehaviour
{
    public TMP_Text textTaxDisplay;
    public SOStats SOStats;
    public TMP_Text textVisit;
    public TMP_Text textNeeded;
    public TMP_Text textHad;
    public Button buttonPay;
    public List<int> taxPerLevel = new();
    [HideInInspector] public int taxAmount = 0;

    private void Awake()
    {
        taxAmount = GetTaxAmount();
        if (SOStats.taxPaid)
        {
            buttonPay.interactable = false;
            textTaxDisplay.color = Color.white;
        }
        else
        {
            if (taxAmount > SOStats.scoreTotal)
            {
                buttonPay.interactable = false;
                textHad.color = Color.red;
            }
            else if (taxAmount <= SOStats.scoreTotal)
            {
                buttonPay.interactable = true;
                textHad.color = Color.white;
            }
        }


        textVisit.text = "Visit #" + SOStats.lobbyVisit.ToString();
        textNeeded.text = "You need: " + taxAmount;
        textHad.text = "You have: " + SOStats.scoreTotal.ToString();
    }

    private int GetTaxAmount()
    {
        if (taxPerLevel.Count >= SOStats.lobbyVisit && SOStats.lobbyVisit != 0)
        {
            return taxPerLevel[SOStats.lobbyVisit - 1];
        }

        return 0;
    }

    public void PayTax()
    {
        buttonPay.interactable = false;
        SOStats.scoreTotal -= taxAmount;
        SOStats.taxPaid = true;
        textTaxDisplay.color = Color.white;
    }
}