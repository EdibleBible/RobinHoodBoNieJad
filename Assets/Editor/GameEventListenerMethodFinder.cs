using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public class MethodFinderEditor : EditorWindow
{
    private string methodNameToFind = "";
    private List<GameObject> foundObjects = new List<GameObject>();

    [MenuItem("Window/Method Finder")]
    public static void ShowWindow()
    {
        GetWindow<MethodFinderEditor>("Method Finder");
    }

    void OnGUI()
    {
        GUILayout.Label("Find Method References in Scene", EditorStyles.boldLabel);
        
        methodNameToFind = EditorGUILayout.TextField("Method Name", methodNameToFind);

        if (GUILayout.Button("Find References"))
        {
            FindMethodReferences();
        }

        GUILayout.Space(10);

        if (foundObjects.Count > 0)
        {
            GUILayout.Label($"Found {foundObjects.Count} GameObject(s):", EditorStyles.boldLabel);
            foreach (var obj in foundObjects)
            {
                if (GUILayout.Button(obj.name, EditorStyles.linkLabel))
                {
                    Selection.activeGameObject = obj;
                }
            }
        }
        else if (!string.IsNullOrEmpty(methodNameToFind))
        {
            GUILayout.Label("No references found.", EditorStyles.miniLabel);
        }
    }

    void FindMethodReferences()
    {
        foundObjects.Clear();
        foreach (var go in FindObjectsOfType<GameObject>(true)) // Przeszukuje wszystkie obiekty, także nieaktywne
        {
            foreach (var component in go.GetComponents<MonoBehaviour>())
            {
                if (component == null) continue;

                foreach (var field in component.GetType().GetFields())
                {
                    if (typeof(UnityEventBase).IsAssignableFrom(field.FieldType))
                    {
                        UnityEventBase unityEvent = field.GetValue(component) as UnityEventBase;
                        if (unityEvent != null && HasMethodReference(unityEvent, methodNameToFind))
                        {
                            foundObjects.Add(go);
                        }
                    }
                }
            }
        }

        Repaint(); // Odświeża okno edytora
    }

    private bool HasMethodReference(UnityEventBase unityEvent, string methodName)
    {
        int count = unityEvent.GetPersistentEventCount();
        for (int i = 0; i < count; i++)
        {
            if (unityEvent.GetPersistentMethodName(i) == methodName)
                return true;
        }
        return false;
    }
}
