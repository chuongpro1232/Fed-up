using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class AimTrainerManager : MonoBehaviour
{
    [Header("Target Settings")]
    public GameObject targetPrefab;
    public int maxTargets = 5;
    public float targetScale = 0.5f;
    public float spawnMinX = -4f;
    public float spawnMaxX = 4f;
    public float spawnMinY = -2.5f;
    public float spawnMaxY = 2.5f;
    public float spawnZ = 8f;
    public float minSpawnDistance = 1.0f;

    [Header("Game Settings")]
    public float gameDuration = 30f;

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

        for (int i = 0; i < maxTargets; i++)
        {
            SpawnTarget();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Target"))
            {
                TargetHit(hit.collider.gameObject);
            }
        }
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
        if (gameEnded || targetPrefab == null)
            return;

        int maxAttempts = 30;

        for (int i = 0; i < maxAttempts; i++)
        {
            float randomX = Random.Range(spawnMinX, spawnMaxX);
            float randomY = Random.Range(spawnMinY, spawnMaxY);

            Vector3 spawnPosition = new Vector3(randomX, randomY, spawnZ);

            if (!IsTooCloseToOtherTargets(spawnPosition))
            {
                GameObject newTarget = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);
                newTarget.transform.localScale = Vector3.one * targetScale;
                return;
            }
        }

        Debug.LogWarning("Could not find a free spawn position for a new target.");
    }

    bool IsTooCloseToOtherTargets(Vector3 newPosition)
    {
        GameObject[] existingTargets = GameObject.FindGameObjectsWithTag("Target");

        foreach (GameObject target in existingTargets)
        {
            float distance = Vector3.Distance(target.transform.position, newPosition);

            if (distance < minSpawnDistance)
            {
                return true;
            }
        }

        return false;
    }

    void EndGame()
    {
        gameEnded = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameObject[] remainingTargets = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject target in remainingTargets)
        {
            Destroy(target);
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
        SceneManager.LoadScene("AimTrainer");
    }

    public void ExitToSampleScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void GoToStudy()
    {
        SceneManager.LoadScene("Laptop");
    }
}