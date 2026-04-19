using TMPro;
using UnityEngine;

public class ObjectiveUIManager : MonoBehaviour
{
    public TMP_Text objectiveTitleText;
    public TMP_Text objectiveBodyText;

    public void SetObjective(string title, string body)
    {
        if (objectiveTitleText != null)
        {
            objectiveTitleText.text = title;
        }

        if (objectiveBodyText != null)
        {
            objectiveBodyText.text = body;
        }
    }

    public void HideObjective()
    {
        gameObject.SetActive(false);
    }

    public void ShowObjective()
    {
        gameObject.SetActive(true);
    }
}