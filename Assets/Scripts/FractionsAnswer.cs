using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FractionsAnswer : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The TMP text component inside this AnswerBar. Auto-assigned if left null.")]
    public TMP_Text answerText;

    [Tooltip("The Button component on this AnswerBar. Auto-assigned if left null.")]
    public Button button;

    [Header("Answer State")]
    public bool isCorrect = false;
    public string expressionText = "";

    private FractionsGameplay gameplayManager;

    void Awake()
    {
        InitComponents();
    }

    void Start()
    {
        InitComponents();
        if (gameplayManager == null)
        {
            gameplayManager = FindFirstObjectByType<FractionsGameplay>();
        }
    }

    private void InitComponents()
    {
        if (answerText == null)
        {
            answerText = GetComponentInChildren<TMP_Text>();
        }

        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.RemoveListener(OnAnswerClicked);
            button.onClick.AddListener(OnAnswerClicked);
        }
    }

    /// <summary>
    /// Configures the answer bar text and whether this answer is correct.
    /// </summary>
    public void SetAnswer(string expression, bool correct)
    {
        expressionText = expression;
        isCorrect = correct;

        if (answerText == null)
        {
            answerText = GetComponentInChildren<TMP_Text>();
        }

        if (answerText != null)
        {
            answerText.text = expression;
        }
    }

    /// <summary>
    /// Called when the player clicks this answer bar button.
    /// </summary>
    public void OnAnswerClicked()
    {
        if (gameplayManager == null)
        {
            gameplayManager = FindFirstObjectByType<FractionsGameplay>();
        }

        if (gameplayManager != null)
        {
            gameplayManager.SelectAnswer(this);
        }
    }
}

