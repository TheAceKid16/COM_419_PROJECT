using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    [Header("Movement Settings")]
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    [Header("Physics")]
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    Vector3 velocity;
    bool isGrounded;

    [Header("Double Jump")]
    public int maxJumps = 2;       // Set to 2 for double jump
    private int jumpsRemaining;    // Tracks how many jumps we have left

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleCursor();

        // 1. Ground Check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpsRemaining = maxJumps; // Reset jumps when we touch the ground
        }

        // 2. Movement Input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }

        // 3. Jumping Logic (Double Jump fixed)
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                // Normal First Jump
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpsRemaining--;
                Debug.Log("First Jump! Jumps left: " + jumpsRemaining);
            }
            else if (jumpsRemaining > 0)
            {
                // Double Jump (Air Jump)
                // Reset velocity.y so falling fast doesn't "eat" your jump height
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpsRemaining--;
                Debug.Log("Double Jump! Jumps left: " + jumpsRemaining);
            }
        }

        // 4. Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleCursor()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}