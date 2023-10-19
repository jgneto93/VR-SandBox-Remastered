using UnityEngine;

public class CameraHeadFollow : MonoBehaviour
{
    private Transform playerHead; // Referência à cabeça do jogador

    private void Start()
    {
        // Encontre o objeto da cabeça do jogador (por exemplo, "CenterEyeAnchor")
        playerHead = FindObjectOfType<OVRCameraRig>().centerEyeAnchor;

        if (playerHead == null)
        {
            Debug.LogError("Cabeça do jogador não encontrada. Certifique-se de que o OVRCameraRig com o CenterEyeAnchor esteja na cena.");
        }
    }

    private void Update()
    {
        // Verifique se a referência à cabeça do jogador está definida
        if (playerHead != null)
        {
            // Obtenha a rotação da cabeça do jogador
            Quaternion playerHeadRotation = playerHead.rotation;

            // Aplique a rotação da cabeça do jogador à câmera
            transform.rotation = playerHeadRotation;
        }
    }
}
