using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class Pen : MonoBehaviour
{
    [Header("Pen Properties")]
    public Material drawingMaterial;
    public Material tipMaterial;
    [Range(0.01f, 0.1f)]
    public float penWidth = 0.01f;
    public Color[] penColors;

    public Terrain terrain;

    private LineRenderer currentDrawing;
    private int index;
    private int currentColorIndex;
    private LineRenderer collisionRayRenderer;

    private void Start()
    {
        currentColorIndex = 0;
        terrain = Terrain.activeTerrain;

        collisionRayRenderer = gameObject.AddComponent<LineRenderer>();
        collisionRayRenderer.material = drawingMaterial;
        collisionRayRenderer.startWidth = collisionRayRenderer.endWidth = 0.005f;
        collisionRayRenderer.useWorldSpace = true;
        collisionRayRenderer.enabled = false;
    }

    public void MudarCor ()
    {
        SwitchColor();
    }

    private void Update()
    {
        bool isRightHandDrawing = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
        bool isLeftHandDrawing = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger);

        if (isRightHandDrawing || isLeftHandDrawing)
        {
            Draw();

            UpdateCollisionRayRenderer();
        }
        else
        {
            if (currentDrawing != null)
            {
                currentDrawing = null;
            }

            // Desative o raio de colisão quando não estiver desenhando
            collisionRayRenderer.enabled = false;
        }
    }

    private void Draw()
    {
        // Crie um raio para detectar a colisão com o terreno
        Ray ray = new Ray(transform.position, transform.up);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Verifique se a colisão ocorre com o terreno
            if (hit.collider.CompareTag("Terrain")) 
            {
                // Use a posição de colisão como a posição do desenho
                Vector3 currentPosition = hit.point;

                if (currentDrawing == null)
                {
                    // Crie um novo desenho se não houver um desenho atual
                    index = 0;
                    currentDrawing = new GameObject().AddComponent<LineRenderer>();
                    currentDrawing.material = tipMaterial;
                    currentDrawing.startColor = currentDrawing.endColor = penColors[currentColorIndex];
                    currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
                    currentDrawing.positionCount = 1;
                    currentDrawing.SetPosition(0, currentPosition);
                }
                else
                {
                    Vector3 currentPos = currentDrawing.GetPosition(index);
                    if (Vector3.Distance(currentPos, currentPosition) > 0.01f)
                    {
                        index++;
                        currentDrawing.positionCount = index + 1;
                        currentDrawing.SetPosition(index, currentPosition);
                    }
                }
            }
        }
    }

    private void SwitchColor()
    {
        if (currentColorIndex == penColors.Length - 1)
        {
            currentColorIndex = 0;
        }
        else
        {
            currentColorIndex++;
        }
        tipMaterial.color = penColors[currentColorIndex];
    }

    private void UpdateCollisionRayRenderer()
    {
        Vector3 rayStartPos = transform.position;
        Vector3 rayDirection = transform.up; //verificar a orientação do feixe

        collisionRayRenderer.enabled = true;
        collisionRayRenderer.startColor = collisionRayRenderer.endColor = penColors[currentColorIndex];
        collisionRayRenderer.SetPosition(0, rayStartPos);
        collisionRayRenderer.SetPosition(1, rayStartPos + rayDirection * 5.0f); // Defina o comprimento do raio como desejar
    }
}
