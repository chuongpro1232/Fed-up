using UnityEngine;
using TMPro;

public class StartCutsceneDialogue : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement playerMovement;
    public Transform playerTransform;
    public Transform speakerPoint;
    public Camera mainCamera;
    public ObjectiveUIManager objectiveUIManager;

    [Header("UI")]
    public RectTransform dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueBodyText;
    public TMP_Text continueText;

    [Header("Dialogue")]
    public string speakerName = "Player";

    [TextArea(2, 5)]
    public string[] dialogueLines;

    [Header("Objective After Dialogue")]
    public string objectiveTitle = "Current Objective";

    [TextArea(2, 4)]
    public string objectiveBody = "Go to the computer by the window. Choose Play or Study to proceed to the next event.";

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

        if (objectiveUIManager != null)
        {
            objectiveUIManager.HideObjective();
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

            ShowObjectivePanel();
            return;
        }

        StartDialogue();
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

        if (Input.anyKeyDown)
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

        if (objectiveUIManager != null)
        {
            objectiveUIManager.HideObjective();
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

        ShowObjectivePanel();
    }

    private void ShowObjectivePanel()
    {
        if (objectiveUIManager != null)
        {
            objectiveUIManager.ShowObjective();
            objectiveUIManager.SetObjective(objectiveTitle, objectiveBody);
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