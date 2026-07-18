using System.Collections;
using UnityEngine;

public class ProbabilityAnimation : MonoBehaviour
{
    [Header("Animators")]
    [SerializeField] private Animator bagButtonAnimator;
    [SerializeField] private Animator buyerAnimator;
    [SerializeField] private Animator sellerAnimator;
    
    [Header("UI")]
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private GameObject answerPanel;

    void Start()
    {
        if (sellerAnimator != null)
        {
            StartCoroutine(PlayQuestionAfterSeller());
        }
        else
        {
            Debug.LogError("Seller Animator is not assigned in the Inspector!");
        }
    }

    private IEnumerator PlayQuestionAfterSeller()
    {
        // Wait until Animator state is fully initialized
        yield return null;

        // Wait until the "Seller" animation finishes playing
        while (sellerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Seller") && 
               sellerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        Debug.Log("Seller animation finished. Attempting to enable Question Panel.");

        // Enable the Question Panel
        if (questionPanel != null)
        {
            questionPanel.SetActive(true);
            Debug.Log("Question Panel set to active!");
        }
        else
        {
            Debug.LogError("Question Panel is not assigned in the Inspector! Please assign it.");
        }

        // Play the "Question" animation
        sellerAnimator.Play("Question");
    }

    // Assign this method to your Next Button's OnClick event in the Inspector
    public void OnNextButtonClicked()
    {
        Debug.Log("Next button clicked. Enabling Bag Button.");

        if (bagButtonAnimator != null)
        {
            // Enable the Bag Button game object
            bagButtonAnimator.gameObject.SetActive(true);
            
            // Play the "Bag" animation clip
            bagButtonAnimator.Play("Bag");
        }
        else
        {
            Debug.LogError("Bag Button Animator is not assigned in the Inspector!");
        }
    }

    // Assign this method to your Bag Button's OnClick event
    public void OnBagButtonClicked()
    {
        Debug.Log("Bag button clicked (Animation script). Enabling Answer Panel and playing Answer animation on Buyer.");

        if (answerPanel != null)
        {
            answerPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Answer Panel is not assigned in the Inspector!");
        }

        if (buyerAnimator != null)
        {
            buyerAnimator.Play("Answer");
        }
        else
        {
            Debug.LogError("Buyer Animator is not assigned in the Inspector!");
        }
    }

    [Header("Picked Fruit")]
    [SerializeField] private GameObject pickedFruitImageObject;
    [SerializeField] private UnityEngine.UI.Image pickedFruitImage;
    [SerializeField] private Animator pickedFruitAnimator;

    public void PlayPickedFruitAnimation(Sprite fruitSprite)
    {
        // 1. Set the sprite
        if (pickedFruitImage != null)
        {
            pickedFruitImage.sprite = fruitSprite;
        }

        // 2. Enable the game object
        if (pickedFruitImageObject != null)
        {
            pickedFruitImageObject.SetActive(true);
        }

        // 3. Play the animation
        if (pickedFruitAnimator != null)
        {
            pickedFruitAnimator.Play("PickedFruitAnimation", 0, 0f); 
        }
    }
}
