using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerQuestSystemUI : MonoBehaviour
{
    [SerializeField] private GameObject uiElementPrefab;
    [SerializeField] private Transform uiContainer;

    private SOPlayerQuest currShowedQuest;
    private List<PlayerQuestUIElement> playerQuestUIElements = new List<PlayerQuestUIElement>();
    
    [Header("Panel Settings")]
    [SerializeField] private RectTransform questPanelRoot;
    [SerializeField] private RectTransform panelGraphic; // To, z czego brana będzie długość

    [Header("Animation Settings")]
    [SerializeField] private float tweenDuration = 0.4f;
    [SerializeField] private Ease tweenEase = Ease.OutCubic;

    private Vector2 hiddenPosition;
    private Vector2 shownPosition;
    private Tween currentTween;
    
    private void OnEnable()
    {
        float width = panelGraphic.rect.width;

        // Ustaw tylko raz pozycje
        hiddenPosition = new Vector2(-width, questPanelRoot.anchoredPosition.y);
        shownPosition = new Vector2(0, questPanelRoot.anchoredPosition.y);

        /*
        questPanelRoot.anchoredPosition = hiddenPosition;
        */
    }
    
    public void SetupQuest(Component sender, object data)
    {
        if (sender is PlayerQuestSystem && data is SOPlayerQuest quest)
        {
            currShowedQuest = quest;
            foreach (var item in quest.RequireItems)
            {
                GameObject obj = Instantiate(uiElementPrefab, uiContainer);
                PlayerQuestUIElement playerQuestUIElement = obj.GetComponent<PlayerQuestUIElement>();
                playerQuestUIElements.Add(playerQuestUIElement);
                playerQuestUIElement.Setup(item);
            }
        }
    }

    public void UpdateValue(Component sender, object data)
    {
        if (data is (ItemType type, QuestAmountData amountData))
        {
            var selectedElement = playerQuestUIElements.FirstOrDefault(x => x.selectedItem == type);
            if (selectedElement == null)
            {
                Debug.Log("No item to update");
            }
            else
            {
                selectedElement.UpdateValue(amountData);
            }
        }
    }

    public void ShowPanel()
    {
        float width = panelGraphic.rect.width;

        // Oblicz pozycje panelu
        hiddenPosition = new Vector2(-width, questPanelRoot.anchoredPosition.y);
        shownPosition = new Vector2(0, questPanelRoot.anchoredPosition.y);
        
        currentTween?.Kill();
        currentTween = questPanelRoot.DOAnchorPos(shownPosition, tweenDuration).SetEase(tweenEase);
    }

    public void HidePanel()
    {
        float width = panelGraphic.rect.width;

        // Oblicz pozycje panelu
        hiddenPosition = new Vector2(-width, questPanelRoot.anchoredPosition.y);
        shownPosition = new Vector2(0, questPanelRoot.anchoredPosition.y);
        
        currentTween?.Kill();
        currentTween = questPanelRoot.DOAnchorPos(hiddenPosition, tweenDuration).SetEase(tweenEase);
    }
}