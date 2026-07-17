using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProbabilityFruit : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image fruitImage;
    [SerializeField] private TextMeshProUGUI countText;
    
    [Header("Fruit Settings")]
    [SerializeField] private Sprite fruitSprite;
    [SerializeField] private int minCount = 1;
    [SerializeField] private int maxCount = 10; // Included in the random range

    void Start()
    {
        // Set the sprite
        if (fruitImage != null && fruitSprite != null)
        {
            fruitImage.sprite = fruitSprite;
        }
        else if (fruitImage == null)
        {
            Debug.LogWarning("Fruit Image is not assigned on " + gameObject.name);
        }

        // Generate and set a random count
        if (countText != null)
        {
            // Random.Range for integers is exclusive for the max, so we add 1
            int randomCount = Random.Range(minCount, maxCount + 1);
            countText.text = randomCount.ToString();
        }
        else
        {
            Debug.LogWarning("Count Text (TMP) is not assigned on " + gameObject.name);
        }
    }
}
