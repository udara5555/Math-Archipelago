using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LogicalGameplay : MonoBehaviour
{
    [Header("UI References")]
    public GameObject hintPanel;

    [Header("Cage System")]
    public GameObject cageAnswerGameObject;
    public TMP_Text[] cageHintTexts;
    public TMP_InputField cagePasscodeInputField;
    private GameObject currentCage;

    [Header("Health")]
    public UnityEngine.UI.Image healthImage;
    [Tooltip("Assign 4 sprites: index 0 = 1 wrong answer, index 1 = 2 wrong, index 2 = 3 wrong, index 3 = 4 wrong (game over)")]
    public Sprite[] healthStages = new Sprite[4];
    public GameObject gameOverMessage;
    private int wrongAnswerCount = 0;

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

        // Ensure the cage answer UI is hidden when the game starts
        if (cageAnswerGameObject != null)
        {
            cageAnswerGameObject.SetActive(false);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Cage"))
        {
            currentCage = collision.gameObject;
            
            if (cageAnswerGameObject != null)
            {
                cageAnswerGameObject.SetActive(true);
            }
            
            if (cageHintTexts != null && cageHintTexts.Length >= 3)
            {
                cageHintTexts[0].text = "The code uses digits 2, 6, and 7.";
                cageHintTexts[1].text = "7 is the last digit.";
                cageHintTexts[2].text = "6 comes before 2.";
            }
            else if (cageHintTexts != null && cageHintTexts.Length == 1)
            {
                cageHintTexts[0].text = "The code uses digits 2, 6, and 7.\n\n7 is the last digit.\n\n6 comes before 2.";
            }
        }
    }

    public void SubmitCagePasscode()
    {
        if (cagePasscodeInputField != null)
        {
            if (cagePasscodeInputField.text == "627")
            {
                Debug.Log("Cage Passcode Correct!");
                
                // Clear the hint texts
                if (cageHintTexts != null && cageHintTexts.Length >= 3)
                {
                    cageHintTexts[0].text = "";
                    cageHintTexts[1].text = "";
                    cageHintTexts[2].text = "";
                }
                else if (cageHintTexts != null && cageHintTexts.Length == 1)
                {
                    cageHintTexts[0].text = "";
                }

                if (cageAnswerGameObject != null)
                {
                    cageAnswerGameObject.SetActive(false);
                }

                if (currentCage != null)
                {
                    // Disable the entire cage prefab (similar to the ghost logic)
                    if (currentCage.transform.parent != null)
                    {
                        currentCage.transform.parent.gameObject.SetActive(false);
                    }
                    else
                    {
                        currentCage.SetActive(false);
                    }
                }
            }
            else
            {
                Debug.Log("Cage Passcode Incorrect!");
                cagePasscodeInputField.text = "";
                TakeDamage();
            }
        }
    }

    public void TakeDamage()
    {
        wrongAnswerCount++;

        if (healthImage != null && wrongAnswerCount <= healthStages.Length && healthStages[wrongAnswerCount - 1] != null)
        {
            healthImage.sprite = healthStages[wrongAnswerCount - 1];
        }

        if (wrongAnswerCount >= healthStages.Length)
        {
            Debug.Log("Game Over!");
            
            // Show Game Over message
            if (gameOverMessage != null)
            {
                gameOverMessage.SetActive(true);
            }

            // Stop movement completely
            moveSpeed = 0f;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            
            // Disable this script to prevent further movement input
            this.enabled = false;
            
            // Optionally disable interaction or reload scene here
            if (cagePasscodeInputField != null)
            {
                cagePasscodeInputField.interactable = false;
            }
        }
    }

    public void RestartGame()
    {
        // Reloads the entire scene, completely resetting everything
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

