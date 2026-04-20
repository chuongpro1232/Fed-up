using UnityEngine;
using UnityEngine.SceneManagement;

public class BlackBoardTrigger : MonoBehaviour
{
    [Tooltip("The name of the scene to load")]
    public string sceneToLoad = "BlackBoard";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save the player's exact position so they spawn right here when returning
            SceneReturnData.hasSavedClassroomPosition = true;
            SceneReturnData.classroomPlayerPosition = other.transform.position;
            
            // Mark that we should skip the intro cutscene upon returning
            SceneReturnData.skipClassroomIntro = true;
            
            // Load the minigame!
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
