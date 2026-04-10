using UnityEngine;
using UnityEngine.SceneManagement;

public class TravelMenuUI : MonoBehaviour
{
    public GameObject travelMenuPanel;
    public PlayerMovement playerMovement;

    private void Start()
    {
        if (travelMenuPanel != null)
        {
            travelMenuPanel.SetActive(false);
        }
    }

    public void OpenMenu()
    {
        if (travelMenuPanel != null)
        {
            travelMenuPanel.SetActive(true);
        }

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }
    }

    public void CloseMenu()
    {
        if (travelMenuPanel != null)
        {
            travelMenuPanel.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }
    }

    public void GoToStore()
    {
        SceneManager.LoadScene("StoreGame");
    }
}