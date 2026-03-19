using UnityEngine;

public class GlassLiquid : MonoBehaviour
{
    private Renderer liquidRenderer;
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");
    private static readonly int BaseColorProperty = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        liquidRenderer = GetComponent<Renderer>();
        SetColor(new Color(1f, 1f, 1f, 0.3f));
        SetVisible(false);
    }

    public void SetColor(Color color)
    {
        if (liquidRenderer == null) return;
        Material mat = liquidRenderer.material;
        if (mat.HasProperty(ColorProperty))
            mat.SetColor(ColorProperty, color);
        if (mat.HasProperty(BaseColorProperty))
            mat.SetColor(BaseColorProperty, color);
    }

    public void SetVisible(bool visible)
    {
        if (liquidRenderer != null)
            liquidRenderer.enabled = visible;
    }
}