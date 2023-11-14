using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Gravador : MonoBehaviour
{
    private List<string> dadosGravados = new List<string>();
    private bool gravacaoAtiva = true;
    
    private Transform playerTransform;
    private Camera mainCamera;
    
    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (gravacaoAtiva)
        {
            GravarDados();
        }
    }
    
    private void GravarDados()
    {
        // Capturar a posição do jogador
        Vector3 playerPosition = playerTransform.position;
        
        // Capturar ações realizadas (exemplo)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dadosGravados.Add("Ação: Pressionou a tecla de espaço");
        }

        // Capturar manipulação de objetos com a tag "isOnTerrain"
        GameObject[] objetosNoTerreno = GameObject.FindGameObjectsWithTag("isOnTerrain");
        foreach (var objeto in objetosNoTerreno)
        {
            Vector3 objetoPosition = objeto.transform.position;
            dadosGravados.Add($"Manipulação de objeto: {objeto.name}, Posição: {objetoPosition}");
        }

        // Capturar mudanças de câmera
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            dadosGravados.Add("Mudança de câmera: Botão A pressionado");
        }
        else if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            dadosGravados.Add("Mudança de câmera: Botão B pressionado");
        }

        // Capturar modificação de zoom
        float thumbstickY = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;
        if (thumbstickY > 0)
        {
            dadosGravados.Add("Modificação de zoom: Zoom In");
        }
        else if (thumbstickY < 0)
        {
            dadosGravados.Add("Modificação de zoom: Zoom Out");
        }

        // Capturar modificação de textura do mapa
        // ...

        // Capturar tempo decorrido
        float tempoDecorrido = Time.time;

        // Armazenar todos os dados em uma lista (ou outro formato de sua escolha)
        dadosGravados.Add($"Tempo decorrido: {tempoDecorrido}");

        // Checar se a gravação deve ser interrompida
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gravacaoAtiva = false;
            SalvarDadosParaCSV();
        }
    }

    private void SalvarDadosParaCSV()
    {
        // Definir o caminho do arquivo CSV
        string filePath = "C:/Users/jerso/Documents/GitHub/VR-SandBox-Remastered/Assets/Resultados/dados_gravados.csv";

        // Verificar se o arquivo já existe
        bool arquivoExiste = File.Exists(filePath);

        // Criar um arquivo CSV e escrever os dados
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            if (!arquivoExiste)
            {
                // Se o arquivo não existir, adicione um cabeçalho
                writer.WriteLine("Ação, Objeto, Tempo");
            }

            foreach (string dado in dadosGravados)
            {
                writer.WriteLine(dado);
            }
        }
    }
}
