using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public TextAsset csvFileToUse;
    public Vector3 boxColliderSize = new Vector3(0.15f, 0.15f, 0.15f);

    public Transform spawnPoint; // Ponto de spawn inicial (pode ser configurado no editor)

    public void SpawnObject(GameObject prefabToSpawn)
    {
        if (prefabToSpawn == null || csvFileToUse == null)
        {
            Debug.LogError("Prefab ou CSV não estão definidos!");
            return;
        }

        GameObject spawnedObject = new GameObject("SpawnedObject");
        spawnedObject.transform.position = spawnPoint.transform.position;
        spawnedObject.tag = "SpawnsZones";

        BoxCollider boxCollider = spawnedObject.AddComponent<BoxCollider>();
        boxCollider.size = boxColliderSize;
        boxCollider.isTrigger = true;

        Spawn2 spawnScript = spawnedObject.GetComponent<Spawn2>();
        if (spawnScript == null)
        {
            spawnScript = spawnedObject.AddComponent<Spawn2>();
        }

        spawnScript.prefab = prefabToSpawn;
        spawnScript.csvFile = csvFileToUse;
    }
}