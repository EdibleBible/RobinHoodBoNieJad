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
    public TextMeshProUGUI nameText;

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
        foreach (Transform obj in QuestListElementHolder)
        {
            Destroy(obj.gameObject);
        }
        foreach (var quest in GameController.Instance.AllPlayerQuest.randomizedQuests)
        {
            var obj = Instantiate(questUIPrefab, questUIPrefab.transform.position, Quaternion.identity);
            obj.transform.SetParent(QuestListElementHolder);
            obj.Setup(quest, ShowDescriptionPanel);
            obj.transform.localScale = Vector3.one;
            obj.transform.localPosition = Vector3.zero;
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
        if (descriptionText != null)
            descriptionText.text = GameController.Instance.AllPlayerQuest.CurrentSelectedQuest.Description;
        if (nameText != null)
            nameText.text = GameController.Instance.AllPlayerQuest.CurrentSelectedQuest.name;
    }
}