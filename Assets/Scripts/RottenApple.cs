using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RottenApple : MonoBehaviour
{
    [Header("Apple Data")]
    [Tooltip("Randomly generated number for this apple between 20 and 100")]
    public int appleNumber;

    [Tooltip("First factor displayed in FirstFactorText (TMP)")]
    public int firstFactor;

    [Header("Apple Visuals")]
    [Tooltip("Text component attached to this apple to display its number")]
    public TMP_Text appleText;

    [Tooltip("Fresh apple sprite to swap to when the problem is solved correctly")]
    public Sprite freshAppleSprite;

    // References to Factor Panel UI elements
    private GameObject currentPanel;
    private TMP_Text numberTextTMP;
    private TMP_Text firstFactorTextTMP;
    private TMP_InputField factorInputField;
    private TMP_InputField[] primeInputFields = new TMP_InputField[4];
    private Button submitButton;
    private Button clearButton;

    private void Awake()
    {
        GenerateAppleData();
    }

    private void Start()
    {
        UpdateAppleDisplay();
    }

    /// <summary>
    /// Generates a random composite number between 20 and 100 and picks a first factor.
    /// </summary>
    public void GenerateAppleData()
    {
        // Pool of suitable composite numbers between 20 and 100
        int[] compositeNumbers = new int[] {
            20, 24, 25, 27, 28, 30, 32, 36, 40, 42, 44, 45, 48, 50,
            52, 54, 56, 60, 63, 64, 66, 70, 72, 75, 80, 81, 84, 88, 90, 96, 99, 100
        };

        appleNumber = compositeNumbers[Random.Range(0, compositeNumbers.Length)];

        // Get factors of appleNumber
        List<int> factors = GetFactors(appleNumber);
        
        // Pick a factor for FirstFactorText (TMP) - e.g. appleNumber itself or one of its factors
        firstFactor = factors[Random.Range(0, factors.Count)];
    }

    private List<int> GetFactors(int number)
    {
        List<int> list = new List<int>();
        for (int i = 1; i <= number; i++)
        {
            if (number % i == 0)
            {
                list.Add(i);
            }
        }
        return list;
    }

    /// <summary>
    /// Displays the apple's number on the apple text component if present.
    /// </summary>
    public void UpdateAppleDisplay()
    {
        if (appleText == null)
        {
            appleText = GetComponentInChildren<TMP_Text>();
        }

        if (appleText != null)
        {
            appleText.text = appleNumber.ToString();
        }
    }

    /// <summary>
    /// Populates the Factor Panel UI elements when Fairy touches this apple and binds Submit/Clear buttons.
    /// </summary>
    public void PopulateFactorPanel(GameObject panel)
    {
        if (panel == null) return;
        currentPanel = panel;

        // 1. Find and update TMP Texts
        TMP_Text[] texts = panel.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text txt in texts)
        {
            if (txt.gameObject.name == "NumberText (TMP)")
            {
                numberTextTMP = txt;
                txt.text = appleNumber.ToString();
            }
            else if (txt.gameObject.name == "FirstFactorText (TMP)")
            {
                firstFactorTextTMP = txt;
                txt.text = firstFactor.ToString();
            }
        }

        // 2. Find and reset Input Fields
        TMP_InputField[] inputFields = panel.GetComponentsInChildren<TMP_InputField>(true);
        foreach (TMP_InputField input in inputFields)
        {
            if (input.gameObject.name == "FactorInputField (TMP)")
            {
                factorInputField = input;
                input.text = "";
            }
            else if (input.gameObject.name == "PrimeAnswerInputField (TMP) (1)")
            {
                primeInputFields[0] = input;
                input.text = "";
            }
            else if (input.gameObject.name == "PrimeAnswerInputField (TMP) (2)")
            {
                primeInputFields[1] = input;
                input.text = "";
            }
            else if (input.gameObject.name == "PrimeAnswerInputField (TMP) (3)")
            {
                primeInputFields[2] = input;
                input.text = "";
            }
            else if (input.gameObject.name == "PrimeAnswerInputField (TMP) (4)")
            {
                primeInputFields[3] = input;
                input.text = "";
            }
        }

        // 3. Find and bind Submit & Clear Buttons
        Button[] buttons = panel.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            if (btn.gameObject.name == "SubmitButton")
            {
                submitButton = btn;
                submitButton.onClick.RemoveAllListeners();
                submitButton.onClick.AddListener(OnSubmitClicked);
            }
            else if (btn.gameObject.name == "ClearButton")
            {
                clearButton = btn;
                clearButton.onClick.RemoveAllListeners();
                clearButton.onClick.AddListener(OnClearClicked);
            }
        }
    }

    /// <summary>
    /// Clears all input fields in the Factor Panel.
    /// </summary>
    public void OnClearClicked()
    {
        if (factorInputField != null)
        {
            factorInputField.text = "";
        }

        for (int i = 0; i < primeInputFields.Length; i++)
        {
            if (primeInputFields[i] != null)
            {
                primeInputFields[i].text = "";
            }
        }
    }

    /// <summary>
    /// Validates player inputs against the generated apple number.
    /// </summary>
    public void OnSubmitClicked()
    {
        // 1. Validate middle line: (FactorInputField) x (FirstFactorText) == appleNumber
        if (factorInputField == null || !int.TryParse(factorInputField.text, out int userFactor))
        {
            Debug.Log("Please enter a valid number in the factor input field.");
            return;
        }

        if (userFactor * firstFactor != appleNumber)
        {
            Debug.Log($"Incorrect! {userFactor} x {firstFactor} = {userFactor * firstFactor}, which does not equal target {appleNumber}.");
            return;
        }

        // 2. Validate primary prime inputs: (P1) x (P2) x (P3) x (P4) == appleNumber
        int primeProduct = 1;
        bool hasEnteredPrimes = false;

        for (int i = 0; i < primeInputFields.Length; i++)
        {
            if (primeInputFields[i] != null && !string.IsNullOrEmpty(primeInputFields[i].text))
            {
                if (int.TryParse(primeInputFields[i].text, out int val))
                {
                    primeProduct *= val;
                    hasEnteredPrimes = true;
                }
                else
                {
                    Debug.Log($"Invalid number in prime input field {i + 1}.");
                    return;
                }
            }
        }

        if (!hasEnteredPrimes || primeProduct != appleNumber)
        {
            Debug.Log($"Incorrect! Product of primary answer input fields = {primeProduct}, which does not equal target {appleNumber}.");
            return;
        }

        // Successful validation!
        Debug.Log($"Correct! Both {userFactor} x {firstFactor} and primary prime product equal {appleNumber}.");

        // Close factor panel
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
        }

        // Disable collider so Fairy can fly freely without re-triggering factor panel
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Swap rotten apple sprite to fresh apple sprite
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && freshAppleSprite != null)
        {
            sr.sprite = freshAppleSprite;
        }
    }
}
