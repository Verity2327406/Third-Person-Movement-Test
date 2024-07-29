using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUI : MonoBehaviour
{
    RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Update()
    {
        float width = rectTransform.rect.width;
        rectTransform.anchoredPosition = new Vector2(width/2, rectTransform.anchoredPosition.y);
    }
}
