using UnityEngine;

public class MoveCube : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Transform cameraTransform;

    void Update()
    {
        // Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = cameraTransform.forward * v + cameraTransform.right * h;
        dir.y = 0;

        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}
