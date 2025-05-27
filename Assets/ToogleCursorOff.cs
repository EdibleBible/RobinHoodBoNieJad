using System;
using UnityEngine;

public class ToogleCursorOff : MonoBehaviour
{
    private void Start()
    {
        GameController.Instance.ToogleCursorOff(false);
    }
}
