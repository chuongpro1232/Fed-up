using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("The character for the camera to follow.")]
    public Transform target;
    
    [Header("Follow Settings")]
    [Tooltip("How smoothly the camera catches up to the target. Lower is faster.")]
    public float smoothTime = 0.15f;
    
    [Tooltip("Offset for the camera. Z is left at -10 to see 2D objects.")]
    public Vector3 offset = new Vector3(0, 0, -10f);

    [Header("Zoom Settings")]
    public float zoomSpeed = 2f;
    private float defaultZoom;
    private float targetZoom;
    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            defaultZoom = cam.orthographicSize;
            targetZoom = defaultZoom;
        }
    }

    // This is used by Vector3.SmoothDamp as a reference to keep track of current velocity.
    private Vector3 velocity = Vector3.zero;

    // We use LateUpdate for camera movement. 
    // This ensures the camera moves *after* the player has moved, preventing stuttering.
    void LateUpdate()
    {
        // If we forgot to assign the target, don't do anything so it doesn't crash!
        if (target == null) return;

        // Calculate the ideal position the camera should be at (Player Position + Offset)
        Vector3 targetPosition = target.position + offset;

        // Smoothly glide the camera from its current position to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // Smoothly zoom the camera
        if (cam != null)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
    }

    public void SetZoom(float newZoom, float speed = 2f)
    {
        targetZoom = newZoom;
        zoomSpeed = speed;
    }

    public void ResetZoom(float speed = 2f)
    {
        targetZoom = defaultZoom;
        zoomSpeed = speed;
    }
}
