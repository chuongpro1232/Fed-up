using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaveManager : MonoBehaviour
{
    // This allows us to access GameSaveManager.Instance from ANY script!
    public static GameSaveManager Instance;

    private void Awake()
    {
        // Make sure there is only ever ONE Save Manager, and it never gets destroyed when changing scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Call this to SAVE the game to a specific slot (1, 2, or 3)
    public void SaveGame(int slotIndex, Vector3 playerPos)
    {
        string prefix = "Slot" + slotIndex + "_";
        
        // 1. Save that a file actually exists in this slot
        PlayerPrefs.SetInt(prefix + "HasSave", 1);
        
        // 2. Save the current Scene Name
        PlayerPrefs.SetString(prefix + "SceneName", SceneManager.GetActiveScene().name);
        
        // 3. Save the exact X, Y, Z coordinates of the player
        PlayerPrefs.SetFloat(prefix + "PosX", playerPos.x);
        PlayerPrefs.SetFloat(prefix + "PosY", playerPos.y);
        PlayerPrefs.SetFloat(prefix + "PosZ", playerPos.z);

        // 4. Save all your important global data!
        PlayerPrefs.SetString(prefix + "PlayerName", PlayerPrefs.GetString("PlayerName", "Player"));
        PlayerPrefs.SetInt(prefix + "CurrentDay", PlayerPrefs.GetInt("CurrentDay", 1));
        PlayerPrefs.SetInt(prefix + "LaptopPuzzleState", PlayerPrefs.GetInt("LaptopPuzzleState", 0));
        PlayerPrefs.SetInt(prefix + "AimTrainerHighScore", PlayerPrefs.GetInt("AimTrainerHighScore", 0));

        PlayerPrefs.Save();
        Debug.Log("Game SUCCESSFULLY Saved to Slot " + slotIndex);
    }

    // Call this to LOAD the game from a specific slot (1, 2, or 3)
    public void LoadGame(int slotIndex)
    {
        string prefix = "Slot" + slotIndex + "_";
        
        // Check if there is actually a save file in this slot
        if (PlayerPrefs.GetInt(prefix + "HasSave", 0) == 1)
        {
            // 1. Restore all the global data back to the normal PlayerPrefs
            PlayerPrefs.SetString("PlayerName", PlayerPrefs.GetString(prefix + "PlayerName"));
            PlayerPrefs.SetInt("CurrentDay", PlayerPrefs.GetInt(prefix + "CurrentDay"));
            PlayerPrefs.SetInt("LaptopPuzzleState", PlayerPrefs.GetInt(prefix + "LaptopPuzzleState"));
            PlayerPrefs.SetInt("AimTrainerHighScore", PlayerPrefs.GetInt(prefix + "AimTrainerHighScore"));
            PlayerPrefs.Save();

            // 2. Grab the saved Scene Name and Coordinates
            string sceneName = PlayerPrefs.GetString(prefix + "SceneName");
            Vector3 pos = new Vector3(
                PlayerPrefs.GetFloat(prefix + "PosX"),
                PlayerPrefs.GetFloat(prefix + "PosY"),
                PlayerPrefs.GetFloat(prefix + "PosZ")
            );

            // 3. Tell SceneReturnData to put the player exactly at these coordinates when the scene opens!
            if (sceneName == "SampleScene")
            {
                SceneReturnData.hasSavedSampleScenePosition = true;
                SceneReturnData.sampleScenePlayerPosition = pos;
                SceneReturnData.skipSampleSceneIntro = true; // Don't play the intro cutscene on load
            }
            else if (sceneName == "Classroom")
            {
                SceneReturnData.hasSavedClassroomPosition = true;
                SceneReturnData.classroomPlayerPosition = pos;
                SceneReturnData.skipClassroomIntro = true; // Don't play intro cutscene on load
            }
            
            // 4. Load the Scene!
            SceneManager.LoadScene(sceneName);
            Debug.Log("Game SUCCESSFULLY Loaded from Slot " + slotIndex);
        }
        else
        {
            Debug.LogWarning("No save data exists in Slot " + slotIndex + "!");
        }
    }
}
