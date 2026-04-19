using UnityEngine;

public class DayManager : MonoBehaviour
{
    [Header("Day References")]
    [Tooltip("The TravelMenuManager object which has the BoxCollider2D trigger for the Door")]
    public GameObject doorObject;
    
    [Tooltip("The ComputerTrigger empty object which acts as the Table interaction")]
    public GameObject tableObject;

    void Start()
    {
        SetupDay();
    }

    public void SetupDay()
    {
        // 1 for Day 1. 2 for Day 2. Default to Day 1.
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        
        if (currentDay == 1)
        {
            // On Day 1: Player must study, cannot leave room.
            if (doorObject != null)
            {
                Collider2D doorCollider = doorObject.GetComponent<Collider2D>();
                if (doorCollider != null)
                {
                    doorCollider.isTrigger = false; // Normal solid wall collider
                }
                
                // Keep the trigger script but wait, if it's not a trigger it won't fire OnTriggerEnter.
            }
            
            if (tableObject != null)
            {
                // Ensure table is enabled and acts as a trigger
                tableObject.SetActive(true);
            }
        }
        else if (currentDay == 2)
        {
            // On Day 2: Student already studied last night. Door is open to go to school.
            if (doorObject != null)
            {
                Collider2D doorCollider = doorObject.GetComponent<Collider2D>();
                if (doorCollider != null)
                {
                    doorCollider.isTrigger = true; // Make it a trigger again to leave room
                }
            }
            
            if (tableObject != null)
            {
                // Disable the trigger GameObject completely so they can't study again
                tableObject.SetActive(false);
            }
        }
    }
}
