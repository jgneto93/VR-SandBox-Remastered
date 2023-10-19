using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckObjectOnTerrain : MonoBehaviour
{
    public Terrain terrain;

    private void Start()
    {
        // Verifica se o terreno foi atribuído ao script
        if (terrain == null)
        {
            Debug.LogWarning("Terrain não atribuído ao script.");
        }
    }

    private void Update()
    {
        // Find all objects with the tag "spawnLeave"
        GameObject[] objectsWithSpawnLeaveTag = GameObject.FindGameObjectsWithTag("spawnLeave");

        foreach (GameObject obj in objectsWithSpawnLeaveTag)
        {
            if (IsObjectOnTerrain(obj))
            {
                // O objeto está em contato com o terreno, atribui a tag "isOnTerrain" se ainda não tiver a tag
                if (!obj.CompareTag("isOnTerrain"))
                {
                    obj.tag = "isOnTerrain";
                    Debug.Log("Objeto em contato com o terreno: " + obj.name);
                }
            }
            else
            {
                // O objeto não está em contato com o terreno, remove a tag "isOnTerrain" se tiver a tag
                if (obj.CompareTag("isOnTerrain"))
                {
                    obj.tag = "Untagged";
                    Debug.Log("Objeto saiu do terreno: " + obj.name);
                }
            }
        }
    }

    private bool IsObjectOnTerrain(GameObject obj)
    {
        if (terrain == null || terrain.terrainData == null)
        {
            Debug.LogWarning("Terrain não possui TerrainData válido.");
            return false;
        }

        BoxCollider objCollider = obj.GetComponent<BoxCollider>();
        if (objCollider != null && objCollider.enabled)
        {
            // Verifica se o centro do BoxCollider está abaixo da altura do terreno
            float terrainHeight = terrain.SampleHeight(objCollider.bounds.center);
            float colliderHeight = objCollider.bounds.center.y - objCollider.bounds.extents.y;

            return colliderHeight <= terrainHeight;
        }

        return false;
    }
}
