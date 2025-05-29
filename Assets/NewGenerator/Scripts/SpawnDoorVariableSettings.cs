using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct SpawnDoorVariableSettings
{
    public List<DoorController> AllDoorsOnScene;
    public List<DoorController> BlockedDoorsByLever;
    public List<DoorController> BlockedDoorsByPlate;


    public int BlockedByLeverMax;
    public int BlockedByPlateMax;

    [HideInInspector] public int BlockedByLeverCount;
    [HideInInspector] public int BlockedByPlateCount;

    public LayerMask WallLayerMask;
    public GameObject LeverPrefab;
    public Vector3 LeverSpawnOffset;
    
    public List<LeverController> SpawnedLevers;

    public void FindAllDoorsOnScene(List<GeneratorRoomData> data)
    {
        AllDoorsOnScene = new List<DoorController>();
        foreach (var room in data)
        {
            if(room.IsSpawn)
                continue;
            AllDoorsOnScene.AddRange(room.AllDoors);
        }
    }

    public void RandomizeDoorsAmount(uint seed)
    {
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);

        BlockedByLeverCount = random.NextInt(0, BlockedByLeverMax);
        BlockedByPlateCount = random.NextInt(0, BlockedByPlateMax);
    }

    public void RandomizeDoors()
    {
        for (int i = 0; i < BlockedByLeverCount; i++)
        {
            int index = UnityEngine.Random.Range(0, AllDoorsOnScene.Count);
            BlockedDoorsByLever.Add(AllDoorsOnScene[index]);
            AllDoorsOnScene.RemoveAt(index);

            if (AllDoorsOnScene.Count == 0)
                return;
        }

        /*for (int i = 0; i < BlockedByPlateCount; i++)
        {

            int index = UnityEngine.Random.Range(0, AllDoorsOnScene.Count);
            BlockedDoorsByPlate.Add(AllDoorsOnScene[index]);
            AllDoorsOnScene.RemoveAt(index);

            if(AllDoorsOnScene.Count == 0)
                return;
        }*/
    }

    public void SelectLeverGridCell(List<GridCellData> allHallwaysGridCells)
    {
        List<GridCellData> selectedCells = new List<GridCellData>();
        List<GridCellData> listCopy = allHallwaysGridCells.ToList();
        listCopy.RemoveAll(cell => cell.GridCellType == E_GridCellType.HallwayTrap);

        for (int i = 0; i < BlockedByLeverCount; i++)
        {
            int index = UnityEngine.Random.Range(0, listCopy.Count);
            GridCellData selectedCell = listCopy[index];
            selectedCell.GridCellType = E_GridCellType.HallwayLever;
            selectedCells.Add(selectedCell);
            listCopy.RemoveAt(index);
        }

        foreach (var cell in selectedCells)
        {
            List<Transform> detectedTransformInCells = cell.FastDetectObjectInCell(WallLayerMask)
                .Where(t => Mathf.Abs(t.position.y) <= 0.1f)
                .ToList();

            if (detectedTransformInCells.Count == 0)
                continue;

            int index = UnityEngine.Random.Range(0, detectedTransformInCells.Count);
            Transform selectedTransform = detectedTransformInCells[index];

            SpawnLeversOnWalls(cell, new List<Transform> { selectedTransform }, LeverSpawnOffset);
        }

    }

    public void SpawnLeversOnWalls(GridCellData cell, List<Transform> wallTransforms, Vector3 localOffset)
    {
        foreach (Transform wall in wallTransforms)
        {
            Vector3 directionToWall = (wall.position - cell.Position).normalized;
            Vector3 snappedDirection = SnapToRightAngle(directionToWall);

            // Lokalny offset przekształcony do kierunku ściany (jakby dźwignia "wiedziała", gdzie ma górę/lewo)
            Quaternion rotationToAlignOffset = Quaternion.LookRotation(snappedDirection);
            Vector3 worldOffset = rotationToAlignOffset * localOffset;

            Vector3 spawnPosition = wall.position + worldOffset;

            // Stała rotacja dźwigni
            Quaternion leverRotation = Quaternion.Euler(0, 180, 0);

            GameObject lever = GameObject.Instantiate(LeverPrefab, spawnPosition, leverRotation,wall);
            lever.transform.localRotation = new Quaternion(0,180,0,0);
            lever.transform.localPosition = localOffset;
            SpawnedLevers.Add(lever.GetComponent<LeverController>());
            lever.transform.SetParent(null);
        }
    }

    private Vector3 SnapToRightAngle(Vector3 dir)
    {
        dir.y = 0;
        dir.Normalize();

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z))
            return dir.x > 0 ? Vector3.right : Vector3.left;
        else
            return dir.z > 0 ? Vector3.forward : Vector3.back;
    }
    
    
    public void AssignLeversToDoors()
    {
        // Tworzymy lokalne kopie, aby móc losowo wybierać bez modyfikacji oryginalnych list
        List<LeverController> availableLevers = new List<LeverController>(SpawnedLevers);
        List<DoorController> doorsToAssign = new List<DoorController>(BlockedDoorsByLever);

        // Jeśli dźwigni jest mniej niż drzwi, przypisujemy tylko do tylu drzwi, ile mamy dźwigni
        int pairsToAssign = Mathf.Min(availableLevers.Count, doorsToAssign.Count);

        for (int i = 0; i < pairsToAssign; i++)
        {
            // Losuj drzwi i dźwignię
            int doorIndex = UnityEngine.Random.Range(0, doorsToAssign.Count);
            int leverIndex = UnityEngine.Random.Range(0, availableLevers.Count);

            DoorController selectedDoor = doorsToAssign[doorIndex];
            LeverController selectedLever = availableLevers[leverIndex];

            // Przypisz drzwi do dźwigni i odwrotnie (jeśli masz takie metody/pola)
            selectedLever.SetObjectToInteract(selectedDoor.gameObject);

            // Usuń przypisane elementy z list
            availableLevers.RemoveAt(leverIndex);
            doorsToAssign.RemoveAt(doorIndex);
        }
    }

}