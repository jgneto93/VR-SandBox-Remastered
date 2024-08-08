using Unity.Collections;
using UnityEngine;
using System;

//I made thsi class to manage the colors of the objects
public class SetColor{
    public void SetObjectColor(Renderer objRenderer, Color color) {
        if (objRenderer != null) {
            objRenderer.material.color = color;
        }
    }

    public void SetObjectColor(GameObject obj, Color color) {
        Renderer objRenderer = obj.GetComponent<Renderer>();
        SetObjectColor(objRenderer, color);
    }

    public Color GetColor(int roleId, int teamId, int? playerIndex = null) {
        // Initialize baseColor to a default value
        Color baseColor = Color.white;

        if (roleId == 1) {  // Player
            if (teamId == 0) { // Team 1 - Blue
                baseColor = Color.blue;
            } else if (teamId == 1) {// Team 2 - Red
                baseColor = Color.red;
            }
        } else if (roleId == 2) {// Goalkeeper
            if (teamId == 0) {// Team 1 Goalkeeper - Light Gray
                baseColor = new Color(0.8f, 0.8f, 0.8f); // Light Gray
            } else if (teamId == 1) {// Team 2 Goalkeeper - Dark Gray
                baseColor = new Color(0.2f, 0.2f, 0.2f); // Dark Gray
            }
        } else if (roleId == 3) {// Referee - Yellow
            baseColor = Color.yellow;
        }

        // If a playerIndex is provided, create a slight variation in the hue of the color
        if (playerIndex.HasValue) {
            float hue, saturation, value;
            Color.RGBToHSV(baseColor, out hue, out saturation, out value);
            // Vary the hue by a small amount
            float variation = playerIndex.Value * 0.01f;
            hue += variation;
            // Make sure the hue is between 0 and 1
            hue = Mathf.Clamp01(hue);
            // Convert the HSV color back to RGB
            baseColor = Color.HSVToRGB(hue, saturation, value);
        }
        return baseColor;
    }   
}