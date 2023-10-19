using UnityEngine;

public class OculusRaycaster : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private GameObject hitObject; // Objeto atualmente atingido pelo raio
    private Material originalMaterial; // Material original do objeto

    public LayerMask raycastLayer;
    public Material highlightMaterial; // Material com a cor de destaque

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayer))
        {
            // Verifique se o objeto atingido é diferente do objeto anteriormente atingido
            if (hitObject != hit.collider.gameObject)
            {
                // Restaure a cor original do objeto anterior
                RestoreOriginalColor();
                Debug.Log("Objeto atingido: " + hit.collider.gameObject.name);

                // Armazene a referência ao novo objeto atingido
                hitObject = hit.collider.gameObject;

                // Armazene o material original do objeto
                originalMaterial = hitObject.GetComponent<Renderer>().material;

                // Aplique o material de destaque ao objeto atingido
                ApplyHighlightMaterial();
            }
        }
        else
        {
            // Se o raio não atingir nada, restaure a cor original do objeto anteriormente atingido
            RestoreOriginalColor();
            Debug.Log("Nada atingido.");

            // Limpe a referência ao objeto atingido
            hitObject = null;
        }
    }

    // Função para aplicar o material de destaque a um objeto
    private void ApplyHighlightMaterial()
    {
        if (hitObject == null)
            return;

        hitObject.GetComponent<Renderer>().material = highlightMaterial;
    }

    // Função para restaurar a cor original de um objeto
    private void RestoreOriginalColor()
    {
        if (hitObject == null || originalMaterial == null)
            return;

        hitObject.GetComponent<Renderer>().material = originalMaterial;
    }
}
