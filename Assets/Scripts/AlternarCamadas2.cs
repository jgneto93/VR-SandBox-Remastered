using UnityEngine;
using System.IO;

public class AlternarCamadas2 : MonoBehaviour
{
    public Terrain terrain;
    public string camada1Nome; // Nome da primeira camada na Layer Palette
    public string camada2Nome; // Nome da segunda camada na Layer Palette

    private bool camada1Ativa = true;
    private float startTime;
    private string filePath;
    private StreamWriter sw;

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

        // Configurar o arquivo CSV
        int fileIndex = 1;
        filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/CamadaAtivaData_{fileIndex}.csv";

        while (File.Exists(filePath))
        {
            fileIndex++;
            filePath = $"C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/CamadaAtivaData_{fileIndex}.csv";
        }

        sw = new StreamWriter(filePath);
        sw.WriteLine("Time,CamadaAtiva");
        startTime = Time.time;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Salva o arquivo CSV ao pressionar a tecla espaço
            sw.Close();
            Debug.Log($"Arquivo CSV salvo em: {filePath}");
            Destroy(this); // Destroi o objeto para evitar que continue a gravação após salvar
        }
    }

    public void MudarCamadas()
    {
        // Registra a mudança de camada e o tempo
        float currentTime = Time.time;
        float duration = currentTime - startTime;
        sw.WriteLine($"{currentTime},{camada1Ativa}");
        startTime = currentTime;

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

    void OnDestroy()
    {
        // Encerra e salva o arquivo CSV quando o objeto é destruído
        sw.Close();
    }
}
