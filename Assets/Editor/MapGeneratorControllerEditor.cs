using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(MapGeneratorController))]
public class MapGeneratorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGeneratorController controller = (MapGeneratorController)target;
        DrawDefaultInspector();
        
        if (GUILayout.Button("Generate With Random"))
        {
            controller.GenerateMap(true);
        }

        if (GUILayout.Button("Generate"))
        {
            controller.GenerateMap(false);
        }
    }
}

#region old
    /*private bool showTriangulation = true;
    private bool showGrid = true;
    private bool showTexture = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapGeneratorController generatorController = (MapGeneratorController)target;

        // Dodaj checkbox, aby użytkownik mógł włączyć/wyłączyć pokazanie triangulacji
        showTriangulation = EditorGUILayout.Toggle("Show Triangulation", showTriangulation);
        showGrid = EditorGUILayout.Toggle("Show Grid", showGrid);
        showTexture = EditorGUILayout.Toggle("Show Texture", showTexture);

        if (GUILayout.Button("Generate With Random"))
        {
            Generate(generatorController);
        }

        if (GUILayout.Button("Generate"))
        {
            Generate(generatorController, generatorController.wallSeed);
        }

    }

    private void Generate(MapGeneratorController generatorController, uint senedSeed = 0)
    {
        uint seed = 0;
        if (senedSeed != 0)
            seed = generatorController.SetSeed(generatorController.wallSeed);
        else
        {
            seed = generatorController.RandimizeSeed();
        }


        ClearGeneratedGrid(generatorController);

        generatorController.GenerateGrid(seed);

        generatorController.RoomGanerateSetting.CreateRoomsOnGrid(generatorController.MainInfoGrid, seed, generatorController.generatedRoomTransform);

        generatorController.GenerateTriangulation();

        generatorController.SelectedEdges = generatorController.GetUsedEdges(generatorController.AllEdges, generatorController.AllPoints);

        // Rejestracja i odinstalowanie OnSceneGUI w zależności od showTriangulation
        if (showTriangulation)
        {
            // Zarejestruj metodę bezpośrednio
            SceneView.duringSceneGui += OnSceneGUI;
        }
        else
        {
            // Usuń metodę
            SceneView.duringSceneGui -= OnSceneGUI;
        }
        generatorController.Hallwaycell = new List<GridCellData>();

        generatorController.RoomPathFind();

        generatorController.DefiniedSpawn();

        generatorController.DebugGridMesh();

        if (!showGrid)
        {
            foreach (var element in generatorController.allObject)
            {
                element.Value.SetActive(false);
            }
        }

        if (showTexture)
            generatorController.GenerateTexture();

        // Wymuś odświeżenie widoku, aby linie się pojawiły
        SceneView.RepaintAll();
    }
    private void ClearGeneratedGrid(MapGeneratorController generatorController)
    {
        for (int i = generatorController.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = generatorController.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        generatorController.MainInfoGrid = null;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        MapGeneratorController generatorController = target as MapGeneratorController;

        if (generatorController?.Triangulator == null || !showTriangulation || generatorController.SelectedEdges == null || generatorController.SelectedEdges.Count == 0)
            return;

        Handles.color = Color.green;  // Ustaw kolor na zielony dla triangulacji

        foreach (Edge edge in generatorController.SelectedEdges)
        {
            Vector3 v1 = new Vector3((float)edge.Point1.X, 1, (float)edge.Point1.Y);
            Vector3 v2 = new Vector3((float)edge.Point2.X, 1, (float)edge.Point2.Y);
            Handles.DrawLine(v1, v2);
        }
    }*/
#endregion