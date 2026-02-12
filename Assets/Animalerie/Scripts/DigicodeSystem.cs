using UnityEngine;
using UnityEngine.UI;

public class DigicodeSystem : MonoBehaviour
{
    public Text ecran;
    private string codeEntree = "";
    private bool ouvert = false;

    public void PressChiffre(int chiffre)
    {
        if (ouvert) return;
        codeEntree += chiffre.ToString();
        UpdateEcran();

        if (codeEntree.Length == 4)
        {
            VerifierCode();
        }
    }

    public void ResetCode()
    {
        codeEntree = "";
        UpdateEcran();
    }

    void VerifierCode()
    {
        if (codeEntree == ManagerMedical.instance.codeSecretGenerated)
        {
            ecran.text = "OUVERT";
            ecran.color = Color.green;
            OuvrirPorte();
        }
        else
        {
            ecran.text = "ERREUR";
            ecran.color = Color.red;
            Invoke("ResetCode", 1.0f);
        }
    }

    void OuvrirPorte()
    {
        ouvert = true;
        // Animation simple : rotation de 90 degrés du parent (la porte)
        transform.parent.Rotate(0, 90, 0);
    }

    void UpdateEcran()
    {
        if (ecran) ecran.text = codeEntree;
    }

    // --- SETUP AUTO ---
    public void SetupDigicodeAuto()
    {
        GameObject canvasGO = new GameObject("Canvas_Digi");
        canvasGO.transform.parent = this.transform;
        canvasGO.transform.localPosition = new Vector3(0, 0, -0.06f);
        canvasGO.transform.localScale = Vector3.one * 0.005f;
        Canvas c = canvasGO.AddComponent<Canvas>();
        c.renderMode = RenderMode.WorldSpace;

        // Ecran
        GameObject t = new GameObject("Ecran");
        t.transform.parent = canvasGO.transform;
        t.transform.localPosition = new Vector3(0, 50, 0);
        ecran = t.AddComponent<Text>();
        ecran.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        ecran.text = "----";
        ecran.alignment = TextAnchor.MiddleCenter;
        ecran.fontSize = 40;
        ecran.color = Color.red;
        t.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);

        // Touches 1, 2, 3, 4 (exemple simplifié)
        CreerTouche(canvasGO.transform, 1, new Vector3(-30, 0, 0));
        CreerTouche(canvasGO.transform, 2, new Vector3(30, 0, 0));
        CreerTouche(canvasGO.transform, 3, new Vector3(-30, -30, 0));
        CreerTouche(canvasGO.transform, 4, new Vector3(30, -30, 0));
        // Note: Dans un vrai projet, fais une boucle 0-9
    }

    void CreerTouche(Transform parent, int chiffre, Vector3 pos)
    {
        GameObject btn = GameObject.CreatePrimitive(PrimitiveType.Cube);
        btn.transform.parent = parent;
        btn.transform.localPosition = pos;
        btn.transform.localScale = new Vector3(25, 25, 5);
        btn.GetComponent<Renderer>().material.color = Color.black;

        var sb = btn.AddComponent<SimpleButton>();
        sb.action = () => PressChiffre(chiffre);

        // Texte chiffre
        GameObject txt = new GameObject("Txt");
        txt.transform.parent = btn.transform;
        txt.transform.localPosition = new Vector3(0, 0, -1);
        txt.transform.localScale = Vector3.one * 0.1f;
        Text t = txt.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = chiffre.ToString();
        t.alignment = TextAnchor.MiddleCenter;
        t.fontSize = 100;
    }
}