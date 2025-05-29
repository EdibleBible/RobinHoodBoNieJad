using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class DecalsDoorController : DoorController
{
    [SerializeField]
    private bool addWallOnAwake = false;
    [SerializeField]
    private LayerMask wallLayer;
    [SerializeField]
    private LayerMask wallDecalLayer;
    [SerializeField]
    private LayerMask decalLayer;

    private List<Transform> doorsSideWalls = new List<Transform>();
    [SerializeField]
    private Vector3 detectWallSize = new Vector3(0.5f, 0.5f, 0.5f);
    private Vector3 detectWallSizeCollider;
    [SerializeField]
    private Vector3 detectWallCenter = new Vector3(0.5f, 0.5f, 0.5f);

    [SerializeField]
    private GameObject leverPrefab;
    [SerializeField]
    private int leverCount;
    [SerializeField]
    private int CorrectLeverCount;
    [SerializeField]
    private float leverSpawnGap;
    [SerializeField]
    private Vector3 leverSpawnOffset;
    [SerializeField]
    private Vector3 leverSize;

    private Dictionary<IInteractable, (Material material, bool isActive)> spawnedLevers
        = new Dictionary<IInteractable, (Material, bool)>();

    [SerializeField]
    private GameObject leverDecalPrefab;
    [SerializeField]
    private List<Material> leverDecalSprite = new List<Material>();
    [SerializeField]
    private Vector3 leverDecalOffset;
    [SerializeField]
    private Vector3 leverDecalSize;
    [SerializeField]
    private Vector3 wallDecalOffset;
    [SerializeField]
    private float wallDistanceDecal;
    [SerializeField]
    private float randomUpDownSpawnRange;
    [SerializeField]
    private float randomRightLeftSpawnRange;
    private List<Transform> allWalls = new List<Transform>();

    public enum LeverSpawnDirection
    {
        Up,
        Right,
        Forward,
        WallRight,
        WallUp
    }

    [SerializeField]
    private LeverSpawnDirection spawnDirection;

    private void Awake()
    {
        DetectSideWalls();

        if (!addWallOnAwake)
        {
            return;
        }

        allWalls.Clear();

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (((1 << obj.layer) & wallDecalLayer.value) !=
                0) // Sprawdza czy obiekt ma warstwę pasującą do wallDecalLayer
            {
                allWalls.Add(obj.transform);
            }
        }


        for (int i = 0; i < doorsSideWalls.Count; i++)
        {
            allWalls.Remove(doorsSideWalls[i]);
        }


        SpawnLever();
        SelectRandomLevers(CorrectLeverCount);

        SpawnCorrectDecals();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            RefreshAllWalls();
        }
    }

    private void RefreshAllWalls()
    {
        allWalls.Clear();

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (((1 << obj.layer) & wallDecalLayer.value) !=
                0) // Sprawdza czy obiekt ma warstwę pasującą do wallDecalLayer
            {
                allWalls.Add(obj.transform);
            }
        }

        foreach (var sideWall in doorsSideWalls)
        {
            allWalls.Remove(sideWall);
        }
    }


    public void DetectSideWalls()
    {
        // Obliczamy połowę rozmiaru pudełka – wymagany parametr dla Physics.OverlapBox
        Vector3 halfExtents = detectWallSize * 0.5f;

        // Używamy rotacji obiektu, aby pudełko obracało się razem z nim
        Collider[] colliders = Physics.OverlapBox(transform.position + detectWallCenter, halfExtents,
            transform.rotation, wallLayer);

        foreach (Collider col in colliders)
        {
            doorsSideWalls.Add(col.transform);
        }
    }

    public void SpawnLever()
    {
        if (doorsSideWalls.Count == 0 || leverDecalSprite.Count < leverCount)
        {
            return;
        }

        Transform wallTransform = doorsSideWalls[0];
        Vector3 direction = GetSpawnDirection(wallTransform);
        List<int> availableIndices = Enumerable.Range(0, leverDecalSprite.Count).ToList();

        for (int i = 0; i < leverCount; i++)
        {
            var leverObj = Instantiate(leverPrefab, wallTransform);
            leverObj.transform.localScale = leverSize;
            leverObj.transform.localRotation = Quaternion.identity;

            Vector3 positionOffset = leverSpawnOffset + (i * leverSpawnGap * direction);
            leverObj.transform.localPosition = positionOffset;
            leverObj.GetComponent<IInteractable>().TwoSideInteraction = true;

            var leverDecalObj = Instantiate(leverDecalPrefab, wallTransform);
            leverDecalObj.transform.localScale = leverDecalSize;
            leverDecalObj.transform.Rotate(0, 270, 0);
            Vector3 decalPositionOffset = positionOffset + leverDecalOffset;
            leverDecalObj.transform.localPosition = decalPositionOffset;

            int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
            availableIndices.Remove(randomIndex);
            leverDecalObj.GetComponent<DecalProjector>().material = leverDecalSprite[randomIndex];
            spawnedLevers.Add(leverObj.GetComponent<IInteractable>(), (leverDecalSprite[randomIndex], false));
            leverObj.transform.SetParent(this.transform);
        }
    }

    void SelectRandomLevers(int count)
    {
        if (spawnedLevers.Count < count)
        {
            return;
        }

        List<IInteractable> availableLevers = spawnedLevers.Keys.ToList();
        List<IInteractable> selectedLevers = new List<IInteractable>();

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableLevers.Count);
            IInteractable selectedLever = availableLevers[randomIndex];

            (Material material, bool isActive) copy = spawnedLevers[selectedLever];
            copy.isActive = true;
            spawnedLevers[selectedLever] = copy;
            selectedLevers.Add(selectedLever);
            availableLevers.RemoveAt(randomIndex);
        }
    }

    bool CheckIfCorrectLeversUsed()
    {
        foreach (var kvp in spawnedLevers)
        {
            IInteractable lever = kvp.Key;
            bool isActive = kvp.Value.isActive;


            if (isActive && !lever.IsUsed)
            {
                return false;
            }

            if (!isActive && lever.IsUsed)
            {
                return false;
            }
        }

        return true;
    }

    private Vector3 GetSpawnDirection(Transform wallTransform)
    {
        return spawnDirection switch
        {
            LeverSpawnDirection.Up => Vector3.up,
            LeverSpawnDirection.Right => Vector3.right,
            LeverSpawnDirection.Forward => Vector3.forward,
            LeverSpawnDirection.WallRight => wallTransform.right,
            LeverSpawnDirection.WallUp => wallTransform.up,
            _ => Vector3.up
        };
    }

    void OnDrawGizmos()
    {
        // Obliczamy połowę rozmiaru, aby rysować zgodnie z używanym w detekcji OverlapBox
        Vector3 halfExtents = detectWallSize * 0.5f;

        // Ustawiamy macierz transformacji dla gizmo, aby rysować z odpowiednią pozycją i rotacją
        Matrix4x4 rotationMatrix =
            Matrix4x4.TRS(transform.position + detectWallCenter, transform.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;

        Gizmos.color = Color.red;
        // Rysujemy pudełko – tutaj podajemy pełny rozmiar, bo Gizmos.DrawWireCube oczekuje rozmiaru pudełka, nie half extents
        Gizmos.DrawWireCube(Vector3.zero, detectWallSize);
        Gizmos.matrix = Matrix4x4.identity;

        Gizmos.color = Color.green;
        foreach (var wall in allWalls)
        {
            // Obliczenie pozycji decalu z offsetem
            Vector3 decalPosition = wall.position + wall.rotation * wallDecalOffset;

            // Zastosowanie macierzy transformacji (obrót i przesunięcie dla Gizmo)
            rotationMatrix = Matrix4x4.TRS(decalPosition, wall.rotation, Vector3.one);
            Gizmos.matrix = rotationMatrix;

            // Rysowanie Gizmo z rotacją ściany
            Gizmos.DrawWireCube(Vector3.zero,
                new Vector3(0, randomUpDownSpawnRange * 2, randomRightLeftSpawnRange * 2));
        }

        // Resetowanie macierzy, żeby inne Gizmo nie były obrócone
        Gizmos.matrix = Matrix4x4.identity;
    }
    
    public override void Interact(Transform player)
    {
        if (!CheckIfCorrectLeversUsed())
        {
            ShowUIEvent.Raise(this, (true, $"You need find: {CorrectLeverCount} correct lever", true));
            return;
        }

        if (IsBlocked)
        {
            ShowUIEvent.Raise(this, (true, BlockedMessage, true));
            return;
        }

        if (isDoorOpenTween != null || !CanInteract)
        {
            return;
        }


        InteractEvent.Raise(this, null);

        if (!IsUsed)
        {
            CheckPlayerPosition();
            PlayDoorSound("Open");

            Debug.Log("endRightDoorOpenPosition" + endRightDoorOpenPosition + "endLeftDoorOpenPosition" +
                      endLeftDoorOpenPosition);


            Tween leftDoorTween = doorPivotLeft.transform.DOLocalRotate(endLeftDoorOpenPosition, openTime)
                .SetEase(openCurve);
            Tween rightDoorTween = doorPivotRight.transform.DOLocalRotate(endRightDoorOpenPosition, openTime)
                .SetEase(openCurve);

            isDoorOpenTween = DOTween.Sequence()
                .Join(leftDoorTween)
                .Join(rightDoorTween)
                .OnComplete(() =>
                {
                    isDoorOpenTween = null;
                    IsUsed = true;
                    if (!TwoSideInteraction) CanInteract = false;
                });

            foreach (var lever in spawnedLevers)
            {
                lever.Key.IsBlocked = true;
            }
        }
    }

    public void SpawnCorrectDecals()
    {
        var leverDictionary = spawnedLevers
            .Where(x => x.Value.isActive)
            .ToDictionary(x => x.Key, x => x.Value.material);

        foreach (var lever in leverDictionary)
        {
            float randomUpDown = Random.Range(-randomUpDownSpawnRange, randomUpDownSpawnRange);
            float randomLeftRight = Random.Range(-randomRightLeftSpawnRange, randomRightLeftSpawnRange);

            var randomWall = allWalls[Random.Range(0, allWalls.Count)];
            var leverDecalObj = Instantiate(leverDecalPrefab, randomWall);
            leverDecalObj.transform.localScale = leverDecalSize;
            leverDecalObj.transform.Rotate(0, 270, 0);
            var newOffset = wallDecalOffset + new Vector3(0, randomUpDown, randomLeftRight);
            Vector3 decalPositionOffset = Vector3.zero + newOffset;
            leverDecalObj.transform.localPosition = decalPositionOffset;
            leverDecalObj.GetComponent<DecalProjector>().material = lever.Value;

            SetLayerRecursively(leverDecalObj, decalLayer); // <-- Ustawianie warstwy
        }
    }

    private void SetLayerRecursively(GameObject obj, LayerMask newLayer)
    {
        if (obj == null) return;
        
        int myNewLayer = LayerMask.NameToLayer("Decal");
        obj.layer = myNewLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}