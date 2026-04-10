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
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Default facing direction (South / Down)
        lastMoveDirection = new Vector2(0, -1);
    }

    void Update()
    {
        if (!canMove)
        {
            movement = Vector2.zero;

            if (animator != null)
            {
                animator.SetFloat("Horizontal", lastMoveDirection.x);
                animator.SetFloat("Vertical", lastMoveDirection.y);
                animator.SetFloat("Speed", 0f);
            }

            return;
        }

        // 1. Get Input (WASD or Arrow Keys)
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Optional: normalize input so diagonal is not stronger
        movement = movement.normalized;

        // 2. Remember the last direction input so idle animation knows which way to face
        if (movement.x != 0 || movement.y != 0)
        {
            lastMoveDirection = movement;
        }

        // 3. Update Animator parameters
        if (animator != null)
        {
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical", lastMoveDirection.y);
            animator.SetFloat("Speed", movement.sqrMagnitude);
        }
    }

    void FixedUpdate()
    {
        if (!canMove)
            return;

        // 4. Move the player using Physics
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void SetCanMove(bool value)
    {
        canMove = value;
        movement = Vector2.zero;

        if (animator != null && !canMove)
        {
            animator.SetFloat("Speed", 0f);
        }
    }
}