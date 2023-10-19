using UnityEngine;

public class DrawOnCollision : MonoBehaviour
{
    public float sphereRadius = 0.5f; // Raio da esfera a ser gerada
    public Color sphereColor = Color.red; // Cor da esfera a ser gerada
    public Transform laserOrigin; // Ponto de origem do raio do laser

    private bool isTriggerPressed = false; // Flag para indicar se o gatilho está pressionado

    void Update()
    {
        // Verifica se o gatilho do controle Oculus Quest 2 está pressionado
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            isTriggerPressed = true;
        }
        else
        {
            isTriggerPressed = false;
        }

        // Verifica se o gatilho está pressionado
        if (isTriggerPressed)
        {
            // Cria a esfera no ponto de origem do laser
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = laserOrigin.position;
            sphere.transform.localScale = Vector3.one * sphereRadius;
            sphere.GetComponent<Renderer>().material.color = sphereColor;
            // Aqui você pode adicionar qualquer lógica adicional à esfera gerada, se necessário
        }
    }
}
