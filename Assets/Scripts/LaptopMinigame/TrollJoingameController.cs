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
        if (csgoBackground == null) 
        {
            Transform csgoObj = transform.parent != null ? transform.parent.Find("csgo_0") : null;
            if (csgoObj != null) csgoBackground = csgoObj.gameObject;
        }

        if (joingameParent != null)
        {
            // Bật active phòng khi LaptopGameManager lỡ tắt cứng nó ở state trước.
            joingameParent.SetActive(true);
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
            Debug.Log("TROLL CONTROLLER START - State 3 detected! Showing CSGO Màn hình lớn.");
            // Trạng thái 3: Bước thẳng vào là CÓ SẴN bảng CSGO to tướng chờ bấm tắt
            if (joingameParent != null) joingameParent.transform.localScale = Vector3.zero;
            if (csgoBackground != null) csgoBackground.SetActive(true);
            else Debug.LogError("csgoBackground is missing! Kéo csgo_0 vào biến csgoBackground!!");
            
            if (exitButton != null) 
            {
                exitButton.gameObject.SetActive(true);
                if (corners != null && corners.Length > 0) exitButton.position = corners[0]; // Đặt cục X ở góc trái trên
            }
            // Đẩy Index lên max để click 1 nhát là đóng luôn (thoát State 3 CSGO)
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
            joingameParent.SetActive(true);
            joingameParent.transform.localScale = originalScale;
            if (exitButton != null) exitButton.gameObject.SetActive(true);
            // Vẫn giữ là State 2 cho tới khi người chơi bấm xong nút X né tránh
        }
    }

    public void OnExitClicked()
    {
        Debug.Log("EXIT CLIKED! Current Index: " + cornerIndex);
        // Khắc phục lỗi mảng corners bị Null khi nút exitButton lỡ bị rớt mất tham chiếu trong Inspector
        if (corners == null) corners = new Vector3[0];

        if (cornerIndex < corners.Length && exitButton != null)
        {
            exitButton.position = corners[cornerIndex];
            cornerIndex++;
        }
        else
        {
            if (cornerIndex == 5)
            {
                Debug.Log("State 2: Reached end of small popup clicks. Showing Big CSGO (no exit button) and waiting 3 seconds.");
                cornerIndex++; // Increment to lock further clicks
                StartCoroutine(ShowBigCSGOAndKick());
            }
            else if (cornerIndex == 99 && LaptopGameManager.Instance.CurrentState == 3)
            {
                // Hành động ở State 3: Bấm X để tắt CSGO và bắt đầu màn chơi cái cân
                Debug.Log("State 3: Clicked Big CSGO X -> Hiding Big CSGO and showing Balance Scale!");
                
                if (csgoBackground != null) csgoBackground.SetActive(false);
                if (exitButton != null) exitButton.gameObject.SetActive(false);
                
                // Trigger lại GameManager để hiện cân
                LaptopGameManager.Instance.gameObject.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    IEnumerator ShowBigCSGOAndKick()
    {
        // Bung bảng bự che chết màn hình
        if (joingameParent != null) joingameParent.transform.localScale = Vector3.zero;
        if (csgoBackground != null) csgoBackground.SetActive(true);
        if (exitButton != null) exitButton.gameObject.SetActive(false); // Dấu luôn nút X, không cho người chơi tắt

        // Tra tấn 3 giây
        yield return new WaitForSeconds(3f);

        // Sau đó tống ra phòng
        Debug.Log("3 seconds passed -> Kick to SampleScene and set State = 3");
        LaptopGameManager.Instance.CurrentState = 3;
        SceneManager.LoadScene("SampleScene");
    }
}
