using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SerumColor
{
    Orange,
    Yellow,
    Green,
    Blue,
    Purple
}

public class FractionsGameplay : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The DialogImage GameObject (child of Wizard). Auto-assigned if left null.")]
    public GameObject dialogImage;

    [Tooltip("The TMP text component inside DialogImage. Auto-assigned if left null.")]
    public TMP_Text dialogText;

    [Tooltip("The Next button inside DialogImage. Auto-assigned if left null.")]
    public Button nextButton;

    [Tooltip("The Answers container GameObject. Auto-assigned if left null.")]
    public GameObject answersGameObject;

    [Header("Greeting Settings")]
    [TextArea(2, 5)]
    [Tooltip("Greeting message to display when Wizard entry animation completes.")]
    public string greetingMessage = "Greetings, young wizard! Welcome to the Fraction Lab!";

    [Header("Animation Settings")]
    [Tooltip("Name of the entry animation state in the Animator.")]
    public string entryAnimationStateName = "WizardEntry";

    [Header("Fraction Task Settings")]
    [Tooltip("Numerator (x) for the task prompt.")]
    public int targetNumerator = 2;

    [Tooltip("Denominator (y) for the task prompt.")]
    public int targetDenominator = 5;

    [Tooltip("Color of serum required for the task.")]
    public SerumColor targetColor = SerumColor.Green;

    [Tooltip("Format template for the task message. {0} = x, {1} = y, {2} = color.")]
    public string taskMessageFormat = "Fill up {0}/{1} amount of flask by {2} serum";

    [Tooltip("If true, automatically picks a random fraction and color when Next is clicked.")]
    public bool randomizeTaskOnNext = false;

    private Animator animator;
    private bool greetingShown = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Auto-assign references if not set in Inspector
        if (dialogImage == null)
        {
            Transform found = transform.Find("DialogImage");
            if (found != null)
            {
                dialogImage = found.gameObject;
            }
        }

        if (dialogImage != null)
        {
            if (dialogText == null)
            {
                dialogText = dialogImage.GetComponentInChildren<TMP_Text>();
            }

            if (nextButton == null)
            {
                Transform btnTransform = dialogImage.transform.Find("NextButton");
                if (btnTransform != null)
                {
                    nextButton = btnTransform.GetComponent<Button>();
                }
            }
        }

        // Auto-bind Next button click listener
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        // Hide dialog image at start while animation plays
        if (dialogImage != null)
        {
            dialogImage.SetActive(false);
        }

        // Auto-find Answers GameObject if not assigned & hide initially
        if (answersGameObject == null)
        {
            answersGameObject = GameObject.Find("Answers");
        }
        if (answersGameObject != null)
        {
            answersGameObject.SetActive(false);
        }

        // Start checking for entry animation completion
        StartCoroutine(WaitForWizardEntry());
    }

    private IEnumerator WaitForWizardEntry()
    {
        if (animator != null)
        {
            // Wait one frame to allow Animator state to update
            yield return null;

            // Wait while WizardEntry animation state is active and running
            while (animator.GetCurrentAnimatorStateInfo(0).IsName(entryAnimationStateName) &&
                   animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }
        }

        ShowGreeting();
    }

    /// <summary>
    /// Enables the DialogImage GameObject and sets the greeting text.
    /// Can also be called directly via an Animation Event in Unity Editor.
    /// </summary>
    public void ShowGreeting()
    {
        if (greetingShown) return;
        greetingShown = true;

        if (dialogImage != null)
        {
            dialogImage.SetActive(true);
        }

        if (dialogText != null)
        {
            dialogText.text = greetingMessage;
        }

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Called when the NextButton inside DialogImage is clicked.
    /// Changes the TMP text to display: "Fill up (x/y) amount of flask by (color) serum".
    /// </summary>
    public void OnNextButtonClicked()
    {
        if (randomizeTaskOnNext)
        {
            GenerateRandomTask();
        }

        UpdateTaskText();
    }

    /// <summary>
    /// Updates the TMP text with the formatted task prompt.
    /// </summary>
    public void UpdateTaskText()
    {
        if (dialogText != null)
        {
            string colorName = targetColor.ToString();
            dialogText.text = string.Format(taskMessageFormat, targetNumerator, targetDenominator, colorName);
        }
    }

    /// <summary>
    /// Programmatically set a specific fraction task.
    /// </summary>
    public void SetTask(int numerator, int denominator, SerumColor color)
    {
        targetNumerator = numerator;
        targetDenominator = denominator;
        targetColor = color;
        UpdateTaskText();
    }

    /// <summary>
    /// Generates a random fraction task (e.g. 1/3, 2/5, 3/4) and random serum color.
    /// </summary>
    public void GenerateRandomTask()
    {
        targetDenominator = Random.Range(2, 6); // 2 to 5
        targetNumerator = Random.Range(1, targetDenominator); // 1 to denominator-1

        SerumColor[] colors = (SerumColor[])System.Enum.GetValues(typeof(SerumColor));
        targetColor = colors[Random.Range(0, colors.Length)];
    }
}


