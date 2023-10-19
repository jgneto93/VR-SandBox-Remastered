using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToObject : MonoBehaviour
{
    public Transform objectToMove; // Referência ao objeto principal que será movido
    public float moveSpeed = 1.0f; // Velocidade de movimento

    private Vector3 initialPosition; // Posição inicial da esfera

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // Calcula o deslocamento da esfera em relação à posição inicial
        Vector3 displacement = transform.position - initialPosition;

        // Calcula o deslocamento desejado para o objeto principal com base no deslocamento da esfera
        Vector3 targetDisplacement = displacement * moveSpeed;

        // Move o objeto principal com base no deslocamento calculado
        objectToMove.position = initialPosition + targetDisplacement;
    }
}

