using Codice.CM.Common;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(MapGeneratorController))]
public class MapGeneratorControllerEditor : Editor
{
    private bool showTriangulation = true;
    private float delay = 0.5f;  // Czas opóźnienia w sekundach
    private float timeElapsed = 0f;
    private int currentStep = 0;
    private bool isGenerating = false;  // Flaga kontrolująca, czy proces generowania jest w trakcie

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MapGeneratorController generatorController = (MapGeneratorController)target;

        // Dodaj checkbox, aby użytkownik mógł włączyć/wyłączyć pokazanie triangulacji
        showTriangulation = EditorGUILayout.Toggle("Show Triangulation", showTriangulation);

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

        generatorController.RoomGanerateSetting.CreateRoomsOnGrid(generatorController.MainInfoGrid, seed);

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

        generatorController.RoomPathFind();

        generatorController.DefiniedSpawn();

        generatorController.DebugGridMesh();

        generatorController.GenerateTexture();

        // Wymuś odświeżenie widoku, aby linie się pojawiły
        SceneView.RepaintAll();
    }



    /*    private void UpdateGeneration()
        {
            // Jeśli generowanie nie jest aktywne, zakończ metodę
            if (!isGenerating)
                return;

            // Aktualizacja czasu
            timeElapsed += Time.deltaTime;

            MapGeneratorController generatorController = (MapGeneratorController)target;

            // Sprawdzenie, czy czas opóźnienia został osiągnięty
            if (timeElapsed >= delay)
            {
                // Obsługa poszczególnych kroków
                switch (currentStep)
                {
                    case 1:
                        generatorController.GenerateGrid(seed);
                        generatorController.DebugGridMesh();
                        break;

                    case 2:
                        generatorController.RoomGanerateSetting.CreateRoomsOnGrid(generatorController.MainInfoGrid, seed);
                        generatorController.DebugGridMesh();
                        break;

                    case 3:
                        generatorController.GenerateTriangulation();
                        generatorController.DebugGridMesh();
                        break;

                    case 4:
                        generatorController.SelectedEdges = generatorController.GetUsedEdges(generatorController.AllEdges, generatorController.AllPoints);
                        generatorController.DebugGridMesh();
                        break;

                    case 5:
                        if (showTriangulation)
                        {
                            SceneView.duringSceneGui += OnSceneGUI;
                        }
                        else
                        {
                            SceneView.duringSceneGui -= OnSceneGUI;
                        }
                        break;

                    case 6:
                        generatorController.RoomPathFindWithDebugging();
                        break;

                    case 7:
                        generatorController.DefiniedSpawn();
                        generatorController.DebugGridMesh();
                        break;

                    case 8:
                        Debug.Log("Generation completed!");
                        break;

                    case 9:
                        // Odświeżenie widoku edytora
                        SceneView.RepaintAll();
                        // Zatrzymanie generowania
                        isGenerating = false;
                        EditorApplication.update -= UpdateGeneration;
                        break;

                    default:
                        Debug.LogWarning($"Unhandled generation step: {currentStep}");
                        isGenerating = false;
                        EditorApplication.update -= UpdateGeneration;
                        break;
                }

                // Zwiększ krok
                currentStep++;
                timeElapsed = 0f;  // Resetuj licznik czasu
            }
        }
        public void TimedGeneration(MapGeneratorController generatorController)
        {
            // Sprawdź, czy proces generowania już trwa
            if (isGenerating)
                return;

            // Rozpocznij proces generowania z opóźnieniem
            isGenerating = true;
            currentStep = 1;  // Zaczynaj od pierwszego kroku
            timeElapsed = 0f;  // Resetuj licznik czasu

            // Wywołaj generowanie z opóźnieniem
            GenerateWithDelay(generatorController);  // <--- Tutaj wywołujemy metodę GenerateWithDelay

            // Zarejestruj metodę update, która będzie wywoływana w każdej klatce edytora
            EditorApplication.update += UpdateGeneration;
        }
        private void GenerateWithDelay(MapGeneratorController generatorController)
        {
            // Wyczyść poprzednie dane (jeśli jakieś istnieją)
            ClearGeneratedGrid(generatorController);

            // Możesz dodać dodatkowe logowanie lub inne działania w tej metodzie
            Debug.Log("Starting delayed generation...");

            // Można dodać dodatkową inicjalizację, jeżeli jest potrzeba
        }*/
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
    }
}




/*    private bool showTriangulation = false;
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
            generatorController.SelectedEdges = generatorController.GetUsedEdges(generatorController.AllEdges, generatorController.AllPoints);
        }

        if (GUILayout.Button("Generate Custom Grid Size"))
        {
            ShowGridSizeWindow(generatorController);
        }

        if (GUILayout.Button("Pathfind Debug"))
        {
            var selectedEdge = generatorController.SelectedEdges[0];
            generatorController.RoomPathFind();
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
                generatorController.SelectedEdges = generatorController.GetUsedEdges(generatorController.AllEdges, generatorController.AllPoints);

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
    }*/
