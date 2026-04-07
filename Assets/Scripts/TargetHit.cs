using UnityEngine;

public class TargetHit : MonoBehaviour
{
    public AimTrainerManager manager;

    public void HitTarget()
    {
        if (manager != null)
        {
            manager.TargetHit(gameObject);
        }
    }
}