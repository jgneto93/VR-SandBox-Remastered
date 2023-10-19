using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenOlder : MonoBehaviour
{
    [Header("Pen Properties")]
    public Transform tip;
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
    private bool isErasing = false;

    private void Start()
    {
        currentColorIndex = 0;
        tipMaterial.color = penColors[currentColorIndex];
        terrain = Terrain.activeTerrain;

        collisionRayRenderer = gameObject.AddComponent<LineRenderer>();
        collisionRayRenderer.material = drawingMaterial;
        collisionRayRenderer.startWidth = collisionRayRenderer.endWidth = 0.005f;
        collisionRayRenderer.useWorldSpace = true;
        collisionRayRenderer.enabled = false;
    }

    private void Update()
    {
        bool isRightHandDrawing = (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger));
        bool isLeftHandDrawing = (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger));;
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

            collisionRayRenderer.enabled = false;
        }
    }

    public void Erase()
    {
        isErasing = true; // Ative o modo de apagamento
        tipMaterial.color = Color.white; // Defina a cor do material para branco
    }

    public void ResumeDrawing()
    {
        isErasing = false; // Desative o modo de apagamento
        tipMaterial.color = penColors[currentColorIndex]; // Restaure a cor do material
        collisionRayRenderer.startColor = collisionRayRenderer.endColor = penColors[currentColorIndex]; // Restaure a cor do raio
    }

    private void Draw()
    {
        if (isErasing)
        {
            // Modo de apagamento ativado
            var currentPosition = tip.position;
            // Crie um raio para detectar colisões com os desenhos existentes
            RaycastHit2D eraseHit = Physics2D.Raycast(currentPosition, Vector2.down);

            if (eraseHit.collider != null)
            {
                // Se o raio colidir com um objeto de desenho, apague-o
                CircleCollider2D drawingToErase = eraseHit.collider.GetComponent<CircleCollider2D>();
                if (drawingToErase != null)
                {
                    Destroy(drawingToErase.gameObject); // Apague o objeto de desenho
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            // Obtenha a posição atual do desenho
            var currentPosition = tip.position;
            // Crie um raio para detectar a colisão com o terreno
            Ray ray = new Ray(currentPosition, Vector3.down);
            int layerMask = LayerMask.GetMask("Terrain");

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                currentPosition = hit.point;
            }
            else
            {
                // Se o raio não colidir com o terreno, ignore o desenho
                return;
            }
            
            if (currentDrawing == null)
            {
                // Crie um novo desenho se não houver um desenho atual
                index = 0;
                currentDrawing = new GameObject().AddComponent<LineRenderer>();
                currentDrawing.material = drawingMaterial;
                currentDrawing.startColor = currentDrawing.endColor = penColors[currentColorIndex];
                currentDrawing.startWidth = currentDrawing.endWidth = penWidth;
                currentDrawing.positionCount = 1;
                currentDrawing.SetPosition(0, currentPosition);
                CircleCollider2D collider = currentDrawing.gameObject.AddComponent<CircleCollider2D>();
            }
            else
            {
                var currentPos = currentDrawing.GetPosition(index);
                if (Vector3.Distance(currentPos, currentPosition) > 0.01f)
                {
                    index++;
                    currentDrawing.positionCount = index + 1;
                    currentDrawing.SetPosition(index, currentPosition);
                }
            }
        }
    }

    public void SwitchColor()
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
        if(isErasing)
        {
            Vector3 rayStartPos = tip.position;
            Vector3 rayDirection = Vector3.down; //verificar a orientação do feixe

            collisionRayRenderer.enabled = true;
            collisionRayRenderer.startColor = collisionRayRenderer.endColor = Color.white;
            collisionRayRenderer.SetPosition(0, rayStartPos);
            collisionRayRenderer.SetPosition(1, rayStartPos + rayDirection * 5.0f); // Defina o comprimento do raio como desejar
        }
        else
        {
            Vector3 rayStartPos = tip.position;
            Vector3 rayDirection = Vector3.down; //verificar a orientação do feixe

            collisionRayRenderer.enabled = true;
            collisionRayRenderer.startColor = collisionRayRenderer.endColor = penColors[currentColorIndex];
            collisionRayRenderer.SetPosition(0, rayStartPos);
            collisionRayRenderer.SetPosition(1, rayStartPos + rayDirection * 5.0f); // Defina o comprimento do raio como desejar
        }
    }
}