using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainScaler : MonoBehaviour
{
    public Terrain terrain;
    public TerrainLayer terrainLayer;
    public float scaleSpeedMultiplier = 0.01f;

    private bool isScaling = false;
    private Vector3 initialScale;
    private Vector2 initialLayerSize;

    public void StartScaling()
    {
        if (!isScaling)
        {
            isScaling = true;
            initialScale = terrain.terrainData.size;
            initialLayerSize = terrainLayer.tileSize;
        }
    }

    public void StopScaling()
    {
        isScaling = false;
    }

    private void Update()
    {
        if (isScaling)
        {
            // Verifica se o botão X (Button.Three) está sendo pressionado para aumentar a escala
            if (OVRInput.Get(OVRInput.Button.Three))
            {
                IncreaseScale();
            }
            // Verifica se o botão Y (Button.Four) está sendo pressionado para diminuir a escala
            else if (OVRInput.Get(OVRInput.Button.Four))
            {
                DecreaseScale();
            }
        }
    }

    private void IncreaseScale()
    {
        // Fator de escala para aumentar (pode ajustar conforme a sensibilidade desejada)
        float scaleFactor = 1.0f + scaleSpeedMultiplier;

        Vector3 newSize = initialScale * scaleFactor;

        // Aplica a nova escala ao terreno
        terrain.terrainData.size = new Vector3(newSize.x, initialScale.y, newSize.z);

        // Aplica a mesma escala à textura da camada do terreno
        ApplyLayerScale(scaleFactor, scaleFactor);
    }

    private void DecreaseScale()
    {
        // Fator de escala inverso para diminuir (pode ajustar conforme a sensibilidade desejada)
        float scaleFactor = 1.0f - scaleSpeedMultiplier;

        Vector3 newSize = initialScale * scaleFactor;

        // Aplica a nova escala ao terreno
        terrain.terrainData.size = new Vector3(newSize.x, initialScale.y, newSize.z);

        // Aplica a mesma escala à textura da camada do terreno
        ApplyLayerScale(scaleFactor, scaleFactor);
    }

    private void ApplyLayerScale(float scaleX, float scaleZ)
    {
        // Verifica se a camada do terreno está atribuída
        if (terrainLayer != null)
        {
            // Aplica a escala nos eixos X e Z para a camada de terreno
            Vector2 newLayerSize = new Vector2(initialLayerSize.x * scaleX, initialLayerSize.y * scaleZ);
            terrainLayer.tileSize = newLayerSize;
        }
    }
}
