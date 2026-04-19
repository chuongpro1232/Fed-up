using UnityEngine;
using UnityEngine.SceneManagement;

public class LaptopGameManager : MonoBehaviour
{
    public static LaptopGameManager Instance;

    [Header("Gỡ lỗi (Cheat Debug)")]
    [Tooltip("Bật cái này lên để Ép Game chạy đúng State bạn muốn test (ví dụ test State 2)")]
    public bool forceState = false;
    [Tooltip("Khúc truyện muốn test (0=Steam, 2=CSGO)")]
    public int forcedStateValue = 2;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Chạy sau AutoReset nên sẽ đè lên để test tiện lợi
        if (forceState)
        {
            CurrentState = forcedStateValue;
            Debug.LogWarning("CHEAT: Đã ép LaptopPuzzleState thành " + forcedStateValue);
        }
    }

    public int CurrentState
    {
        get { return PlayerPrefs.GetInt("LaptopPuzzleState", 0); }
        set { PlayerPrefs.SetInt("LaptopPuzzleState", value); }
    }

    // Call this if developing/debugging
    public void ResetState()
    {
        CurrentState = 0;
    }
}
