using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AlarmedTest : MonoBehaviour
{
    private Camera _camera;
    public Vector3 pos;

    void Start()
    {
        _camera = Camera.main; // Pobieramy główną kamerę
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sprawdzenie kliknięcia lewym przyciskiem myszy
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log(hit.point);
                var position = hit.point; // Ustawienie pozycji obiektu na miejsce kliknięcia
                List<EnemyMovement> list = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None).ToList();
                foreach (var obj in list)
                {
                    pos = obj.FindRandomPointAroundPoint(position, 3f);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (pos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(pos, 0.5f);
        }
    }
}