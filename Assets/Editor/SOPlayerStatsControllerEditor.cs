using System.Collections.Generic;
using Script.ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SOPlayerStatsController))]
public class SOPlayerStatsControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SOPlayerStatsController statsController = (SOPlayerStatsController)target;

        List<E_ModifiersType> existingTypes = new List<E_ModifiersType>();

        for (int i = 0; i < statsController.PlayerStats.Count; i++)
        {
            SOStatsModifiers stat = statsController.PlayerStats[i];

            if (stat == null) continue;

            // Sprawdzamy, czy enum już wystąpił
            if (existingTypes.Contains(stat.ModifiersType))
            {
                EditorGUILayout.HelpBox($"Stat '{stat.ModifiersType}' już istnieje! Usuń duplikat.", MessageType.Warning);
            }
            else
            {
                existingTypes.Add(stat.ModifiersType);
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            // Pole do edycji obiektu ScriptableObject
            statsController.PlayerStats[i] = (SOStatsModifiers)EditorGUILayout.ObjectField(stat, typeof(SOStatsModifiers), false);

            if (GUILayout.Button("Usuń", GUILayout.Width(60)))
            {
                statsController.PlayerStats.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();

            // Jeśli obiekt nie jest null, wyświetlamy edycję jego wartości
            if (stat != null)
            {
                EditorGUILayout.LabelField($"Modyfikator: {stat.ModifiersType}", EditorStyles.boldLabel);
                stat.Additive = EditorGUILayout.FloatField("Additive", stat.Additive);
                stat.Multiplicative = EditorGUILayout.FloatField("Multiplicative", stat.Multiplicative);

                // Przycisk Reset
                if (GUILayout.Button("Reset", GUILayout.Width(100)))
                {
                    stat.Additive = 0;
                    stat.Multiplicative = 1;
                }
            }

            EditorGUILayout.EndVertical();
        }

        // Dodawanie nowego statystyk modyfikatora
        EditorGUILayout.Space();
        SOStatsModifiers newModifier = (SOStatsModifiers)EditorGUILayout.ObjectField("Dodaj nowy stat", null, typeof(SOStatsModifiers), false);

        if (newModifier != null)
        {
            if (existingTypes.Contains(newModifier.ModifiersType))
            {
                EditorGUILayout.HelpBox("Ten typ statystyk już istnieje!", MessageType.Error);
            }
            else
            {
                statsController.PlayerStats.Add(newModifier);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
