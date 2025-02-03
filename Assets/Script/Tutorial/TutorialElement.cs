using TMPro;
using UnityEngine;

public class TutorialElement : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TutorialTextMesh;
    [SerializeField] private GameObject tutorialObject;
    
    public void ShowTutorial(string TutorialText)
    {
        tutorialObject.SetActive(true);
        Debug.Log(TutorialText);
        TutorialTextMesh.text = TutorialText;
        
    }

    public void HideTutorial()
    {
        tutorialObject.SetActive(false);
    }
}