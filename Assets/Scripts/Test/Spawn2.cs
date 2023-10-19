using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn2 : MonoBehaviour
{
    public GameObject prefab;
    private bool canSpawn = true;
    private float distanceThreshold = 0.1f;
    private Vector3 startingPosition;
    private GameObject shelfObject;
    //private RaycasterController raycasterController;
    string[,] grid;
    public TextAsset csvFile;
    //private LineRenderer raycaster;

    void Start()
    {
        grid = SplitData(csvFile.text);
        startingPosition = transform.position;
        shelfObject = Instantiate(prefab, transform.position, Quaternion.identity);
        shelfObject.name = prefab.name;
        shelfObject.tag = "SpawnTrigger";

        //raycasterController = FindObjectOfType<RaycasterController>();
    }

    void Update()
    {
        if (canSpawn && Vector3.Distance(transform.position, shelfObject.transform.position) > distanceThreshold)
        {
            SpawnObject();
            canSpawn = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (canSpawn && other.CompareTag("SpawnTrigger"))
        {
            canSpawn = false;
            Light lightComponent = shelfObject.GetComponentInChildren<Light>();
            if (lightComponent != null)
            {
                lightComponent.enabled = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!canSpawn && other.CompareTag("SpawnTrigger"))
        {
            canSpawn = true;
            other.gameObject.tag = "spawnLeave";
            Light lightComponent = shelfObject.GetComponentInChildren<Light>();
            if (lightComponent != null)
            {
                lightComponent.enabled = true;
            }
            
        }
    }

    private void SpawnObject()
    {
        Light lightComponent = shelfObject.GetComponentInChildren<Light>();
        lightComponent.renderMode = LightRenderMode.ForcePixel;

        if (lightComponent != null)
        {
            int cor = int.Parse(GetValue(prefab.name, "Pais"));
            if (cor == 1)
            {
                lightComponent.color = Color.blue;
            }
            else if (cor == 2)
            {
                lightComponent.color = Color.red;
            }
            float newHeight = float.Parse(GetValue(prefab.name, "Altura")) / 1000f;
            Vector3 newPosition = lightComponent.transform.localPosition;
            newPosition.y = newHeight;
            lightComponent.transform.localPosition = newPosition;

            lightComponent.range = float.Parse(GetValue(prefab.name, "Alcance_Util")) / 1000;
            lightComponent.innerSpotAngle = -float.Parse(GetValue(prefab.name, "Elevacao_Minima")) + float.Parse(GetValue(prefab.name, "Elevacao_Maxima"));
            lightComponent.spotAngle = float.Parse(GetValue(prefab.name, "Campo_de_Tiro"));
            lightComponent.intensity = 50f;

            lightComponent.shadows = LightShadows.Hard;  
            lightComponent.shadowStrength = 1f;          
            lightComponent.shadowBias = 0f;             
            lightComponent.shadowNormalBias = 0f;       
            lightComponent.shadowNearPlane = 0.02f;

            // Deixa a spotlight desativada inicialmente
            //lightComponent.enabled = false;
        }

        shelfObject = Instantiate(prefab, transform.position, Quaternion.identity);
        shelfObject.name = prefab.name;
        shelfObject.tag = "SpawnTrigger";
    }

    static public string[,] SplitData(string csvText)
    {
        string[] lines = csvText.Split("\n"[0]);
        int width = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = SplitCsvLine(lines[i]);
            width = Mathf.Max(width, row.Length);
        }

        string[,] outputGrid = new string[width + 1, lines.Length + 1];

        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = SplitCsvLine(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }
        return outputGrid;
    }

    public string GetValue(string prefabName, string columnName)
    {
        int columnIndex = -1;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            if (grid[i, 0] == columnName)
            {
                columnIndex = i;
                break;
            }
        }

        int rowIndex = -1;
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            if (grid[0, i] == prefabName)
            {
                rowIndex = i;
                break;
            }
        }

        if (columnIndex != -1 && rowIndex != -1)
        {
            return grid[columnIndex, rowIndex];
        }
        else
        {
            return "0";
        }
    }

    static public string[] SplitCsvLine(string line)
    {
        return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
            pattern: @"(((?<x>[^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value).ToArray();
    }
}
