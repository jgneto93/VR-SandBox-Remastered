using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class CaptureObjectPositions : MonoBehaviour
{
    public Terrain terrain;
    public string objectTag = "SpawnTrigger";
    public OVRInput.Button captureButton = OVRInput.Button.One;

    private void Update()
    {
        if (OVRInput.GetDown(captureButton))
        {
            CapturePositions();
        }
    }

    private void CapturePositions()
    {
        int width = terrain.terrainData.heightmapResolution;
        int height = terrain.terrainData.heightmapResolution;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        GameObject[] objectsToCapture = GameObject.FindGameObjectsWithTag(objectTag);

        foreach (GameObject obj in objectsToCapture)
        {
            Vector3 position = obj.transform.position;
            float normalizedX = (position.x - terrain.transform.position.x) / terrain.terrainData.size.x;
            float normalizedZ = (position.z - terrain.transform.position.z) / terrain.terrainData.size.z;

            int texX = Mathf.FloorToInt(normalizedX * width);
            int texY = Mathf.FloorToInt(normalizedZ * height);

            texture.SetPixel(texX, texY, Color.red);
        }

        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath, "ObjectPositions.png");
        File.WriteAllBytes(filePath, bytes);

        Debug.Log("Object positions captured and saved to: " + filePath);
    }
}
