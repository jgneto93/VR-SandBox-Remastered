using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using Oculus.Interaction;

public class ChangeColorInSelectionCone : MonoBehaviour
{
    private DistanceGrabInteractable distanceGrabInteractable;
    public Material highlightMaterial; // Material com a cor de destaque
    public float grabDistance = 0.1f; // Distância de agarrar desejada

    private MeshRenderer[] childMeshRenderers; // Array de MeshRenderers dos filhos
    private Material[] originalMaterials; // Array de materiais originais dos filhos

    void Start()
    {
        // Obtém a referência ao componente DistanceGrabber
        distanceGrabInteractable = GetComponent<DistanceGrabInteractable>();

        // Obtém todos os MeshRenderers dos filhos
        Transform[] childTransforms = GetComponentsInChildren<Transform>();
        List<MeshRenderer> meshRenderersList = new List<MeshRenderer>();

        foreach (Transform childTransform in childTransforms)
        {
            MeshRenderer meshRenderer = childTransform.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderersList.Add(meshRenderer);
            }
        }

        childMeshRenderers = meshRenderersList.ToArray();
        originalMaterials = new Material[childMeshRenderers.Length];

        for (int i = 0; i < childMeshRenderers.Length; i++)
        {
            originalMaterials[i] = childMeshRenderers[i].material;
        }
    }

    void Update()
    {
        // Verifica a distância entre o objeto e o controlador
        float distanceToController = Vector3.Distance(transform.position, distanceGrabInteractable.transform.position);

        // Verifica se o objeto está dentro do raio de alcance desejado
        if (distanceToController <= grabDistance)
        {
            // Se estiver dentro do alcance, aplique o material de destaque a todos os filhos com MeshRenderer
            for (int i = 0; i < childMeshRenderers.Length; i++)
            {
                childMeshRenderers[i].material = highlightMaterial;
            }
        }
        else
        {
            // Caso contrário, restaure os materiais originais de todos os filhos com MeshRenderer
            for (int i = 0; i < childMeshRenderers.Length; i++)
            {
                childMeshRenderers[i].material = originalMaterials[i];
            }
        }
    }
}
