using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class DynamicAspectRatioCamera : MonoBehaviour
{
    [Header("Reference Aspect Ratio")]
    [Tooltip("The aspect ratio (width / height) you want your game to maintain. E.g., 16/9 = 1.777 for widescreen, 4/3 = 1.333 for older displays.")]
    public float targetAspectRatio = 16f / 9f; // Example: Widescreen 16:9

    [Header("Camera Settings")]
    [Tooltip("The vertical FOV that looks good at your target aspect ratio.")]
    public float baseFieldOfView = 60f; 

    private Camera gameCamera;

    void Awake()
    {
        gameCamera = GetComponent<Camera>();
        if (gameCamera == null || gameCamera.orthographic)
        {
            Debug.LogWarning("DynamicAspectRatioCamera requires a Perspective Camera component.");
            enabled = false; // Disable the script if not a perspective camera
            return;
        }

        UpdateCameraViewport();
    }

    // Call this whenever the screen size might change (e.g., orientation change, window resize)
    void Update()
    {
        // For efficiency, you might want to call this only when Screen.width or Screen.height changes
        // or during an explicit orientation change event.
        // For development and testing, calling it in Update is fine.
        UpdateCameraViewport(); 
    }

    void UpdateCameraViewport()
    {
        // Calculate the current screen's aspect ratio
        float currentScreenAspectRatio = (float)Screen.width / Screen.height;

        // Reset viewport to full screen initially
        Rect rect = new Rect(0, 0, 1, 1);

        // Calculate the scale factor needed to maintain the target aspect ratio
        float scaleHeight = currentScreenAspectRatio / targetAspectRatio;
        float scaleWidth = targetAspectRatio / currentScreenAspectRatio;

        if (scaleHeight < 1f)
        {
            // Current screen is too tall (relative to targetAspectRatio)
            // This means we need to add horizontal bars (pillarbox effect)
            rect.width = 1f; // Use full width
            rect.height = scaleHeight;
            rect.x = 0; // Center horizontally
            rect.y = (1f - scaleHeight) / 2f; // Center vertically
        }
        else if (scaleWidth < 1f)
        {
            // Current screen is too wide (relative to targetAspectRatio)
            // This means we need to add vertical bars (letterbox effect)
            rect.width = scaleWidth;
            rect.height = 1f; // Use full height
            rect.x = (1f - scaleWidth) / 2f; // Center horizontally
            rect.y = 0; // Center vertically
        }
        // If scaleHeight >= 1f and scaleWidth >= 1f, the screen's aspect ratio matches or is closer
        // to the target in a way that doesn't require bars. The rect remains (0,0,1,1).

        gameCamera.rect = rect;
        
        // Always set the FOV based on the desired vertical FOV for a consistent 'zoom'
        gameCamera.fieldOfView = baseFieldOfView;
    }
}