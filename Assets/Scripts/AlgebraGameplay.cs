using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Algebra treasure-chest game.
/// A chest starts with a hidden number of coins (≥ 51).
/// The player can Put (add) or Get (remove) coins — each time a random
/// amount (0-20) is chosen and the running total is displayed.
/// The player must figure out the initial number of coins.
/// </summary>
public class AlgebraGameplay : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The TotalCoins parent GameObject — disabled at start, enabled after first action")]
    public GameObject totalCoinsObject;

    [Tooltip("TMP inside TotalCoins — shows current coin count in chest")]
    public TMP_Text totalCoinsText;

    [Tooltip("TMP that shows how many coins were ADDED (put)")]
    public TMP_Text tmpAdd;

    [Tooltip("TMP that shows how many coins were REMOVED (get)")]
    public TMP_Text tmpSubtract;

    [Tooltip("Button the player clicks to PUT coins into the chest")]
    public Button putCoinsButton;

    [Tooltip("Button the player clicks to GET coins from the chest")]
    public Button getCoinsButton;

    [Tooltip("InputField for the sign (+ or -)")]
    public TMP_InputField inputFieldSign;

    [Tooltip("InputField for the value added or removed")]
    public TMP_InputField inputFieldValue;

    [Tooltip("InputField for the total coins")]
    public TMP_InputField inputFieldTotal;

    [Tooltip("The parent GameObject containing the math formula fields")]
    public GameObject mathPartGameObject;

    [Tooltip("InputField where the player types their answer")]
    public TMP_InputField answerInput;

    [Tooltip("Button the player clicks to submit their answer")]
    public Button submitButton;

    [Tooltip("TMP for feedback messages (Correct / Wrong)")]
    public TMP_Text feedbackText;

    [Header("Health")]
    public Image healthImage;

    [Tooltip("Assign 5 sprites: index 0 = 1 wrong, index 1 = 2 wrong, ..., index 4 = 5 wrong (game over)")]
    public Sprite[] healthStages = new Sprite[5];

    [Tooltip("Image for the ghost's health bar (Filled mode)")]
    public Image ghostHealthImage;

    [Header("Game Settings")]
    [Tooltip("Minimum initial coins (must be > 50)")]
    public int minInitialCoins = 51;

    [Tooltip("Maximum initial coins")]
    public int maxInitialCoins = 100;

    [Tooltip("Minimum random coins per action")]
    public int minRandom = 0;

    [Tooltip("Maximum random coins per action (inclusive)")]
    public int maxRandom = 20;

    // Internal state
    int initialCoins;   // the secret answer
    int currentCoins;   // running total after puts / gets
    int totalAdded;     // cumulative coins put in
    int totalRemoved;   // cumulative coins taken out
    string lastActionSign = "";
    int lastActionValue = 0;
    int wrongAnswerCount = 0;

    void Start()
    {
        StartNewRound();

        // Wire button listeners
        if (putCoinsButton != null)
            putCoinsButton.onClick.AddListener(OnPutCoins);

        if (getCoinsButton != null)
            getCoinsButton.onClick.AddListener(OnGetCoins);

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitAnswer);
    }

    /// <summary>
    /// Initialises a fresh round with a new random initial coin value.
    /// </summary>
    public void StartNewRound()
    {
        initialCoins = Random.Range(minInitialCoins, maxInitialCoins + 1);
        currentCoins = initialCoins;
        totalAdded = 0;
        totalRemoved = 0;

        // Hide the total coins and math part until the player performs an action
        if (totalCoinsObject != null)
            totalCoinsObject.SetActive(false);
            
        if (mathPartGameObject != null)
            mathPartGameObject.SetActive(false);

        UpdateTotalCoinsDisplay();

        // Clear the add / subtract displays
        if (tmpAdd != null)
            tmpAdd.text = "";

        if (tmpSubtract != null)
            tmpSubtract.text = "";

        // Clear answer fields and feedback
        if (inputFieldSign != null) inputFieldSign.text = "";
        if (inputFieldValue != null) inputFieldValue.text = "";
        if (inputFieldTotal != null) inputFieldTotal.text = "";
        if (answerInput != null) answerInput.text = "";

        lastActionSign = "";
        lastActionValue = 0;

        ClearFeedback();

        Debug.Log($"[AlgebraGame] New round — initial coins = {initialCoins}");
    }

    /// <summary>
    /// Called when the player clicks the Put Coins button.
    /// Adds a random number of coins (0-20) to the chest.
    /// </summary>
    void OnPutCoins()
    {
        // Show the total coins display and math part on first action
        if (totalCoinsObject != null && !totalCoinsObject.activeSelf)
            totalCoinsObject.SetActive(true);
            
        if (mathPartGameObject != null && !mathPartGameObject.activeSelf)
            mathPartGameObject.SetActive(true);

        int amount = Random.Range(minRandom, maxRandom + 1);
        currentCoins += amount;
        totalAdded += amount;
        lastActionSign = "+";
        lastActionValue = amount;

        if (tmpAdd != null)
            tmpAdd.text = $"{amount}";

        // Clear the subtract display so only the latest action shows
        if (tmpSubtract != null)
            tmpSubtract.text = "";

        UpdateTotalCoinsDisplay();
    }

    /// <summary>
    /// Called when the player clicks the Get Coins button.
    /// Removes a random number of coins (0-20) from the chest.
    /// </summary>
    void OnGetCoins()
    {
        // Show the total coins display and math part on first action
        if (totalCoinsObject != null && !totalCoinsObject.activeSelf)
            totalCoinsObject.SetActive(true);
            
        if (mathPartGameObject != null && !mathPartGameObject.activeSelf)
            mathPartGameObject.SetActive(true);

        int amount = Random.Range(minRandom, maxRandom + 1);

        // Prevent going below zero
        if (amount > currentCoins)
            amount = currentCoins;

        currentCoins -= amount;
        totalRemoved += amount;
        lastActionSign = "-";
        lastActionValue = amount;

        if (tmpSubtract != null)
            tmpSubtract.text = $"{amount}";

        // Clear the add display so only the latest action shows
        if (tmpAdd != null)
            tmpAdd.text = "";

        UpdateTotalCoinsDisplay();
    }

    /// <summary>
    /// Called when the player submits their answer for the initial coin count.
    /// </summary>
    void OnSubmitAnswer()
    {
        // Hide total coins and math part after submitting
        if (totalCoinsObject != null)
            totalCoinsObject.SetActive(false);
            
        if (mathPartGameObject != null)
            mathPartGameObject.SetActive(false);

        bool isCorrect = true;

        // Validate Sign
        if (inputFieldSign != null && inputFieldSign.text.Trim() != lastActionSign)
            isCorrect = false;

        // Validate Value
        if (inputFieldValue != null)
        {
            if (!int.TryParse(inputFieldValue.text.Trim(), out int playerValue) || playerValue != lastActionValue)
                isCorrect = false;
        }

        // Validate Total
        if (inputFieldTotal != null)
        {
            if (!int.TryParse(inputFieldTotal.text.Trim(), out int playerTotal) || playerTotal != currentCoins)
                isCorrect = false;
        }

        // Validate Initial Answer
        if (answerInput != null)
        {
            if (!int.TryParse(answerInput.text.Trim(), out int playerAnswer) || playerAnswer != initialCoins)
                isCorrect = false;
        }

        if (isCorrect)
        {
            ShowFeedback("Correct! The chest started with " + initialCoins + " coins!", Color.green);
            Debug.Log("[AlgebraGame] Correct answer!");

            // Reduce ghost health by 10%
            if (ghostHealthImage != null)
            {
                ghostHealthImage.fillAmount -= 0.1f;
                if (ghostHealthImage.fillAmount <= 0)
                {
                    Debug.Log("[AlgebraGame] Ghost Defeated!");
                }
            }
        }
        else
        {
            ShowFeedback("Wrong! Try again.", Color.red);
            Debug.Log("[AlgebraGame] Wrong answer on one or more fields!");

            wrongAnswerCount++;

            if (healthImage != null && healthStages != null && wrongAnswerCount <= healthStages.Length && healthStages[wrongAnswerCount - 1] != null)
            {
                healthImage.sprite = healthStages[wrongAnswerCount - 1];
            }

            if (healthStages != null && wrongAnswerCount >= healthStages.Length)
            {
                Debug.Log("[AlgebraGame] Game Over!");
                ShowFeedback("Game Over!", Color.red);
                if (submitButton != null) submitButton.interactable = false;
                if (putCoinsButton != null) putCoinsButton.interactable = false;
                if (getCoinsButton != null) getCoinsButton.interactable = false;
            }
        }

        // Reset all input fields, signs, and values after submit
        if (inputFieldSign != null) inputFieldSign.text = "";
        if (inputFieldValue != null) inputFieldValue.text = "";
        if (inputFieldTotal != null) inputFieldTotal.text = "";
        if (answerInput != null) answerInput.text = "";

        if (tmpAdd != null) tmpAdd.text = "";
        if (tmpSubtract != null) tmpSubtract.text = "";

        lastActionSign = "";
        lastActionValue = 0;

        // Reset values from memory and generate a new initial coin value
        initialCoins = Random.Range(minInitialCoins, maxInitialCoins + 1);
        currentCoins = initialCoins;
        totalAdded = 0;
        totalRemoved = 0;
        UpdateTotalCoinsDisplay();
        
        Debug.Log($"[AlgebraGame] New round started after submit — new secret coins = {initialCoins}");
    }

    void UpdateTotalCoinsDisplay()
    {
        if (totalCoinsText != null)
            totalCoinsText.text = currentCoins.ToString();
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;

            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 2f);
        }
    }

    void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }
}
