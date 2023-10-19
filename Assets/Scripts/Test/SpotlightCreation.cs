using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spotlight : MonoBehaviour
{
    public GameObject spotlightPrefab; // Prefab da spotlight
    private GameObject spotlightInstance; // Instância da spotlight criada

    private void Start()
    {
        // Instancia a spotlightPrefab
        spotlightInstance = Instantiate(spotlightPrefab, transform.position, Quaternion.identity);
        
        // Ajusta a posição da spotlight para a parte central posterior do prefab
        spotlightInstance.transform.parent = transform;
        spotlightInstance.transform.localPosition = new Vector3(0f, 0f, -1f); // Exemplo: -1f ajusta a posição posterior
        
        // Desativa a spotlight
        spotlightInstance.SetActive(false);
    }
}
