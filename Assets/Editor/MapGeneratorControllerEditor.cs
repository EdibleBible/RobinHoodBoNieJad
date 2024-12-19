using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
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

        if (GUILayout.Button("Create Edge MST"))
        {
            if (generatorController.AllEdges == null || generatorController.AllPoints == null)
            {
                Debug.LogError($"Edges or points are null");
                return;
            }
            generatorController.SelectedEdges = generatorController.GetUsedEgdes(generatorController.AllEdges, generatorController.AllPoints);
        }

        if (GUILayout.Button("Debug Room on Point"))
        {
            foreach(var point in generatorController.AllPoints)
            {
                Debug.Log($"{point.PointRoom.cetroid} in room id: {point.PointRoom.RoomID}" );
            }
        }

        // Twój przycisk Toggle
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
            generatorController.MainGridData.GenerateEmptyGrid();
            generatorController.RoomGanerateSetting.CreateRoomsOnGrid(generatorController.MainGridData);
        }

        generatorController.GenerateDebugMesh();
    }

    private void ClearGeneratedGrid(MapGeneratorController generatorController)
    {
        for (int i = generatorController.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = generatorController.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        generatorController.MainGridData.AllGridCell.Clear();
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

        if(showUnusedEdges)
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

        /*        foreach (var triangle in generatorController.GenerateTriangulation())
                {
                    Vector3 v1 = new Vector3((float)triangle.Vertices[0].X, 1, (float)triangle.Vertices[0].Y);
                    Vector3 v2 = new Vector3((float)triangle.Vertices[1].X, 1, (float)triangle.Vertices[1].Y);
                    Vector3 v3 = new Vector3((float)triangle.Vertices[2].X, 1, (float)triangle.Vertices[2].Y);

                    // Rysowanie krawędzi trójkątów
                    Handles.DrawLine(v1, v2);
                    Handles.DrawLine(v2, v3);
                    Handles.DrawLine(v3, v1);

                    // Rysowanie wierzchołków
                    Handles.color = Color.yellow;  // Ustaw kolor na żółty dla wierzchołków
                    Handles.SphereHandleCap(0, v1, Quaternion.identity, 0.2f, EventType.Repaint);
                    Handles.SphereHandleCap(0, v2, Quaternion.identity, 0.2f, EventType.Repaint);
                    Handles.SphereHandleCap(0, v3, Quaternion.identity, 0.2f, EventType.Repaint);
                }

                // Rysowanie krawędzi w MST (Minimal Spanning Tree), jeśli SelectedEdges są ustawione
                if (generatorController.SelectedEdges != null && generatorController.SelectedEdges.Count > 0)
                {
                    foreach (var edge in generatorController.SelectedEdges)
                    {
                        Vector3 v1 = new Vector3((float)edge.Point1.X, 1, (float)edge.Point1.Y);
                        Vector3 v2 = new Vector3((float)edge.Point2.X, 1, (float)edge.Point2.Y);

                        Handles.color = Color.red;  // Kolor czerwony dla krawędzi w MST
                        Handles.DrawLine(v1, v2);
                    }
                }

                // Rysowanie niewykorzystanych krawędzi, jeśli showUnusedEdges jest true
                if (showUnusedEdges)
                {
                    foreach (var edge in generatorController.AllEdges)
                    {
                        // Sprawdzamy, czy krawędź nie jest częścią MST
                        if (!generatorController.SelectedEdges.Contains(edge))
                        {
                            Vector3 v1 = new Vector3((float)edge.Point1.X, 1, (float)edge.Point1.Y);
                            Vector3 v2 = new Vector3((float)edge.Point2.X, 1, (float)edge.Point2.Y);

                            Handles.color = Color.gray;  // Kolor szary dla niewykorzystanych krawędzi
                            Handles.DrawLine(v1, v2);
                        }
                    }
                }*/

        SceneView.RepaintAll();
    }
    public class EdgeDrawingInfo
    {
        public Edge Edge { get; }
        public Color Color { get; }

        public EdgeDrawingInfo(Edge edge, Color color)
        {
            Edge = edge;
            Color = color;
        }
    }
}
