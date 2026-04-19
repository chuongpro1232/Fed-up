using UnityEngine;
using UnityEngine.SceneManagement;

public class ComputerMenuUI : MonoBehaviour
{
    public GameObject computerMenuPanel;
    public PlayerMovement playerMovement;
    public Transform playerTransform;

    public bool IsMenuOpen
    {
        get
        {
            return computerMenuPanel != null && computerMenuPanel.activeSelf;
        }
    }

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

    private void SaveReturnData()
    {
        if (playerTransform != null)
        {
            SceneReturnData.sampleScenePlayerPosition = playerTransform.position;
            SceneReturnData.hasSavedSampleScenePosition = true;
        }

        SceneReturnData.skipSampleSceneIntro = true;
    }

    public void GoToPlayGame()
    {
        SaveReturnData();
        SceneManager.LoadScene("AimTrainer");
    }

    public void GoToStudy()
    {
        SaveReturnData();
        SceneManager.LoadScene("Laptop");
    }
}