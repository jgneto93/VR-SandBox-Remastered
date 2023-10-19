using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

public class ActivateOVRGrabbableOnSpheres : MonoBehaviour
{
    void Start()
    {
        // Encontra todos os objetos com nome "Sphere" na cena
        GameObject[] spheres = GameObject.FindGameObjectsWithTag("Sphere");

        foreach (GameObject sphere in spheres)
        {
            // Adiciona e ativa o componente OVRGrabbable
            var grabbable = sphere.AddComponent<OVRGrabbable>();
            grabbable.enabled = true;
        }
    }
}
