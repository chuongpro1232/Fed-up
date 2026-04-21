using UnityEngine;

public class Target2DHit : MonoBehaviour
{
    private ShootingMinigameManager manager;

    public void Setup(ShootingMinigameManager gameManager)
    {
        manager = gameManager;
    }

    private void OnMouseDown()
    {
        if (manager != null)
        {
            manager.TargetHit(gameObject);
        }
    }
}
