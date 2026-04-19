using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class BlackBoardLevelManager : MonoBehaviour
{
    [Tooltip("Danh sách các level. Sẽ tự động lấy các GameObject con có tên chứa chữ 'level' nếu để trống.")]
    public List<GameObject> levels = new List<GameObject>();
    
    [Tooltip("Tên Scene sẽ load sau khi hoàn thành toàn bộ level")]
    public string sceneToLoadOnFinish = "Laptop";
    
    [Header("Thời gian & Luật chơi")]
    [Tooltip("Giới hạn thời gian sống sót của minigame (giây).")]
    public float maxTime = 60f;
    [Tooltip("Hiển thị thời gian còn lại ra màn hình")]
    public TextMeshProUGUI timerText;
    [Tooltip("Tên Scene sẽ bị văng về khi hết giờ (ví dụ: Classroom)")]
    public string timeoutScene = "Classroom";
    
    [Tooltip("Luật chơi hiển thị cho game thủ")]
    [TextArea(3, 5)]
    public string ruleString = "Luật chơi:\n- Kéo thả để ghép nối các phân tử.\n- Không được để trống bất kì cái móc nào.\n- Phải tạo thành một khối phân tử duy nhất.";
    public TextMeshProUGUI ruleText;
    [Tooltip("Thời gian hiển thị bảng luật (giây)")]
    public float ruleDisplayDuration = 15f;

    private float currentTime;
    private bool isGameOver = false;
    private bool isShowingRules = false;
    private int currentLevelIndex = 0;
    private GameObject currentLevelClone;

    void Start()
    {
        // Tự động tìm level trong các game object con nếu list trống
        if (levels.Count == 0)
        {
            foreach (Transform child in transform)
            {
                if (child.name.ToLower().Contains("level"))
                {
                    levels.Add(child.gameObject);
                }
            }
        }

        // Khởi tạo trạng thái ban đầu: Tắt hết các level gốc để làm khuôn mẫu
        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].SetActive(false);
        }

        // Sinh ra bản sao của level hiện tại
        SpawnLevel(currentLevelIndex);

        // Cài đặt thời gian và ui rules
        if (ruleText != null && ruleDisplayDuration > 0)
        {
            isShowingRules = true;
            currentTime = ruleDisplayDuration;
            ruleText.text = ruleString;
            ruleText.gameObject.SetActive(true);
            
            if (timerText != null) timerText.gameObject.SetActive(false);
        }
        else
        {
            isShowingRules = false;
            currentTime = maxTime;
            if (ruleText != null) ruleText.gameObject.SetActive(false);
            if (timerText != null) timerText.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (isGameOver) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            
            if (isShowingRules)
            {
                if (currentTime <= 0)
                {
                    isShowingRules = false;
                    currentTime = maxTime; // Bắt đầu tính giờ chơi chính thức
                    
                    if (ruleText != null) ruleText.gameObject.SetActive(false);
                    if (timerText != null) timerText.gameObject.SetActive(true);
                }
            }
            else
            {
                // Cập nhật UI
                if (timerText != null)
                {
                    int min = Mathf.FloorToInt(currentTime / 60);
                    int sec = Mathf.FloorToInt(currentTime % 60);
                    timerText.text = string.Format("{0:00}:{1:00}", min, sec);
                }

                // Hết giờ
                if (currentTime <= 0)
                {
                    currentTime = 0;
                    isGameOver = true;
                    Debug.Log("Hết giờ!");
                    if (!string.IsNullOrEmpty(timeoutScene))
                    {
                        SceneManager.LoadScene(timeoutScene);
                    }
                }
            }
        }
    }

    void OnEnable()
    {
        AtomHook.OnConnectionChanged += TryCheckLevelComplete;
    }

    void OnDisable()
    {
        AtomHook.OnConnectionChanged -= TryCheckLevelComplete;
    }

    void TryCheckLevelComplete()
    {
        // Hủy các lần check đang chờ trước đó để tránh check liên tục
        CancelInvoke(nameof(CheckLevelComplete));
        // Đợi một khoảng nhỏ để đảm bảo tất cả các component xử lý xong liên kết
        Invoke(nameof(CheckLevelComplete), 0.2f);
    }

    void CheckLevelComplete()
    {
        if (isGameOver) return;

        // Object.FindObjectsByType là API mới của Unity 6 (thay cho FindObjectsOfType)
        AtomHook[] activeHooks = Object.FindObjectsByType<AtomHook>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        if (activeHooks.Length == 0) return;

        bool allConnected = true;
        foreach(var hook in activeHooks)
        {
            if (hook.connectedHook == null)
            {
                allConnected = false;
                break;
            }
        }

        if (allConnected)
        {
            // Kiểm tra thêm điều kiện: tất cả các phân tử (MoleculeAtom) phải nối thành 1 khối duy nhất
            MoleculeAtom[] activeAtoms = Object.FindObjectsByType<MoleculeAtom>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            if (activeAtoms.Length > 0)
            {
                int connectedCount = activeAtoms[0].GetConnectedAtoms().Count;
                if (connectedCount != activeAtoms.Length)
                {
                    // Tồn tại nhiều hơn 1 cụm phân tử rách rời
                    return;
                }
            }

            Debug.Log("Level Complete!");
            // Đợi 1 giây rồi đổi sang level mới cho ngầu
            Invoke(nameof(NextLevel), 1.0f);
        }
    }

    public void ResetLevel()
    {
        if (isGameOver) return;
        SpawnLevel(currentLevelIndex);
    }

    void SpawnLevel(int index)
    {
        if (currentLevelClone != null)
        {
            Destroy(currentLevelClone);
        }

        if (index < levels.Count)
        {
            currentLevelClone = Instantiate(levels[index], transform);
            currentLevelClone.SetActive(true);
            
            // Reset các check đang chờ
            CancelInvoke(nameof(CheckLevelComplete));
        }
    }

    void NextLevel()
    {
        if (currentLevelIndex < levels.Count)
        {
            currentLevelIndex++;
            
            // Bật level tiếp theo nếu còn
            if (currentLevelIndex < levels.Count)
            {
                SpawnLevel(currentLevelIndex);
            }
            else
            {
                isGameOver = true; // Dừng đếm thời gian
                // Nếu đã chơi hết tất cả các level
                Debug.Log("Minigame Complete!");
                if (!string.IsNullOrEmpty(sceneToLoadOnFinish))
                {
                    SceneManager.LoadScene(sceneToLoadOnFinish);
                }
            }
        }
    }
}
