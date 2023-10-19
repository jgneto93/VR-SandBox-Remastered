using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VisMenu : MonoBehaviour
{
    [SerializeField] private string nomeTarefa;
    public List<GameObject> terrainList;
    private int currentTerrainIndex = 0;

    public void Change()
    {
        SceneManager.LoadScene(nomeTarefa);
    }

    public void NextTerrain()
    {
        // Desativa o terreno atual.
        terrainList[currentTerrainIndex].SetActive(false);

        // Incrementa o índice para obter o próximo terreno.
        currentTerrainIndex++;

        // Verifica se chegamos ao final da lista de terrenos.
        if (currentTerrainIndex >= terrainList.Count)
        {
            // Volta para o início da lista, se necessário.
            currentTerrainIndex = 0;
        }

        // Ativa o próximo terreno.
        terrainList[currentTerrainIndex].SetActive(true);
    }

    // Método para adicionar manualmente os terrenos à lista.
    public void AddTerrainToList(GameObject terrain)
    {
        if (!terrainList.Contains(terrain))
        {
            terrainList.Add(terrain);
            terrain.SetActive(false);
        }
    }
}

