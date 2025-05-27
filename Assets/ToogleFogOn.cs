using System;
using UnityEngine;

public class ToogleFogOn : MonoBehaviour
{
    private void Start()
    {
        GameController.Instance.ToggleFullScreenPass(true);
    }
}
