using UnityEngine;

public class Wall_Crawling : MonoBehaviour
{
    [Header("Detection")]
    public LayerMask whatIsCrawlable;
    public float wallCheckDistance = 1.0f;

    [Header("Movement")]
    public float crawlSpeed = 5f;
    public float rotationSpeed = 10f;

    private Rigidbody rb;
    private PlayerMovement pm; // Updated to match your script name
    private CharacterController charController;
    private RaycastHit wallHit;
    private bool isCrawling;

    private void Start()
    {
        // Get references
        pm = GetComponent<PlayerMovement>();
        charController = GetComponent<CharacterController>();

        // Crawling works best if we toggle the CharacterController 
        // because it fights with manual rotations.
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();

        if (isCrawling)
        {
            CrawlMovement();
        }
    }

    private void CheckForWall()
    {
        // Look forward from the player
        // We use transform.forward because in 3rd person, you walk INTO the wall to climb it
        Debug.DrawRay(transform.position, transform.forward * wallCheckDistance, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out wallHit, wallCheckDistance, whatIsCrawlable))
        {
            // Optional: You can add a tag check here too
            // if(wallHit.collider.CompareTag("Crawlable")) ...
        }
    }

    private void StateMachine()
    {
        float vInput = Input.GetAxisRaw("Vertical");

        // START CRAWLING: If hitting a crawlable wall and moving forward
        if (wallHit.collider != null && vInput > 0 && !isCrawling)
        {
            StartCrawl();
        }

        // STOP CRAWLING: If we lose the wall or jump off
        if (wallHit.collider == null && isCrawling)
        {
            StopCrawl();
        }

        if (Input.GetButtonDown("Jump") && isCrawling)
        {
            StopCrawl();
        }
    }

    private void StartCrawl()
    {
        isCrawling = true;
        pm.enabled = false; // Turn off normal movement so it doesn't fight us

        // If your PlayerMovement uses a Rigidbody, we'd handle it here. 
        // Since you use a CharacterController, we'll move manually.
    }

    private void StopCrawl()
    {
        isCrawling = false;
        pm.enabled = true; // Give control back to your main script

        // Reset rotation so player is upright again
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }

    private void CrawlMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 1. Align to Wall Surface (The Spider Look)
        // This makes the player's 'Up' match the wall's 'Normal'
        Quaternion targetRot = Quaternion.FromToRotation(transform.up, wallHit.normal) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSpeed);

        // 2. Move along the wall
        Vector3 moveDir = transform.up * v + transform.right * h;

        // Use the CharacterController's Move or transform.Translate
        charController.Move(moveDir * crawlSpeed * Time.deltaTime);

        // 3. Stick to wall (Pull the player toward the surface)
        charController.Move(-wallHit.normal * 0.1f);
    }
}