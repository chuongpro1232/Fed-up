using UnityEngine;
using System.Collections;

public class NetCutsceneManager : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public Transform speakerPoint; // Usually the Player's TalkPoint
    public string speakerName = "Player";
    
    [TextArea(2, 5)]
    public string[] dialogueLines;

    [Header("Walk Settings")]
    public Transform playerTransform;
    public PlayerMovement playerMovement;
    public Transform walkWaypoint; // Drag the TriggerPoint here
    public float walkSpeed = 2f;

    private void Start()
    {
        // Give DialogueManager a tiny fraction of a second to initialize
        Invoke("StartSequence", 0.1f);
    }

    private void StartSequence()
    {
        StartCoroutine(PlayCutsceneSequence());
    }

    private IEnumerator PlayCutsceneSequence()
    {
        // Lock player movement during cutscene
        if (playerMovement != null) playerMovement.SetCanMove(false);

        // 1. Create a Black Screen covering the UI
        GameObject canvasObj = new GameObject("FadeCanvas");
        Canvas c = canvasObj.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 999;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();

        GameObject imageObj = new GameObject("BlackImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        UnityEngine.UI.Image img = imageObj.AddComponent<UnityEngine.UI.Image>();
        img.color = Color.black;
        
        RectTransform rt = img.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;

        // Wait a tiny bit before fading
        yield return new WaitForSeconds(0.5f);

        // 2. Fade from black to transparent (Game starts!)
        float fadeTime = 2f;
        float elapsed = 0f;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            img.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        Destroy(canvasObj); // Screen is bright now, remove the black image completely!

        yield return new WaitForSeconds(0.2f);

        // 3. Play Dialogue
        if (DialogueManager.Instance != null && dialogueLines != null && dialogueLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(speakerPoint, speakerName, dialogueLines);

            // Wait until the player finishes reading the dialogue
            while (DialogueManager.Instance.dialoguePanel.gameObject.activeSelf)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("DialogueManager missing or no lines set.");
        }

        // 4. Automatically walk to the Trigger Point!
        if (playerTransform != null && walkWaypoint != null)
        {
            Animator anim = playerTransform.GetComponent<Animator>();
            
            while (Vector3.Distance(playerTransform.position, walkWaypoint.position) > 0.05f)
            {
                Vector3 dir = (walkWaypoint.position - playerTransform.position).normalized;
                playerTransform.position = Vector3.MoveTowards(playerTransform.position, walkWaypoint.position, walkSpeed * Time.deltaTime);

                if (anim != null)
                {
                    anim.SetFloat("Horizontal", dir.x);
                    anim.SetFloat("Vertical", dir.y);
                    anim.SetFloat("Speed", 1f); 
                }
                yield return null;
            }

            // Stop walking animation when they arrive
            if (anim != null)
            {
                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", 1); 
                anim.SetFloat("Speed", 0f);
            }
        }

        // 5. End Cutscene
        if (playerMovement != null) playerMovement.SetCanMove(true);
    }
}
