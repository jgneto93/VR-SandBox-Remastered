using UnityEngine;

public class PaintingTool : MonoBehaviour
{
    public OVRInput.Button triggerButton = OVRInput.Button.PrimaryIndexTrigger;
    public OVRInput.Button startPaintingButton = OVRInput.Button.One;
    public float paintingDistance = 10f;
    public Color paintColor = Color.black;
    public float paintRadius = 0.1f;

    private void Update()
    {
        if (OVRInput.GetDown(startPaintingButton))
        {
            if (OVRInput.Get(triggerButton))
            {
                StartPainting();
            }
        }

        if (OVRInput.Get(triggerButton))
        {
            Paint();
        }
    }

    private void StartPainting()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, paintingDistance))
        {
            PaintAt(hit.point);
        }
    }

    private void Paint()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, paintingDistance))
        {
            PaintAt(hit.point);
        }
    }

    private void PaintAt(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, paintRadius);
        foreach (Collider collider in colliders)
        {
            MeshRenderer meshRenderer = collider.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material.color = paintColor;
            }
        }
    }
}
