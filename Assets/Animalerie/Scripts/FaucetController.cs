using UnityEngine;
using TMPro;

public class FaucetController : MonoBehaviour
{
    public GameObject instructionText;
    public GameObject handle;
    public ParticleSystem water;
    public AudioSource waterSound; // Nouvelle ligne pour le son
    public float rotationSpeed = 90f;

    private bool isPlayerNearby = false;
    private bool isWaterOn = false;

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ToggleFaucet();
        }

        if (isWaterOn)
        {
            handle.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }

    void ToggleFaucet()
    {
        isWaterOn = !isWaterOn;

        if (isWaterOn)
        {
            water.Play();
            waterSound.Play(); // On lance le son
        }
        else
        {
            water.Stop();
            waterSound.Stop(); // On arrête le son
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            instructionText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            instructionText.SetActive(false);
        }
    }
}