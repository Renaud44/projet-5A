using UnityEngine;

public class SeringueSystem : MonoBehaviour
{
    // Peut être une bouteille OU une seringue
    public bool isBottle;

    [Header("Si Bouteille")]
    public float currentVolume;
    public float labelVolume;

    [Header("Si Seringue")]
    public float volumeAspire = 0;

    // Pour la seringue : détection
    private SeringueSystem bouteilleDetectee;

    void Update()
    {
        // Logique Seringue : Si on clique GAUCHE et qu'on touche une bouteille
        if (!isBottle && Input.GetMouseButton(0) && bouteilleDetectee != null)
        {
            Aspirer();
        }
    }

    // Collision (Trigger)
    void OnTriggerEnter(Collider other)
    {
        if (!isBottle) // Si je suis la seringue
        {
            SeringueSystem bouteille = other.GetComponent<SeringueSystem>();
            if (bouteille != null && bouteille.isBottle)
            {
                bouteilleDetectee = bouteille;
                Debug.Log("Seringue en contact avec : " + other.name);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!isBottle) bouteilleDetectee = null;
    }

    void Aspirer()
    {
        if (bouteilleDetectee.currentVolume > 0)
        {
            float quantite = Time.deltaTime * 5.0f; // Vitesse d'aspiration

            bouteilleDetectee.currentVolume -= quantite;
            volumeAspire += quantite;

            Debug.Log($"Aspiration... Seringue: {volumeAspire:F1}ml | Bouteille: {bouteilleDetectee.currentVolume:F1}ml");

            // Feedback visuel : grossir un peu la seringue
            transform.localScale += Vector3.up * 0.001f;
        }
        else
        {
            Debug.Log("Bouteille vide !");
        }
    }

    // Fonction pour vérifier l'anomalie (pour le scénario du clipboard)
    public bool AUneAnomalie()
    {
        // On tolère une petite marge d'erreur float
        return Mathf.Abs(currentVolume - labelVolume) > 0.1f;
    }
}