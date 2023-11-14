using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class Capture : MonoBehaviour
{
    public Terrain terrain;
    public KeyCode captureKey = KeyCode.Space;

    private Texture2D texture;
    private List<Vector2Int> objectPositions = new List<Vector2Int>();
    private Color red = Color.red;
    private Color blue = Color.blue;
    private bool capturing = true;

    private int captureNumber = 1; // Número da captura atual
    private string savePath = "C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/"; // Diretório de salvamento

    private void Start()
    {
        int width = terrain.terrainData.heightmapResolution;
        int height = terrain.terrainData.heightmapResolution;

        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Inicie a captura automaticamente quando a cena é iniciada
        CapturePositions();
    }

    private void Update()
    {
        if (capturing)
        {
            // Continue a captura enquanto a tecla de captura não for acionada
            if (Input.GetKeyDown(captureKey))
            {
                capturing = false; // Pare a captura quando a tecla de espaço for pressionada
                UpdateTexture();
                captureNumber++; // Incrementar o número da captura
            }
            else
            {
                CapturePositions();
            }
        }
    }

    private void CapturePositions()
    {
        if (capturing)
        {
            GameObject[] objectsToCapture = GameObject.FindGameObjectsWithTag("isOnTerrain");

            foreach (GameObject obj in objectsToCapture)
            {
                Vector3 position = obj.transform.position;
                float normalizedX = (position.x - terrain.transform.position.x) / terrain.terrainData.size.x;
                float normalizedZ = (position.z - terrain.transform.position.z) / terrain.terrainData.size.z;

                int texX = Mathf.FloorToInt(normalizedX * texture.width);
                int texY = Mathf.FloorToInt(normalizedZ * texture.height);

                objectPositions.Add(new Vector2Int(texX, texY));
            }
        }
    }

    private void UpdateTexture()
    {
        // Construa o nome do arquivo com base no número da captura
        string fileName = "ObjectCapture" + captureNumber + ".png";
        string filePath = Path.Combine(savePath, fileName);

        // Verifique se o arquivo já existe e adicione um número incremental ao nome, se necessário
        int fileIndex = 1;
        while (File.Exists(filePath))
        {
            fileIndex++;
            fileName = "ObjectCapture" + captureNumber + "_" + fileIndex + ".png";
            filePath = Path.Combine(savePath, fileName);
        }

        // Preencha a textura com vermelho para todas as posições capturadas anteriormente.
        foreach (Vector2Int position in objectPositions)
        {
            texture.SetPixel(position.x, position.y, red);
        }

        // Defina a última posição como azul.
        if (objectPositions.Count > 0)
        {
            Vector2Int lastPosition = objectPositions[objectPositions.Count - 1];
            texture.SetPixel(lastPosition.x, lastPosition.y, blue);
        }

        // Salve a textura como arquivo de imagem.
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Object positions captured and saved to: " + filePath);
    }
}
