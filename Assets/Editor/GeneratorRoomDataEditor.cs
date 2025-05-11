using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

[CustomEditor(typeof(GeneratorRoomData))]
public class GeneratorRoomDataEditor : Editor
{
    private int fromLayer1Index = 0;
    private int toLayer1Index = 0;
    private int fromLayer2Index = 0;
    private int toLayer2Index = 0;

    private bool initialized = false;

    private void OnEnable()
    {
        if (initialized) return;
        initialized = true;

        string[] layerNames = InternalEditorUtility.layers;

        fromLayer1Index = System.Array.IndexOf(layerNames, "Floor");
        toLayer1Index = System.Array.IndexOf(layerNames, "FloorSpawn");
        fromLayer2Index = System.Array.IndexOf(layerNames, "WallPass");
        toLayer2Index = System.Array.IndexOf(layerNames, "WallPassSpawn");

        // Fallback na 0 jeśli warstwy nie istnieją
        if (fromLayer1Index < 0) fromLayer1Index = 0;
        if (toLayer1Index < 0) toLayer1Index = 0;
        if (fromLayer2Index < 0) fromLayer2Index = 0;
        if (toLayer2Index < 0) toLayer2Index = 0;
    }

    public override void OnInspectorGUI()
    {
        GeneratorRoomData generator = (GeneratorRoomData)target;

        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Ustawienia przypisywania:", EditorStyles.boldLabel);

        string[] layerNames = InternalEditorUtility.layers;
        int mask = LayerMaskToMask(generator.floorLayers, layerNames);
        mask = EditorGUILayout.MaskField("Warstwy podłóg (do przypisania)", mask, layerNames);
        generator.floorLayers = MaskToLayerMask(mask, layerNames);

        if (GUILayout.Button("Wyszukaj podłogi"))
        {
            generator.AllFloors.Clear();
            Transform[] allTransforms = generator.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in allTransforms)
            {
                int objLayer = t.gameObject.layer;
                if (((1 << objLayer) & generator.floorLayers.value) != 0)
                {
                    generator.AllFloors.Add(t);
                }
            }

            EditorUtility.SetDirty(generator);
            Debug.Log("Zaktualizowano AllFloors.");
        }

        if (GUILayout.Button("Wyszukaj drzwi"))
        {
            generator.AllDoors.Clear();
            Transform[] allTransforms = generator.GetComponentsInChildren<Transform>(true);

            foreach (Transform t in allTransforms)
            {
                DoorController door = t.GetComponent<DoorController>();
                if (door != null)
                {
                    generator.AllDoors.Add(door);
                }
            }

            EditorUtility.SetDirty(generator);
            Debug.Log("Zaktualizowano AllDoors.");
        }
        
        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Ustawianie pozycji spawnu:", EditorStyles.boldLabel);

        if (GUILayout.Button("Ustaw SpawnPosition z obiektu 'Spawn'"))
        {
            Transform[] allTransforms = generator.GetComponentsInChildren<Transform>(true);
            Transform spawnTransform = null;

            foreach (Transform t in allTransforms)
            {
                if (string.Equals(t.name, "Spawn", System.StringComparison.OrdinalIgnoreCase))
                {
                    spawnTransform = t;
                    break;
                }
            }

            if (spawnTransform != null)
            {
                generator.SpawnPosition = spawnTransform;
                Debug.Log($"Ustawiono SpawnPosition na {spawnTransform.position}");
            }
            else
            {
                generator.SpawnPosition = null;
                Debug.LogWarning("Nie znaleziono obiektu o nazwie 'Spawn'. Ustawiono (0, 0, 0).");
            }

            EditorUtility.SetDirty(generator);
        }

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Zamiana warstw obiektów:", EditorStyles.boldLabel);
        
        

        // Sekcja dla pary 1
        EditorGUILayout.LabelField("Zamiana 1:", EditorStyles.miniBoldLabel);
        fromLayer1Index = EditorGUILayout.Popup("Zamień warstwę", fromLayer1Index, layerNames);
        toLayer1Index = EditorGUILayout.Popup("Na warstwę", toLayer1Index, layerNames);

        EditorGUILayout.Space(5);

        // Sekcja dla pary 2
        EditorGUILayout.LabelField("Zamiana 2:", EditorStyles.miniBoldLabel);
        fromLayer2Index = EditorGUILayout.Popup("Zamień warstwę", fromLayer2Index, layerNames);
        toLayer2Index = EditorGUILayout.Popup("Na warstwę", toLayer2Index, layerNames);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("Zamień warstwy"))
        {
            string fromLayer1 = layerNames[fromLayer1Index];
            string toLayer1 = layerNames[toLayer1Index];
            string fromLayer2 = layerNames[fromLayer2Index];
            string toLayer2 = layerNames[toLayer2Index];

            int from1 = LayerMask.NameToLayer(fromLayer1);
            int to1 = LayerMask.NameToLayer(toLayer1);
            int from2 = LayerMask.NameToLayer(fromLayer2);
            int to2 = LayerMask.NameToLayer(toLayer2);

            Transform[] allTransforms = generator.GetComponentsInChildren<Transform>(true);
            int changed = 0;

            foreach (Transform t in allTransforms)
            {
                if (t.gameObject.layer == from1)
                {
                    Undo.RecordObject(t.gameObject, "Zmień warstwę");
                    t.gameObject.layer = to1;
                    changed++;
                }
                else if (t.gameObject.layer == from2)
                {
                    Undo.RecordObject(t.gameObject, "Zmień warstwę");
                    t.gameObject.layer = to2;
                    changed++;
                }
            }

            EditorUtility.SetDirty(generator);
            Debug.Log($"Zmieniono warstwę dla {changed} obiektów.");
        }
    }

    private int LayerMaskToMask(LayerMask layerMask, string[] layerNames)
    {
        int mask = 0;
        for (int i = 0; i < layerNames.Length; i++)
        {
            int layer = LayerMask.NameToLayer(layerNames[i]);
            if ((layerMask.value & (1 << layer)) != 0)
            {
                mask |= (1 << i);
            }
        }
        return mask;
    }

    private LayerMask MaskToLayerMask(int mask, string[] layerNames)
    {
        int layerMask = 0;
        for (int i = 0; i < layerNames.Length; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                int layer = LayerMask.NameToLayer(layerNames[i]);
                layerMask |= (1 << layer);
            }
        }
        return layerMask;
    }
}
