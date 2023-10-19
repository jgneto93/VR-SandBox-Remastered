using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusSampleRigScaler : MonoBehaviour
{
    public OVRInput.Button increaseButton;
    public OVRInput.Button decreaseButton;
    public OVRInput.Button resetButton;
    public OVRInput.Button saveButton;
    public OVRInput.Button switchToBigButton;
    public OVRInput.Button switchToSmallButton;
    public float scaleSpeed = 1.0f;
    public float maxScale = 5.0f;
    public float minScale = 0.02f;

    private bool isScaling = false;
    private bool isResetting = false;
    private float bigScaleValue;
    private float smallScaleValue;
    private float currentScale = 1.0f;
    private float originalScale = 1.0f;

    private bool switchToBig = true; // Variável para alternar entre Big e Small

    private void Start()
    {
        originalScale = transform.localScale.x;
    }

    private void Update()
    {
        float thumbstickY = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y; // Captura o valor do thumbstick Y

        float scaleDirection = 0.0f;

        if (OVRInput.Get(increaseButton) || thumbstickY > 0) // Se o botão de aumento ou o thumbstick estiverem sendo pressionados para cima
        {
            scaleDirection = 1.0f * Mathf.Abs(thumbstickY); // Use o valor absoluto do thumbstickY para controlar a velocidade do zoom
        }
        else if (OVRInput.Get(decreaseButton) || thumbstickY < 0) // Se o botão de diminuição ou o thumbstick estiverem sendo pressionados para baixo
        {
            scaleDirection = -1.0f * Mathf.Abs(thumbstickY); // Use o valor absoluto do thumbstickY para controlar a velocidade do zoom
        }
        else if (OVRInput.GetDown(resetButton))
        {
            ResetScaling();
        }

        if (OVRInput.GetDown(increaseButton) || OVRInput.GetDown(decreaseButton))
        {
            StartScaling(scaleDirection);
        }

        if (OVRInput.GetUp(increaseButton) || OVRInput.GetUp(decreaseButton))
        {
            StopScaling();
        }

        if (OVRInput.GetDown(saveButton))
        {
            SaveScaleValue();
        }

        if (OVRInput.GetDown(switchToBigButton))
        {
            switchToBig = true;
            ApplySavedScale();
        }

        if (OVRInput.GetDown(switchToSmallButton))
        {
            switchToBig = false;
            ApplySavedScale();
        }

        if (isScaling && !isResetting)
        {
            currentScale += scaleDirection * scaleSpeed * Time.deltaTime;
            currentScale = Mathf.Clamp(currentScale, minScale, maxScale);

            // Aplica a escala no Oculus Sample Rig
            transform.localScale = Vector3.one * currentScale;

        }
    }

    private void StartScaling(float direction)
    {
        if (!isScaling && !isResetting)
        {
            isScaling = true;
        }
    }

    private void StopScaling()
    {
        isScaling = false;
    }

    private void ResetScaling()
    {
        if (!isResetting)
        {
            isResetting = true;
            StartCoroutine(ResetCoroutine());
        }
    }

    private void SaveScaleValue()
    {
        if (currentScale >= 1f)
        {
            bigScaleValue = currentScale;
            Debug.Log("Big Scale Value Saved: " + bigScaleValue);
        }
        else
        {
            smallScaleValue = currentScale;
            Debug.Log("Small Scale Value Saved: " + smallScaleValue);
        }
    }

    private IEnumerator ResetCoroutine()
    {
        Vector3 initialScale = Vector3.one * originalScale;

        while (transform.localScale != initialScale)
        {
            currentScale -= scaleSpeed * Time.deltaTime;
            currentScale = Mathf.Max(currentScale, originalScale);
            transform.localScale = Vector3.MoveTowards(transform.localScale, initialScale, scaleSpeed * Time.deltaTime);
            yield return null;
        }

        isResetting = false;
        currentScale = originalScale;
        transform.localScale = initialScale;
    }

    private void ApplySavedScale()
    {
        float savedScale = switchToBig ? bigScaleValue : smallScaleValue;

        if (Mathf.Approximately(currentScale, savedScale))
        {
            Debug.Log("Already at Saved Scale: " + savedScale);
            return;
        }

        isScaling = true;
        StartCoroutine(ApplyScaleCoroutine(savedScale));
    }

    private IEnumerator ApplyScaleCoroutine(float targetScale)
    {
        Vector3 initialScale = transform.localScale;
        float startTime = Time.time;
        float journeyLength = Mathf.Abs(targetScale - currentScale);

        while (currentScale != targetScale)
        {
            float distCovered = (Time.time - startTime) * scaleSpeed;
            float fracJourney = distCovered / journeyLength;

            currentScale = Mathf.Lerp(initialScale.x, targetScale, fracJourney);
            transform.localScale = Vector3.one * currentScale;

            yield return null;
        }

        currentScale = targetScale;
        transform.localScale = Vector3.one * currentScale;
        isScaling = false;

        Debug.Log("Applied Saved Scale: " + currentScale);
    }
}
