using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceMeasure : MonoBehaviour
{
    public LineRenderer laserLineRenderer;
    public GameObject spherePrefab;
    public Transform terrain;
    public LayerMask terrainLayer;
    public TextMesh distanceTextPrefab;

    private bool isMenuActive = false;

    private List<GameObject> spheres = new List<GameObject>();
    private TextMesh distanceText;

    private void Start()
    {
        laserLineRenderer.enabled = false;
        laserLineRenderer.material.color = Color.black; // Define a cor da linha como preto
        distanceText = Instantiate(distanceTextPrefab);
        distanceText.gameObject.SetActive(false);
    }

    public void OnButtonClick()
    {
        isMenuActive = !isMenuActive;
        laserLineRenderer.enabled = isMenuActive;
        distanceText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isMenuActive)
        {
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
            {
                ShootLaser();
            }
        }
    }

    private void ShootLaser()
    {
        Vector3 startPos = laserLineRenderer.transform.position;
        Vector3 direction = laserLineRenderer.transform.forward;

        if (Physics.Raycast(startPos, direction, out RaycastHit hit, terrainLayer))
        {
            Vector3 collisionPoint = hit.point;
            CreateSphereAtPoint(collisionPoint);

            if (spheres.Count > 1)
            {
                Vector3 lastPoint = spheres[spheres.Count - 2].transform.position;
                Vector3 currentPoint = spheres[spheres.Count - 1].transform.position;

                // Calcula a distância euclidiana 3D entre os pontos
                float distance = Vector3.Distance(lastPoint, currentPoint);

                // Converte a distância para a escala do terreno
                float terrainScaleX = terrain.localScale.x;
                float terrainScaleZ = terrain.localScale.z;
                float realDistanceX = (distance / terrainScaleX) * 13840; // 13.84 km em escala real
                float realDistanceZ = (distance / terrainScaleZ) * 13840; // 13.84 km em escala real

                // Atualiza o TextMesh com a distância
                distanceText.text = $"Distância: X:{realDistanceX:F2} km, Z:{realDistanceZ:F2} km";
                distanceText.transform.position = currentPoint + Vector3.up * 0.1f;
                distanceText.gameObject.SetActive(true);
            }
        }
    }

    private void CreateSphereAtPoint(Vector3 position)
    {
        GameObject sphere = Instantiate(spherePrefab, position, Quaternion.identity);
        spheres.Add(sphere);
        sphere.GetComponent<Renderer>().material.color = Color.black; // Define a cor da esfera como preto

        if (spheres.Count > 1)
        {
            GameObject sphere1 = spheres[spheres.Count - 2];
            GameObject sphere2 = spheres[spheres.Count - 1];

            // Conecte as esferas com uma linha
            LineRenderer lineRenderer = sphere1.AddComponent<LineRenderer>();
            lineRenderer.material.color = Color.black; // Define a cor da linha como preto
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, sphere1.transform.position);
            lineRenderer.SetPosition(1, sphere2.transform.position);
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
        }
    }
}
