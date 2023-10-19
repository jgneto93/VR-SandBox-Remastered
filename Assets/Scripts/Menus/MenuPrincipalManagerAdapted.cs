using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPrincipalManagerAdapted : MonoBehaviour
{
    public GameObject Menu;
    public GameObject DrawnMenu;
    public GameObject penDraw;
    public GameObject laserPointer;
    private Vector3 penPosition;
    public Transform drawnPoint;

    public void ChangeMenus()
    {
        DrawnMenu.SetActive(!DrawnMenu.activeSelf);
        Menu.SetActive(!Menu.activeSelf);
    }

    public void DrawingPen()
    {
        penDraw.SetActive(!penDraw.activeSelf);
        
        if (penDraw.activeSelf)
        {
            penDraw.transform.position = drawnPoint.transform.position;
        }
    }

    public void ActiveLaser()
    {
        LineRenderer lineRenderer = laserPointer.GetComponent<LineRenderer>();
        lineRenderer.enabled = !lineRenderer.enabled;
    }
    
    public void DestroySpawns()
    {
        GameObject[] spawnObjects = GameObject.FindGameObjectsWithTag("SpawnsZones");
        
        foreach (GameObject spawnObject in spawnObjects)
        {
            Destroy(spawnObject);
        }

        GameObject[] spawnPrefabs = GameObject.FindGameObjectsWithTag("SpawnTrigger");
        
        foreach (GameObject spawnPrefab in spawnPrefabs)
        {
            Destroy(spawnPrefab);
        }

    }
}
