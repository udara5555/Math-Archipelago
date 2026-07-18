using UnityEngine;
using TMPro;

public class ProbabilityGameplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GameObject fruitPanel;

    [SerializeField] private ProbabilityFruit[] allFruits;
    [SerializeField] private UnityEngine.UI.Button randomPickButton;
    
    private int currentQuestionType = 0; // 0 = Most, 1 = Least

    void Start()
    {
        // Set sample text when the game starts
        if (questionText != null)
        {
            questionText.text = "Hey kiddo... If you answer my questions correctly, I'll give you a fruit bag for free!";
        }
        else
        {
            Debug.LogError("Question Text (TMP) is not assigned in the Inspector!");
        }
        
        // Ensure random pick button is disabled initially
        if (randomPickButton != null)
        {
            randomPickButton.interactable = false;
        }
    }

    // Assign this method to the Bag Button's OnClick event
    public void OnBagButtonClicked()
    {
        Debug.Log("Bag button clicked. Enabling Fruit Panel.");
        if (fruitPanel != null)
        {
            fruitPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Fruit Panel is not assigned in the Inspector!");
        }
    }

    // Assign this method to the Next Button's OnClick event
    public void OnNextButtonClicked()
    {
        if (questionText != null)
        {
            // Pick a random question type (0 = Most, 1 = Least)
            currentQuestionType = Random.Range(0, 2); 
            
            if (currentQuestionType == 0)
            {
                questionText.text = "If you close your eyes and reach into the bag, which fruit is MOST likely to be picked?";
            }
            else
            {
                questionText.text = "If you close your eyes and reach into the bag, which fruit is LEAST likely to be picked?";
            }
        }
    }

    public void CheckAnswer(ProbabilityFruit selectedFruit)
    {
        if (allFruits == null || allFruits.Length == 0)
        {
            Debug.LogError("All Fruits array is not assigned or empty in ProbabilityGameplay!");
            return;
        }

        bool isCorrect = true;
        int selectedCount = selectedFruit.GetFruitCount();

        // Check if the selected fruit satisfies the condition (Most or Least)
        foreach (var fruit in allFruits)
        {
            if (currentQuestionType == 0) // Most probable (max count)
            {
                if (fruit.GetFruitCount() > selectedCount)
                {
                    isCorrect = false;
                    break;
                }
            }
            else // Least probable (min count)
            {
                if (fruit.GetFruitCount() < selectedCount)
                {
                    isCorrect = false;
                    break;
                }
            }
        }

        if (isCorrect)
        {
            Debug.Log("Correct Answer! Enabling Random Pick button.");
            if (randomPickButton != null)
            {
                randomPickButton.interactable = true;
            }
        }
        else
        {
            Debug.Log("Wrong Answer. Try again!");
        }
    }
}
