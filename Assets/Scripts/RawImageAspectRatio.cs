using UnityEngine;
using UnityEngine.UI;

public class RawImageAspectRatio : MonoBehaviour
{
    public RawImage rawImage;
    public RenderTexture renderTexture;

    void Start()
    {
        AdjustAspectRatio();
    }

    void AdjustAspectRatio()
    {
        if (rawImage == null || renderTexture == null) return;

        RectTransform rectTransform = rawImage.rectTransform;
        float aspectRatio = (float)renderTexture.width / renderTexture.height;
        float parentAspectRatio = rectTransform.rect.width / rectTransform.rect.height;

        if (aspectRatio > parentAspectRatio)
        {
            // Render texture is wider than parent
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, rectTransform.rect.width / aspectRatio);
        }
        else
        {
            // Render texture is taller than parent
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.height * aspectRatio, rectTransform.rect.height);
        }
    }
}
