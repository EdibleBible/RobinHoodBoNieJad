using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Controller))]
public class ControllerEditor : Editor
{
    private ReorderableList reorderableList;
    private List<bool> foldouts = new();

    private void OnEnable()
    {
        SerializedProperty listProp = serializedObject.FindProperty("elements");

        reorderableList = new ReorderableList(serializedObject, listProp, true, true, true, true);

        reorderableList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Elements");
        };

        reorderableList.onAddCallback = (ReorderableList list) =>
        {
            list.serializedProperty.arraySize++;
            list.index = list.serializedProperty.arraySize - 1;
            list.serializedProperty.GetArrayElementAtIndex(list.index).objectReferenceValue = null;
            foldouts.Add(false);
        };

        reorderableList.onRemoveCallback = (ReorderableList list) =>
        {
            int index = list.index;
            list.serializedProperty.DeleteArrayElementAtIndex(index);
            if (foldouts.Count > index) foldouts.RemoveAt(index);
        };

        reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty elementProp = reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            Element element = elementProp.objectReferenceValue as Element;

            if (foldouts.Count <= index)
                foldouts.Add(false);

            Rect foldoutRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            Rect objectFieldRect = new Rect(rect.x + 15, rect.y + EditorGUIUtility.singleLineHeight + 2, rect.width - 15, EditorGUIUtility.singleLineHeight);

            foldouts[index] = EditorGUI.Foldout(foldoutRect, foldouts[index], $"Element {index + 1}: {(element ? element.name : "None")}", true);

            EditorGUI.BeginChangeCheck();
            elementProp.objectReferenceValue = EditorGUI.ObjectField(objectFieldRect, element, typeof(Element), false);
            if (EditorGUI.EndChangeCheck())
            {
                reorderableList.serializedProperty.serializedObject.ApplyModifiedProperties();
            }

            if (foldouts[index] && element != null)
            {
                SerializedObject elementSerialized = new SerializedObject(element);
                elementSerialized.Update();

                Rect fontRect = new Rect(rect.x + 30, rect.y + 2 * EditorGUIUtility.singleLineHeight + 4, rect.width - 30, EditorGUIUtility.singleLineHeight);
                Rect imageRect = new Rect(rect.x + 30, rect.y + 3 * EditorGUIUtility.singleLineHeight + 6, rect.width - 30, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(fontRect, elementSerialized.FindProperty("fontSize"));
                EditorGUI.PropertyField(imageRect, elementSerialized.FindProperty("image"));

                elementSerialized.ApplyModifiedProperties();
            }
        };

        reorderableList.elementHeightCallback = (int index) =>
        {
            bool expanded = foldouts.Count > index && foldouts[index];
            return expanded ? EditorGUIUtility.singleLineHeight * 4 + 10 : EditorGUIUtility.singleLineHeight * 2 + 4;
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
