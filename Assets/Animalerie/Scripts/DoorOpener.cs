using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [Header("Références")]
    public Transform viewer; // MainCamera (drag & drop)

    [Header("Détection")]
    public float openDistance = 5.0f;

    [Header("Rotation")]
    public float openAngle = -90f;     // +90 ou -90 selon le sens
    public float speed = 3f;          // vitesse d'ouverture/fermeture
    public Vector3 rotationAxis = new Vector3(0, 1, 0); // (0,1,0)=Y ; (0,0,1)=Z

    private Quaternion closedRot;
    private Quaternion openRot;

    void Start()
    {
        if (viewer == null && Camera.main != null)
            viewer = Camera.main.transform;

        closedRot = transform.localRotation;

        Vector3 axis = rotationAxis.normalized;
        openRot = closedRot * Quaternion.AngleAxis(openAngle, axis);
    }

    void Update()
    {
        if (viewer == null) return;

        float d = Vector3.Distance(viewer.position, transform.position);
        bool shouldOpen = d <= openDistance;

        Quaternion target = shouldOpen ? openRot : closedRot;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, speed * Time.deltaTime);
    }
}
