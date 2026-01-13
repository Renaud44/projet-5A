using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public float mouseSensitivity = 100f;
    public Transform playerBody;        // Référence au corps du joueur pour le mouvement

    private float xRotation = 0f;     // Rotation verticale (haut/bas)
    
    void Update()
    {

        // Récupérer les mouvements de la souris pour la rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotation du corps du joueur horizontal (gauche/droite)
        playerBody.Rotate(Vector3.up * mouseX);

        //Appliquer la rotation verticale de la caméra (haut/bas)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Limite la rotation verticale

        //Appliquer la rotation de la caméra (verticale)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
    
    }
}
