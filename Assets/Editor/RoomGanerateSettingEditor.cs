using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RoomGanerateSetting))]
public class RoomGanerateSettingEditor : PropertyDrawer
{
    private bool baseInfoFoldout = true;
    private bool additionalWayFoldout = false;
    private bool secretWayFoldout = false;
    private bool roomsFoldout = false;
    private bool inroomGeneratorFoldout = false;
    private bool doorSpawnFoldout = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty minRoomDistance = property.FindPropertyRelative("MinRoomDistance");
        SerializedProperty maxRoomDistance = property.FindPropertyRelative("MaxRoomDistance");

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        baseInfoFoldout = EditorGUILayout.Foldout(baseInfoFoldout, "Base Info", true);
        if (baseInfoFoldout)
        {
            DrawProperty(property, "MaxRoomSize");
            DrawProperty(property, "MinRoomSize");
            DrawProperty(property, "RoomCount");

            // Custom Min-Max Slider for Room Distance
            DrawMinMaxSlider("Distance Between Rooms", minRoomDistance, maxRoomDistance, 0, 100);

            DrawProperty(property, "MaxAttempts");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        additionalWayFoldout = EditorGUILayout.Foldout(additionalWayFoldout, "Additional Way", true);
        if (additionalWayFoldout)
        {
            DrawProperty(property, "UseAdditionalEdges");
            DrawProperty(property, "AdditionSelectedEdges");
            DrawProperty(property, "ChanceToSelectEdge");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        secretWayFoldout = EditorGUILayout.Foldout(secretWayFoldout, "Secret Way", true);
        if (secretWayFoldout)
        {
            DrawProperty(property, "CreateSecretWay");
            DrawProperty(property, "SecretWayMaxCount");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        roomsFoldout = EditorGUILayout.Foldout(roomsFoldout, "Rooms", true);
        if (roomsFoldout)
        {
            DrawProperty(property, "CreatedRoom");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        inroomGeneratorFoldout = EditorGUILayout.Foldout(inroomGeneratorFoldout, "Inroom Generator", true);
        if (inroomGeneratorFoldout)
        {
            DrawProperty(property, "PlayerPrefab");
            DrawProperty(property, "ObjectToPickList");
            DrawProperty(property, "DepositPointPrefab");
            DrawProperty(property, "InroomObjectLayer");
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        doorSpawnFoldout = EditorGUILayout.Foldout(doorSpawnFoldout, "Door Spawn", true);
        if (doorSpawnFoldout)
        {
            DrawProperty(property, "DoorPrefab");
            DrawProperty(property, "DoorSpawnOffset");
        }

        EditorGUILayout.EndVertical();

        EditorGUI.EndProperty();
    }

    private void DrawProperty(SerializedProperty property, string propertyName)
    {
        SerializedProperty prop = property.FindPropertyRelative(propertyName);
        if (prop != null)
        {
            EditorGUILayout.PropertyField(prop, true);
        }
    }

    private void DrawMinMaxSlider(string label, SerializedProperty minProp, SerializedProperty maxProp, float minLimit,
        float maxLimit)
    {
        float minVal = minProp.intValue;
        float maxVal = maxProp.intValue;

        EditorGUILayout.LabelField(label);
        EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, minLimit, maxLimit);

        minVal = Mathf.Clamp(minVal, minLimit, maxVal); // Ensure min <= max
        maxVal = Mathf.Clamp(maxVal, minVal, maxLimit); // Ensure max >= min

        minProp.intValue = Mathf.RoundToInt(minVal);
        maxProp.intValue = Mathf.RoundToInt(maxVal);

        EditorGUILayout.BeginHorizontal();
        minProp.intValue = EditorGUILayout.IntField("Min", minProp.intValue);
        maxProp.intValue = EditorGUILayout.IntField("Max", maxProp.intValue);
        EditorGUILayout.EndHorizontal();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}