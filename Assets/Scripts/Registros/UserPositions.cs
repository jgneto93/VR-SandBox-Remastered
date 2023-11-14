using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class UserPositions : MonoBehaviour
{
    public KeyCode captureKey = KeyCode.Space;
    public GameObject scenePlane; // Adicione um campo público para o plano da cena
    public string savePath = "C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/"; // Diretório de salvamento

    private Texture2D texture;
    private List<Vector2Int> userPositions = new List<Vector2Int>();
    private Vector2Int lastUserPosition;
    private Color pathColor = Color.red;
    private bool capturing = true;
    private int captureNumber = 1; // Número da captura atual

    private void Start()
    {
        // Obtenha as dimensões do plano da cena automaticamente
        float planeWidth = scenePlane.transform.localScale.x; // Largura do plano
        float planeHeight = scenePlane.transform.localScale.z; // Altura do plano

        int width = 2048; // Ajuste o tamanho da textura
        int height = 2048; // Ajuste o tamanho da textura

        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Inicie a captura a cada 1 segundo
        InvokeRepeating("CapturePositions", 0f, 1f); // Chama CapturePositions a cada 1 segundo a partir de 0 segundos
    }

    private void Update()
    {
        if (capturing && Input.GetKeyDown(captureKey))
        {
            capturing = false; // Pare a captura quando a tecla de espaço for pressionada
            UpdateTexture();

            // Incrementar o número da captura
            captureNumber++;
        }
    }

    private void CapturePositions()
    {
        if (capturing)
        {
            // Capturar a posição do usuário (use a posição da câmera, jogador ou outro objeto, dependendo do seu cenário)
            Vector3 userPosition = transform.position; // Por exemplo, a posição do objeto ao qual o script está anexado

            // Converta as coordenadas do mundo para as coordenadas do plano
            float normalizedX = (userPosition.x - scenePlane.transform.position.x) / scenePlane.transform.localScale.x;
            float normalizedY = (userPosition.z - scenePlane.transform.position.z) / scenePlane.transform.localScale.z;

            int texX = Mathf.FloorToInt(normalizedX * texture.width);
            int texY = Mathf.FloorToInt(normalizedY * texture.height);

            userPositions.Add(new Vector2Int(texX, texY));

            // Se não for o primeiro ponto, desenhe uma linha do ponto anterior para o atual.
            if (lastUserPosition != null)
            {
                DrawLine(lastUserPosition, new Vector2Int(texX, texY), pathColor);
            }

            lastUserPosition = new Vector2Int(texX, texY);
        }
    }

    private void DrawLine(Vector2Int start, Vector2Int end, Color color)
    {
        int dx = Mathf.Abs(end.x - start.x);
        int dy = Mathf.Abs(end.y - start.y);
        int sx = (start.x < end.x) ? 1 : -1;
        int sy = (start.y < end.y) ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            texture.SetPixel(start.x, start.y, color);

            if (start.x == end.x && start.y == end.y)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                start.x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                start.y += sy;
            }
        }
    }

    private void UpdateTexture()
    {
        // Construa o nome do arquivo com base no número da captura
        string fileName = "Capture" + captureNumber + ".png";
        string filePath = Path.Combine(savePath, fileName);

        // Verifique se o arquivo já existe e adicione um número incremental ao nome, se necessário
        int fileIndex = 1;
        while (File.Exists(filePath))
        {
            fileIndex++;
            fileName = "Capture" + captureNumber + "_" + fileIndex + ".png";
            filePath = Path.Combine(savePath, fileName);
        }

        // Salve a textura como arquivo de imagem
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("User positions captured and saved to: " + filePath);
    }
}
