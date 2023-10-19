using UnityEngine;

public class ActivateAndSetParent : MonoBehaviour
{
    public GameObject parentObject;
    public GameObject childObject;

    public void SwapHierarchyObjects()
    {
        // Verifica se os objetos são válidos
        if (parentObject != null && childObject != null)
        {
            // Salva a posição e rotação do objeto filho em relação ao objeto pai
            Vector3 localPosition = childObject.transform.localPosition;
            Quaternion localRotation = childObject.transform.localRotation;

            // Remove o objeto filho da hierarquia do objeto pai
            childObject.transform.parent = null;

            // Define o objeto filho como pai do objeto pai
            parentObject.transform.parent = childObject.transform;

            // Restaura a posição e rotação do objeto pai em relação ao objeto filho
            parentObject.transform.localPosition = Vector3.zero;
            parentObject.transform.localRotation = Quaternion.identity;

            // Define a posição e rotação do objeto filho em relação ao antigo pai
            childObject.transform.localPosition = localPosition;
            childObject.transform.localRotation = localRotation;
        }
        else
        {
            Debug.LogWarning("Objetos não atribuídos corretamente.");
        }
    }
}