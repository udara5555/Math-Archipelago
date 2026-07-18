using UnityEngine;
using UnityEngine.UI;

public class ProbabilityAnswer : MonoBehaviour
{
    [Header("Target Fruit Link")]
    [SerializeField] private ProbabilityFruit targetFruit;

    private Image answerImage;

    void Start()
    {
        // Automatically get the Image component attached to this button
        answerImage = GetComponent<Image>();

        if (answerImage != null && targetFruit != null)
        {
            // Get the sprite from the linked Fruit script and apply it to this button's image
            answerImage.sprite = targetFruit.GetFruitSprite();
        }
        else
        {
            Debug.LogWarning("Answer Image component not found, or Target Fruit is not assigned on " + gameObject.name);
        }

        // Add a click listener to the button
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OnAnswerClicked);
        }
    }

    private void OnAnswerClicked()
    {
        ProbabilityGameplay gameplay = FindFirstObjectByType<ProbabilityGameplay>();
        if (gameplay != null)
        {
            gameplay.CheckAnswer(targetFruit);
        }
        else
        {
            Debug.LogError("Could not find ProbabilityGameplay in the scene!");
        }
    }
}
