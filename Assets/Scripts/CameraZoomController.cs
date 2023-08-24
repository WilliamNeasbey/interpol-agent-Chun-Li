using UnityEngine;
using Cinemachine;

public class CameraZoomController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float zoomDuration = 1.0f;
    public float targetFOV = 30.0f; // Adjust this value as needed

    private bool hasStartedZoom = false;
    private bool hasFinishedZoom = false;
    private float initialFOV;
    private float zoomStartTime;

    private void Start()
    {
        // Cache the initial field of view
        initialFOV = virtualCamera.m_Lens.FieldOfView;
    }

    private void Update()
    {
        if (!hasFinishedZoom)
        {
            // Check if it's time to start the zoom
            if (!hasStartedZoom && Time.time >= 3.0f)
            {
                hasStartedZoom = true;
                zoomStartTime = Time.time;
            }

            // Perform the zoom
            if (hasStartedZoom)
            {
                float timeSinceZoomStart = Time.time - zoomStartTime;
                float t = Mathf.Clamp01(timeSinceZoomStart / zoomDuration);

                // Calculate the new FOV based on a smooth interpolation
                float newFOV = Mathf.Lerp(initialFOV, targetFOV, t);

                // Update the virtual camera's field of view
                virtualCamera.m_Lens.FieldOfView = newFOV;

                // Check if the zoom animation should stop
                if (t >= 1.0f)
                {
                    hasFinishedZoom = true;
                }
            }
        }
    }
}
