using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ButtonPressRecorder : MonoBehaviour
{
    private float buttonThreePressStartTime = 0.0f;
    private float buttonFourPressStartTime = 0.0f;
    private float thumbstickUpPressStartTime = 0.0f;
    private float thumbstickDownPressStartTime = 0.0f;

    private string filePath;
    private StreamWriter sw;

    void Start()
    {
        int fileIndex = 1;
        filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/ButtonPressData_{fileIndex}.csv";

        while (File.Exists(filePath))
        {
            fileIndex++;
            filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/ButtonPressData_{fileIndex}.csv";
        }

        sw = new StreamWriter(filePath);
        sw.WriteLine("Button,PressStartTime,PressDuration");

        StartCoroutine(RecordButtonPress());
    }

    IEnumerator RecordButtonPress()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (OVRInput.GetDown(OVRInput.Button.Three))
            {
                buttonThreePressStartTime = Time.time;
            }
            else if (OVRInput.GetUp(OVRInput.Button.Three))
            {
                float pressDuration = Time.time - buttonThreePressStartTime;
                sw.WriteLine($"Three,{buttonThreePressStartTime},{pressDuration}");
            }

            if (OVRInput.GetDown(OVRInput.Button.Four))
            {
                buttonFourPressStartTime = Time.time;
            }
            else if (OVRInput.GetUp(OVRInput.Button.Four))
            {
                float pressDuration = Time.time - buttonFourPressStartTime;
                sw.WriteLine($"Four,{buttonFourPressStartTime},{pressDuration}");
            }

            Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

            if (thumbstick.y > 0.5f && thumbstickUpPressStartTime == 0.0f) // Assuming threshold for "up" movement
            {
                thumbstickUpPressStartTime = Time.time;
            }
            else if (thumbstick.y <= 0.5f && thumbstickUpPressStartTime > 0.0f)
            {
                float pressDuration = Time.time - thumbstickUpPressStartTime;
                sw.WriteLine($"ThumbstickUp,{thumbstickUpPressStartTime},{pressDuration}");
                thumbstickUpPressStartTime = 0.0f;
            }

            if (thumbstick.y < -0.5f && thumbstickDownPressStartTime == 0.0f) // Assuming threshold for "down" movement
            {
                thumbstickDownPressStartTime = Time.time;
            }
            else if (thumbstick.y >= -0.5f && thumbstickDownPressStartTime > 0.0f)
            {
                float pressDuration = Time.time - thumbstickDownPressStartTime;
                sw.WriteLine($"ThumbstickDown,{thumbstickDownPressStartTime},{pressDuration}");
                thumbstickDownPressStartTime = 0.0f;
            }

            yield return null;
        }

        // Encerrando e salvando o arquivo quando a tecla de espaço for pressionada
        sw.Close();
    }
}
