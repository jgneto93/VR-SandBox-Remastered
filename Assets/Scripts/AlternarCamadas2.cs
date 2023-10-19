using UnityEngine;

public class AlternarCamadas2 : MonoBehaviour
{
    public Terrain terrain;
    public string camada1Nome; // Nome da primeira camada na Layer Palette
    public string camada2Nome; // Nome da segunda camada na Layer Palette

    private bool camada1Ativa = true;

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("O componente Terrain não foi atribuído ao script.");
            return;
        }

        if (string.IsNullOrEmpty(camada1Nome) || string.IsNullOrEmpty(camada2Nome))
        {
            Debug.LogError("Os nomes das camadas não estão configurados corretamente.");
            return;
        }

        terrain.terrainData.SetAlphamaps(0, 0, ObterMistura(camada1Ativa));
    }

    public void MudarCamadas()
    {
        camada1Ativa = !camada1Ativa;
        float[,,] mistura = ObterMistura(camada1Ativa);
        terrain.terrainData.SetAlphamaps(0, 0, mistura);
    }
    

    private float[,,] ObterMistura(bool camada1Ativa)
    {
        int numCamadas = terrain.terrainData.alphamapLayers;

        float[,,] mistura = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, numCamadas];

        for (int i = 0; i < terrain.terrainData.alphamapWidth; i++)
        {
            for (int j = 0; j < terrain.terrainData.alphamapHeight; j++)
            {
                for (int k = 0; k < numCamadas; k++)
                {
                    // Defina a mistura como 1.0 na camada ativa e 0.0 na camada inativa
                    if (terrain.terrainData.terrainLayers[k].name == camada1Nome)
                    {
                        mistura[i, j, k] = camada1Ativa ? 1.0f : 0.0f;
                    }
                    else if (terrain.terrainData.terrainLayers[k].name == camada2Nome)
                    {
                        mistura[i, j, k] = camada1Ativa ? 0.0f : 1.0f;
                    }
                    else
                    {
                        mistura[i, j, k] = 0.0f; // Outras camadas não mudam
                    }
                }
            }
        }

        return mistura;
    }
}
