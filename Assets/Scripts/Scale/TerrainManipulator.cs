using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManipulator : MonoBehaviour
{
    public TerrainScaler terrainScaler;
    public OVRInput.Button primaryHandTrigger;
    public OVRInput.Button secondaryHandTrigger;


    private void Update()
    {
        // Verifica se os botões dos controles estão sendo pressionados para iniciar a manipulação do terreno
        if (OVRInput.GetDown(primaryHandTrigger) && OVRInput.GetDown(secondaryHandTrigger))
        {
            terrainScaler.StartScaling();
        }

        // Verifica se os botões dos controles foram soltos para parar a manipulação do terreno
        if (OVRInput.GetUp(primaryHandTrigger) || OVRInput.GetUp(secondaryHandTrigger))
        {
            terrainScaler.StopScaling();
        }
    }
}
