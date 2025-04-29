using UnityEditor;
using UnityEngine;
using System.IO;

public class MoveAndOrganizeRoomPrefabs
{
    [MenuItem("Tools/Move And Organize Room Prefabs")]
    public static void MoveAndOrganizePrefabs()
    {
        string sourceFolder = "Assets/_Prefabs";
        string targetBaseFolder = "Assets/StreamingAssets/RoomsPrefabs";

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { sourceFolder });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string prefabName = Path.GetFileNameWithoutExtension(assetPath);
            string newFolderPath = Path.Combine(targetBaseFolder, prefabName);

            // Utwórz folder docelowy, jeśli nie istnieje
            if (!AssetDatabase.IsValidFolder(newFolderPath))
            {
                AssetDatabase.CreateFolder(targetBaseFolder, prefabName);
            }

            string newAssetPath = Path.Combine(newFolderPath, Path.GetFileName(assetPath));

            // Przenieś plik prefab
            string moveResult = AssetDatabase.MoveAsset(assetPath, newAssetPath);
            if (!string.IsNullOrEmpty(moveResult))
            {
                Debug.LogError($"Błąd przenoszenia {assetPath} -> {newAssetPath}: {moveResult}");
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Przenoszenie i organizacja prefabów zakończona.");
    }
}