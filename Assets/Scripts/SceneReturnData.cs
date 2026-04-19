using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneReturnData
{
    public static Vector3 sampleScenePlayerPosition;
    public static bool hasSavedSampleScenePosition = false;
    public static bool skipSampleSceneIntro = false;
    
    // New flag to handle bedtime cutscene
    public static bool hasFinishedStudy = false;

    public static void FinishStudyAndReturn()
    {
        hasFinishedStudy = true;
        // Make sure we skip the normal intro, because we play the bedtime one instead.
        skipSampleSceneIntro = true;
        SceneManager.LoadScene("SampleScene");
    }
}