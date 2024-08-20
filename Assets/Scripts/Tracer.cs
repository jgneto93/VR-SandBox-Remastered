using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracer : MonoBehaviour {
    LineRenderer lineRenderer;
    public Color lineColor;

    void Awake() {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }

    public void AddVector3ToLineRenderer(Vector3 coord) {
        if (lineRenderer.enabled == true) { 
            lineRenderer.material.color = lineColor;

            if (lineRenderer.positionCount >= 720) {
                // Remove the first point to keep the count at 360
                for (int i = 1; i < lineRenderer.positionCount; i++) {
                    lineRenderer.SetPosition(i - 1, lineRenderer.GetPosition(i));
                }
                lineRenderer.positionCount--;
            }

            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, coord);
        }
    }
}