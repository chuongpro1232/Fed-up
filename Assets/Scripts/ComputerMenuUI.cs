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
        int state = PlayerPrefs.GetInt("LaptopPuzzleState", 0);
        
        // Progress states if returning from cutscenes
        if (state == 1)
        {
            PlayerPrefs.SetInt("LaptopPuzzleState", 2);
        }
        else if (state == 4)
        {
            PlayerPrefs.SetInt("LaptopPuzzleState", 5);
        }

        SceneManager.LoadScene("Laptop");
    }

    public void GoToPlayGame()
    {
        SceneManager.LoadScene("AimTrainer");
    }
}