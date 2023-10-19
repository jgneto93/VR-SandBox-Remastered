using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResizeObjects : MonoBehaviour
{
    public float reductionFactor = 0.5f;
    public float resizeTime = 1.0f; // The time it takes to resize the objects.

    private bool resizingInProgress = false;
    private Dictionary<GameObject, Vector3> initialScales = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> targetScales = new Dictionary<GameObject, Vector3>();
    private bool resizedOnce = false; // Flag to track if objects have been resized

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

        // Check if objects have lost the "isOnTerrain" tag and restore their size.
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
        // Encontra todos os objetos com a tag "isOnTerrain"
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

        // Store the initial and target scales for each object
        foreach (GameObject obj in objects)
        {
            initialScales[obj] = obj.transform.localScale;
            targetScales[obj] = obj.transform.localScale * reductionFactor;
        }

        // Perform the resize over time
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
    }

    private void RestoreSize(GameObject obj)
    {
        // Check if the object has been resized before
        if (initialScales.TryGetValue(obj, out Vector3 initialScale))
        {
            if (!resizingInProgress)
            {
                StartCoroutine(RestoreSizeOverTime(obj, initialScale));
                resizedOnce = false; // Reset the flag when restoring size
            }
            else
            {
                // If resizing is in progress, restore the size immediately
                obj.transform.localScale = initialScale;
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
                resizedOnce = false; // Reset the flag when restoring size
            }
            else
            {
                // If resizing is in progress, restore the size immediately
                obj.transform.localScale = initialScales[obj];
            }
        }
    }

    private IEnumerator RestoreSizeOverTime(GameObject obj, Vector3 initialScale)
    {
        float elapsedTime = 0.0f;
        Vector3 currentScale = obj.transform.localScale;

        // Restore the size over time
        while (elapsedTime < resizeTime)
        {
            elapsedTime += Time.deltaTime;
            obj.transform.localScale = Vector3.Lerp(currentScale, initialScale, elapsedTime / resizeTime);
            yield return null;
        }
    }
}