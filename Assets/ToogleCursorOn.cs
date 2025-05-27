using System;
using UnityEngine;

public class ToogleCursorOn : MonoBehaviour
{
    private void Start()
    {
        GameController.Instance.ToogleCursorOn(false);
    }
}
