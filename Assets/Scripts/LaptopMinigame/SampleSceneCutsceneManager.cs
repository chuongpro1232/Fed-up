using UnityEngine;

public class SampleSceneCutsceneManager : MonoBehaviour
{
    [Tooltip("Check this to reset the state for testing")]
    public bool resetStateOnStart = false;

    void Start()
    {
        if (resetStateOnStart)
        {
            PlayerPrefs.SetInt("LaptopPuzzleState", 0);
            Debug.Log("Reset LaptopPuzzleState to 0.");
        }

        int state = PlayerPrefs.GetInt("LaptopPuzzleState", 0);
        
        if (state == 1)
        {
            Debug.Log("--> SAMPLE SCENE: Play Steam Cutscene here!");
            // Add timeline or Animator Trigger here later
        }
        else if (state == 4)
        {
            Debug.Log("--> SAMPLE SCENE: Returned from CS:GO. Click Table to go back to puzzle!");
            // Add any other logic when returning the second time
        }
    }
}
