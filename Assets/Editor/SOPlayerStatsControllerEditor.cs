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

        EditorGUILayout.LabelField("★ Aktualne Modyfikatory Gracza", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        for (int i = 0; i < playerStatsProperty.arraySize; i++)
        {
            SerializedProperty statProperty = playerStatsProperty.GetArrayElementAtIndex(i);
            SerializedProperty modifierTypeProperty = statProperty.FindPropertyRelative("ModifiersType");
            SerializedProperty additiveProperty = statProperty.FindPropertyRelative("Additive");
            SerializedProperty multiplicativeProperty = statProperty.FindPropertyRelative("Multiplicative");

            // Poprawione: zgodne nazwy
            SerializedProperty levelCountProperty = statProperty.FindPropertyRelative("CurrLevel");
            SerializedProperty upgradeCostProperty = statProperty.FindPropertyRelative("UpgradeBaseCost");
            SerializedProperty currentUpgradeCostProperty = statProperty.FindPropertyRelative("UpgradeCurrCost");
            SerializedProperty upgradeValueProperty = statProperty.FindPropertyRelative("statsToUpgrade");
            SerializedProperty iconProperty = statProperty.FindPropertyRelative("Icon");

            E_ModifiersType modifierType = (E_ModifiersType)modifierTypeProperty.enumValueIndex;

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

            EditorGUILayout.LabelField($"Modyfikator: {modifierType}", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(additiveProperty, new GUIContent("Additive"));
            EditorGUILayout.PropertyField(multiplicativeProperty, new GUIContent("Multiplicative"));

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Ulepszenia", EditorStyles.miniBoldLabel);

            EditorGUILayout.PropertyField(levelCountProperty, new GUIContent("Level Count"));
            EditorGUILayout.PropertyField(upgradeCostProperty, new GUIContent("Upgrade Cost"));

            // Wylicz aktualny koszt
            currentUpgradeCostProperty.intValue = upgradeCostProperty.intValue * levelCountProperty.intValue;
            EditorGUILayout.LabelField($"Current Upgrade Cost:{currentUpgradeCostProperty.intValue}");

            EditorGUILayout.PropertyField(upgradeValueProperty, new GUIContent("Upgrade Value"), true);
            EditorGUILayout.PropertyField(iconProperty, new GUIContent("Ikona"));

            if (GUILayout.Button("Reset", GUILayout.Width(100)))
            {
                additiveProperty.floatValue = 0;
                multiplicativeProperty.floatValue = 1;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Dodaj nowy stat"))
        {
            playerStatsProperty.arraySize++;
            SerializedProperty newElement =
                playerStatsProperty.GetArrayElementAtIndex(playerStatsProperty.arraySize - 1);
            newElement.FindPropertyRelative("ModifiersType").enumValueIndex = 0;
            newElement.FindPropertyRelative("Additive").floatValue = 0;
            newElement.FindPropertyRelative("Multiplicative").floatValue = 1;
            newElement.FindPropertyRelative("CurrLevel").intValue = 1;
            newElement.FindPropertyRelative("UpgradeBaseCost").intValue = 10;
            newElement.FindPropertyRelative("UpgradeCurrCost").intValue = 10; // będzie i tak nadpisany automatycznie
        }


        // ====== PlayerBaseModifiers ======
        SerializedProperty baseModifiersProperty = serializedObject.FindProperty("PlayerBaseModifiers");

        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("★ Bazowe Wartości (PlayerBaseModifiers)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        for (int i = 0; i < baseModifiersProperty.arraySize; i++)
        {
            SerializedProperty baseProperty = baseModifiersProperty.GetArrayElementAtIndex(i);
            SerializedProperty baseType = baseProperty.FindPropertyRelative("ModifiersType");
            SerializedProperty baseAdd = baseProperty.FindPropertyRelative("Additive");
            SerializedProperty baseMult = baseProperty.FindPropertyRelative("Multiplicative");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(baseType, new GUIContent("Typ modyfikatora"));

            if (GUILayout.Button("Usuń", GUILayout.Width(60)))
            {
                baseModifiersProperty.DeleteArrayElementAtIndex(i);
                break;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(baseAdd, new GUIContent("Additive"));
            EditorGUILayout.PropertyField(baseMult, new GUIContent("Multiplicative"));

            if (GUILayout.Button("Reset", GUILayout.Width(100)))
            {
                baseAdd.floatValue = 0;
                baseMult.floatValue = 1;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Dodaj bazowy stat"))
        {
            baseModifiersProperty.arraySize++;
            SerializedProperty newBase =
                baseModifiersProperty.GetArrayElementAtIndex(baseModifiersProperty.arraySize - 1);
            newBase.FindPropertyRelative("ModifiersType").enumValueIndex = 0;
            newBase.FindPropertyRelative("Additive").floatValue = 0;
            newBase.FindPropertyRelative("Multiplicative").floatValue = 1;
        }

        EditorGUILayout.Space(10);
        if (GUILayout.Button("Resetuj wszystkie bazowe modyfikatory"))
        {
            statsController.ClearPlayerBaseModifiers();
        }

        serializedObject.ApplyModifiedProperties();
    }
}