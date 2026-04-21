using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ShootingMinigameManager : MonoBehaviour
{
    [Header("Target Settings")]
    [Tooltip("Danh sách các Prefab Atom và Tờ ghi chú")]
    public GameObject[] targetPrefabs; 
    public int maxTargets = 5;
    public float targetScale = 1.0f;

    [Header("Spawn Area")]
    [Tooltip("Kéo một BoxCollider2D có phạm vi bằng đúng cái màn hình vào đây. Script sẽ lấy form giới hạn của nó để spawn.")]
    public BoxCollider2D spawnArea;
    [Tooltip("Nếu không dùng Spawn Area ở trên, dùng thông số thủ công này.")]
    public float spawnMinX = -7f;
    public float spawnMaxX = 7f;
    public float spawnMinY = -4f;
    public float spawnMaxY = 4f;
    public float minSpawnDistance = 1.5f;

    [Header("Game Settings")]
    public float gameDuration = 30f;

    [Header("Cursor Settings")]
    [Tooltip("Ảnh con trỏ hình chấm đỏ (hoặc tâm ngắm) ngắm bắn")]
    public Texture2D crosshairTexture;
    public Vector2 cursorHotspot = new Vector2(16, 16);

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text finalScoreText;

    private int score = 0;
    private float timer;
    private bool gameEnded = false;

    void Start()
    {
        timer = gameDuration;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateScoreUI();
        UpdateTimerUI();

        if (crosshairTexture != null)
        {
            Cursor.SetCursor(crosshairTexture, cursorHotspot, CursorMode.Auto);
        }

        for (int i = 0; i < maxTargets; i++)
        {
            SpawnTarget();
        }
    }

    void OnDestroy()
    {
        // Phục hồi con trỏ chuột mặc định khi thoát khỏi màn chơi này
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        if (gameEnded)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = 0f;
            EndGame();
        }

        UpdateTimerUI();
    }

    public void TargetHit(GameObject targetObject)
    {
        if (gameEnded)
            return;

        score++;
        UpdateScoreUI();

        Destroy(targetObject);
        SpawnTarget();
    }

    void SpawnTarget()
    {
        if (gameEnded || targetPrefabs == null || targetPrefabs.Length == 0)
            return;

        int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX;
            float randomY;

            if (spawnArea != null)
            {
                Bounds bounds = spawnArea.bounds;
                randomX = Random.Range(bounds.min.x, bounds.max.x);
                randomY = Random.Range(bounds.min.y, bounds.max.y);
            }
            else
            {
                randomX = Random.Range(spawnMinX, spawnMaxX);
                randomY = Random.Range(spawnMinY, spawnMaxY);
            }

            Vector3 spawnPosition = new Vector3(randomX, randomY, 0f);

            if (!IsTooCloseToOtherTargets(spawnPosition))
            {
                int randomIndex = Random.Range(0, targetPrefabs.Length);
                GameObject prefabToSpawn = targetPrefabs[randomIndex];
                
                GameObject newTarget = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                newTarget.transform.localScale = Vector3.one * targetScale;

                Target2DHit targetComponent = newTarget.GetComponent<Target2DHit>();
                if (targetComponent != null)
                {
                    targetComponent.Setup(this);
                }
                else
                {
                    Debug.LogWarning("Prefab không có Target2DHit component. Aim trainer sẽ không nhận diện click cho vật thể này!");
                }

                return;
            }
        }

        Debug.LogWarning("Có quá nhiểu mục tiêu hoặc không gian quá hẹp, không tìm được chỗ spawn mới.");
    }

    bool IsTooCloseToOtherTargets(Vector3 newPosition)
    {
        Target2DHit[] existingTargets = FindObjectsOfType<Target2DHit>();

        foreach (Target2DHit target in existingTargets)
        {
            if (target != null)
            {
                float distance = Vector3.Distance(target.transform.position, newPosition);

                if (distance < minSpawnDistance)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void EndGame()
    {
        gameEnded = true;

        Target2DHit[] remainingTargets = FindObjectsOfType<Target2DHit>();
        foreach (Target2DHit target in remainingTargets)
        {
            if (target != null)
            {
                Destroy(target.gameObject);
            }
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "Final Score: " + score;
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = "Time: " + Mathf.CeilToInt(timer);
        }
    }

    public void TryAgain()
    {
        SceneManager.LoadScene("Shooting");
    }

    public void ExitToNetScene()
    {
        int lastScore = PlayerPrefs.GetInt("ShootingMinigameHighScore", 0);
        
        if (score > lastScore)
        {
            PlayerPrefs.SetInt("ShootingMinigameHighScore", score);
        }

        SceneManager.LoadScene("Net");
    }
}
