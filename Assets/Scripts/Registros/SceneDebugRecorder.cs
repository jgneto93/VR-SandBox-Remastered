using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SceneDebugRecorder : MonoBehaviour
{
    private string filePath;
    private StreamWriter sw;

    void Start()
    {
        int fileIndex = 1;
        filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/DebugData_{fileIndex}.txt";

        while (File.Exists(filePath))
        {
            fileIndex++;
            filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/DebugData_{fileIndex}.txt";
        }

        sw = new StreamWriter(filePath);
        sw.WriteLine("Time,DebugMessage");

        StartCoroutine(RecordDebug());
    }

    IEnumerator RecordDebug()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            string debugMessage = "Debug information: " + Time.time;

            // Registrar a mensagem no console
            Debug.Log(debugMessage);

            // Registrar a mensagem no arquivo de texto
            sw.WriteLine($"{Time.time},{debugMessage}");

            yield return new WaitForSeconds(1.0f); // Ajuste conforme necessário
        }

        // Encerrando e salvando o arquivo quando a tecla de espaço for pressionada
        sw.Close();
    }
}
