using UnityEngine;

public class FollowObject : MonoBehaviour
{
    public GameObject Leader;

    private Vector3 offset;

    void Start()
    {
        // Calculate the initial offset between the two objects
        offset = this.transform.position - Leader.transform.position;
    }

    void Update()
    {
        // Update the position of objectA to maintain the initial offset from objectB
        this.transform.position = Leader.transform.position + offset;
    }
}