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
        lineRenderer.material.color = lineColor;
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, coord);
    }
}