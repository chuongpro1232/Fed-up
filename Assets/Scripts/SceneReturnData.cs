using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneReturnData
{
    public static Vector3 sampleScenePlayerPosition;
    public static bool hasSavedSampleScenePosition = false;
    public static bool skipSampleSceneIntro = false;
    
    // New flag to handle bedtime cutscene
    public static bool hasFinishedStudy = false;

    // Aim Trainer tracking
    public static bool justFinishedAimTrainer = false;
    public static int latestAimTrainerScore = 0;
    public static int previousAimTrainerScore = 0;

    // Classroom tracking
    public static bool hasSavedClassroomPosition = false;
    public static Vector3 classroomPlayerPosition;
    public static bool skipClassroomIntro = false;

    public static void FinishStudyAndReturn()
    {
        hasFinishedStudy = true;
        // Make sure we skip the normal intro, because we play the bedtime one instead.
        skipSampleSceneIntro = true;
        SceneManager.LoadScene("SampleScene");
    }
}