using System.Collections;
using UnityEngine;

public class ProbabilityAnimation : MonoBehaviour
{
    [SerializeField] private Animator bagButtonAnimator;
    [SerializeField] private Animator buyerAnimator;
    [SerializeField] private Animator sellerAnimator;
    [SerializeField] private GameObject questionPanel;

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

        // Wait a frame to allow the animator to transition to the "Question" state
        yield return null;

        // Wait until the "Question" animation finishes playing
        while (sellerAnimator.GetCurrentAnimatorStateInfo(0).IsName("Question") && 
               sellerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        Debug.Log("Question animation finished. Enabling Bag Button.");

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
}
