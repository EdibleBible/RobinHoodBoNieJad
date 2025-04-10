using UnityEngine;
using FMODUnity;

public class PickupSounds : MonoBehaviour
{
    public EventReference pickupEvent;

    public MaterialType materialType = MaterialType.Cloth;

    public void PlayPickupSound()
    {
        if (pickupEvent.IsNull) return;

        var instance = RuntimeManager.CreateInstance(pickupEvent);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject));
        instance.setParameterByName("MaterialType", (int)materialType);
        instance.start();
        instance.release();
    }
}

public enum MaterialType
{
    Cloth = 0,
    Porcelain = 1,
    Wood = 2,
    Glass = 3,
    Hammer = 4,
    Keys = 5
}

