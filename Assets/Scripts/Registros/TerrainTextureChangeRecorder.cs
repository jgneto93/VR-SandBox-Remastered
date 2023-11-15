using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TerrainTextureChangeRecorder : MonoBehaviour
{
    private Terrain terrain;
    private TerrainLayer[] terrainLayers;
    private int activeLayerIndex;
    private float startTime;
    private StreamWriter sw;

    void Start()
    {
        terrain = GetComponent<Terrain>();

        if (terrain != null)
        {
            terrainLayers = terrain.terrainData.terrainLayers;
            activeLayerIndex = GetCurrentActiveLayerIndex();

            int fileIndex = 1;
            string filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/TerrainTextureChangeData_{fileIndex}.csv";

            while (File.Exists(filePath))
            {
                fileIndex++;
                filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/TerrainTextureChangeData_{fileIndex}.csv";
            }

            sw = new StreamWriter(filePath);
            sw.WriteLine("Time,LayerIndex,LayerName,Duration");

            startTime = Time.time;
            StartCoroutine(CheckTerrainTextureChange());
        }
        else
        {
            Debug.LogError("Terrain component not found on the GameObject.");
        }
    }

    int GetCurrentActiveLayerIndex()
    {
        int currentLayerIndex = 0;
        float[,,] alphaMaps = terrain.terrainData.GetAlphamaps(0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);

        for (int i = 0; i < terrainLayers.Length; i++)
        {
            if (HasAlphaValueInLayer(alphaMaps, i))
            {
                currentLayerIndex = i;
                break;
            }
        }

        return currentLayerIndex;
    }

    bool HasAlphaValueInLayer(float[,,] alphaMaps, int layerIndex)
    {
        int alphaMapWidth = alphaMaps.GetLength(0);
        int alphaMapHeight = alphaMaps.GetLength(1);

        for (int x = 0; x < alphaMapWidth; x++)
        {
            for (int y = 0; y < alphaMapHeight; y++)
            {
                if (alphaMaps[x, y, layerIndex] > 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator CheckTerrainTextureChange()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            int newActiveLayerIndex = GetCurrentActiveLayerIndex();

            if (newActiveLayerIndex != activeLayerIndex)
            {
                // Troca de camada detectada
                float duration = Time.time - startTime;

                // Salva no CSV
                sw.WriteLine($"{Time.time},{newActiveLayerIndex},{terrainLayers[newActiveLayerIndex].diffuseTexture.name},{duration}");

                // Atualiza os valores
                activeLayerIndex = newActiveLayerIndex;
                startTime = Time.time;
            }

            yield return null;
        }

        // Encerra e salva o arquivo quando a tecla de espaço for pressionada
        sw.Close();
    }
}
