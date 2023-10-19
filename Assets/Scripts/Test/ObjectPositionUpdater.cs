using UnityEngine;

public class ObjectPositionUpdater : MonoBehaviour
{
    public Transform sphere; // Reference to the associated sphere

    private void Update()
    {
        // Update the object's position to match the sphere's position
        transform.position = sphere.position;
    }
}