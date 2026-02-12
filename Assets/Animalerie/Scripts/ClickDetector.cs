using UnityEngine;

public class ClickDetector : MonoBehaviour
{
    // Référence au cerveau principal
    public SystemeHygiene parentScript;

    // Est-ce que c'est le bouton du gel ? (Sinon c'est le savon)
    public bool isGel = false;

    void OnMouseDown()
    {
        // On vérifie que le script parent est bien assigné pour éviter un crash
        if (parentScript != null)
        {
            if (isGel)
            {
                parentScript.ActionPrendreGel();
            }
            else
            {
                parentScript.ActionPrendreSavon();
            }
        }
    }
}