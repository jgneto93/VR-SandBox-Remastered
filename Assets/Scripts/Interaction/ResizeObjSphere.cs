using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using System.Linq;

public class ResizeObjSphere : MonoBehaviour
{
    public float reductionFactor = 0.5f;
    public float resizeTime = 1.0f;
    public GameObject spherePrefab;

    private Dictionary<GameObject, Vector3> initialScales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> targetScales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, GameObject> attachedSpheres = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, LineRenderer> attachedLineRenderers = new Dictionary<GameObject, LineRenderer>();
    private bool resizingInProgress = false;
    private bool hasResized = false;
    
    private void Update()
    {
        if (!resizingInProgress)
        {
            if (!hasResized && OVRInput.GetDown(OVRInput.Button.One))
            {
                StartResize();
            }
            else if (hasResized && OVRInput.GetDown(OVRInput.Button.Two))
            {
                RestoreAllSizes();
            }
        }
        
        GameObject[] objectsWithIsOnTerrainTag = GameObject.FindGameObjectsWithTag("isOnTerrain");
        List<GameObject> objectsToRemove = new List<GameObject>();

        foreach (GameObject obj in initialScales.Keys)
        {
            if (!objectsWithIsOnTerrainTag.Contains(obj))
            {
                objectsToRemove.Add(obj);
            }
        }

        foreach (GameObject obj in objectsToRemove)
        {
            RestoreSize(obj);
        }

        foreach (GameObject obj in attachedSpheres.Keys)
        {
            if (attachedSpheres.TryGetValue(obj, out GameObject sphere) && attachedLineRenderers.TryGetValue(obj, out LineRenderer lineRenderer))
            {
                Vector3 spherePosition = sphere.transform.position;
                Vector3 lineRendererEndPosition = obj.transform.position;

                lineRenderer.SetPosition(0, spherePosition);
                lineRenderer.SetPosition(1, lineRendererEndPosition);
            }
        }
    }

    public void StartResize()
    {
        if (!resizingInProgress && !hasResized)
        {
            GameObject[] objectsToResize = GameObject.FindGameObjectsWithTag("isOnTerrain");

            if (objectsToResize.Length > 0)
            {
                foreach (GameObject obj in objectsToResize)
                {
                    if (!hasResized) // Verifique se o objeto não foi redimensionado ainda
                    {
                        DistanceGrabInteractable grabbable = obj.GetComponent<DistanceGrabInteractable>();
                        if (grabbable != null)
                        {
                            grabbable.enabled = false;
                        }
                    }
                }

                StartCoroutine(ResizeObjectsOnTerrain(objectsToResize));

                hasResized = true; // Marque o objeto como redimensionado
            }
            else
            {
                Debug.LogWarning("Nenhum objeto em contato com o terreno encontrado para redimensionar.");
            }
        }
    }

    private IEnumerator ResizeObjectsOnTerrain(GameObject[] objects)
    {
        resizingInProgress = true;

        float elapsedTime = 0.0f;
        initialScales.Clear();
        targetScales.Clear();

        foreach (GameObject obj in objects)
        {
            initialScales[obj] = obj.transform.localScale;
            targetScales[obj] = obj.transform.localScale * reductionFactor;
        }

        while (elapsedTime < resizeTime)
        {
            elapsedTime += Time.deltaTime;

            foreach (GameObject obj in objects)
            {
                obj.transform.localScale = Vector3.Lerp(initialScales[obj], targetScales[obj], elapsedTime / resizeTime);
            }

            yield return null;
        }

        resizingInProgress = false;

        foreach (GameObject obj in objects)
        {
            AttachSphere(obj);
        }
    }

    private void AttachSphere(GameObject obj)
    {
        if (!attachedSpheres.ContainsKey(obj))
        {
            GameObject sphereCopy = Instantiate(spherePrefab);
            sphereCopy.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Vector3 spherePosition = obj.transform.position;
            spherePosition.y += 0.7f;
            sphereCopy.transform.position = spherePosition;

            // Faça a esfera copiada ser filha do objeto
            obj.transform.SetParent(sphereCopy.transform, true);

            // Add a LineRenderer component to draw the line between sphere and vehicle
            LineRenderer newLineRenderer = sphereCopy.AddComponent<LineRenderer>();
            newLineRenderer.positionCount = 2;
            newLineRenderer.startWidth = 0.005f;
            newLineRenderer.endWidth = 0.005f;

            newLineRenderer.startColor = Color.white;
            newLineRenderer.endColor = Color.white;
            newLineRenderer.material.color = Color.white;
            newLineRenderer.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
            Debug.Log("Linha feita");

            // Store a reference to the attached sphere and LineRenderer
            attachedSpheres[obj] = sphereCopy;
            attachedLineRenderers[obj] = newLineRenderer;

            Debug.LogWarning("Esfera criada");
        }
    }

    private void RestoreSize(GameObject obj)
    {
        if (initialScales.TryGetValue(obj, out Vector3 initialScale))
        {
            // Remover o objeto pai da esfera
            obj.transform.SetParent(null);

            if (!resizingInProgress)
            {
                StartCoroutine(RestoreSizeOverTime(obj, initialScale));

                // Destruir a esfera e o LineRenderer associados ao objeto
                if (attachedSpheres.TryGetValue(obj, out GameObject attachedSphere))
                {
                    Destroy(attachedSphere);
                    attachedSpheres.Remove(obj);
                }
                if (attachedLineRenderers.TryGetValue(obj, out LineRenderer lineRenderer))
                {
                    Destroy(lineRenderer);
                    attachedLineRenderers.Remove(obj);
                }
            }
            else
            {
                obj.transform.localScale = initialScale;
            }
        }
    }

    public void RestoreAllSizes()
    {
        if (!resizingInProgress)  // Verifica se não está redimensionando
        {
            resizingInProgress = false;
            foreach (GameObject obj in initialScales.Keys)
            {
                // Remover o objeto pai da esfera
                obj.transform.SetParent(null);

                DistanceGrabInteractable grabbable = obj.GetComponent<DistanceGrabInteractable>();
                if (grabbable != null)
                {
                    grabbable.enabled = true;
                }

                if (!resizingInProgress)
                {
                    StartCoroutine(RestoreSizeOverTime(obj, initialScales[obj]));

                    // Destruir a esfera e o LineRenderer associados ao objeto
                    if (attachedSpheres.TryGetValue(obj, out GameObject attachedSphere))
                    {
                        Destroy(attachedSphere);
                    }
                    if (attachedLineRenderers.TryGetValue(obj, out LineRenderer lineRenderer))
                    {
                        Destroy(lineRenderer);
                    }

                    attachedSpheres.Remove(obj);
                    attachedLineRenderers.Remove(obj);
                }
                else
                {
                    obj.transform.localScale = initialScales[obj];
                }

                hasResized = false;
            }
        }
    }

    private IEnumerator RestoreSizeOverTime(GameObject obj, Vector3 initialScale)
    {
        float elapsedTime = 0.0f;
        Vector3 currentScale = obj.transform.localScale;

        while (elapsedTime < resizeTime)
        {
            elapsedTime += Time.deltaTime;
            obj.transform.localScale = Vector3.Lerp(currentScale, initialScale, elapsedTime / resizeTime);
            yield return null;
        }

    }
}
