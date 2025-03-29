using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameEventListenerFinder : EditorWindow
{
    private GameEvent targetEvent;
    private List<GameEventListener> foundListeners = new List<GameEventListener>();

    [MenuItem("Tools/Find GameEvent Listeners")]
    public static void ShowWindow()
    {
        GetWindow<GameEventListenerFinder>("GameEvent Listener Finder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Find GameEvent Listeners", EditorStyles.boldLabel);

        targetEvent = (GameEvent)EditorGUILayout.ObjectField("Target GameEvent:", targetEvent, typeof(GameEvent), false);

        if (GUILayout.Button("Find Listeners"))
        {
            FindListeners();
        }

        if (foundListeners.Count > 0)
        {
            GUILayout.Label("Found Listeners:", EditorStyles.boldLabel);
            foreach (var listener in foundListeners)
            {
                EditorGUILayout.ObjectField(listener.gameObject.name, listener, typeof(GameEventListener), true);
            }
        }
        else if (targetEvent != null)
        {
            GUILayout.Label("No listeners found.");
        }
    }

    private void FindListeners()
    {
        foundListeners.Clear();
        GameEventListener[] listeners = FindObjectsOfType<GameEventListener>();
        
        foreach (var listener in listeners)
        {
            if (listener.gameEvent == targetEvent)
            {
                foundListeners.Add(listener);
            }
        }
    }
}