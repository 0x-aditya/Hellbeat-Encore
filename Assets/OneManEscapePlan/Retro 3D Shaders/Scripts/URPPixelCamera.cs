using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Renders the game at a fixed vertical resolution in URP by driving the URP asset's
/// Render Scale and forcing point (nearest-neighbor) upscaling.
/// Original settings are restored when the component is disabled.
/// </summary>
[ExecuteAlways]
public class URPPixelCamera : MonoBehaviour
{
    [Min(32)]
    [SerializeField] private int verticalResolution = 244;

    private UniversalRenderPipelineAsset urpAsset;
    private float originalScale;
    private UpscalingFilterSelection originalFilter;
    private bool saved;

    void OnEnable()
    {
        urpAsset = UniversalRenderPipeline.asset;
        if (urpAsset == null)
        {
            Debug.LogWarning("URPPixelCamera: no URP asset is active.");
            return;
        }

        originalScale = urpAsset.renderScale;
        originalFilter = urpAsset.upscalingFilter;
        saved = true;
        Apply();
    }

    void Update()
    {
        Apply(); // keeps the resolution correct if the window is resized
    }

    void Apply()
    {
        if (urpAsset == null || Screen.height == 0) return;

        urpAsset.renderScale = Mathf.Clamp(verticalResolution / (float)Screen.height, 0.1f, 2f);
        urpAsset.upscalingFilter = UpscalingFilterSelection.Point;
    }

    void OnDisable()
    {
        if (!saved || urpAsset == null) return;

        urpAsset.renderScale = originalScale;
        urpAsset.upscalingFilter = originalFilter;
        saved = false;
    }
}
