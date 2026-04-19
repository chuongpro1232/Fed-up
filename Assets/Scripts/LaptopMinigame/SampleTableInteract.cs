using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))] 
public class SampleTableInteract : MonoBehaviour
{
    [Tooltip("Clicking this object will load the Laptop scene.")]
    void OnMouseUpAsButton() // Requires Physics2DRaycaster or standard physics raycasting
    {
        int state = PlayerPrefs.GetInt("LaptopPuzzleState", 0);
        
        if (state == 0)
        {
            // Lần đầu bước tới bàn: Kích State lên 1 để vào Laptop gặp Steam Icon
            PlayerPrefs.SetInt("LaptopPuzzleState", 1);
        }
        else if (state == 4)
        {
            // Lần về cuối cùng: Kích lên 5 để không bị lặp lại thoại
            PlayerPrefs.SetInt("LaptopPuzzleState", 5);
        }
        // Riêng State 2 (Đã xem xong Steam) thì giữ nguyên State 2 để chui vào Laptop gặp Troll Game
        
        SceneManager.LoadScene("Laptop");
    }
}
