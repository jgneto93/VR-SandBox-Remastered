using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Terrain Width, Length, All Game Objects (vehicles, trees, lights (intensity, fov)),
public class Scale : MonoBehaviour
{
    public float scaleDivider = 1.0f; // Specify the value by which you want to divide the scale
    private float atualScale = 1.0f;
    private Vector3 originalTerrainSize;
    public GameObject terrainObject;
    private TerrainData originalTerrainData;

    void Start(){
        Terrain terrain = terrainObject.GetComponent<Terrain>();
        originalTerrainData = terrain.terrainData;
        originalTerrainSize = originalTerrainData.size;
    }

    private void Update()
    {
        if (scaleDivider != atualScale){
            atualScale = scaleDivider;
            
            GameObject[] terrainObjects = GameObject.FindGameObjectsWithTag("Terrain");
            Terrain terrain = terrainObject.GetComponent<Terrain>();
            TerrainData terrainData = terrain.terrainData;
            originalTerrainData.terrainLayers = terrainData.terrainLayers;
            Vector3 terrainSize = terrainData.size;
            terrainSize /= scaleDivider;
            terrainData.size = terrainSize;

            TerrainLayer[] layers = terrainData.terrainLayers;
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].tileSize /= scaleDivider;
                layers[i].tileOffset /= scaleDivider;
            }
            terrainData.terrainLayers = layers;
            terrain.Flush();

            GameObject[] lightObjects = GameObject.FindGameObjectsWithTag("Light");
            foreach (GameObject lightObject in lightObjects)
            {
                lightObject.transform.localScale /= scaleDivider;
                lightObject.transform.position /= scaleDivider;
            }

            GameObject[] prefabObjects = GameObject.FindGameObjectsWithTag("Prefab");
            foreach (GameObject prefabObject in prefabObjects)
            {
                prefabObject.transform.localScale /= scaleDivider;
                prefabObject.transform.position /= scaleDivider;

            }
        }
    }
    void OnDisable(){

            Terrain terrain = terrainObject.GetComponent<Terrain>();
            TerrainData terrainData = terrain.terrainData;

            terrainData.size = originalTerrainSize;
            terrainData.terrainLayers = originalTerrainData.terrainLayers;

            // Reset other properties like tree density, detail density, etc. if necessary

            terrain.Flush();
    }
}
