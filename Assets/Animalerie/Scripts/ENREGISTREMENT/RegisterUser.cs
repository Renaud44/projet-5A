using TMPro;
using UnityEngine;

public class RegisterUser : MonoBehaviour
{
    public TMP_InputField nameInput;  // Ton champ input
    public GameObject panel;          // Le panel à désactiver

    public void SaveName()
    {
        if (string.IsNullOrEmpty(nameInput.text))
            return;

        // Sauvegarde le nom
        PlayerPrefs.SetString("UserName", nameInput.text);
        PlayerPrefs.Save();

        Debug.Log("Utilisateur enregistré : " + nameInput.text);

        // Désactive le panel
        if (panel != null)
            panel.SetActive(false);
    }
}
