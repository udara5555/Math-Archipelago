using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to each drop zone circle.
/// Stores the category this zone accepts, and arranges dropped animals inside the circle.
/// </summary>
public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("Zone Settings")]
    [Tooltip("The category this zone accepts, e.g. Mammal, Bird, Fish")]
    public string zoneCategory;

    [Header("Layout Settings")]
    [Tooltip("Scale for animal cards when placed inside the zone (e.g. 0.2 = 20% of original size)")]
    public float placedCardScale = 0.2f;

    [Tooltip("Radius within which to arrange placed animals (fraction of zone size). 0.35 keeps them well inside the circle.")]
    public float arrangementRadius = 0.35f;

    // Track animals placed in this zone
    List<RectTransform> placedAnimals = new List<RectTransform>();

    public void OnDrop(PointerEventData eventData)
    {
        // No action here — DraggableAnimal handles the result.
        // This interface is needed so the drop zone is recognized as a valid target.
    }

    /// <summary>
    /// Checks whether an animal belongs in this zone.
    /// </summary>
    public bool IsCorrectZone(string animalCategory)
    {
        return zoneCategory == animalCategory;
    }

    /// <summary>
    /// Places an animal inside this zone and arranges all placed animals
    /// so they spread out inside the circle instead of stacking.
    /// </summary>
    public void PlaceAnimal(RectTransform animalRect)
    {
        // Reparent the animal under this zone (worldPositionStays = false to reset position)
        animalRect.SetParent(transform, false);

        // Reset anchors and pivot to center
        animalRect.anchorMin = new Vector2(0.5f, 0.5f);
        animalRect.anchorMax = new Vector2(0.5f, 0.5f);
        animalRect.pivot = new Vector2(0.5f, 0.5f);

        // Shrink the card using scale only — this always works regardless of Image settings
        animalRect.localScale = new Vector3(placedCardScale, placedCardScale, 1f);

        // Add to our tracking list
        placedAnimals.Add(animalRect);

        // Rearrange all placed animals inside the circle
        ArrangeAnimals();
    }

    /// <summary>
    /// Distributes all placed animals evenly inside the zone circle.
    /// 1 animal  = center
    /// 2+ animals = spread in a ring pattern
    /// </summary>
    void ArrangeAnimals()
    {
        RectTransform zoneRect = GetComponent<RectTransform>();
        float zoneRadius = Mathf.Min(zoneRect.rect.width, zoneRect.rect.height) * arrangementRadius;

        int count = placedAnimals.Count;

        if (count == 1)
        {
            // Single animal goes to center
            placedAnimals[0].anchoredPosition = Vector2.zero;
        }
        else
        {
            // Spread animals in a circle pattern
            float angleStep = 360f / count;

            for (int i = 0; i < count; i++)
            {
                float angle = angleStep * i * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * zoneRadius;
                float y = Mathf.Sin(angle) * zoneRadius;

                placedAnimals[i].anchoredPosition = new Vector2(x, y);
            }
        }
    }
}
