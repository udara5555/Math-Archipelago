using UnityEngine;
using TMPro;

public class LogicalGhost : MonoBehaviour
{
    [Header("References")]
    public GameObject answerGameObject;

    [Header("Passcode System")]
    public TMP_Text[] hintTexts;
    public TMP_InputField passcodeInputField;
    public string currentPasscode = "";

    // Internal strings for this specific ghost's hints
    private string hint1;
    private string hint2;
    private string hint3;

    void Start()
    {
        // Ensure the answer object is disabled when the game starts
        if (answerGameObject != null)
        {
            answerGameObject.SetActive(false);
        }

        GenerateNewPasscode();
    }

    public void GenerateNewPasscode()
    {
        // Generate 3 distinct random digits
        int a, b, c;
        do
        {
            a = Random.Range(0, 10);
            b = Random.Range(0, 10);
            c = Random.Range(0, 10);
        } while (a == b || a == c || b == c);

        currentPasscode = $"{a}{b}{c}";

        // Create a shuffled array to list the digits without giving away the order
        int[] digits = { a, b, c };
        for (int i = 0; i < digits.Length; i++)
        {
            int temp = digits[i];
            int randomIndex = Random.Range(i, digits.Length);
            digits[i] = digits[randomIndex];
            digits[randomIndex] = temp;
        }

        hint1 = $"The code uses digits {digits[0]}, {digits[1]}, and {digits[2]}.";

        int template = Random.Range(0, 4);
        switch (template)
        {
            case 0:
                hint2 = $"{c} is the last digit.";
                hint3 = $"{a} comes before {b}.";
                break;
            case 1:
                hint2 = $"{b} is the middle digit.";
                hint3 = $"{a} comes before {c}.";
                break;
            case 2:
                hint2 = $"{a} is not the last digit.";
                hint3 = $"{c} comes immediately after {b}.";
                break;
            case 3:
            default:
                hint2 = $"{c} is not the first digit.";
                hint3 = $"{b} comes immediately after {a}.";
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object that collided with the ghost is our Hero
        // We can verify this by checking the name or looking for the LogicalGameplay script
        if (collision.gameObject.name == "Hero" || collision.gameObject.GetComponent<LogicalGameplay>() != null)
        {
            if (answerGameObject != null)
            {
                // Enable the answer game object
                answerGameObject.SetActive(true);

                // Update the global Hint Panel to show THIS ghost's specific hints
                if (hintTexts != null && hintTexts.Length >= 3)
                {
                    hintTexts[0].text = hint1;
                    hintTexts[1].text = hint2;
                    hintTexts[2].text = hint3;
                }
                else if (hintTexts != null && hintTexts.Length == 1)
                {
                    hintTexts[0].text = $"{hint1}\n\n{hint2}\n\n{hint3}";
                }
            }
        }
    }

    public void SubmitPasscode()
    {
        if (passcodeInputField != null)
        {
            if (passcodeInputField.text == currentPasscode)
            {
                Debug.Log("Passcode Correct!");
                
                // Clear the hint texts since this ghost is solved
                if (hintTexts != null && hintTexts.Length >= 3)
                {
                    hintTexts[0].text = "";
                    hintTexts[1].text = "";
                    hintTexts[2].text = "";
                }
                else if (hintTexts != null && hintTexts.Length == 1)
                {
                    hintTexts[0].text = "";
                }

                // Hide the answer UI
                if (answerGameObject != null)
                {
                    answerGameObject.SetActive(false);
                }

                // Disable the whole ghost prefab entirely so the player can pass
                if (transform.parent != null)
                {
                    transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.Log("Passcode Incorrect!");
                // Clear the input field for another try
                passcodeInputField.text = "";

                // Hurt the player!
                LogicalGameplay hero = FindAnyObjectByType<LogicalGameplay>();
                if (hero != null)
                {
                    hero.TakeDamage();
                }
            }
        }
        else
        {
            Debug.LogWarning("Passcode Input Field is not assigned!");
        }
    }
}
