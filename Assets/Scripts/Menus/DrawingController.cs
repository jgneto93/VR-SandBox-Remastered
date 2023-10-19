using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawingController : MonoBehaviour
{
    public Texture2D squareTexture;
    public Texture2D triangleTexture;
    public Texture2D ellipseTexture;

    public Transform drawingParent;

    public List<Color> availableColors = new List<Color>();
    public Dropdown colorDropdown;

    public Slider sizeSlider;

    private Color currentColor;
    private GameObject currentShape;

    private void Start()
    {
        // Setup color dropdown options
        colorDropdown.ClearOptions();
        List<string> colorNames = new List<string>();
        foreach (Color color in availableColors)
        {
            colorNames.Add(color.ToString());
        }
        colorDropdown.AddOptions(colorNames);

        // Initialize default color and size
        currentColor = availableColors[0];
        sizeSlider.value = 1.0f;
    }

    public void CreateSquare()
    {
        DestroyCurrentShape();

        currentShape = CreateShape(squareTexture);
    }

    public void CreateTriangle()
    {
        DestroyCurrentShape();

        currentShape = CreateShape(triangleTexture);
    }

    public void CreateEllipse()
    {
        DestroyCurrentShape();

        currentShape = CreateShape(ellipseTexture);
    }

    private GameObject CreateShape(Texture2D shapeTexture)
    {
        GameObject shapeObject = new GameObject("Shape");
        shapeObject.transform.parent = drawingParent;

        SpriteRenderer spriteRenderer = shapeObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(shapeTexture, new Rect(0, 0, shapeTexture.width, shapeTexture.height), Vector2.one * 0.5f);

        spriteRenderer.color = currentColor;
        shapeObject.transform.localScale = Vector3.one * sizeSlider.value;

        return shapeObject;
    }

    public void OnColorDropdownValueChanged(int index)
    {
        currentColor = availableColors[index];
        if (currentShape != null)
        {
            currentShape.GetComponent<SpriteRenderer>().color = currentColor;
        }
    }

    public void OnSizeSliderValueChanged(float value)
    {
        if (currentShape != null)
        {
            currentShape.transform.localScale = Vector3.one * value;
        }
    }

    private void DestroyCurrentShape()
    {
        if (currentShape != null)
        {
            Destroy(currentShape);
        }
    }
}
