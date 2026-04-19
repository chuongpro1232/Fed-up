using UnityEngine;

public class SampleSceneCutsceneManager : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void AutoResetOnPlay()
    {
        // Tự động xoá trí nhớ của game mỗi lần ấn nút Play (khởi động game)
        PlayerPrefs.SetInt("LaptopPuzzleState", 0);
        Debug.Log("Game started: Automatically reset LaptopPuzzleState to 0.");
    }

    void Start()
    {
        int state = PlayerPrefs.GetInt("LaptopPuzzleState", 0);
        
        if (state == 1)
        {
            PlayDialogue(new string[] { "Mình lại mất tập trung nữa rồi..." });
        }
        else if (state == 4)
        {
            PlayDialogue(new string[] { "Tôi chán ngấy với cái việc học bài xong hiện ra những thứ này rồi!" });
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
