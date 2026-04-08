using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    [Header("Rotation Limits")]
    public float maxRightAngle = 60f;
    public float maxLeftAngle = -60f;
    public float maxUpAngle = -60f; // Negative is looking up in Unity
    public float maxDownAngle = 60f; // Positive is looking down

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // Force the camera to always snap to looking perfectly straight ahead when the game starts!
        yRotation = 0f;
        xRotation = 0f;
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calculate up/down rotation and clamp it 
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxUpAngle, maxDownAngle);

        // Calculate left/right rotation and clamp it
        yRotation += mouseX;
        yRotation = Mathf.Clamp(yRotation, maxLeftAngle, maxRightAngle);

        // Apply rotations to our camera
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
