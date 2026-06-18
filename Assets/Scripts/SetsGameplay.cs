using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Game manager for the Sets zoo game (Level 1).
/// Spawns animal cards from the data list and tracks correct placements.
/// Attach to an empty GameObject in the Sets scene.
/// </summary>
public class SetsGameplay : MonoBehaviour
{
    [Header("Animal Data")]
    [Tooltip("Define all animals here — set name, sprite, and category for each")]
    public AnimalData[] animals;

    [Header("UI References")]
    [Tooltip("The prefab for animal cards (Image + AnimalName TMP child)")]
    public GameObject animalCardPrefab;

    [Tooltip("Parent panel where animal cards are spawned")]
    public Transform animalCardsParent;

    [Tooltip("Text to show feedback like 'Correct!' or 'Try Again!'")]
    public TMP_Text feedbackText;

    [Tooltip("Text to show progress like '3/9 sorted'")]
    public TMP_Text scoreText;

    [Tooltip("Button that appears after all animals are sorted")]
    public GameObject nextLevelButton;

    [Header("Health")]
    public Image healthImage;

    [Tooltip("Assign health stage sprites (like Arithmetic game)")]
    public Sprite[] healthStages;

    int correctCount;
    int wrongCount;
    int totalAnimals;

    void Start()
    {
        correctCount = 0;
        wrongCount = 0;
        totalAnimals = animals.Length;
        UpdateScoreText();
        ClearFeedback();
        SpawnAnimalCards();

        // Hide next level button until all animals are sorted
        if (nextLevelButton != null)
            nextLevelButton.SetActive(false);
    }

    /// <summary>
    /// Shuffles the animals array and spawns a card for each animal.
    /// </summary>
    void SpawnAnimalCards()
    {
        // Shuffle the animals so they appear in random order
        ShuffleAnimals();

        foreach (AnimalData animal in animals)
        {
            // Instantiate a card from the prefab
            GameObject card = Instantiate(animalCardPrefab, animalCardsParent);
            card.SetActive(true);

            // Set the animal image
            Image cardImage = card.GetComponent<Image>();
            if (cardImage != null)
                cardImage.sprite = animal.icon;

            // Set the animal name text (child TMP_Text named "AnimalName")
            TMP_Text nameText = card.GetComponentInChildren<TMP_Text>();
            if (nameText != null)
                nameText.text = animal.animalName;

            // Attach the drag component and set its category
            DraggableAnimal draggable = card.GetComponent<DraggableAnimal>();
            if (draggable == null)
                draggable = card.AddComponent<DraggableAnimal>();

            draggable.animalCategory = animal.category;
        }
    }

    /// <summary>
    /// Fisher-Yates shuffle for the animals array.
    /// </summary>
    void ShuffleAnimals()
    {
        for (int i = animals.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            AnimalData temp = animals[i];
            animals[i] = animals[j];
            animals[j] = temp;
        }
    }

    /// <summary>
    /// Called by DraggableAnimal when an animal is placed in the correct zone.
    /// </summary>
    public void OnAnimalPlacedCorrectly()
    {
        correctCount++;
        UpdateScoreText();
        ShowFeedback("Correct!", Color.green);

        if (correctCount >= totalAnimals)
        {
            ShowFeedback("Level Complete!", Color.yellow);
            Debug.Log("Level 1 Complete! All animals sorted.");

            // Show the next level button
            if (nextLevelButton != null)
                nextLevelButton.SetActive(true);
        }
    }

    /// <summary>
    /// Called by DraggableAnimal when an animal is placed in the wrong zone.
    /// </summary>
    public void OnWrongPlacement()
    {
        wrongCount++;
        ShowFeedback("Try Again!", Color.red);

        // Update health display
        if (healthImage != null && healthStages != null &&
            wrongCount <= healthStages.Length && healthStages[wrongCount - 1] != null)
        {
            healthImage.sprite = healthStages[wrongCount - 1];
        }

        // Game over check
        if (healthStages != null && wrongCount >= healthStages.Length)
        {
            ShowFeedback("Game Over!", Color.red);
            Debug.Log("Game Over!");
            // Disable all remaining draggable cards
            DraggableAnimal[] remainingCards = FindObjectsByType<DraggableAnimal>(FindObjectsSortMode.None);
            foreach (var card in remainingCards)
            {
                card.enabled = false;
            }
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = $"{correctCount}/{totalAnimals} sorted";
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;

            // Auto-clear feedback after 1.5 seconds
            CancelInvoke(nameof(ClearFeedback));
            Invoke(nameof(ClearFeedback), 1.5f);
        }
    }

    void ClearFeedback()
    {
        if (feedbackText != null)
            feedbackText.text = "";
    }
}
