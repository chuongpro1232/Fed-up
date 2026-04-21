using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("Drag the MainMenuContainer object here")]
    public GameObject mainMenuContainer;

    [Tooltip("Drag the NewGamePanelDialogue object here")]
    public GameObject newGameDialoguePanel;

    private void Start()
    {
        // Ensure the popout panel is hidden when the game first starts
        if (newGameDialoguePanel != null)
        {
            newGameDialoguePanel.SetActive(false);
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

    // Call this when the player clicks "Quit" on the main screen
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
}
