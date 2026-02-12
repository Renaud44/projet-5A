using UnityEngine;
using UnityEngine.UI;

public class SystemeHygiene : MonoBehaviour
{
    [Header("--- Configuration des Assets ---")]
    public Transform pointRobinet;
    public ParticleSystem eauParticules;
    public Transform distributeurSavon;
    public Transform distributeurGel;
    public Transform mainGauche;
    public Transform mainDroite;

    [Header("--- Param�tres ---")]
    public float distanceDetectionEau = 1.0f;
    public float distanceFrottement = 0.4f;
    public float tempsFrottementRequis = 5.0f;

    [Header("--- Interface (Optionnel) ---")]
    public Text textInstructions;

    private bool aDuSavon = false;
    private float timerFrottement = 0.0f;
    private bool mainsLavees = false;
    private bool scenarioTermine = false;

    // Variables pour le test souris
    private Vector3 offsetSouris;
    private float zCoord;

    // CORRECTION 1 : Variable pour stocker la cam�ra
    private Camera cam;

    void Start()
    {
        // CORRECTION 1 : On r�cup�re la cam�ra une seule fois au d�but
        cam = Camera.main;
        if (cam == null) cam = FindObjectOfType<Camera>();

        // Si les champs sont vides, on g�n�re la sc�ne de test
        if (pointRobinet == null) SetupEnvironnementTest();

        // CORRECTION 2 : On applique les r�glages d'eau (via une fonction propre)
        if (eauParticules != null)
        {
            eauParticules.Stop();
            ConfigurerParticulesEau(eauParticules);
        }

        MettreAJourUI("Approchez les mains du robinet pour mouiller, puis cliquez sur le savon.");
    }

    void Update()
    {
        if (scenarioTermine) return;

        GererEau();

        if (aDuSavon && !mainsLavees)
        {
            GererFrottement();
        }

        BougerMainSouris();
    }

    // CORRECTION 2 : Fonction d�di�e pour r�gler l'eau (plus propre)
    void ConfigurerParticulesEau(ParticleSystem ps)
    {
        // --- 1. Forcer l'apparence VISUELLE (Le plus important) ---
        var renderer = ps.GetComponent<ParticleSystemRenderer>();

        // On remplace le mat�riau "nuageux" par un mat�riau "solide" (Sprites/Default)
        // Cela permet � la couleur d'�tre exactement celle qu'on veut, sans ombre grise.
        renderer.material = new Material(Shader.Find("Sprites/Default"));

        // --- 2. D�finir la couleur et la taille ---
        var main = ps.main;
        // Un bleu pur, tr�s visible (R:0, G:0.6, B:1) et 100% opaque (A:1)
        main.startColor = new Color(0.0f, 0.6f, 1.0f, 0.5f);
        main.startSize = 0.15f; // Un peu plus gros pour bien voir

        // --- 3. Le mouvement de l'eau ---
        main.startSpeed = 5.0f;
        main.gravityModifier = 1.0f;
        main.startLifetime = 1.0f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        // --- 4. Le d�bit ---
        var emission = ps.emission;
        emission.rateOverTime = 50;

        // --- 5. La forme du jet ---
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 5.0f; // Tr�s serr� (comme un filet d'eau)
        shape.radius = 0.25f;
    }

    void GererEau()
    {
        if (pointRobinet == null || mainGauche == null || mainDroite == null) return;

        float distG = Vector3.Distance(pointRobinet.position, mainGauche.position);
        float distD = Vector3.Distance(pointRobinet.position, mainDroite.position);

        bool mainsSousRobinet = (distG < distanceDetectionEau || distD < distanceDetectionEau);

        if (eauParticules != null)
        {
            if (mainsSousRobinet && !eauParticules.isPlaying) eauParticules.Play();
            else if (!mainsSousRobinet && eauParticules.isPlaying) eauParticules.Stop();
        }
    }

    void GererFrottement()
    {
        float distanceMains = Vector3.Distance(mainGauche.position, mainDroite.position);

        if (distanceMains < distanceFrottement)
        {
            timerFrottement += Time.deltaTime;
            ChangeCouleurMains(Color.Lerp(Color.white, Color.green, timerFrottement / tempsFrottementRequis));
            MettreAJourUI($"Frottement : {timerFrottement:F1} / {tempsFrottementRequis} sec");

            if (timerFrottement >= tempsFrottementRequis)
            {
                mainsLavees = true;
                MettreAJourUI("Mains propres ! Cliquez sur le Gel pour finir.");
            }
        }
    }

    public void ActionPrendreSavon()
    {
        if (!aDuSavon)
        {
            aDuSavon = true;
            Debug.Log("Savon pris !");
            ChangeCouleurMains(Color.cyan);
            MettreAJourUI("Savon appliqu�. Frottez vos mains !");
        }
    }

    public void ActionPrendreGel()
    {
        if (mainsLavees)
        {
            scenarioTermine = true;
            MettreAJourUI("SC�NARIO VALID� ! Bravo.");
            ChangeCouleurMains(Color.blue);
        }
        else
        {
            MettreAJourUI("Lavez-vous les mains 5 secondes avant de mettre le gel !");
        
        }
    }

    void ChangeCouleurMains(Color c)
    {
        if (mainGauche) mainGauche.GetComponent<Renderer>().material.color = c;
        if (mainDroite) mainDroite.GetComponent<Renderer>().material.color = c;
    }

    void MettreAJourUI(string message)
    {
        if (textInstructions != null) textInstructions.text = message;
        else Debug.Log(message);
    }

    // --- SETUP AUTOMATIQUE ---
    void SetupEnvironnementTest()
    {
        GameObject robinetGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        robinetGO.name = "Robinet_Test";
        robinetGO.transform.position = new Vector3(0, 2, 0);
        robinetGO.transform.localScale = new Vector3(0.2f, 1, 0.2f);
        pointRobinet = robinetGO.transform;

        GameObject eauGO = new GameObject("Eau_Particules");
        eauGO.transform.position = pointRobinet.position - Vector3.up * 0.6f;
        eauGO.transform.Rotate(90, 0, 0);
        eauParticules = eauGO.AddComponent<ParticleSystem>();

        // On applique aussi la config ici pour que le test soit joli
        ConfigurerParticulesEau(eauParticules);

        // Savon
        GameObject savonGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        savonGO.name = "Savon_Bouton";
        savonGO.transform.position = new Vector3(-1.5f, 1, 0);
        savonGO.GetComponent<Renderer>().material.color = Color.red;
        distributeurSavon = savonGO.transform;

        var clickS = savonGO.AddComponent<ClickDetector>();
        clickS.parentScript = this;
        clickS.isGel = false;

        // Gel
        GameObject gelGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
        gelGO.name = "Gel_Bouton";
        gelGO.transform.position = new Vector3(1.5f, 1, 0);
        gelGO.GetComponent<Renderer>().material.color = Color.green;
        distributeurGel = gelGO.transform;

        var clickG = gelGO.AddComponent<ClickDetector>();
        clickG.parentScript = this;
        clickG.isGel = true;

        GameObject mainG = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mainG.name = "Main_Gauche_Fixe";
        mainG.transform.position = new Vector3(-0.5f, 0.5f, 0);
        mainGauche = mainG.transform;

        GameObject mainD = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mainD.name = "Main_Droite_Mobile";
        mainD.transform.position = new Vector3(0.5f, 0.5f, 0);
        mainD.GetComponent<Renderer>().material.color = Color.yellow;
        mainDroite = mainD.transform;

        // CORRECTION 1 : Utilisation de 'cam' au lieu de Camera.main
        if (cam != null)
        {
            cam.transform.position = new Vector3(0, 2, -4);
            cam.transform.LookAt(Vector3.zero);
        }
    }

    void BougerMainSouris()
    {
        // CORRECTION 1 : S�curit� Cam�ra
        if (mainDroite == null || cam == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            // CORRECTION 1 : Utilisation de 'cam'
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == mainDroite)
                {
                    zCoord = cam.WorldToScreenPoint(mainDroite.position).z;
                    offsetSouris = mainDroite.position - GetMouseWorldPos();
                }
            }
        }
        if (Input.GetMouseButton(0) && zCoord > 0)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit) || (hit.transform == mainDroite))
            {
                mainDroite.position = GetMouseWorldPos() + offsetSouris;
            }
        }
    }

    Vector3 GetMouseWorldPos()
    {
        if (cam == null) return Vector3.zero;
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord;
        return cam.ScreenToWorldPoint(mousePoint);
    }
}