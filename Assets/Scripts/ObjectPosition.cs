using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ObjectPosition : MonoBehaviour
{
    // Coordenadas de latitude e longitude do ponto de referência da carta topográfica
    public float latRef = -22.951199f; // Substitua pelas coordenadas reais
    public float lonRef = -43.210577f; // Substitua pelas coordenadas reais

    // Escala de pixels
    public float pixelScale = 1000.0f; // Cada metro do terreno digital corresponde a 1 km no mundo real


    public void Salvar()
    {

         // Procura todos os objetos na cena com a tag "ObjectOnTerrain"
         GameObject[] objectsOnTerrain = GameObject.FindGameObjectsWithTag("isOnTerrain");

        // Cria um arquivo de texto para salvar as coordenadas
        StreamWriter writer = new StreamWriter("object_coordinates.txt");

        // Salva as coordenadas de todos os objetos na cena com a tag "ObjectOnTerrain"
        foreach (GameObject obj in objectsOnTerrain)
        {
                // Verifica se o objeto está posicionado sobre o terreno
            if (obj.transform.position.y >= Terrain.activeTerrain.SampleHeight(obj.transform.position))
            {
                // Obtém a posição do objeto em coordenadas de pixel
                Vector3 positionInPixels = obj.transform.position;

                // Converte as coordenadas de pixel em coordenadas de latitude e longitude
                float lat = latRef + (positionInPixels.z / pixelScale);
                float lon = lonRef + (positionInPixels.x / pixelScale);

                // Salva as coordenadas em um arquivo de texto
                writer.WriteLine(obj.name + ":");
                writer.WriteLine("Latitude: " + lat);
                writer.WriteLine("Longitude: " + lon);
                writer.WriteLine("");
                }
        }

        // Fecha o arquivo de texto
        writer.Close();

        Debug.Log("Coordenadas dos objetos salvos em object_coordinates.txt");
    }
}
