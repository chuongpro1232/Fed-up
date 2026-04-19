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
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
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

            if (ePrompt != null)
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


            {
                ePrompt.SetActive(true);
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