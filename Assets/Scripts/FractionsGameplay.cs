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

    [Tooltip("The Blast effect GameObject (enabled when wrong answer selected). Auto-assigned if left null.")]
    public GameObject blastGameObject;

    [Tooltip("The RestartPanel GameObject (enabled when blast animation finishes). Auto-assigned if left null.")]
    public GameObject restartPanel;

    [Header("Greeting Settings")]
    [TextArea(2, 5)]
    [Tooltip("Greeting message to display when Wizard entry animation completes.")]
    public string greetingMessage = "Greetings, young wizard! Welcome to the Fraction Lab!";

    [Header("Animation Settings")]
    [Tooltip("Name of the entry animation state in the Animator.")]
    public string entryAnimationStateName = "WizardEntry";

    [Tooltip("Name of the blast animation state in the Blast Animator.")]
    public string blastAnimationStateName = "BlastAnimation";

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

    [Header("Health")]
    [Tooltip("Health image indicator (e.g. heart bar). Auto-assigned if left null.")]
    public Image healthImage;

    [Tooltip("Health stage sprites for wrong answer penalty.")]
    public Sprite[] healthStages;

    private Animator animator;
    private bool greetingShown = false;
    private bool hasDisplayedFirstTask = false;
    private int wrongAnswerCount = 0;

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

        if (healthImage == null)
        {
            GameObject healthObj = GameObject.Find("Health");
            if (healthObj != null)
            {
                healthImage = healthObj.GetComponent<Image>();
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

        // Auto-find Blast GameObject if not assigned & hide initially
        if (blastGameObject == null)
        {
            blastGameObject = GameObject.Find("Blast");
        }
        if (blastGameObject != null)
        {
            blastGameObject.SetActive(false);
        }

        // Auto-find RestartPanel GameObject if not assigned & hide initially
        if (restartPanel == null)
        {
            restartPanel = GameObject.Find("RestartPanel");
        }
        if (restartPanel != null)
        {
            restartPanel.SetActive(false);

            // Auto-bind RestartButton listener
            Transform restartBtnTransform = restartPanel.transform.Find("RestartButton");
            if (restartBtnTransform != null)
            {
                Button restartBtn = restartBtnTransform.GetComponent<Button>();
                if (restartBtn != null)
                {
                    restartBtn.onClick.AddListener(RestartLevel);
                }
            }
        }

        // Ensure serum buttons start as un-interactable
        SetSerumButtonsInteractable(false);

        // Start checking for entry animation completion
        StartCoroutine(WaitForWizardEntry());
    }

    /// <summary>
    /// Enables or disables the interactable state of all SerumButton instances.
    /// </summary>
    public void SetSerumButtonsInteractable(bool interactable)
    {
        SerumButton[] serumButtons = FindObjectsByType<SerumButton>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var sb in serumButtons)
        {
            Button btn = sb.GetComponent<Button>();
            if (btn != null)
            {
                btn.interactable = interactable;
            }
        }
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
            nextButton.interactable = true;
        }

        // Keep serum buttons un-interactable during greeting
        SetSerumButtonsInteractable(false);
    }

    /// <summary>
    /// Called when the NextButton inside DialogImage is clicked.
    /// Step 2: Task message appears, Next button goes un-interactable, and serum buttons become interactable.
    /// </summary>
    public void OnNextButtonClicked()
    {
        if (hasDisplayedFirstTask || randomizeTaskOnNext)
        {
            GenerateRandomTask();
        }
        hasDisplayedFirstTask = true;

        UpdateTaskText();

        // Make Next button un-interactable
        if (nextButton != null)
        {
            nextButton.interactable = false;
        }

        // Enable interactable state on serum button instances
        SetSerumButtonsInteractable(true);
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
        targetDenominator = Random.Range(2, 9); // 2 to 8
        targetNumerator = Random.Range(1, targetDenominator); // 1 to targetDenominator-1

        SerumColor[] colors = (SerumColor[])System.Enum.GetValues(typeof(SerumColor));
        targetColor = colors[Random.Range(0, colors.Length)];
    }

    /// <summary>
    /// Generates 1 correct fraction expression (addition or multiplication equal to targetNumerator/targetDenominator)
    /// and 2 wrong options of the same format, then sets them on the 3 AnswerBar instances.
    /// </summary>
    public void GenerateAnswers()
    {
        // Hide blast effect when generating new answers
        if (blastGameObject != null)
        {
            blastGameObject.SetActive(false);
        }

        int N = targetNumerator;
        int D = targetDenominator;

        if (D <= 0) D = 1;
        if (N <= 0) N = 1;

        bool useAddition = Random.value > 0.5f;

        if (N <= 1)
        {
            useAddition = false;
        }

        string correctAnswer = "";
        System.Collections.Generic.List<string> wrongAnswers = new System.Collections.Generic.List<string>();

        if (useAddition)
        {
            // Correct Addition: a/D + b/D = N/D where a + b = N
            int a = Random.Range(1, N);
            int b = N - a;
            
            correctAnswer = $"{a}/{D} + {b}/{D}";

            // Wrong Additions of same format: (wa/D) + (wb/D) where wa + wb != N
            System.Collections.Generic.HashSet<int> wrongSums = new System.Collections.Generic.HashSet<int>();
            while (wrongSums.Count < 2)
            {
                int wSum = Random.Range(2, N + 5);
                if (wSum != N)
                {
                    wrongSums.Add(wSum);
                }
            }

            foreach (int wSum in wrongSums)
            {
                int wa = Random.Range(1, wSum);
                int wb = wSum - wa;
                wrongAnswers.Add($"{wa}/{D} + {wb}/{D}");
            }
        }
        else
        {
            // Correct Multiplication: (n1/d1) x (n2/d2) = N/D
            var dPairs = GetFactorPairs(D);
            var (d1, d2) = dPairs[Random.Range(0, dPairs.Count)];

            var nPairs = GetFactorPairs(N);
            var (n1, n2) = nPairs[Random.Range(0, nPairs.Count)];

            correctAnswer = $"{n1}/{d1} × {n2}/{d2}";

            // Wrong Multiplications of same format
            System.Collections.Generic.HashSet<string> wrongSet = new System.Collections.Generic.HashSet<string>();
            while (wrongSet.Count < 2)
            {
                int wn1 = Random.Range(1, N + 4);
                int wn2 = Random.Range(1, 4);

                if (wn1 * wn2 != N)
                {
                    string wrongExpr = $"{wn1}/{d1} × {wn2}/{d2}";
                    wrongSet.Add(wrongExpr);
                }
            }
            wrongAnswers.AddRange(wrongSet);
        }

        // Combine all 3 answers
        var allAnswers = new System.Collections.Generic.List<(string expr, bool isCorrect)>
        {
            (correctAnswer, true),
            (wrongAnswers[0], false),
            (wrongAnswers[1], false)
        };

        // Shuffle answer order
        for (int i = 0; i < allAnswers.Count; i++)
        {
            int rand = Random.Range(i, allAnswers.Count);
            var temp = allAnswers[i];
            allAnswers[i] = allAnswers[rand];
            allAnswers[rand] = temp;
        }

        // Find the 3 FractionsAnswer scripts on the AnswerBar instances
        FractionsAnswer[] bars = GetAnswerBars();
        for (int i = 0; i < bars.Length && i < allAnswers.Count; i++)
        {
            bars[i].SetAnswer(allAnswers[i].expr, allAnswers[i].isCorrect);
        }
    }

    private System.Collections.Generic.List<(int, int)> GetFactorPairs(int number)
    {
        var pairs = new System.Collections.Generic.List<(int, int)>();
        for (int i = 1; i <= number; i++)
        {
            if (number % i == 0)
            {
                pairs.Add((i, number / i));
            }
        }
        if (pairs.Count == 0) pairs.Add((1, number));
        return pairs;
    }

    private FractionsAnswer[] GetAnswerBars()
    {
        if (answersGameObject != null)
        {
            return answersGameObject.GetComponentsInChildren<FractionsAnswer>(true);
        }
        return FindObjectsByType<FractionsAnswer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    /// <summary>
    /// Called when player clicks an answer bar.
    /// </summary>
    public void SelectAnswer(FractionsAnswer selectedAnswer)
    {
        if (selectedAnswer.isCorrect)
        {
            Debug.Log("Correct Fraction Answer!");
            if (dialogText != null)
            {
                dialogText.text = "Correct! You successfully combined the fractions!";
            }

            // Hide answer bars and blast effect on correct answer
            if (answersGameObject != null)
            {
                answersGameObject.SetActive(false);
            }

            if (blastGameObject != null)
            {
                blastGameObject.SetActive(false);
            }

            // Step 4: Re-enable interactable state on nextButton
            if (nextButton != null)
            {
                nextButton.interactable = true;
            }
        }
        else
        {
            Debug.Log("Wrong Fraction Answer!");
            wrongAnswerCount++;

            // Update health display
            if (healthImage != null && healthStages != null && wrongAnswerCount <= healthStages.Length && healthStages[wrongAnswerCount - 1] != null)
            {
                healthImage.sprite = healthStages[wrongAnswerCount - 1];
            }

            if (dialogText != null)
            {
                dialogText.text = "Incorrect fraction! Try again.";
            }

            // Game Over check: enable blast effect only when health reaches zero
            int maxHealthStages = (healthStages != null && healthStages.Length > 0) ? healthStages.Length : 4;
            if (wrongAnswerCount >= maxHealthStages)
            {
                Debug.Log("Game Over!");
                if (dialogText != null)
                {
                    dialogText.text = "Game Over! You ran out of magic energy.";
                }

                if (blastGameObject != null)
                {
                    blastGameObject.SetActive(true);
                    StartCoroutine(WaitForBlastAnimation());
                }
                else if (restartPanel != null)
                {
                    restartPanel.SetActive(true);
                }
            }
        }
    }

    private IEnumerator WaitForBlastAnimation()
    {
        Animator blastAnimator = blastGameObject != null ? blastGameObject.GetComponent<Animator>() : null;
        if (blastAnimator == null && blastGameObject != null)
        {
            blastAnimator = blastGameObject.GetComponentInChildren<Animator>();
        }

        if (blastAnimator != null)
        {
            // Wait one frame to allow Animator state to update
            yield return null;

            // Wait while BlastAnimation is playing
            while (blastAnimator.GetCurrentAnimatorStateInfo(0).IsName(blastAnimationStateName) &&
                   blastAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }
        }
        else
        {
            // Fallback duration if animator is not found
            yield return new WaitForSeconds(1.0f);
        }

        // Enable RestartPanel once blast animation completes
        if (restartPanel != null)
        {
            restartPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Reloads the active scene to restart the level.
    /// Can also be assigned to RestartButton OnClick event.
    /// </summary>
    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}



