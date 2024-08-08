using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour {
    public Transform target; // Referência para o objeto que a câmera deve seguir (o prefab BallPrefab).
    public float minFOV = 25.0f; // O FOV mínimo quando a câmera está mais distante.
    public float maxFOV = 50.0f; // O FOV máximo quando a câmera está mais próxima.
    public float zoomSpeed = 20.0f; // Velocidade de ajuste do FOV.
    private float minDistance = 100.0f; // Inicializa a menor distância com um valor alto.
    private float maxDistance = 0.0f; // Inicializa a maior distância com zero.
    private float oldFOV; // Armazena o valor anterior do FOV.

    void Start() {
        oldFOV = Mathf.Lerp(minFOV, maxFOV, 0.5f); // Inicializa o FOV com um valor intermediário.
    }

    void Update() {
        if (target != null) { // Verifique se o objeto alvo não é nulo.
            float distanceToTarget = Vector3.Distance(transform.position, target.position);

            // Atualize a menor e maior distância registrada.
            minDistance = Mathf.Min(minDistance, distanceToTarget);
            maxDistance = Mathf.Max(maxDistance, distanceToTarget);

            // Calcule o novo FOV com base na distância, mas suavize a transição.
            float newFOV = Mathf.Lerp(maxFOV, minFOV, (distanceToTarget - minDistance) / (maxDistance - minDistance) * zoomSpeed);

            // Garanta que o novo FOV não seja menor que o FOV mínimo.
            newFOV = Mathf.Max(newFOV, minFOV);

            // Suavize a transição do FOV.
            newFOV = Mathf.Lerp(oldFOV, newFOV, Time.deltaTime * zoomSpeed);

            Camera.main.fieldOfView = newFOV;
            oldFOV = newFOV;
            transform.LookAt(target); // Faça a câmera olhar para o objeto alvo.
        }
    }
}
