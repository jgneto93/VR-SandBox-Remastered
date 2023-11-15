using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ObjectPositionRecorder : MonoBehaviour
{
    public string tagToRecord = "isOnTerrain";
    private string filePath;
    private StreamWriter sw;
    private Dictionary<string, List<Vector3>> objectMovements = new Dictionary<string, List<Vector3>>();

    void Start()
    {
        int fileIndex = 1;
        filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/ObjectPositionData_{fileIndex}.csv";

        while (File.Exists(filePath))
        {
            fileIndex++;
            filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/ObjectPositionData_{fileIndex}.csv";
        }

        sw = new StreamWriter(filePath);
        sw.WriteLine("Time,ObjectName,PositionX,PositionZ");

        StartCoroutine(RecordObjectPosition());
    }

    IEnumerator RecordObjectPosition()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            float time = Time.time;
            GameObject[] objectsToRecord = GameObject.FindGameObjectsWithTag(tagToRecord);

            foreach (GameObject obj in objectsToRecord)
            {
                string objectName = GetObjectName(obj);

                if (!objectMovements.ContainsKey(objectName))
                {
                    objectMovements.Add(objectName, new List<Vector3>());
                }

                Vector3 objectPosition = obj.transform.position;
                objectMovements[objectName].Add(objectPosition);

                // Adiciona a posição no CSV
                sw.WriteLine($"{time},{objectName},{objectPosition.x},{objectPosition.z}");
            }

            yield return new WaitForSeconds(1.0f); // Ajuste conforme necessário
        }

        // Encerra e salva o arquivo quando a tecla de espaço for pressionada
        sw.Close();
    }

    string GetObjectName(GameObject obj)
    {
        // Utiliza a função GetObjectIndex para obter um nome único para cada objeto
        int objectIndex = GetObjectIndex(obj);
        return $"{tagToRecord}_{objectIndex}";
    }

    int GetObjectIndex(GameObject obj)
    {
        GameObject[] objectsWithSameTag = GameObject.FindGameObjectsWithTag(tagToRecord);
        return System.Array.IndexOf(objectsWithSameTag, obj) + 1;
    }
}
