using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabSelector : MonoBehaviour
{
    public GameObject[] availablePrefabs; // Uma matriz de prefabs disponíveis
    public SpawnManager spawnManager; // Referência ao seu SpawnManager

    public Dropdown prefabDropdown; // O Dropdown UI que permite selecionar o prefab

    private void Start()
    {
        // Certifique-se de que o Dropdown e o SpawnManager estejam definidos
        if (prefabDropdown == null || spawnManager == null)
        {
            Debug.LogError("Dropdown ou SpawnManager não estão definidos!");
            return;
        }

        // Preencha o Dropdown com os nomes dos prefabs disponíveis
        List<string> prefabNames = new List<string>();
        foreach (GameObject prefab in availablePrefabs)
        {
            prefabNames.Add(prefab.name);
        }
        prefabDropdown.AddOptions(prefabNames);
    }

    // Função chamada quando o usuário seleciona um novo prefab no Dropdown
    public void OnPrefabSelected()
    {
        // Obtenha o índice do prefab selecionado no Dropdown
        int selectedIndex = prefabDropdown.value;

        // Obtenha o prefab correspondente ao índice selecionado
        GameObject selectedPrefab = availablePrefabs[selectedIndex];
        Debug.Log(selectedPrefab.name);

        // Atualize o prefab no SpawnManager
        spawnManager.SpawnObject(selectedPrefab);
    }
}
