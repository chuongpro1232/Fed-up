using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public RectTransform dialoguePanel;
    public TMP_Text speakerNameText;
    public TMP_Text dialogueBodyText;
    public TMP_Text continueText;

    [Header("References")]
    public PlayerMovement playerMovement;
    public Camera mainCamera;

    [Header("Typing Settings")]
    public float typeSpeed = 0.03f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip typeBlipClip;
    public int playBlipEveryNLetters = 2;
    public float blipVolume = 0.5f;

    [Header("Bubble Position")]
    public Vector3 screenOffset = new Vector3(0f, 60f, 0f);

    private Transform currentSpeaker;
    private string[] currentLines;
    private int currentLineIndex;

    private string fullCurrentLine;
    private Coroutine typingCoroutine;

    private bool dialogueActive = false;
    private bool lineFullyShown = false;
    private float ignoreInputUntil = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (continueText != null)
        {
            continueText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!dialogueActive)
            return;

        UpdateBubblePosition();

        if (Time.unscaledTime < ignoreInputUntil)
            return;

        if (Input.anyKeyDown)
        {
            if (!lineFullyShown)
            {
                ShowWholeLineImmediately();
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(Transform speaker, string speakerName, string[] lines)
    {
        if (lines == null || lines.Length == 0)
            return;

        currentSpeaker = speaker;
        currentLines = lines;
        currentLineIndex = 0;
        dialogueActive = true;

        if (dialoguePanel != null)
        {
            dialoguePanel.gameObject.SetActive(true);
        }

        if (speakerNameText != null)
        {
            speakerNameText.text = speakerName;
        }

        if (playerMovement != null)
        {
            playerMovement.SetCanMove(false);
        }

        ignoreInputUntil = Time.unscaledTime + 0.15f;
        ShowCurrentLine();
        UpdateBubblePosition();
    }

    private void ShowCurrentLine()
    {
        fullCurrentLine = currentLines[currentLineIndex];

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        typingCoroutine = StartCoroutine(TypeLine(fullCurrentLine));
    }

    private IEnumerator TypeLine(string line)
    {
        lineFullyShown = false;

        if (dialogueBodyText != null)
        {
            dialogueBodyText.text = "";
        }

        if (continueText != null)
        {
            continueText.gameObject.SetActive(false);
        }

        for (int i = 0; i < line.Length; i++)
        {
            if (dialogueBodyText != null)
            {
                dialogueBodyText.text += line[i];
            }

            if (typeBlipClip != null && audioSource != null)
            {
                if (!char.IsWhiteSpace(line[i]) && i % playBlipEveryNLetters == 0)
                {
                    audioSource.pitch = Random.Range(0.95f, 1.05f);
                    audioSource.PlayOneShot(typeBlipClip, blipVolume);
                }
            }

            yield return new WaitForSeconds(typeSpeed);
        }

        lineFullyShown = true;
        typingCoroutine = null;

        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
        }
    }

    private void ShowWholeLineImmediately()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        if (dialogueBodyText != null)
        {
            dialogueBodyText.text = fullCurrentLine;
        }

        lineFullyShown = true;

        if (continueText != null)
        {
            continueText.gameObject.SetActive(true);
        }

        ignoreInputUntil = Time.unscaledTime + 0.05f;
    }

    private void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentLines.Length)
        {
            EndDialogue();
            return;
        }

        ignoreInputUntil = Time.unscaledTime + 0.05f;
        ShowCurrentLine();
    }

    private void EndDialogue()
    {
        dialogueActive = false;
        currentSpeaker = null;
        currentLines = null;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

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
        if (dialoguePanel == null || currentSpeaker == null || mainCamera == null)
            return;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(currentSpeaker.position);

        if (screenPosition.z > 0)
        {
            dialoguePanel.position = screenPosition + screenOffset;
        }
    }
}