using UnityEngine;

public class PlayerItemReach : MonoBehaviour
{
    public void ModifyPlayerReach(int reachRadius)
    {
        this.GetComponent<CapsuleCollider>().radius = reachRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<IPlayerReach>(out IPlayerReach playerReach))
            playerReach.IsReachable(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<IPlayerReach>(out IPlayerReach playerReach))
            playerReach.IsReachable(false);    }
}
