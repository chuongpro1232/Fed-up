using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject escPanel;
    public GameObject saveSlotPanel;

    private bool isPaused = false;
    private PlayerMovement playerMovement;

    void Start()
    {
        // Make sure panels are hidden when the game starts
        if (escPanel != null) escPanel.SetActive(false);
        if (saveSlotPanel != null) saveSlotPanel.SetActive(false);
        
        // Find the player in the scene so we can get their position for saving
        playerMovement = Object.FindFirstObjectByType<PlayerMovement>();
    }

    void Update()
    {
        // Listen for the ESC key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Freeze time!
        
        if (playerMovement != null) playerMovement.SetCanMove(false);
        
        if (escPanel != null) escPanel.SetActive(true);
        if (saveSlotPanel != null) saveSlotPanel.SetActive(false);
    }

    // Call this from a "Resume" button if you make one, or it triggers when hitting ESC again
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Unfreeze time!
        
        if (playerMovement != null) playerMovement.SetCanMove(true);
        
        if (escPanel != null) escPanel.SetActive(false);
        if (saveSlotPanel != null) saveSlotPanel.SetActive(false);
    }

    // Call this when clicking "Save" on the Esc Panel
    public void OpenSavePanel()
    {
        if (escPanel != null) escPanel.SetActive(false);
        if (saveSlotPanel != null) saveSlotPanel.SetActive(true);
    }

    // Call this when clicking "Return" on the Save Slot Panel
    public void CloseSavePanel()
    {
        if (saveSlotPanel != null) saveSlotPanel.SetActive(false);
        if (escPanel != null) escPanel.SetActive(true);
    }

    // Call this when clicking "Save Slot 1", passing in 1. (etc)
    public void SaveToSlot(int slotIndex)
    {
        if (GameSaveManager.Instance != null && playerMovement != null)
        {
            GameSaveManager.Instance.SaveGame(slotIndex, playerMovement.transform.position);
            
            // Optionally, close the menu immediately after saving!
            ResumeGame();
        }
        else
        {
            Debug.LogError("Cannot save! Missing GameSaveManager in the scene, or could not find PlayerMovement.");
        }
    }

    // Call this when clicking "Main Menu" on the Esc Panel
    public void ReturnToMainMenu()
    {
        // Always set time scale back to 1 before changing scenes!
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
