using UnityEngine;

public class ClassroomCutsceneManager : MonoBehaviour
{
    [Header("Cutscene Settings")]
    public Transform speakerPoint; // Who the bubble floats above (e.g. Player's TalkPoint or Teacher)
    public string speakerName = "Teacher";
    
    [TextArea(2, 5)]
    public string[] dialogueLines;

    private void Start()
    {
        // Give the scene a tiny fraction of a second to load the DialogueManager Instance first
        Invoke("PlayCutscene", 0.1f);
    }

    private void PlayCutscene()
    {
        if (DialogueManager.Instance != null && dialogueLines != null && dialogueLines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(speakerPoint, speakerName, dialogueLines);
        }
        else
        {
            Debug.LogWarning("Dialogue Manager is missing or no lines were written!");
        }
    }
}
