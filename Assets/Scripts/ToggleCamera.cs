/*using UnityEngine;
using System;
using UnityEngine.XR;
//Script to set the player's camera to the active camera
public class ToggleCamera : MonoBehaviour{
    public RenderTexture CurrentRenderTexture;
    public RenderTexture NextRenderTexture;
    private Camera oldActiveCamera;
    private Camera NewActiveCamera;
    public void SetCamera(Camera newCamera, bool isCurrent) {
        if(isCurrent) {  
            if (oldActiveCamera != null) {
                oldActiveCamera.gameObject.SetActive(false);
            }

            oldActiveCamera = newCamera;
        
            newCamera.targetTexture = NextRenderTexture;
        } else {
            if (oldActiveCamera != null) {
                oldActiveCamera.gameObject.SetActive(false);
            }

            oldActiveCamera = newCamera;

            newCamera.targetTexture = NextRenderTexture;
        }
    }

}*/