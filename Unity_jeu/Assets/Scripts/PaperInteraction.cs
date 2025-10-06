using UnityEngine;

public class PaperInteraction : MonoBehaviour
{
    public GameObject interactionUI;
    public GameObject paperViewUI;
    private bool playerInRange = false;
    private bool isReading = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionUI.SetActive(false);
        paperViewUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && !isReading && Input.GetKeyDown(KeyCode.E))
        {
            OpenPaper();
        }
        else if (isReading && Input.GetKeyDown(KeyCode.E))
        {
            ClosePaper();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactionUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactionUI.SetActive(false);
        }
    }

    private void OpenPaper()
    {
        isReading = true;
        interactionUI.SetActive(false);
        paperViewUI.SetActive(true);

        //Bloquer joueur
        PlayerController movement = FindFirstObjectByType<PlayerController>();
        if (movement != null)
            movement.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ClosePaper()
    {
        isReading = false;
        paperViewUI.SetActive(false);
        interactionUI.SetActive(true);

        //Débloquer joueur
        PlayerController movement = FindFirstObjectByType<PlayerController>();
        if (movement != null)
            movement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
