using UnityEngine;
using UnityEngine.SceneManagement;

public class LaptopGameManager : MonoBehaviour
{
    public static LaptopGameManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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
