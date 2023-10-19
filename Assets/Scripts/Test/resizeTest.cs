using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OculusSampleFramework;

public class resizeTest : MonoBehaviour
{
    public float reductionFactor = 0.5f;
    public float resizeTime = 1.0f;
    public LineRenderer lineRenderer;

    private bool resizingInProgress = false;
    private Dictionary<GameObject, Vector3> initialScales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> targetScales = new Dictionary<GameObject, Vector3>();
    private bool resizedOnce = false; 
    private GameObject attachedSphere = null;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One) && !resizingInProgress && !resizedOnce)
        {
            StartResize();
        }

        if (OVRInput.GetDown(OVRInput.Button.Two) && !resizingInProgress && resizedOnce)
        {
            RestoreAllSizes();
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
    }

    private void StartResize()
    {
        GameObject[] objectsToResize = GameObject.FindGameObjectsWithTag("isOnTerrain");

        if (objectsToResize.Length > 0)
        {
            StartCoroutine(ResizeObjectsOnTerrain(objectsToResize));
            resizedOnce = true;
        }
        else
        {
            Debug.LogWarning("Nenhum objeto em contato com o terreno encontrado para redimensionar.");
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
        GameObject sphere = obj.transform.Find("Sphere").gameObject;

        if (sphere != null)
        {
            // Enable the sphere
            sphere.SetActive(true);
        }
    }

    private void RestoreSize(GameObject obj)
    {
        if (initialScales.TryGetValue(obj, out Vector3 initialScale))
        {
            if (!resizingInProgress)
            {
                StartCoroutine(RestoreSizeOverTime(obj, initialScale));
                resizedOnce = false; 
                
                GameObject sphere = obj.transform.Find("Sphere").gameObject;
                if (sphere != null)
                {
                    // Disable the sphere
                    sphere.SetActive(false);
                }
            
                else
                {
                    obj.transform.localScale = initialScale;
                }
            }
        }
    }

    private void RestoreAllSizes()
    {
        foreach (GameObject obj in initialScales.Keys)
        {
            if (!resizingInProgress)
            {
                StartCoroutine(RestoreSizeOverTime(obj, initialScales[obj]));
                resizedOnce = false;

                GameObject sphere = obj.transform.Find("Sphere").gameObject;
                if (sphere != null)
                {
                    // Disable the sphere
                    sphere.SetActive(false);
                }
            }
            
            else
            {
                obj.transform.localScale = initialScales[obj];
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