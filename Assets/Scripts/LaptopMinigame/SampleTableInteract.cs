using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))] 
public class SampleTableInteract : MonoBehaviour
{
    [Tooltip("Clicking this object will load the Laptop scene.")]
    void OnMouseUpAsButton() // Requires Physics2DRaycaster or standard physics raycasting
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
}
