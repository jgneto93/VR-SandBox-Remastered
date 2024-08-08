using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timeline : MonoBehaviour{
    public Slider timelineSlider;
    private JSONReader jsonReader;
    private int currentFrameIndex;

    private IEnumerator Start(){ // Wait until the next frame to ensure that MatchInterpreter Start method has completed
        yield return null;
        GameObject matchInterpreter = GameObject.Find("MatchInterpreter");
        jsonReader = matchInterpreter.GetComponent<JSONReader>();
        timelineSlider.maxValue = jsonReader.GetTotalFrames();
        timelineSlider.onValueChanged.AddListener(OnSliderValueChanged);

    }
    private void OnSliderValueChanged(float value){
        currentFrameIndex = Mathf.RoundToInt(timelineSlider.value);
        jsonReader.SetFrameIndex(currentFrameIndex);
        jsonReader.InstantiatePlayersFromFrame(currentFrameIndex);
        timelineSlider.value = currentFrameIndex;
    }
    
  
    public void AddEventIconAboveSlider(int frameIndex, int type, float IconWidth){
        // 0 = pass, 1 = finish, 2 = goal, 3 = yellow card, 4 = red card, 5 = substitution
        GameObject eventIcon = new GameObject("EventIcon");
        RectTransform rectTransform = eventIcon.AddComponent<RectTransform>();
        Image image = eventIcon.AddComponent<Image>();
        image.color = Color.red;
        rectTransform.sizeDelta = new Vector2(IconWidth, 10);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0, 0);
        // Calculate the relative position
        float relativePosition = (float)frameIndex /timelineSlider.maxValue;
        // Get the RectTransform component of the slider
        RectTransform sliderRectTransform = timelineSlider.GetComponent<RectTransform>();
        // Set the anchoredPosition proportionally above the slider
        rectTransform.anchoredPosition = new Vector2(relativePosition * sliderRectTransform.rect.width, sliderRectTransform.rect.height);
        eventIcon.transform.SetParent(timelineSlider.transform, false);
    }

    public int GetSliderValue(){ 
        return Mathf.RoundToInt(timelineSlider.value);
    }
    public void SetSliderValue(int frameIndex){ // Called in JSONReader 
        timelineSlider.value = frameIndex;
    }
}