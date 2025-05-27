using System;
using UnityEngine;

public class ToogleCursorOn : MonoBehaviour
{
    private void OnEnable()
    {
        GameController.Instance.ToogleCursorOn(false);
    }
}
