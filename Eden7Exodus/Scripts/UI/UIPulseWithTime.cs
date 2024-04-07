using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPulseWithTime : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    private void Update()
    {
        float alpha = Mathf.PingPong(Time.time, 1f);
        image.color = ColorWithNewAlpha(image.color, alpha);
        text.color = ColorWithNewAlpha(text.color, alpha);
    }

    private Color ColorWithNewAlpha(Color originalColor, float newAlpha)
    {
        Color color = originalColor;
        color.a = newAlpha;
        return color;
    }
}
