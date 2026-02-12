using UnityEngine;
using UnityEngine.UI;

public class ClipboardSystem : MonoBehaviour
{
    [Header("UI References")]
    public Text nomDisplay;
    public Text codeDisplay;

    private GameObject groupBoutonSaisie;
    private GameObject groupClavier;
    private string currentNom = "";
    private string alphabet = "AZERTYUIOPQSDFGHJKLMWXCVBN";

    public void OuvrirClavier()
    {
        groupBoutonSaisie.SetActive(false);
        groupClavier.SetActive(true);
        if (currentNom == "") UpdateDisplay("...");
    }

    public void AjouterLettre(string lettre)
    {
        if (currentNom.Length < 10) { currentNom += lettre; UpdateDisplay(currentNom); }
    }

    public void Effacer()
    {
        if (currentNom.Length > 0) { currentNom = currentNom.Substring(0, currentNom.Length - 1); UpdateDisplay(currentNom); }
    }

    public void ValiderSaisie()
    {
        if (currentNom.Length > 0 && currentNom != "...")
        {
            groupClavier.SetActive(false);
            if (ManagerMedical.instance != null)
            {
                ManagerMedical.instance.GenererCodeSecret();
                codeDisplay.text = "CODE : " + ManagerMedical.instance.codeSecretGenerated;
                codeDisplay.color = Color.green;
                nomDisplay.color = Color.black;
            }
        }
    }

    public void AnnulerSaisie()
    {
        groupClavier.SetActive(false);
        groupBoutonSaisie.SetActive(true);
        currentNom = "";
        UpdateDisplay("NOM...");
    }

    public void SignalerAnomalie()
    {
        codeDisplay.text = "ANOMALIE SIGNALÉE";
        codeDisplay.color = Color.red;
    }

    void UpdateDisplay(string text) { if (nomDisplay) nomDisplay.text = text; }

    // --- CONSTRUCTION ---
    public void SetupClipboardAuto()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);

        // 1. TABLETTE
        GameObject tablette = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tablette.transform.parent = this.transform;
        tablette.transform.localPosition = Vector3.zero;
        tablette.transform.localRotation = Quaternion.identity;
        tablette.transform.localScale = new Vector3(0.6f, 0.8f, 0.02f);
        tablette.GetComponent<Renderer>().material.color = Color.gray;
        Destroy(tablette.GetComponent<BoxCollider>());

        // 2. ECRAN
        GameObject canvasGO = new GameObject("Ecran_UI");
        canvasGO.transform.parent = this.transform;
        canvasGO.transform.localPosition = new Vector3(0, 0, -0.03f);
        canvasGO.transform.localRotation = Quaternion.identity;
        canvasGO.transform.localScale = Vector3.one * 0.001f;

        Canvas c = canvasGO.AddComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;

        GameObject tNom = CreateTextObj(canvasGO.transform, "NOM...", 380);
        nomDisplay = tNom.GetComponent<Text>();
        nomDisplay.fontSize = 70;

        GameObject tCode = CreateTextObj(canvasGO.transform, "CODE: ----", 320);
        codeDisplay = tCode.GetComponent<Text>();
        codeDisplay.fontSize = 60;
        codeDisplay.color = Color.yellow;

        // BOUTON ANOMALIE
        // (Lui c'est un bouton spécial large, donc on met 'true' à la fin)
        CreerTouche3D(this.transform, "SIGNALER\nANOMALIE", new Vector3(0, -0.35f, -0.05f), new Vector3(0.4f, 0.08f, 0.02f), Color.red, () => SignalerAnomalie(), true);

        // 3. GROUPE START
        groupBoutonSaisie = new GameObject("Groupe_Start");
        groupBoutonSaisie.transform.parent = this.transform;
        groupBoutonSaisie.transform.localPosition = Vector3.zero;
        groupBoutonSaisie.transform.localRotation = Quaternion.identity;

        // BOUTON SAISIR (C'est lui qui posait problème, on active la correction 'true')
        CreerTouche3D(groupBoutonSaisie.transform, "SAISIR NOM", new Vector3(0, 0.0f, -0.05f), new Vector3(0.45f, 0.1f, 0.02f), Color.cyan, () => OuvrirClavier(), true);

        // 4. GROUPE CLAVIER
        groupClavier = new GameObject("Groupe_Clavier");
        groupClavier.transform.parent = this.transform;
        // Position Y = 1f (Hologramme en haut)
        groupClavier.transform.localPosition = new Vector3(0f, 1f, -0.15f);
        groupClavier.transform.localRotation = Quaternion.identity;

        GenererTouches(groupClavier.transform);
        groupClavier.SetActive(false);
    }

    void GenererTouches(Transform parent)
    {
        GameObject fond = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fond.transform.parent = parent;
        fond.transform.localPosition = new Vector3(0, 0.0f, 0.01f);
        fond.transform.localRotation = Quaternion.identity;
        fond.transform.localScale = new Vector3(0.75f, 0.75f, 0.01f);
        fond.GetComponent<Renderer>().material.color = Color.black;
        Destroy(fond.GetComponent<BoxCollider>());

        int cols = 7;
        float gap = 0.07f;
        float startX = -0.21f;
        float startY = 0.25f;
        float zKey = -0.05f;

        for (int i = 0; i < alphabet.Length; i++)
        {
            string lettre = alphabet[i].ToString();
            int row = i / cols;
            int col = i % cols;
            Vector3 pos = new Vector3(startX + (col * gap), startY - (row * gap), zKey);

            // ICI : On met 'false' (ou rien) pour ne PAS appliquer la correction bizarre
            CreerTouche3D(parent, lettre, pos, new Vector3(0.05f, 0.05f, 0.02f), Color.white, () => AjouterLettre(lettre), false);
        }

        float yBottom = startY - (4.2f * gap);

        // Les boutons du bas sont un peu larges, mais restons standard 'false' pour qu'ils soient nets
        CreerTouche3D(parent, "X", new Vector3(-0.18f, yBottom, zKey), new Vector3(0.10f, 0.06f, 0.02f), Color.red, () => AnnulerSaisie(), false);
        CreerTouche3D(parent, "EFF", new Vector3(0, yBottom, zKey), new Vector3(0.10f, 0.06f, 0.02f), new Color(1, 0.5f, 0), () => Effacer(), false);
        CreerTouche3D(parent, "OK", new Vector3(0.18f, yBottom, zKey), new Vector3(0.14f, 0.06f, 0.02f), Color.green, () => ValiderSaisie(), false);
    }

    // J'ai ajouté le paramètre 'isSpecialButton' à la fin
    void CreerTouche3D(Transform parent, string label, Vector3 localPos, Vector3 taille, Color c, UnityEngine.Events.UnityAction action, bool isSpecialButton = false)
    {
        GameObject btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
        btn.name = "Btn_" + label;
        btn.transform.parent = parent;
        btn.transform.localPosition = localPos;
        btn.transform.localRotation = Quaternion.identity;
        btn.transform.localScale = taille;
        btn.GetComponent<Renderer>().material.color = c;

        var s = btn.AddComponent<SimpleButton>();
        s.action = action;

        // Label
        GameObject canvasTxt = new GameObject("Label");
        canvasTxt.transform.parent = btn.transform;
        canvasTxt.transform.localPosition = new Vector3(0, 0, -0.55f);
        canvasTxt.transform.localRotation = Quaternion.identity;

        // --- C'EST ICI QUE LA MAGIE OPÈRE ---
        if (isSpecialButton)
        {
            // Mode "SAISIR NOM" : On inverse l'échelle pour compenser le bouton rectangle
            float ratioX = 1.0f / taille.x;
            float ratioY = 1.0f / taille.y;
            canvasTxt.transform.localScale = new Vector3(ratioX * 0.002f, ratioY * 0.002f, 0.01f);
        }
        else
        {
            // Mode CLAVIER STANDARD : On touche à rien, échelle classique
            canvasTxt.transform.localScale = Vector3.one * 0.01f;
        }

        Canvas cv = canvasTxt.AddComponent<Canvas>();
        cv.renderMode = RenderMode.WorldSpace;

        Text t = canvasTxt.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = label;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.black;
        t.rectTransform.sizeDelta = new Vector2(100, 100);

        if (isSpecialButton)
        {
            // Pour le gros bouton, on laisse le texte s'adapter
            t.resizeTextForBestFit = true;
            t.resizeTextMinSize = 5;
            t.resizeTextMaxSize = 100;
        }
        else
        {
            // Pour le clavier, on force une taille fixe et lisible
            t.fontSize = 50;
            t.resizeTextForBestFit = false;
        }
    }

    GameObject CreateTextObj(Transform parent, string content, float yPos)
    {
        GameObject go = new GameObject("Txt_" + content);
        go.transform.parent = parent;
        go.transform.localPosition = new Vector3(0, yPos, 0);
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;

        Text t = go.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = content;
        t.alignment = TextAnchor.MiddleCenter;
        t.color = Color.black;
        t.rectTransform.sizeDelta = new Vector2(600, 100);
        return go;
    }
}