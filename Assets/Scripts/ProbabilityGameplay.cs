using UnityEngine;
using TMPro;

public class ProbabilityGameplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private GameObject fruitPanel;

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
}
