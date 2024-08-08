using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastAndInstantiate : MonoBehaviour
{
    public OVRInput.Button triggerButton = OVRInput.Button.PrimaryIndexTrigger;
    public GameObject spherePrefab;

    private PointerEventData pointerEventData;
    private GameObject laserPointer;
    private RaycastHit hit;

    private void Start()
    {
        // Obter referência para o laser pointer do pacote "UI Helpers Laser pointer"
        laserPointer = FindObjectOfType<LaserPointer>().gameObject;
    }

    private void Update()
    {
        if (OVRInput.GetDown(triggerButton, OVRInput.Controller.RTouch))
        {
            // Verificar se o raio do laser colidiu com algum objeto
            if (Physics.Raycast(laserPointer.transform.position, laserPointer.transform.forward, out hit))
            {
                // Instanciar a esfera no ponto de colisão
                Instantiate(spherePrefab, hit.point, Quaternion.identity);
            }
        }
    }
}