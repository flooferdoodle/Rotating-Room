using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PanelFader : MonoBehaviour
{
    public Image panelImage; // Assign your panel's Image component

    public void SetPanelAlpha(float alpha)
    {
        Color color = panelImage.color;
        color.a = alpha;
        panelImage.color = color;
    }

    public IEnumerator FadePanel(float targetAlpha, float duration)
    {
        float startAlpha = panelImage.color.a;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            SetPanelAlpha(newAlpha);
            yield return null;
        }

        SetPanelAlpha(targetAlpha);
    }
}
