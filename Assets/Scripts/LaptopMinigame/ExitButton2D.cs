using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ExitButton2D : MonoBehaviour
{
    public TrollJoingameController controller;

    void OnMouseDown()
    {
        if (controller != null)
        {
            controller.OnExitClicked();
        }
    }
}
