using TMPro;
using UnityEngine;

public class CollectionPages : MonoBehaviour
{
    public int page = 0;
    public GameObject[] grids;
    public GameObject gridCurrent;
    public TMP_Text nameText;
    public string[] namesTable = {"Goblets","Vases","Books"};

    public void ChangePage(bool isForward)
    {
        Wrap(isForward);
        gridCurrent.SetActive(false);
        gridCurrent = grids[page];
        gridCurrent.SetActive(true);
        nameText.text = namesTable[page];
    }

    public void Wrap(bool isForward)
    {
        if (isForward)
            page = (page + 1) % 3;
        else
            page = (page + 2) % 3;
    }
}
