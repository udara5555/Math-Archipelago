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

        string hint1 = $"The sum of the 3 digits is {a + b + c}.";
        string hint2 = $"Digit 2 is {(b > a ? "greater" : "less")} than Digit 1 by {Mathf.Abs(b - a)}.";
        string hint3 = $"Digit 3 is {(c > b ? "greater" : "less")} than Digit 2 by {Mathf.Abs(c - b)}.";

        if (hintTexts != null && hintTexts.Length >= 3)
        {
            hintTexts[0].text = hint1;
            hintTexts[1].text = hint2;
            hintTexts[2].text = hint3;
        }
        else if (hintTexts != null && hintTexts.Length == 1)
        {
            // Fallback if only one TMP Text is assigned for all hints
            hintTexts[0].text = $"{hint1}\n\n{hint2}\n\n{hint3}";
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
            }
        }
        else
        {
            Debug.LogWarning("Passcode Input Field is not assigned!");
        }
    }
}
