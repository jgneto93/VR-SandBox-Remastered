using UnityEngine;

public class SphereMovement1 : MonoBehaviour
{
    public float yOffset = 0.1f; // Altura acima do terreno

    private Vector3 previousPosition;
    private Transform childObject; // Referência ao objeto filho

    private void Start()
    {
        previousPosition = transform.position;

        // Localize automaticamente o objeto filho com base na tag "isOnTerrain"
        childObject = FindChildObjectWithTag(transform, "isOnTerrain");

        if (childObject == null)
        {
            Debug.LogError("Objeto filho com a tag 'isOnTerrain' não encontrado na hierarquia da esfera.");
        }
    }

    private void Update()
    {
        // Verifique se a posição da esfera foi alterada nos eixos Z ou X
        Vector3 currentPosition = transform.position;
        if (currentPosition.x != previousPosition.x || currentPosition.z != previousPosition.z)
        {
            // A esfera foi movida nos eixos Z ou X
            UpdateChildObjectPosition(currentPosition);
        }
        previousPosition = currentPosition;
    }

    private void UpdateChildObjectPosition(Vector3 currentPosition)
    {
        if (childObject != null)
        {
            // Lance um raio a partir da posição da esfera em direção ao solo
            Ray ray = new Ray(currentPosition, Vector3.down);
            RaycastHit hit;

            // Verifique se o raio atinge o terreno
            if (Physics.Raycast(ray, out hit))
            {
                // Ajuste a posição do objeto filho para que ele fique na altura correta e acompanhe a esfera
                Vector3 newPosition = new Vector3(currentPosition.x, hit.point.y + yOffset, currentPosition.z);
                childObject.position = newPosition;
            }
        }
    }

    private Transform FindChildObjectWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
            Transform foundChild = FindChildObjectWithTag(child, tag);
            if (foundChild != null)
            {
                return foundChild;
            }
        }
        return null;
    }
}
