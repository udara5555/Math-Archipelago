using System.Collections;
using UnityEngine;

public class ArithmeticAnimation : MonoBehaviour
{
    public Animator heroAnimator;
    public Animator shipAnimator;

    void Start()
    {
        StartCoroutine(PlayStartThenIdle(heroAnimator, "StartHero", "IdleHero"));
        StartCoroutine(PlayStartThenIdle(shipAnimator, "StartShip", "IdleShip"));
    }

    IEnumerator PlayStartThenIdle(Animator animator, string startState, string idleState)
    {
        animator.Play(startState);

        // Wait one frame for the animator state to update
        yield return null;

        // Wait until the start animation finishes (normalizedTime >= 1)
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        // Transition to idle animation (loops continuously)
        animator.Play(idleState);
    }
}
