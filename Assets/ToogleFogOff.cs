using System;
using UnityEngine;

public class ToogleFogOff : MonoBehaviour
{
    private void Start()
    {
        GameController.Instance.ToggleFullScreenPass(false);
    }
}
