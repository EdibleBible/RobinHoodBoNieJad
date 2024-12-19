using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(MapGeneratorController))]
public class MapGeneratorControllerEditor : Editor
{
    private bool showTriangulation = false;
    private bool previousShowTriangulation = false;

    private bool showUnusedEdges = false;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapGeneratorController generatorController = (MapGeneratorController)target;

        if (GUILayout.Button("Generate Random Grid"))
        {
            GenerateGrid(generatorController, randomSize: true);
            generatorController.GenerateTriangulation();
            generatorController.SelectedEdges = generatorController.GetUsedEgdes(generatorController.AllEdges, generatorController.AllPoints);
        }

        if (GUILayout.Button("Generate Custom Grid Size"))
        {
            ShowGridSizeWindow(generatorController);
        }

        if (GUILayout.Button("Pathfind Debug"))
        {
            var selectedEdge = generatorController.SelectedEdges[0];
            generatorController.RoomPathFind();
            generatorController.SetupHallwayDebugMesh();
        }

        showTriangulation = GUILayout.Toggle(showTriangulation, "Show Triangulation");
        showUnusedEdges = GUILayout.Toggle(showUnusedEdges, "show Unused Edges");

        // Sprawdzenie, czy stan się zmienił
        if (showTriangulation != previousShowTriangulation)
        {
            if (showTriangulation)
            {
                SceneView.duringSceneGui -= OnSceneGUI;
                SceneView.duringSceneGui += OnSceneGUI;
                generatorController.GenerateTriangulation();
                generatorController.SelectedEdges = generatorController.GetUsedEgdes(generatorController.AllEdges, generatorController.AllPoints);

            }
            else
            {
                SceneView.duringSceneGui -= OnSceneGUI;

                generatorController.AllEdges = null;
                generatorController.SelectedEdges = null;
                generatorController.AllPoints = null;

            }

            // Aktualizacja poprzedniego stanu
            previousShowTriangulation = showTriangulation;
        }
    }
    private void ShowGridSizeWindow(MapGeneratorController generatorController)
    {
        GridSizeWindow.Open(generatorController);
    }
    private void GenerateGrid(MapGeneratorController generatorController, bool randomSize)
    {
        ClearGeneratedGrid(generatorController);

        if (randomSize)
        {
            generatorController.GenerateGrid();
            generatorController.RoomGanerateSetting.CreateRoomsOnGrid(generatorController.MainInfoGrid);
        }

        generatorController.DebugGridMesh();
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
        if (!(target is MapGeneratorController generatorController) || generatorController.Triangulator == null)
            return;

        // Rysowanie całej triangulacji, jeśli showTriangulation jest true

        Handles.color = Color.green;  // Ustaw kolor na zielony dla triangulacji

        if (generatorController.SelectedEdges == null)
        {
            Debug.LogError("Selected Edges is null");
            return;
        }

        foreach (Edge edge in generatorController.SelectedEdges)
        {
            Vector3 v1 = new Vector3((float)edge.Point1.X, 1, (float)(float)edge.Point1.Y);
            Vector3 v2 = new Vector3((float)edge.Point2.X, 1, (float)(float)edge.Point2.Y);
            Handles.DrawLine(v1, v2);
        }

        if (showUnusedEdges)
        {
            Handles.color = Color.red;

            foreach (Edge edge in generatorController.AllEdges)
            {
                if (generatorController.SelectedEdges.Contains(edge))
                    continue;

                Vector3 v1 = new Vector3((float)edge.Point1.X, 1, (float)(float)edge.Point1.Y);
                Vector3 v2 = new Vector3((float)edge.Point2.X, 1, (float)(float)edge.Point2.Y);
                Handles.DrawLine(v1, v2);
            }
        }

        generatorController.SetupPassDebugMesh();
    }
}