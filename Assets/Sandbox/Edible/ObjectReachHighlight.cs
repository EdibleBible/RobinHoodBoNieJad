using UnityEngine;

public class ObjectReachHighlight : MonoBehaviour
{
    public Material materialHighlightOn;
    public Material materialHighlightOff;

    public void ActivateHighlight(bool toActivate)
    {
        if (toActivate)
        {
            gameObject.GetComponent<Renderer>().material = materialHighlightOn;
        } else
        {
            gameObject.GetComponent<Renderer>().material = materialHighlightOff;
        }
    }
}
