using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;

[RequireComponent(typeof(OVRGrabbable))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OVRGrabber))]

public class test : MonoBehaviour
{
    private OVRGrabbable grabbable;
    private Rigidbody rb;

    private void Awake()
    {
        grabbable = GetComponent<OVRGrabbable>();
        rb = GetComponent<Rigidbody>();

        // Configurar o componente Grabbable
        if (grabbable == null)
        {
            grabbable = gameObject.AddComponent<OVRGrabbable>();
        }

        // Configurar o componente Rigidbody
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    private void Start()
    {
        // Inicializar o componente Grabbable
        grabbable.enabled = true;
    }
}