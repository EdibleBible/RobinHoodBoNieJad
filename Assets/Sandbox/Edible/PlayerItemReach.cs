using UnityEngine;

public class PlayerItemReach : MonoBehaviour
{
    public void ModifyPlayerReach(int reachRadius)
    {
        this.GetComponent<CapsuleCollider>().radius = reachRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<IPlayerReach>().IsReachable(true);
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.GetComponent<IPlayerReach>().IsReachable(false);
    }
}
