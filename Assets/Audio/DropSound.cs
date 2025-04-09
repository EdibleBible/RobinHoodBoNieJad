using FMODUnity;
using UnityEngine;

public class DropSound : MonoBehaviour
{
    [SerializeField] private EventReference dropSoundEvent;
    [SerializeField] private EItemSoundType itemSoundType;

    public void PlayDropSound()
    {
        Debug.Log("Playing Drop Sound for itemSoundType: " + itemSoundType);

        if (dropSoundEvent.IsNull)
        {
            Debug.LogWarning("DropSoundEvent is null!");
            return;
        }

        var instance = RuntimeManager.CreateInstance(dropSoundEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        instance.setParameterByName("MaterialTypeDrop", (int)itemSoundType);
        instance.start();
        instance.release();
    }

    public enum EItemSoundType
    {
        Cloth = 0,
        Porcelain = 1,
        Wood = 2,
        Glass = 3,
        Hammer = 4,
        Keys = 5
    }

}
