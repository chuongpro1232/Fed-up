using UnityEngine;

public class SampleSceneCutsceneManager : MonoBehaviour
{
    [Header("Stage Dialogues")]
    [TextArea(2, 4)]
    public string[] stage1EndLines = new string[] { "I am losing focus. I need to focus on my study and delete CS2." };

    [TextArea(2, 4)]
    public string[] stage2EndLines = new string[] { "What on earth is wrong with this machine?!" };

    [Header("Aim Trainer Dialogues")]
    [TextArea(2, 4)]
    public string[] betterScoreLines = new string[] { "This is much better! I got {0} points, which beats my old {1}." };
    [TextArea(2, 4)]
    public string[] worseScoreLines = new string[] { "I did slightly worse than last time... I only got {0}." };
    [TextArea(2, 4)]
    public string[] tieScoreLines = new string[] { "Not bad, I got {0} points, just like before." };


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoResetOnPlay()
    {
        // Tự động xoá trí nhớ của game mỗi lần ấn nút Play (khởi động game)
        PlayerPrefs.SetInt("LaptopPuzzleState", 0);
        PlayerPrefs.SetInt("CurrentDay", 1); // <--- ADDED THIS RESET
        Debug.Log("Game started: Automatically reset LaptopPuzzleState to 0 and CurrentDay to 1.");
    }

    void Start()
    {
        // First check if coming exactly from Aim Trainer Exit
        if (SceneReturnData.justFinishedAimTrainer)
        {
            SceneReturnData.justFinishedAimTrainer = false;
            
            int score = SceneReturnData.latestAimTrainerScore;
            int prevScore = SceneReturnData.previousAimTrainerScore;

            string[] chosenLines;
            if (score > prevScore || prevScore == 0) chosenLines = betterScoreLines;
            else if (score < prevScore) chosenLines = worseScoreLines;
            else chosenLines = tieScoreLines;

            // Replace {0} with current score, and {1} with previous score
            string[] formattedLines = new string[chosenLines.Length];
            for (int i = 0; i < chosenLines.Length; i++)
            {
                formattedLines[i] = chosenLines[i].Replace("{0}", score.ToString()).Replace("{1}", prevScore.ToString());
            }

            PlayDialogue(formattedLines);
            return; // Don't process laptop puzzle states
        }

        int state = PlayerPrefs.GetInt("LaptopPuzzleState", 0);
        
        if (state == 2)
        {
            PlayDialogue(stage1EndLines);
        }
        else if (state == 3)
        {
            PlayDialogue(stage2EndLines);
        }
    }

    void PlayDialogue(string[] lines)
    {
        if (DialogueManager.Instance != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Transform speaker = player != null ? player.transform : transform;
            DialogueManager.Instance.StartDialogue(speaker, "Player", lines);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy DialogueManager để diễn thoại: " + lines[0]);
        }
    }
}
