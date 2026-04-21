using UnityEngine;

public class ComputerInteractTrigger : MonoBehaviour
{
    public ComputerMenuUI computerMenuUI;
    public GameObject ePrompt;

    private bool playerInRange = false;

    private PlayerMovement currentPlayer;

    private void Start()
    {
        if (ePrompt != null)
        {
            ePrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (currentPlayer != null && !currentPlayer.CanMove)
        {
            if (ePrompt != null && ePrompt.activeSelf)
            {
                ePrompt.SetActive(false);
            }
            return;
        }

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
            currentPlayer = other.GetComponent<PlayerMovement>();

            if (currentPlayer != null && !currentPlayer.CanMove)
                return;

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
            if (currentPlayer == null) 
            {
                currentPlayer = other.GetComponent<PlayerMovement>();
            }

            if (currentPlayer != null && !currentPlayer.CanMove)
            {
                if (ePrompt != null && ePrompt.activeSelf) ePrompt.SetActive(false);
                return;
            }

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
            currentPlayer = null;

            if (ePrompt != null)
            {
                ePrompt.SetActive(false);
            }
        }
    }
}