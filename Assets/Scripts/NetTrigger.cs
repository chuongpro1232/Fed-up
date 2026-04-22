using UnityEngine;
using UnityEngine.SceneManagement;

public class NetTrigger : MonoBehaviour
{
    [Tooltip("The exact name of the Scene you want to load when touched")]
    public string sceneToLoad = "Shooting";

    private void OnTriggerEnter2D(Collider2D other)
    {
        // When an object hits this trigger, check if it is the Player
        if (other.CompareTag("Player"))
        {
            // If it is the Player, load the Shooting scene!
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
