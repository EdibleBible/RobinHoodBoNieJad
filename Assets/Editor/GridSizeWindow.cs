using UnityEditor;
using UnityEngine;

public class GridSizeWindow : EditorWindow
{
/*    private int width = 10;  // Domyślna szerokość siatki
    private int height = 10; // Domyślna wysokość siatki
    private int seed = 100;
    

    private MapGeneratorController generatorController;

    public static void Open(MapGeneratorController controller)
    {
        // Otwiera okno z parametrami w MapGeneratorController
        GridSizeWindow window = GetWindow<GridSizeWindow>("Set Grid Size");
        window.generatorController = controller;
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Custom Grid Size", EditorStyles.boldLabel);

        // Pole do ustawienia szerokości i wysokości siatki
        width = EditorGUILayout.IntField("Width", width);
        height = EditorGUILayout.IntField("Height", height);
        seed = EditorGUILayout.IntField("Seed", seed);

        // Przyciski do generowania siatki
        if (GUILayout.Button("Generate Grid"))
        {
            if (generatorController != null)
            {
                ClearGeneratedGrid(generatorController);

                // Ustawienie rozmiaru siatki na podstawie wprowadzonych wartości
                generatorController.GenerateGrid(width, height);

                // Dodanie pokoi na siatkę
                generatorController.RoomGanerateSetting.CreateRoomsOnGrid(generatorController.MainInfoGrid,random);

                // Zamknięcie okna po wygenerowaniu siatki
                Close();

                // Generowanie obiektów na scenie
                generatorController.DebugGridMesh();
            }
        }

        // Przycisk do zamknięcia okna bez generowania siatki
        if (GUILayout.Button("Cancel"))
        {
            Close();
        }
    }

    private void ClearGeneratedGrid(MapGeneratorController generatorController)
    {
        // Usuń wszystkie istniejące obiekty dzieci w GameObject
        for (int i = generatorController.transform.childCount - 1; i >= 0; i--)
        {
            Transform child = generatorController.transform.GetChild(i);
            DestroyImmediate(child.gameObject);
        }

        // Wyczyszczenie danych w GridData
        generatorController.MainInfoGrid = null;
    }*/
}
