using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListenerMethodFinder : EditorWindow
{
    private string methodName = "";
    private List<GameEventListener> foundListeners = new List<GameEventListener>();
    private Dictionary<GameEventListener, List<Component>> relatedComponents = new Dictionary<GameEventListener, List<Component>>();

    [MenuItem("Tools/Find GameEvent Listeners by Method")]
    public static void ShowWindow()
    {
        GetWindow<GameEventListenerMethodFinder>("GameEvent Listener Finder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Find GameEvent Listeners by Method", EditorStyles.boldLabel);
        methodName = EditorGUILayout.TextField("Method Name:", methodName);

        if (GUILayout.Button("Find Listeners") && !string.IsNullOrEmpty(methodName))
        {
            FindListenersByMethod();
        }

        if (foundListeners.Count > 0)
        {
            GUILayout.Label("Found Listeners:", EditorStyles.boldLabel);
            foreach (var listener in foundListeners)
            {
                EditorGUILayout.ObjectField(listener.gameObject.name, listener, typeof(GameEventListener), true);
                if (relatedComponents.ContainsKey(listener))
                {
                    GUILayout.Label("Related Components:");
                    foreach (var component in relatedComponents[listener])
                    {
                        EditorGUILayout.ObjectField(component.gameObject.name, component, typeof(Component), true);
                    }
                }
            }
        }
        else if (!string.IsNullOrEmpty(methodName))
        {
            GUILayout.Label("No listeners found using the specified method.");
        }
    }

    private void FindListenersByMethod()
    {
        foundListeners.Clear();
        relatedComponents.Clear();
        GameEventListener[] listeners = FindObjectsOfType<GameEventListener>();
        
        foreach (var listener in listeners)
        {
            int eventCount = listener.response.GetPersistentEventCount();
            for (int i = 0; i < eventCount; i++)
            {
                Object target = listener.response.GetPersistentTarget(i);
                string method = listener.response.GetPersistentMethodName(i);
                
                if (method == methodName)
                {
                    foundListeners.Add(listener);
                    FindRelatedComponents(listener);
                    break;
                }
            }
        }
    }

    private void FindRelatedComponents(GameEventListener listener)
    {
        List<Component> components = new List<Component>();
        Component[] allComponents = FindObjectsOfType<Component>();

        foreach (var component in allComponents)
        {
            SerializedObject serializedObject = new SerializedObject(component);
            SerializedProperty prop = serializedObject.GetIterator();
            while (prop.NextVisible(true))
            {
                if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceValue == listener.gameEvent)
                {
                    components.Add(component);
                    break;
                }
            }
        }

        if (components.Count > 0)
        {
            relatedComponents[listener] = components;
        }
    }
}
