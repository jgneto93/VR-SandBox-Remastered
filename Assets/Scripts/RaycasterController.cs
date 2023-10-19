using UnityEngine;

public class RaycasterController : MonoBehaviour
{
    public LineRenderer raycaster;

    private void Start()
    {
        raycaster = gameObject.AddComponent<LineRenderer>();
        raycaster.startWidth = 0.01f;
        raycaster.endWidth = 0.01f;
        raycaster.useWorldSpace = true;
    }

    private void Update()
    {
        Vector3 raycastOrigin = transform.position;
        Vector3 raycastDirection = Vector3.down;

        int layerMask = 1 << LayerMask.NameToLayer("Default"); 

        RaycastHit hit;
        if (Physics.Raycast(raycastOrigin, raycastDirection, out hit, 10f, layerMask))
        {
            raycaster.SetPosition(0, raycastOrigin);
            raycaster.SetPosition(1, hit.point);
        }
        else
        {
            raycaster.SetPosition(0, raycastOrigin);
            raycaster.SetPosition(1, raycastOrigin + raycastDirection * 10f);
        }
    }
}
