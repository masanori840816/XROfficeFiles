using UnityEngine;
using PrimeTween;

public class PanelSlider : MonoBehaviour
{
    [SerializeField] private RectTransform targetPanel;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutBack;
    private Tween activeTween;

    /// <summary>
    /// Show the panel at the center of the screen.
    /// </summary>
    public void ShowPanel()
    {
        // Stop last animation
        if (activeTween.isAlive)
        {
            activeTween.Stop();
        }
        activeTween = Tween.UIAnchoredPositionX(targetPanel, endValue: 0f, duration: duration, ease: easeType);
    }

    /// <summary>
    /// Hide the panel on the left side of the screen.
    /// </summary>
    public void HidePanel()
    {
        // Stop last animation
        if (activeTween.isAlive)
        {
            activeTween.Stop();
        }
        float screenWidth = targetPanel.rect.width;
        activeTween = Tween.UIAnchoredPositionX(targetPanel, endValue: -screenWidth, duration: duration, ease: Ease.InBack);
    }
}
