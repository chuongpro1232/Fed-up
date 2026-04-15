using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TrollJoingameController : MonoBehaviour
{
    public GameObject joingameParent; // The overall group for Joingame
    public GameObject csgoBackground; // The CSGO image BG
    public Transform exitButton;

    private int cornerIndex = 0;
    private Vector3[] corners;

    void Start()
    {
        if (joingameParent != null) joingameParent.SetActive(false);
        if (csgoBackground != null) csgoBackground.SetActive(false);

        // Calculate 4 corners within orthographic bounds
        Camera cam = Camera.main;
        if (cam != null && exitButton != null)
        {
            float vertExtent = cam.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;

            Vector3 camPos = cam.transform.position;

            corners = new Vector3[5];
            corners[0] = new Vector3(camPos.x - horzExtent + 2f, camPos.y + vertExtent - 2f, exitButton.position.z); // Top-Left
            corners[1] = new Vector3(camPos.x + horzExtent - 2f, camPos.y + vertExtent - 2f, exitButton.position.z); // Top-Right
            corners[2] = new Vector3(camPos.x + horzExtent - 2f, camPos.y - vertExtent + 2f, exitButton.position.z); // Bot-Right
            corners[3] = new Vector3(camPos.x - horzExtent + 2f, camPos.y - vertExtent + 2f, exitButton.position.z); // Bot-Left
            corners[4] = new Vector3(camPos.x, camPos.y, exitButton.position.z); // Center
        }

        if (LaptopGameManager.Instance.CurrentState == 2)
        {
            // Dự phòng nếu load lại scene
            StartTrollTimer();
        }
    }

    public void StartTrollTimer()
    {
        StartCoroutine(WaitAndShowJoingame());
    }

    IEnumerator WaitAndShowJoingame()
    {
        yield return new WaitForSeconds(20f);
        if (joingameParent != null)
        {
            joingameParent.SetActive(true);
            LaptopGameManager.Instance.CurrentState = 3;
        }
    }

    public void OnExitClicked()
    {
        if (cornerIndex < corners.Length)
        {
            exitButton.position = corners[cornerIndex];
            cornerIndex++;
        }
        else
        {
            // Last click was in the center
            if (joingameParent != null) joingameParent.SetActive(false);
            if (csgoBackground != null) csgoBackground.SetActive(true);
            
            StartCoroutine(ReturnToSampleScene());
        }
    }

    IEnumerator ReturnToSampleScene()
    {
        yield return new WaitForSeconds(3f); // Display CS:GO for 3 seconds
        LaptopGameManager.Instance.CurrentState = 4;
        SceneManager.LoadScene("SampleScene");
    }
}
