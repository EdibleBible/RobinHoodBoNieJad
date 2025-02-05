using UnityEngine;

public class FoV : MonoBehaviour
{
    private Quaternion ParentRot;
    private Quaternion NewRot;
    Vector3 eulerRotation;
    private float height;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        height = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y = height;
        transform.position = currentPosition;



        ParentRot = transform.parent.rotation;
        eulerRotation = ParentRot.eulerAngles;

        eulerRotation.z = 0f;
        eulerRotation.x = 0f;

        NewRot = Quaternion.Euler(eulerRotation);
        transform.rotation = NewRot;

    }
}
