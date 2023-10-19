using UnityEngine;

public class CameraToggle : MonoBehaviour
{
    public Camera cameraDown;
    public Camera cameraUp;
    public OVRInput.Button cameraDownButton;
    public OVRInput.Button cameraUpButton;

    private bool isButtonPressed = false;

    private void Update()
    {
        // Detect input from Oculus Touch controllers (A and B buttons)
        if (OVRInput.GetDown(cameraDownButton)) // Button A
        {
            isButtonPressed = true;
            SwitchCamera(cameraDown);
        }
        else if (OVRInput.GetDown(cameraUpButton)) // Button B
        {
            isButtonPressed = true;
            SwitchCamera(cameraUp);
        }

        if (isButtonPressed && !OVRInput.Get(cameraDownButton) && !OVRInput.Get(cameraUpButton))
        {
            isButtonPressed = false;
            SwitchCamera(null); // Switch back to default camera
        }
    }

    private void SwitchCamera(Camera targetCamera)
    {
        cameraDown.enabled = false;
        cameraUp.enabled = false;

        if (targetCamera != null)
        {
            targetCamera.enabled = true;
        }
    }
}
