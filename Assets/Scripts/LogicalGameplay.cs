using UnityEngine;

public class LogicalGameplay : MonoBehaviour
{
    [Header("UI References")]
    public GameObject hintPanel;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private float moveInput;

    void Start()
    {
        // Try to get the components on this GameObject
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        if (rb == null)
        {
            Debug.LogError("No Rigidbody2D found on " + gameObject.name + ". Please attach one for movement to work.");
        }
    }

    void Update()
    {
        // Get input from A/D keys or Left/Right arrows (-1 for left, 1 for right, 0 for nothing)
        moveInput = Input.GetAxisRaw("Horizontal");

        // Flip the sprite based on movement direction
        if (moveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput < 0)
        {
            spriteRenderer.flipX = true;
        }

        // Trigger walk animation based on whether there's input
        if (animator != null)
        {
            bool isWalking = Mathf.Abs(moveInput) > 0.01f;
            animator.SetBool("isWalking", isWalking);
        }
    }

    void FixedUpdate()
    {
        if (rb != null)
        {
            // Apply horizontal velocity while maintaining current vertical velocity (for gravity/falling)
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }
    }

    // Call this from the Instructions Button's OnClick event
    public void OpenHintPanel()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(true);
        }
    }

    // Call this from the Close Button's OnClick event
    public void CloseHintPanel()
    {
        if (hintPanel != null)
        {
            hintPanel.SetActive(false);
        }
    }
}

