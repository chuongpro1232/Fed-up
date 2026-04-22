using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("Drag the MainMenuContainer object here")]
    public GameObject mainMenuContainer;

    [Tooltip("Drag the NewGamePanelDialogue object here")]
    public GameObject newGameDialoguePanel;
    
    [Tooltip("Drag the SaveSlotPanel object here")]
    public GameObject loadSlotPanel;

    [Tooltip("Drag the LoadGamePanelDialogue object here")]
    public GameObject loadGameDialoguePanel;

    private void Start()
    {
        // Ensure the popout panels are hidden when the game first starts
        if (newGameDialoguePanel != null)
        {
            newGameDialoguePanel.SetActive(false);
        }
        if (loadSlotPanel != null)
        {
            loadSlotPanel.SetActive(false);
        }
        if (loadGameDialoguePanel != null)
        {
            loadGameDialoguePanel.SetActive(false);
        }
    }

    // Call this when the player clicks the "New Game" button on the main screen
    public void OnClickNewGameButton()
    {
        if (newGameDialoguePanel != null)
        {
            newGameDialoguePanel.SetActive(true);
        }
        if (mainMenuContainer != null)
        {
            mainMenuContainer.SetActive(false);
        }
    }

    // Call this when the player clicks "Yes" on the popout warning
    public void OnClickNewGameYes()
    {
        // Erase all previous saves to start a completely fresh game!
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Load the game!
        SceneManager.LoadScene("SampleScene");
    }

    // Call this when the player clicks "No" on the popout warning
    public void OnClickNewGameNo()
    {
        // Just hide the panel to go back to the main menu
        if (newGameDialoguePanel != null)
        {
            newGameDialoguePanel.SetActive(false);
        }

        // Re-enable the main menu container!
        if (mainMenuContainer != null)
        {
            mainMenuContainer.SetActive(true);
        }
    }

    // Call this when clicking "Main Menu" on the Esc Panel
    public void OnClickQuitButton()
    {
        Debug.Log("Quit Game Activated!");
        
        // This closes the built game
        Application.Quit();

        // This makes it stop playing if you are testing inside the Unity Editor!
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // --- NEW LOAD SYSTEM ---

    // Call this when clicking "Load Game" on the main menu
    public void OnClickLoadGameButton()
    {
        // Show the warning dialogue first!
        if (loadGameDialoguePanel != null)
        {
            loadGameDialoguePanel.SetActive(true);
        }
        if (mainMenuContainer != null)
        {
            mainMenuContainer.SetActive(false);
        }
    }

    // Call this when clicking "Yes" on the Load Warning Dialogue
    public void OnClickLoadGameYes()
    {
        // Hide the warning and show the actual slots
        if (loadGameDialoguePanel != null)
        {
            loadGameDialoguePanel.SetActive(false);
        }
        if (loadSlotPanel != null)
        {
            loadSlotPanel.SetActive(true);
        }
    }

    // Call this when clicking "No" on the Load Warning Dialogue
    public void OnClickLoadGameNo()
    {
        // Hide the warning and go back to main menu
        if (loadGameDialoguePanel != null)
        {
            loadGameDialoguePanel.SetActive(false);
        }
        if (mainMenuContainer != null)
        {
            mainMenuContainer.SetActive(true);
        }
    }

    // Call this when clicking "Return" on the Load Slot Panel
    public void OnClickCloseLoadPanel()
    {
        if (loadSlotPanel != null)
        {
            loadSlotPanel.SetActive(false);
        }
        if (mainMenuContainer != null)
        {
            mainMenuContainer.SetActive(true);
        }
    }

    // Call this from Load Slot 1 (passing in 1), Slot 2 (passing in 2), etc.
    public void LoadFromSlot(int slotIndex)
    {
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.LoadGame(slotIndex);
        }
        else
        {
            Debug.LogError("Cannot load! GameSaveManager is missing from the scene.");
        }
    }
}
