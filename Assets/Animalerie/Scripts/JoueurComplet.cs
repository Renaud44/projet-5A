using UnityEngine;

public class JoueurComplet : MonoBehaviour
{
    [Header("Mouvement")]
    public float vitesse = 5.0f;
    public float sensibiliteSouris = 2.0f;

    [Header("Interaction")]
    public float distanceInteraction = 3.0f;

    private float rotationX = 0;
    private Camera cam;
    private bool cursorLocked = true;

    // --- NOUVEAUTÉS POUR LE LASER ---
    private LineRenderer laserLine;
    private GameObject pointeurImpact; // La petite boule au bout du laser

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
        LockCursor(true);

        // 1. CRÉATION AUTOMATIQUE DU LASER
        if (!GetComponent<LineRenderer>())
        {
            laserLine = gameObject.AddComponent<LineRenderer>();
        }
        else
        {
            laserLine = GetComponent<LineRenderer>();
        }

        // Configuration du style du laser
        laserLine.startWidth = 0.005f; // Très fin
        laserLine.endWidth = 0.005f;
        laserLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        laserLine.startColor = Color.red;
        laserLine.endColor = new Color(1, 0, 0, 0.5f); // Rouge semi-transparent au bout

        // 2. CRÉATION DE LA BILLE D'IMPACT (Le point rouge)
        pointeurImpact = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        pointeurImpact.name = "Curseur_Laser";
        pointeurImpact.transform.localScale = Vector3.one * 0.02f; // Petite bille de 2cm
        Destroy(pointeurImpact.GetComponent<Collider>()); // Pas de physique pour le curseur
        pointeurImpact.GetComponent<Renderer>().material.color = Color.red;
    }

    void Update()
    {
        // GESTION SOURIS (Echap pour quitter)
        if (Input.GetKeyDown(KeyCode.Escape)) LockCursor(false);
        if (Input.GetMouseButtonDown(0) && !cursorLocked) LockCursor(true);

        if (!cursorLocked) return;

        // 1. ROTATION (Regard)
        float mouseX = Input.GetAxis("Mouse X") * sensibiliteSouris;
        float mouseY = Input.GetAxis("Mouse Y") * sensibiliteSouris;

        transform.Rotate(Vector3.up * mouseX);
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        if (cam != null) cam.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);

        // 2. DÉPLACEMENT
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        transform.Translate(move * vitesse * Time.deltaTime, Space.World);

        // 3. GESTION DU LASER ET INTERACTION
        GererLaserEtClic();
    }

    void GererLaserEtClic()
    {
        // Le laser part des yeux (Caméra)
        Vector3 depart = cam.transform.position;
        // Il va tout droit devant
        Vector3 direction = cam.transform.forward;

        RaycastHit hit;

        // Est-ce qu'on touche quelque chose ?
        bool toucheQuelqueChose = Physics.Raycast(depart, direction, out hit, distanceInteraction);

        // Mise à jour visuelle du laser (Début -> Fin)
        laserLine.SetPosition(0, depart + (direction * 0.2f)); // Un peu devant les yeux pour pas gêner la vue

        if (toucheQuelqueChose)
        {
            // Le laser s'arrête sur l'objet
            laserLine.SetPosition(1, hit.point);

            // On déplace la petite bille sur le point d'impact
            pointeurImpact.transform.position = hit.point;
            pointeurImpact.SetActive(true);

            // --- DETECTION INTELLIGENTE ---
            // On vérifie si l'objet touché a le script "SimpleButton"
            SimpleButton bouton = hit.transform.GetComponent<SimpleButton>();

            if (bouton != null)
            {
                // C'est un bouton ! Laser VERT
                laserLine.startColor = Color.green;
                laserLine.endColor = Color.green;
                pointeurImpact.GetComponent<Renderer>().material.color = Color.green;

                // SI ON CLIQUE
                if (Input.GetMouseButtonDown(0))
                {
                    bouton.OnPress();
                }
            }
            else
            {
                // C'est un mur ou un décor : Laser ROUGE
                laserLine.startColor = Color.red;
                laserLine.endColor = Color.red;
                pointeurImpact.GetComponent<Renderer>().material.color = Color.red;
            }
        }
        else
        {
            // On ne touche rien (laser dans le vide)
            Vector3 pointFinal = depart + (direction * distanceInteraction);
            laserLine.SetPosition(1, pointFinal);

            // On cache la bille d'impact car on touche le vide
            pointeurImpact.SetActive(false);

            // Couleur par défaut
            laserLine.startColor = Color.red;
            laserLine.endColor = new Color(1, 0, 0, 0);
        }
    }

    void LockCursor(bool isLocked)
    {
        cursorLocked = isLocked;
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }
}