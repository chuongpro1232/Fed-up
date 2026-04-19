using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TrollJoingameController : MonoBehaviour
{
    public GameObject joingameParent; // The overall group for Joingame
    public GameObject csgoBackground; // The CSGO image BG
    public Transform exitButton;
    [Tooltip("Thời gian chờ trước khi Troll CSGO hiện ra")]
    public float waitDelay = 20f;

    private int cornerIndex = 0;
    private Vector3[] corners;

    [Header("Giới hạn khu vực màn hình Laptop")]
    public float minX = -3.8f;
    public float maxX = 4.0f;
    public float minY = -0.8f;
    public float maxY = 2.6f;

    private Vector3 originalScale;

    void Start()
    {
        if (joingameParent != null)
        {
            originalScale = joingameParent.transform.localScale;
            joingameParent.transform.localScale = Vector3.zero;
        }
        if (csgoBackground != null) csgoBackground.SetActive(false);
        if (exitButton != null) exitButton.gameObject.SetActive(false);

        if (exitButton != null)
        {
            float padding = 0.5f;

            corners = new Vector3[5];
            // 4 góc của màn hình Laptop (có thu hẹp 1 chút padding để nút X không bị lẹm ra ngoài viền)
            corners[0] = new Vector3(minX + padding, maxY - padding, exitButton.position.z); // Top-Left
            corners[1] = new Vector3(maxX - padding, maxY - padding, exitButton.position.z); // Top-Right
            corners[2] = new Vector3(maxX - padding, minY + padding, exitButton.position.z); // Bot-Right
            corners[3] = new Vector3(minX + padding, minY + padding, exitButton.position.z); // Bot-Left
            
            // Lần click cuối (tâm màn hình)
            corners[4] = new Vector3((minX + maxX)/2f, (minY + maxY)/2f, exitButton.position.z); // Center
        }

        int state = LaptopGameManager.Instance.CurrentState;
        
        if (state == 2)
        {
            // Trạng thái 2: Kích hoạt đồng hồ chờ để vọt ra cái bảng nhỏ Troll
            StartTrollTimer();
        }
        else if (state == 3)
        {
            // Trạng thái 3: Nếu Cheat thẳng vào State 3, sẽ hiện ngay bảng CSGO to tướng
            if (joingameParent != null) joingameParent.transform.localScale = Vector3.zero;
            if (csgoBackground != null) csgoBackground.SetActive(true);
            if (exitButton != null) 
            {
                exitButton.gameObject.SetActive(true);
                exitButton.position = corners[0]; // Đặt tạm nút tĩnh ở góc trái trên
            }
            // Đẩy Index lên max để click 1 nhát là đóng luôn (thoát State 3)
            cornerIndex = 99;
        }
    }

    public void StartTrollTimer()
    {
        StartCoroutine(WaitAndShowJoingame());
    }

    IEnumerator WaitAndShowJoingame()
    {
        yield return new WaitForSeconds(waitDelay);
        if (joingameParent != null)
        {
            joingameParent.transform.localScale = originalScale;
            if (exitButton != null) exitButton.gameObject.SetActive(true);
            // Vẫn giữ là State 2 cho tới khi người chơi bấm xong nút X né tránh
        }
    }

    public void OnExitClicked()
    {
        if (cornerIndex < corners.Length)
        {
            exitButton.position = corners[cornerIndex];
            cornerIndex++;
        }
        else
        {
            if (LaptopGameManager.Instance.CurrentState == 2)
            {
                // Vượt qua ải Troll nút chạy nhảy (State 2) -> Bật bảng CSGO bự (State 3)
                LaptopGameManager.Instance.CurrentState = 3;
                
                if (joingameParent != null) joingameParent.transform.localScale = Vector3.zero;
                if (csgoBackground != null) csgoBackground.SetActive(true);
                
                if (exitButton != null)
                {
                    exitButton.position = corners[0]; // Di chuyển nút X về góc trái trên của bảng CSGO bự
                }
                
                cornerIndex = 99; // Lần click tiếp theo sẽ chui xuống nhánh else bên dưới
            }
            else if (LaptopGameManager.Instance.CurrentState == 3)
            {
                // Lần click cuối cùng ở bảng bự: Tắt sạch quảng cáo troll và chơi tiếp game cân
                if (csgoBackground != null) csgoBackground.SetActive(false);
                if (exitButton != null) exitButton.gameObject.SetActive(false);
                
                // Ở đây State vẫn là 3, người chơi bắt đầu ung dung giải đó cái Cân
            }
        }
    }
}
