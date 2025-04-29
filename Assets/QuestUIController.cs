using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class QuestUIController : MonoBehaviour
{
    public Transform QuestListElementHolder;
    public QuestUiObject questUIPrefab;
    public GameObject DescriptionPanel;
    public TextMeshProUGUI descriptionText;

    private void OnEnable()
    {
        ShowAllQuests();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (DescriptionPanel.activeSelf)
            {
                HideDescriptionPanel();
            }
        }
    }

    public void ShowAllQuests()
    {
        foreach (var quest in GameController.Instance.AllPlayerQuest.randomizedQuests)
        {
            var obj = Instantiate(questUIPrefab, questUIPrefab.transform.position, Quaternion.identity);
            obj.transform.SetParent(QuestListElementHolder);
            obj.Setup(quest,ShowDescriptionPanel);
        }
    }


    public void ShowDescriptionPanel()
    {
        FilLDescriptionPanel();
        DescriptionPanel.SetActive(true);
    }

    public void HideDescriptionPanel()
    {
        DescriptionPanel.SetActive(false);
    }
    
    public void FilLDescriptionPanel()
    {
        descriptionText.text = GameController.Instance.AllPlayerQuest.CurrentSelectedQuest.Description;
    }
}