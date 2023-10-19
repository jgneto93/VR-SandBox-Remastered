using UnityEngine;

public class SphereMovement : MonoBehaviour
{
    public Transform associatedObject; // Reference to the associated object

    private void Update()
    {
        // Update the associated object's local position on the X and Z axes to match the sphere's local position
        associatedObject.localPosition = new Vector3(transform.localPosition.x, associatedObject.localPosition.y, transform.localPosition.z);

        Debug.Log("Sphere Position: " + transform.localPosition);
        Debug.Log("Associated Object Position: " + associatedObject.localPosition);
    }
}
