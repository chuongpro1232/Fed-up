using UnityEngine;
using TMPro;

public class StartCutsceneDialogue : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public Transform playerTransform;
    public Transform speakerPoint;
    public Camera mainCamera;

    [Header("Cutscene: Wake Up (Chỉ chạy ở State 0)")]
    public Transform bedWaypoint;
    public Transform standWaypoint;
    public float walkSpeed = 2f;

    [Header("UI")]
    public RectTransform dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueBodyText;
    public TMP_Text continueText;

    [Header("Dialogue")]
    public string speakerName = "Player";

    [TextArea(2, 5)]
    public string[] dialogueLines;


    [Header("Position")]
    public Vector2 screenOffset = new Vector2(0f, 60f);

    private int currentLineIndex = 0;
    private bool cutsceneActive = false;
    private float inputDelay = 0.2f;

    private Canvas parentCanvas;
    private RectTransform canvasRect;

    private void Awake()
    {
        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(false);
            parentCanvas = dialoguePanel.GetComponentInParent<Canvas>();

            if (parentCanvas != null)
            {
                canvasRect = parentCanvas.GetComponent<RectTransform>();
            }
        }

    }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (SceneReturnData.hasSavedSampleScenePosition && playerTransform != null)
        {
            playerTransform.position = SceneReturnData.sampleScenePlayerPosition;
        }

        if (SceneReturnData.hasFinishedStudy)
        {
            SceneReturnData.hasFinishedStudy = false;
            SceneReturnData.skipSampleSceneIntro = false;
            StartCoroutine(PlayBedtimeCutscene());
            return;
        }


        // If returning from another scene, skip intro and show objective immediately
        if (SceneReturnData.skipSampleSceneIntro)
        {
            SceneReturnData.skipSampleSceneIntro = false;

            if (dialoguePanel != null)
            {
                dialoguePanel.gameObject.SetActive(false);
            }

            if (playerMovement != null)
            {
                playerMovement.SetCanMove(true);
            }

            return;
        }

        int state = PlayerPrefs.GetInt("LaptopPuzzleState", 0);
        if (state == 0)
        {
            StartCoroutine(PlayIntroCutscene());
        }
        else
        {
            StartDialogue();
        }
    }

    private System.Collections.IEnumerator PlayIntroCutscene()
    {
        cutsceneActive = true;
        
        if (playerMovement != null) playerMovement.SetCanMove(false);

        // Tạo màn hình đen tàng hình bọc toàn màn hình
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas c = canvasObj.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 999;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();

        // Tạm thời tắt va chạm của người chơi để không bị văng ra khỏi giường
        Collider2D playerCollider = null;
        if (playerTransform != null)
        {
            playerCollider = playerTransform.GetComponent<Collider2D>();
            if (playerCollider != null) playerCollider.enabled = false;
        }

        GameObject imageObj = new GameObject("BlackImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image img = imageObj.AddComponent<UnityEngine.UI.Image>();
        img.color = Color.black;
        
        RectTransform rt = img.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        // Dịch chuyển người chơi lên giường (nằm)
        if (playerTransform != null && bedWaypoint != null)
        {
            playerTransform.position = bedWaypoint.position;
        }

        // Hiện màu đen, đợi một tí cho ổn định
        yield return new WaitForSeconds(1f);

        // Bắt đầu sáng dần lên trong 2 giây
        float fadeTime = 2f;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            img.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        Destroy(canvasObj); // Xóa Canvas sau khi màn hình đã sáng bừng

        yield return new WaitForSeconds(0.5f);

        // Bắt đầu tự động đi bộ ra giữa phòng
        if (playerTransform != null && standWaypoint != null)
        {
            Animator anim = playerTransform.GetComponent<Animator>();
            
            while (Vector3.Distance(playerTransform.position, standWaypoint.position) > 0.05f)
            {
                Vector3 dir = (standWaypoint.position - playerTransform.position).normalized;
                playerTransform.position = Vector3.MoveTowards(playerTransform.position, standWaypoint.position, walkSpeed * Time.deltaTime);

                if (anim != null)
                {
                    anim.SetFloat("Horizontal", dir.x);
                    anim.SetFloat("Vertical", dir.y);
                    anim.SetFloat("Speed", 1f); // Giả lập đang đi bộ
                }
                yield return null;
            }

            // Dừng chân, đứng quay mặt xuống dưới (South)
            if (anim != null)
            {
                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", -1);
                anim.SetFloat("Speed", 0f);
            }
        }
        
        yield return new WaitForSeconds(0.5f);

        // Lúc này người chơi đã đi xog ra giữa phòng, bật lại va chạm
        if (playerCollider != null) playerCollider.enabled = true;

        // Bắt đầu hiện hội thoại
        StartDialogue();
    }

    private System.Collections.IEnumerator PlayBedtimeCutscene()
    {
        cutsceneActive = true;
        if (playerMovement != null) playerMovement.SetCanMove(false);

        Collider2D playerCollider = null;
        if (playerTransform != null)
        {
            playerCollider = playerTransform.GetComponent<Collider2D>();
            if (playerCollider != null) playerCollider.enabled = false;
        }

        // Walk back to bed
        if (playerTransform != null && bedWaypoint != null)
        {
            Animator anim = playerTransform.GetComponent<Animator>();
            
            while (Vector3.Distance(playerTransform.position, bedWaypoint.position) > 0.05f)
            {
                Vector3 dir = (bedWaypoint.position - playerTransform.position).normalized;
                playerTransform.position = Vector3.MoveTowards(playerTransform.position, bedWaypoint.position, walkSpeed * Time.deltaTime);

                if (anim != null)
                {
                    anim.SetFloat("Horizontal", dir.x);
                    anim.SetFloat("Vertical", dir.y);
                    anim.SetFloat("Speed", 1f); 
                }
                yield return null;
            }

            if (anim != null)
            {
                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", 1);
                anim.SetFloat("Speed", 0f);
            }
        }

        // Fade out
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas c = canvasObj.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 999;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();

        GameObject imageObj = new GameObject("BlackImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image img = imageObj.AddComponent<UnityEngine.UI.Image>();
        img.color = new Color(0, 0, 0, 0);
        
        RectTransform rt = img.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        float fadeTime = 2f;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeTime);
            img.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        // Next Day
        PlayerPrefs.SetInt("CurrentDay", 2);
        yield return new WaitForSeconds(1f);
        
        DayManager dm = Object.FindObjectOfType<DayManager>();
        if (dm != null) dm.SetupDay();

        if (playerTransform != null && standWaypoint != null)
        {
            // Morning! Walk to standWaypoint again
            
            elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                img.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            Destroy(canvasObj);

            Animator anim = playerTransform.GetComponent<Animator>();
            while (Vector3.Distance(playerTransform.position, standWaypoint.position) > 0.05f)
            {
                Vector3 dir = (standWaypoint.position - playerTransform.position).normalized;
                playerTransform.position = Vector3.MoveTowards(playerTransform.position, standWaypoint.position, walkSpeed * Time.deltaTime);

                if (anim != null)
                {
                    anim.SetFloat("Horizontal", dir.x);
                    anim.SetFloat("Vertical", dir.y);
                    anim.SetFloat("Speed", 1f); 
                }
                yield return null;
            }

            if (anim != null)
            {
                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", -1);
                anim.SetFloat("Speed", 0f);
            }
        }
        else
        {
            elapsed = 0f;
            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
                img.color = new Color(0, 0, 0, alpha);
                yield return null;
            }
            Destroy(canvasObj);
        }

        if (playerCollider != null) playerCollider.enabled = true;
        if (playerMovement != null) playerMovement.SetCanMove(true);
        cutsceneActive = false;
    }

    private void Update()
    {
        if (!cutsceneActive)
            return;

        UpdateBubblePosition();

        if (inputDelay > 0f)
        {
            inputDelay -= Time.deltaTime;
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }

    public void StartDialogue()
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            EndDialogue();
            return;
        }

        cutsceneActive = true;
        currentLineIndex = 0;
        inputDelay = 0.2f;

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(true);
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = speakerName;
        }

        ShowCurrentLine();
        UpdateBubblePosition();
    }

    private void ShowCurrentLine()
    {
        if (dialogueBodyText != null)
        {
            dialogueBodyText.text = dialogueLines[currentLineIndex];
        }

        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
        }
    }

    private void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        cutsceneActive = false;

        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(false);
        }

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(true);
        }
    }

    private void UpdateBubblePosition()
    {
        if (dialoguePanel == null || speakerPoint == null || mainCamera == null || canvasRect == null)
            return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(speakerPoint.position);

        if (screenPos.z <= 0f)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,
            out Vector2 localPoint
        );

        dialoguePanel.anchoredPosition = localPoint + screenOffset;
    }
}