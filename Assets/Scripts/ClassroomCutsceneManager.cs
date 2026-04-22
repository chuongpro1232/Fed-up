using UnityEngine;

public class ClassroomCutsceneManager : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public Transform speakerPoint; // Who the bubble floats above (e.g. Player's TalkPoint or Teacher)
    public string speakerName = "Teacher";
    
    [TextArea(2, 5)]
    public string[] dialogueLines;

    [Header("Cutscene Walk Settings")]
    public Transform playerTransform;
    public PlayerMovement playerMovement;
    public Transform walkWaypoint; // The point you want the character to walk to
    public float walkSpeed = 2f;

    [Header("Teacher Sequence")]
    public Transform teacherTransform;
    public CameraFollow cameraFollow;
    public float teacherZoomLevel = 4f;
    public float zoomSpeed = 2f;
    public string teacherSpeakerName = "Teacher";
    [TextArea(2, 5)]
    public string[] teacherDialogueLines;

    [Header("Post-Minigame Sequence")]
    public GameObject blackboardTrigger; // The trigger to disable so we don't loop
    
    [TextArea(2, 5)]
    public string[] postMinigameTeacherLines;
    public Transform exitClassWaypoint; // Where the player walks to leave
    public string nextSceneName = "Net";

    private void Start()
    {
        if (SceneReturnData.skipClassroomIntro)
        {
            SceneReturnData.skipClassroomIntro = false;
            
            // Re-position the player to exactly where they were!
            if (SceneReturnData.hasSavedClassroomPosition && playerTransform != null)
            {
                playerTransform.position = SceneReturnData.classroomPlayerPosition;
            }

            // Disable the trigger so we don't get stuck in an infinite loop!
            if (blackboardTrigger != null) blackboardTrigger.SetActive(false);

            // Automatically walk them away from the blackboard
            StartCoroutine(WalkBackFromBlackboard());
            return;
        }

        // Give the scene a tiny fraction of a second to load the DialogueManager Instance first
        Invoke("StartSequence", 0.1f);
    }

    private void StartSequence()
    {
        StartCoroutine(PlayCutsceneSequence());
    }

    private System.Collections.IEnumerator PlayCutsceneSequence()
    {
        // 1. Play Opening Dialogue
        if (DialogueManager.Instance != null && dialogueLines != null && dialogueLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(speakerPoint, speakerName, dialogueLines);

            // Wait until dialogue is closed
            while (DialogueManager.Instance.dialoguePanel.gameObject.activeSelf)
            {
                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("Dialogue Manager is missing or no lines were written!");
        }

        // 2. Automatically Walk Player to Waypoint
        if (playerTransform != null && walkWaypoint != null)
        {
            if (playerMovement != null) playerMovement.SetCanMove(false);
            
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

            // Stand still when arrived
            if (anim != null)
            {
                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", 1); // 1 = facing up (towards teacher)
                anim.SetFloat("Speed", 0f);
            }
        }

        // 3. Pan and Zoom Camera to Teacher
        if (cameraFollow != null && teacherTransform != null)
        {
            cameraFollow.target = teacherTransform;
            cameraFollow.SetZoom(teacherZoomLevel, zoomSpeed);
            
            // Wait for 1.5 seconds for the camera to smoothly pan and zoom over
            yield return new WaitForSeconds(1.5f);
        }

        // 4. Play Teacher Dialogue
        if (DialogueManager.Instance != null && teacherDialogueLines != null && teacherDialogueLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(teacherTransform, teacherSpeakerName, teacherDialogueLines);

            // Wait until dialogue is closed
            while (DialogueManager.Instance.dialoguePanel.gameObject.activeSelf)
            {
                yield return null;
            }
        }

        // 5. End of Sequence - Give everything back to the player
        if (cameraFollow != null && playerTransform != null)
        {
            cameraFollow.target = playerTransform;
            cameraFollow.ResetZoom(zoomSpeed);
        }
            
        // Release player to move again
        if (playerMovement != null) playerMovement.SetCanMove(true);
    }

    private System.Collections.IEnumerator WalkBackFromBlackboard()
    {
        // Lock player from moving
        if (playerMovement != null) playerMovement.SetCanMove(false);
        
        // Wait a small moment before walking
        yield return new WaitForSeconds(0.5f);

        // Walk back to TriggerPoint (walkWaypoint)
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

            // Stand still when arrived and face the teacher (up)
            if (anim != null)
            {
                anim.SetFloat("Horizontal", 0);
                anim.SetFloat("Vertical", 1); 
                anim.SetFloat("Speed", 0f);
            }
        }

        // --- NEW EXTENDED SEQUENCE ---

        // 1. Zoom Camera to Teacher
        if (cameraFollow != null && teacherTransform != null)
        {
            cameraFollow.target = teacherTransform;
            cameraFollow.SetZoom(teacherZoomLevel, zoomSpeed);
            yield return new WaitForSeconds(1.5f);
        }

        // 2. Play Post-Minigame Teacher Dialogue
        if (DialogueManager.Instance != null && postMinigameTeacherLines != null && postMinigameTeacherLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(teacherTransform, teacherSpeakerName, postMinigameTeacherLines);

            // Wait until dialogue is closed
            while (DialogueManager.Instance.dialoguePanel.gameObject.activeSelf)
            {
                yield return null;
            }
        }

        // 3. Zoom Camera back to Player
        if (cameraFollow != null && playerTransform != null)
        {
            cameraFollow.target = playerTransform;
            cameraFollow.ResetZoom(zoomSpeed);
            yield return new WaitForSeconds(1.0f);
        }

        // 4. Player walks out of class
        if (playerTransform != null && exitClassWaypoint != null)
        {
            Animator anim = playerTransform.GetComponent<Animator>();
            
            while (Vector3.Distance(playerTransform.position, exitClassWaypoint.position) > 0.05f)
            {
                Vector3 dir = (exitClassWaypoint.position - playerTransform.position).normalized;
                playerTransform.position = Vector3.MoveTowards(playerTransform.position, exitClassWaypoint.position, walkSpeed * Time.deltaTime);

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

        // 5. Fade out to black
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

        // 6. Load Next Scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
    }
}
