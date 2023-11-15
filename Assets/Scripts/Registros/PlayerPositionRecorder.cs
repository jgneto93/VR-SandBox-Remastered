using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerPositionRecorder : MonoBehaviour
{
    private string filePath;
    private StreamWriter sw;
    private Transform playerTransform;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        int fileIndex = 1;
        filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/PlayerPositionData_{fileIndex}.csv";

        while (File.Exists(filePath))
        {
            fileIndex++;
            filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/PlayerPositionData_{fileIndex}.csv";
        }

        sw = new StreamWriter(filePath);
        sw.WriteLine("Time,PositionX,PositionZ,DeltaX,DeltaZ");

        // Encontrar o objeto do jogador com a tag "Player"
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            lastPlayerPosition = playerTransform.position;

            StartCoroutine(RecordPlayerPosition());
        }
        else
        {
            Debug.LogError("Objeto do jogador não encontrado na cena.");
        }
    }

    IEnumerator RecordPlayerPosition()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            float time = Time.time;
            Vector3 playerPosition = playerTransform.position;

            float deltaX = playerPosition.x - lastPlayerPosition.x;
            float deltaZ = playerPosition.z - lastPlayerPosition.z;

            sw.WriteLine($"{time},{playerPosition.x},{playerPosition.z},{deltaX},{deltaZ}");

            lastPlayerPosition = playerPosition;

            yield return new WaitForSeconds(1.0f); // Ajuste conforme necessário
        }

        // Encerrando e salvando o arquivo quando a tecla de espaço for pressionada
        sw.Close();
    }
}
