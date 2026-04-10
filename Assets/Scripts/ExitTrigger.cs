using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    public TravelMenuUI travelMenuUI;

    private bool menuOpened = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (menuOpened) return;

        if (other.CompareTag("Player"))
        {
            menuOpened = true;

            if (travelMenuUI != null)
            {
                travelMenuUI.OpenMenu();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            menuOpened = false;
        }
    }
}