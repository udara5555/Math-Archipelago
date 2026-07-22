using UnityEngine;

public class FactorsGameplay : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Assign the Factor Panel UI GameObject here in the Inspector")]
    public GameObject factorPanel;

    [Header("Movement Settings")]
    [Tooltip("Movement speed of the fairy")]
    public float moveSpeed = 5f;

    [Header("Sprite Settings")]
    [Tooltip("Check this if your default sprite graphic faces right instead of left")]
    public bool defaultFacingRight = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveInput;
    private GameObject currentApple;

    void Start()
    {
        // Obtain references to components attached to the Fairy
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure the factor panel is hidden when the game starts
        if (factorPanel != null)
        {
            factorPanel.SetActive(false);
        }

        // Gravity does not affect the fairy; set gravityScale to 0 if Rigidbody2D is attached
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    void Update()
    {
        // Read input for WASD and Arrow keys
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) moveY += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) moveY -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) moveX -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) moveX += 1f;

        // Normalize movement vector so diagonal flying speed is consistent
        moveInput = new Vector2(moveX, moveY).normalized;

        bool isMoving = moveInput.sqrMagnitude > 0f;

        // Play or stop fairy move animation by setting the 'isMoving' parameter in the Animator Controller
        if (animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }

        // Flip sprite left/right based on movement direction
        if (spriteRenderer != null && moveX != 0f)
        {
            if (defaultFacingRight)
            {
                spriteRenderer.flipX = (moveX < 0f);
            }
            else
            {
                // Fairy default sprite faces left
                spriteRenderer.flipX = (moveX > 0f);
            }
        }

        // Fallback translation if no Rigidbody2D component is present
        if (rb == null && isMoving)
        {
            transform.Translate(moveInput * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void FixedUpdate()
    {
        // Move via Physics if Rigidbody2D is present
        if (rb != null)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }

    // Called when Fairy collides with a non-trigger 2D collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckAppleContact(collision.gameObject);
    }

    // Called when Fairy enters a trigger 2D collider
    private void OnTriggerEnter2D(Collider2D collider)
    {
        CheckAppleContact(collider.gameObject);
    }

    private void CheckAppleContact(GameObject target)
    {
        // Check if the collided object is a Rotten Apple (by component or name)
        RottenApple apple = target.GetComponent<RottenApple>();
        if (apple != null || target.name.Contains("RottenApple") || target.name.Contains("Apple"))
        {
            currentApple = target;
            if (apple != null)
            {
                apple.PopulateFactorPanel(factorPanel);
            }
            OpenFactorPanel();
        }
    }

    public void OpenFactorPanel()
    {
        if (factorPanel != null)
        {
            factorPanel.SetActive(true);
        }
    }

    public void CloseFactorPanel()
    {
        if (factorPanel != null)
        {
            factorPanel.SetActive(false);
        }
    }
}
