using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SteamIconEvent : MonoBehaviour
{
    public GameObject steamBackground;
    public float moveSpeed = 3f;
    [Tooltip("Thời gian chờ trước khi Steam xuất hiện và bay (Ví dụ: 3 giây)")]
    public float startDelay = 3f; 
    
    [Tooltip("Thời gian chờ để hiển thị nền Steam trước khi qua SampleScene")]
    public float backgroundShowTime = 7f;

    [Header("Giới hạn khu vực bay (Movement Bounds)")]
    [Tooltip("Tọa độ X (Ngang) giới hạn bên trái màn hình Laptop")]
    public float minX = -3.8f;
    [Tooltip("Tọa độ X (Ngang) giới hạn bên phải màn hình Laptop")]
    public float maxX = 4.0f;
    [Tooltip("Tọa độ Y (Dọc) giới hạn bên dưới màn hình Laptop")]
    public float minY = -0.8f;
    [Tooltip("Tọa độ Y (Dọc) giới hạn bên trên màn hình Laptop")]
    public float maxY = 2.6f;

    private Vector3 targetPos;
    private bool isClicked = false;
    private bool isMoving = false;

    void Start()
    {
        if (LaptopGameManager.Instance != null && LaptopGameManager.Instance.CurrentState != 0)
        {
            if (steamBackground != null) steamBackground.SetActive(false);
            gameObject.SetActive(false); // Only show at the beginning
            return;
        }

        if (steamBackground != null) steamBackground.SetActive(false);
        
        // Ẩn lúc ban đầu
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        StartCoroutine(WaitToStartMoving());
    }

    IEnumerator WaitToStartMoving()
    {
        yield return new WaitForSeconds(startDelay);
        
        // Hiện lên và bắt đầu di chuyển
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = true;

        GetNewRandomPosition();
        isMoving = true;
    }

    void Update()
    {
        if (!isClicked && isMoving && gameObject.activeSelf)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                GetNewRandomPosition();
            }
        }
    }

    void GetNewRandomPosition()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);
        targetPos = new Vector3(randomX, randomY, transform.position.z);
    }

    [Header("Khi Click trúng")]
    [Tooltip("Danh sách các đồ vật sẽ ẩn đi khi hiện nền Steam (ví dụ: cái bàn cân, ranking, các cục nguyên tử)")]
    public GameObject[] objectsToHide;

    void OnMouseDown()
    {
        if (isClicked || !isMoving) return;
        isClicked = true;

        if (steamBackground != null) steamBackground.SetActive(true);

        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;
        
        // Ẩn tất cả các đối tượng đồ họa của việc giải đố
        if (objectsToHide != null)
        {
            foreach (var obj in objectsToHide)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
        
        StartCoroutine(HandleSteamEvent());
    }

    IEnumerator HandleSteamEvent()
    {
        yield return new WaitForSeconds(5f); // Wait for 5s
        
        if (LaptopGameManager.Instance != null) LaptopGameManager.Instance.CurrentState = 1; // Kích hoạt trạng thái chuyển qua SampleScene
        
        SceneManager.LoadScene("SampleScene");
    }
}
