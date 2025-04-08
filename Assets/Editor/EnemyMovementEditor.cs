using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyMovement))]
public class EnemyMovementEditor : Editor
{
    private EnemyMovement enemyMovement;

    private void OnEnable()
    {
        enemyMovement = (EnemyMovement)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (enemyMovement == null) return;

        // Sprawdzamy, czy kolejka destinations nie jest pusta
        if (enemyMovement.GetDestinationCount() == 0)
        {
            GUILayout.Label("Destinations Queue is empty");
        }
        else
        {
            GUILayout.Label("Current Destinations in Queue:");
            
            // Wyświetlamy kolejkę w formie listy w inspektorze
            List<Vector3> tempList = new List<Vector3>(enemyMovement.destinations);
            for (int i = 0; i < tempList.Count; i++)
            {
                EditorGUILayout.LabelField($"Destination {i + 1}: {tempList[i]}");
            }
        }

        // Odświeżamy widok edytora, aby lista była dynamicznie aktualizowana
        Repaint();
    }
}