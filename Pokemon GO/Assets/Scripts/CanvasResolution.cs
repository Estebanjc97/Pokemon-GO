using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasResolution : MonoBehaviour
{
    float aspect;
    public float rounded;
    private CanvasScaler canvasScaler;
    public RectTransform canvasToModify;

    [Header("Screen 18:9")]
    public float newAnchoredX_18_9;
    public float newAnchoredY_18_9;
    public float newHeight_18_9;
    public float newWidth_18_9;

    [Header("Screen 2960x1440")]
    public float newAnchoredX_2960x1440;
    public float newAnchoredY_2960x1440;
    public float newHeight_2960x1440;
    public float newWidth_2960x1440;

    [Header("Screen 16:9")]
    public float newAnchoredX_16_9;
    public float newAnchoredY_16_9;
    public float newHeight_16_9;
    public float newWidth_16_9;

    [Header("Screen 800x480")]
    public float newAnchoredX_800x480;
    public float newAnchoredY_800x480;
    public float newHeight_800x480;
    public float newWidth_800x480;


    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        aspect = Camera.main.aspect;
        rounded = (int)(aspect * 100.0f) / 100.0f;

        if(rounded>=1.76f && rounded<=1.78f) //si el aspecto de pantalla es 16:9
        {
            AddRatios(0f);
            canvasToModify.anchoredPosition = new Vector2(newAnchoredX_16_9, newAnchoredY_16_9);
            canvasToModify.sizeDelta = new Vector2(newWidth_16_9, newHeight_16_9);
        }

        if (rounded == 2) //si el aspecto de pantalla es 18:9
        {
            AddRatios(0.5f);
            canvasToModify.anchoredPosition = new Vector2(newAnchoredX_18_9, newAnchoredY_18_9);
            canvasToModify.sizeDelta = new Vector2(newWidth_18_9, newHeight_18_9);


        }
        else if (rounded == 2.04f || (rounded == 2.05f) || (rounded == 2.06f)) //si nuestra resolución es de 2960x1440
        {
            AddRatios(0.6f);
            canvasToModify.anchoredPosition = new Vector2(newAnchoredX_2960x1440, newAnchoredY_2960x1440);
            canvasToModify.sizeDelta = new Vector2(newWidth_2960x1440, newHeight_2960x1440);

        }
        else if (rounded == 1.66f || (rounded == 1.67f) || (rounded == 1.68f)) //800x480
        {
            AddRatios(0.5f);
            canvasToModify.anchoredPosition = new Vector2(newAnchoredX_800x480, newAnchoredY_800x480);
            canvasToModify.sizeDelta = new Vector2(newWidth_800x480, newHeight_800x480);

        }

    }
    void AddRatios(float m)
    {
        if(canvasScaler!=null)
        {
            canvasScaler.matchWidthOrHeight = m;
            
        }
    }
    
}
