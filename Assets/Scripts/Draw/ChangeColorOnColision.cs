using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColorOnColision : MonoBehaviour
{
    public Material highlightMaterial; // Material com a cor de destaque
    private Material[] originalMaterials; // Materiais originais do objeto
    private bool isHighlighted = false; // Flag para controlar o estado de destaque

    void Start()
    {
        // Obtém os materiais originais do objeto
        MeshRenderer[] childMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalMaterials = new Material[childMeshRenderers.Length];

        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            originalMaterials[i] = childMeshRenderers[i].material;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Verifica se a colisão foi com o objeto "frustum" e se o objeto está marcado como "is trigger"
        if (collision.gameObject.CompareTag("frustum") && collision.collider.isTrigger)
        {
            // Aplica o material de destaque
            SetHighlightMaterial();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Verifica se a colisão com "frustum" saiu e se o objeto estava destacado
        if (collision.gameObject.CompareTag("frustum") && collision.collider.isTrigger && isHighlighted)
        {
            // Restaura os materiais originais
            RestoreOriginalMaterials();
        }
    }

    // Função para aplicar o material de destaque
    void SetHighlightMaterial()
    {
        if (!isHighlighted)
        {
            MeshRenderer[] childMeshRenderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer childMeshRenderer in childMeshRenderers)
            {
                childMeshRenderer.material = highlightMaterial;
            }
            isHighlighted = true;
        }
    }

    // Função para restaurar os materiais originais
    void RestoreOriginalMaterials()
    {
        MeshRenderer[] childMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            childMeshRenderers[i].material = originalMaterials[i];
        }
        isHighlighted = false;
    }
}
