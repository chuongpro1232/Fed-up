using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private Vector2 lastMoveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Default facing direction (e.g., South/Down)
        lastMoveDirection = new Vector2(0, -1);
    }

    void Update()
    {
        // 1. Get Input (WASD or Arrow Keys)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // 2. Remember the last direction input so the idle animation knows which way to face
        if (movement.x != 0 || movement.y != 0)
        {
            lastMoveDirection = movement;
        }

        // 3. Update Animator parameters
        if (animator != null)
        {
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical", lastMoveDirection.y);
            // sqrMagnitude is more performant than magnitude and works well for checking if we're moving (Speed > 0)
            animator.SetFloat("Speed", movement.sqrMagnitude); 
        }
    }

    void FixedUpdate()
    {
        // 4. Move the player using Physics
        // We normalize the movement vector so diagonal movement isn't faster
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}
