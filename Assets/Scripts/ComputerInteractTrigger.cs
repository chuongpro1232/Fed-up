using UnityEngine;

public class ComputerInteractTrigger : MonoBehaviour
{
    public ComputerMenuUI computerMenuUI;
    public GameObject ePrompt;

    private bool playerInRange = false;

    private void Start()
    {
        if (ePrompt != null)
        {
            ePrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && (computerMenuUI == null || !computerMenuUI.IsMenuOpen))
        {
            if (computerMenuUI != null)
            {
                computerMenuUI.OpenMenu();
            }

            if (ePrompt != null)
            {
                ePrompt.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (ePrompt != null && (computerMenuUI == null || !computerMenuUI.IsMenuOpen))
            {
                ePrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (ePrompt != null)
            {
                bool shouldShowCap = computerMenuUI == null || !computerMenuUI.IsMenuOpen;
                if (ePrompt.activeSelf != shouldShowCap)
                {
                    ePrompt.SetActive(shouldShowCap);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (ePrompt != null)
            {
                ePrompt.SetActive(false);
            }
        }
    }
}