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

    [Header("Name Input")]
    public GameObject nameChoosePanel;
    public TMP_InputField nameInputField;

    [TextArea(2, 5)]
    public string[] dialogueLines;

    [Header("End of Study Dialogue")]
    [TextArea(2, 5)]
    public string[] finalStudyLines = new string[] { "I am so fed up with studying and these distractions!" };


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
            StartCoroutine(PlayFinalDialogueThenBed());
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

        // Before fading in, let's get the player's name!
        string savedName = PlayerPrefs.GetString("PlayerName", "");
        if (string.IsNullOrEmpty(savedName) || savedName == "Player")
        {
            // Show the name panel
            if (nameChoosePanel != null)
            {
                nameChoosePanel.SetActive(true);
                
                int oldSort = 0;
                if (parentCanvas != null)
                {
                    oldSort = parentCanvas.sortingOrder;
                    parentCanvas.sortingOrder = 1000; // Put UI on top of black screen (999)
                }

                if (nameInputField != null)
                {
                    nameInputField.text = ""; // clear it
                    nameInputField.Select();
                    nameInputField.ActivateInputField();
                }

                // Wait forever until the player submits their name and closes the panel
                while (nameChoosePanel.activeSelf)
                {
                    yield return null;
                }

                if (parentCanvas != null)
                {
                    parentCanvas.sortingOrder = oldSort; // Restore sorting order
                }
            }
        }

        // Tạm dừng một tí trước khi màn hình sáng lên
        yield return new WaitForSeconds(0.5f);

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

    private System.Collections.IEnumerator PlayFinalDialogueThenBed()
    {
        // 1. Play final dialogue
        Transform speaker = playerTransform != null ? playerTransform : transform;
        
        if (DialogueManager.Instance != null && finalStudyLines != null && finalStudyLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(speaker, "Player", finalStudyLines);

            // Wait until dialogue panel is closed
            while (DialogueManager.Instance.dialoguePanel.gameObject.activeSelf)
            {
                yield return null;
            }
        }
        
        // 2. Play the Bedtime Sequence
        yield return StartCoroutine(PlayBedtimeCutscene());
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
            string pName = PlayerPrefs.GetString("PlayerName", speakerName);
            if (string.IsNullOrEmpty(pName)) pName = speakerName;
            speakerNameText.text = pName;
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

    // Called by the Submit Button on the Name Choose Panel
    public void SubmitPlayerName()
    {
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text.Trim()))
        {
            string newName = nameInputField.text.Trim();
            PlayerPrefs.SetString("PlayerName", newName);
            PlayerPrefs.Save();
            
            // Hiding the panel will automatically resume the intro cutscene Coroutine!
            if (nameChoosePanel != null)
            {
                nameChoosePanel.SetActive(false);
            }
        }
    }
}