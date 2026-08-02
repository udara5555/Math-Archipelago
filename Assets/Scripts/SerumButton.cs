using UnityEngine;
using UnityEngine.UI;

public class SerumButton : MonoBehaviour
{
    [Header("Serum Configuration")]
    [Tooltip("The color associated with this serum button.")]
    public SerumColor serumColor;

    [Header("UI References")]
    [Tooltip("The Answers GameObject to enable when clicked. Auto-assigned if left null.")]
    public GameObject answersGameObject;

    [Tooltip("If true, only enables Answers when clicked color matches the wizard's target color. If false, enables Answers on click regardless.")]
    public bool checkMatchingTargetColor = true;

    private Button button;
    private FractionsGameplay fractionsGameplay;

    void Start()
    {
        // Get or auto-bind Button component
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnSerumButtonClicked);
        }

        // Auto-find Answers GameObject if not assigned
        if (answersGameObject == null)
        {
            // Search sibling in parent (e.g. Table -> Answers)
            if (transform.parent != null)
            {
                Transform found = transform.parent.Find("Answers");
                if (found != null)
                {
                    answersGameObject = found.gameObject;
                }
            }

            // Fallback: search anywhere in scene
            if (answersGameObject == null)
            {
                answersGameObject = GameObject.Find("Answers");
            }
        }

        // Hide Answers initially if found
        if (answersGameObject != null)
        {
            answersGameObject.SetActive(false);
        }

        // Find reference to main FractionsGameplay script
        fractionsGameplay = FindFirstObjectByType<FractionsGameplay>();
    }

    /// <summary>
    /// Called when player clicks this serum button. Enables the Answers GameObject.
    /// </summary>
    public void OnSerumButtonClicked()
    {
        if (fractionsGameplay == null)
        {
            fractionsGameplay = FindFirstObjectByType<FractionsGameplay>();
        }

        bool shouldEnable = true;

        if (checkMatchingTargetColor && fractionsGameplay != null)
        {
            // Enable when clicked serum color matches the wizard's target color
            shouldEnable = (serumColor == fractionsGameplay.targetColor);
        }

        if (shouldEnable && answersGameObject != null)
        {
            answersGameObject.SetActive(true);
        }
    }
}

