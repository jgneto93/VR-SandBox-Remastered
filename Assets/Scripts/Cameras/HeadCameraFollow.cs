using UnityEngine;

public class HeadCameraFollow : MonoBehaviour
{
    public Transform headTransform; // O Transform da cabeça do jogador (geralmente é o mesmo usado pela câmera principal)

    private void Update()
    {
        // Atualiza a posição e a rotação da câmera com base na posição e rotação da cabeça
        transform.position = headTransform.position;
        transform.rotation = headTransform.rotation;
    }
}
