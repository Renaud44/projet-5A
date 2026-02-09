using UnityEngine;

public class XRKeyboardMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    float rotX = 0f;

    void Start()
    {
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = cameraTransform.forward * v + cameraTransform.right * h;
        dir.y = 0;

        transform.position += dir * moveSpeed * Time.deltaTime;

        // Rotation
        float mx = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float my = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        rotX -= my;
        rotX = Mathf.Clamp(rotX, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(rotX, 0, 0);
        transform.Rotate(Vector3.up * mx);
    }
}
