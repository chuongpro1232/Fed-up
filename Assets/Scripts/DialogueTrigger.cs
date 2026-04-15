using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public Transform speakerPoint;
    public string speakerName = "NPC";

    [TextArea(2, 5)]
    public string[] dialogueLines;

    public bool triggerOnce = true;
    private bool hasTriggered = false;

    public void TriggerDialogue()
    {
        if (hasTriggered && triggerOnce)
            return;

        if (dialogueManager == null)
        {
            dialogueManager = DialogueManager.Instance;
        }

        if (dialogueManager == null)
        {
            Debug.LogWarning("No DialogueManager found in scene.");
            return;
        }

        Transform targetPoint = speakerPoint != null ? speakerPoint : transform;
        dialogueManager.StartDialogue(targetPoint, speakerName, dialogueLines);

        if (triggerOnce)
        {
            hasTriggered = true;
        }
    }
}