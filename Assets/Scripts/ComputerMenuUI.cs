using UnityEngine;
using UnityEngine.SceneManagement;

public class ComputerMenuUI : MonoBehaviour
{
    public GameObject computerMenuPanel;
    public PlayerMovement playerMovement;

    private void Start()
    {
        if (computerMenuPanel != null)
        {
            computerMenuPanel.SetActive(false);
        }
    }

    public void OpenMenu()
    {
        if (computerMenuPanel != null)
        {
            computerMenuPanel.SetActive(true);
        }

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }
    }

    public void CloseMenu()
    {
        if (computerMenuPanel != null)
        {
            computerMenuPanel.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }
    }

    public void GoToStudy()
    {
        SceneManager.LoadScene("MiniGameMath");
    }

    public void GoToPlayGame()
    {
        SceneManager.LoadScene("AimTrainer");
    }
}