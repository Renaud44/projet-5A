using UnityEngine;
using UnityEngine.UI;

public class ManagerMedical : MonoBehaviour
{
    public static ManagerMedical instance; // Singleton pour accès facile depuis les autres scripts
    public string codeSecretGenerated = ""; // Le code que le joueur devra retenir

    // Références (remplies auto ou manuelles dans l'inspecteur)
    public GameObject porteArmoire;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Si la porte n'est pas assignée, on considère qu'on est en mode "Test Vierge"
        // et on construit tout le décor.
        if (porteArmoire == null) SetupEnvironnementTest();
    }

    // Fonction appelée par le Clipboard quand le joueur valide son nom
    public void GenererCodeSecret()
    {
        int code = Random.Range(1000, 9999);
        codeSecretGenerated = code.ToString();
        Debug.Log("CODE SECRET GÉNÉRÉ : " + codeSecretGenerated);
    }

    // --- SETUP AUTOMATIQUE (CONSTRUCTION DU NIVEAU) ---
    void SetupEnvironnementTest()
    {
        // 1. SOL
        GameObject sol = GameObject.CreatePrimitive(PrimitiveType.Plane);
        sol.name = "Sol";
        sol.transform.localScale = new Vector3(2, 1, 2);

        // 2. ARMOIRE ET PORTE (AU FOND)
        GameObject armoire = GameObject.CreatePrimitive(PrimitiveType.Cube);
        armoire.name = "Armoire";
        armoire.transform.position = new Vector3(0, 1.5f, 4); // Au fond de la salle
        armoire.transform.localScale = new Vector3(2, 3, 1);
        armoire.GetComponent<Renderer>().material.color = Color.gray;

        porteArmoire = GameObject.CreatePrimitive(PrimitiveType.Cube);
        porteArmoire.name = "Porte_Armoire";
        porteArmoire.transform.parent = armoire.transform; // Enfant de l'armoire pour pivoter avec
        porteArmoire.transform.localPosition = new Vector3(0, 0, -0.55f); // Devant l'armoire
        porteArmoire.transform.localScale = new Vector3(0.9f, 0.9f, 0.1f);
        porteArmoire.GetComponent<Renderer>().material.color = new Color(0.8f, 0.8f, 0.8f);

        // DIGICODE (Sur la porte)
        GameObject digicodeGO = new GameObject("Digicode");
        digicodeGO.transform.parent = porteArmoire.transform;
        digicodeGO.transform.localPosition = new Vector3(0, 0, -0.6f); // Devant la porte
        digicodeGO.transform.localRotation = Quaternion.identity;
        digicodeGO.transform.localScale = Vector3.one; // Reset scale
        // On ajoute le script Digicode et on lance son setup auto
        digicodeGO.AddComponent<DigicodeSystem>().SetupDigicodeAuto();

        // 3. TABLE ET CLIPBOARD (A GAUCHE)
        GameObject table = GameObject.CreatePrimitive(PrimitiveType.Cube);
        table.name = "Table_Bureau";
        table.transform.position = new Vector3(-1.5f, 0.7f, 0);
        table.transform.localScale = new Vector3(1, 1, 1);

        GameObject clipboardGO = new GameObject("Clipboard_Interactif");
        clipboardGO.transform.position = new Vector3(-1.5f, 1.3f, 0);
        clipboardGO.transform.Rotate(0, 0, 0); // Orienté vers le centre
        // On ajoute le script Clipboard et on lance son setup auto
        clipboardGO.AddComponent<ClipboardSystem>().SetupClipboardAuto();

        // 4. PHARMACIE (A DROITE)
        GameObject tablePharma = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tablePharma.name = "Table_Pharma";
        tablePharma.transform.position = new Vector3(2.5f, 0.5f, 1);

        // Bouteille OK (Verte)
        GameObject b1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        b1.name = "Bouteille_Ok";
        b1.transform.position = new Vector3(2.2f, 1.2f, 1);
        b1.transform.localScale = new Vector3(0.2f, 0.4f, 0.2f);
        b1.GetComponent<Renderer>().material.color = Color.green;
        b1.AddComponent<BoxCollider>().isTrigger = true; // Pour détection seringue
        var scriptB1 = b1.AddComponent<SeringueSystem>();
        scriptB1.isBottle = true;
        scriptB1.currentVolume = 10;
        scriptB1.labelVolume = 10;

        // Bouteille ERREUR (Rouge)
        GameObject b2 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        b2.name = "Bouteille_Anomalie";
        b2.transform.position = new Vector3(2.8f, 1.2f, 1);
        b2.transform.localScale = new Vector3(0.2f, 0.4f, 0.2f);
        b2.GetComponent<Renderer>().material.color = Color.red;
        b2.AddComponent<BoxCollider>().isTrigger = true; // Pour détection seringue
        var scriptB2 = b2.AddComponent<SeringueSystem>();
        scriptB2.isBottle = true;
        scriptB2.currentVolume = 8; // <--- Simulation Anomalie (8ml au lieu de 15)
        scriptB2.labelVolume = 15;

        // Seringue Joueur
        GameObject seringue = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        seringue.name = "Seringue_Joueur";
        seringue.transform.position = new Vector3(2.5f, 1.2f, 0.5f);
        seringue.transform.localScale = new Vector3(0.1f, 0.5f, 0.1f);
        seringue.transform.Rotate(90, 0, 0);

        Rigidbody rb = seringue.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true; // Elle flotte pour le test

        var scriptS = seringue.AddComponent<SeringueSystem>();
        scriptS.isBottle = false; // C'est la seringue

        // 5. LE JOUEUR (FPS Mobile)
        // C'est ici qu'on remplace l'ancienne caméra fixe
        GameObject joueur = new GameObject("Joueur_Mobile");
        joueur.transform.position = new Vector3(0, 1.5f, -3); // Départ un peu en arrière

        // Physique du joueur
        CapsuleCollider collider = joueur.AddComponent<CapsuleCollider>();
        collider.height = 2.0f;
        collider.radius = 0.5f;

        // Les Yeux (Caméra)
        GameObject camGO = new GameObject("Yeux");
        camGO.transform.parent = joueur.transform;
        camGO.transform.localPosition = new Vector3(0, 0.6f, 0);
        camGO.AddComponent<Camera>();

        // Ajout du script de contrôle (IMPORTANT : il faut avoir créé le fichier JoueurComplet.cs avant)
        joueur.AddComponent<JoueurComplet>();
    }
}