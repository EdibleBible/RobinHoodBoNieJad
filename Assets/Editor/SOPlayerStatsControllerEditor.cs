using System.Collections.Generic;
using Script.ScriptableObjects;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SOPlayerStatsController))]
public class SOPlayerStatsControllerEditor : Editor
{
    private SerializedProperty playerStatsProperty;

    private void OnEnable()
    {
        playerStatsProperty = serializedObject.FindProperty("PlayerStats");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        SOPlayerStatsController statsController = (SOPlayerStatsController)target;

        List<E_ModifiersType> existingTypes = new List<E_ModifiersType>();

        for (int i = 0; i < playerStatsProperty.arraySize; i++)
        {
            SerializedProperty statProperty = playerStatsProperty.GetArrayElementAtIndex(i);
            SerializedProperty modifierTypeProperty = statProperty.FindPropertyRelative("ModifiersType");
            SerializedProperty additiveProperty = statProperty.FindPropertyRelative("Additive");
            SerializedProperty multiplicativeProperty = statProperty.FindPropertyRelative("Multiplicative");

            E_ModifiersType modifierType = (E_ModifiersType)modifierTypeProperty.enumValueIndex;

            // Sprawdzamy, czy enum już wystąpił
            if (existingTypes.Contains(modifierType))
            {
                EditorGUILayout.HelpBox($"Stat '{modifierType}' już istnieje! Usuń duplikat.", MessageType.Warning);
            }
            else
            {
                existingTypes.Add(modifierType);
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(modifierTypeProperty, new GUIContent("Typ modyfikatora"));

            if (GUILayout.Button("Usuń", GUILayout.Width(60)))
            {
                playerStatsProperty.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();

            // Edycja wartości
            EditorGUILayout.LabelField($"Modyfikator: {modifierType}", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(additiveProperty, new GUIContent("Additive"));
            EditorGUILayout.PropertyField(multiplicativeProperty, new GUIContent("Multiplicative"));

            // Przycisk Reset
            if (GUILayout.Button("Reset", GUILayout.Width(100)))
            {
                additiveProperty.floatValue = 0;
                multiplicativeProperty.floatValue = 1;
            }

            EditorGUILayout.EndVertical();
        }

        // Dodawanie nowego statystyk modyfikatora
        EditorGUILayout.Space();

        if (GUILayout.Button("Dodaj nowy stat"))
        {
            playerStatsProperty.arraySize++;
            SerializedProperty newElement = playerStatsProperty.GetArrayElementAtIndex(playerStatsProperty.arraySize - 1);
            newElement.FindPropertyRelative("ModifiersType").enumValueIndex = 0; // Domyślny enum
            newElement.FindPropertyRelative("Additive").floatValue = 0;
            newElement.FindPropertyRelative("Multiplicative").floatValue = 1;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
