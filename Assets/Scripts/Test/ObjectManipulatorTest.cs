using UnityEngine;
using OculusSampleFramework;

public class ObjectManipulatorTest : MonoBehaviour
{
    public Transform targetObject; // O objeto que será manipulado
    public float positionSpeed = 1.0f; // Velocidade de atualização da posição
    public OVRGrabbable grabbable;

    private Vector3 initialPositionOffset;
    private Vector3 lastControllerPosition;

    private bool isGrabbing = false; // Indica se a manipulação está ocorrendo

    private void Start()
    {
        if (targetObject == null || grabbable == null)
        {
            Debug.LogError("O objeto alvo ou o grabbable não foi atribuído!");
            enabled = false;
            return;
        }

        initialPositionOffset = targetObject.position - transform.position;
    }

    private void Update()
    {
        bool isRightHandGrabbing = grabbable.grabbedBy == OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        bool isLeftHandGrabbing = grabbable.grabbedBy == OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);

        if (isRightHandGrabbing || isLeftHandGrabbing)
        {
            Move();
            isGrabbing = true; // Mantém o movimento ativo enquanto agarrado
        }
        else if (isGrabbing)
        {
            isGrabbing = false; // Encerra o movimento quando o objeto não está sendo agarrado
        }
    }

    private void Move()
    {
        Vector3 targetPosition = transform.position + initialPositionOffset;
        targetObject.position = Vector3.Lerp(targetObject.position, targetPosition, Time.deltaTime * positionSpeed);
    }
}

