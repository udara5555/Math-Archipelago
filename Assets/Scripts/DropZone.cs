using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Which animal attribute this zone checks against.
/// </summary>
public enum AnimalAttribute
{
    None,
    HaveFur,
    CanFly,
    ProduceMilk,
    HaveFins
}

/// <summary>
/// What region of the Venn diagram this zone represents.
/// </summary>
public enum VennRegion
{
    CategoryZone,   // Level 1: uses zoneCategory string matching
    SetA,           // Circle A in the Venn diagram
    SetB,           // Circle B in the Venn diagram
    Outside         // Outside both circles (neither attribute)
}

/// <summary>
/// Attach to each drop zone circle.
/// Supports both Level 1 (category matching) and Level 2 (Venn diagram attributes).
/// Intersection is detected automatically when circles overlap.
/// </summary>
public class DropZone : MonoBehaviour, IDropHandler
{
    void Start()
    {
        // Make the drop zone only respond to clicks/drops inside the visible circle,
        // not the full rectangular bounds. Requires Read/Write enabled on the sprite.
        Image img = GetComponent<Image>();
        if (img != null)
            img.alphaHitTestMinimumThreshold = 0.5f;
    }

    [Header("Level 1 - Category Zone")]
    [Tooltip("The category this zone accepts, e.g. Mammal, Bird, Fish (Level 1 only)")]
    public string zoneCategory;

    [Header("Level 2 - Venn Diagram Settings")]
    [Tooltip("What region of the Venn diagram this zone represents")]
    public VennRegion vennRegion = VennRegion.CategoryZone;

    [Tooltip("First attribute (Set A / Set X)")]
    public AnimalAttribute attributeA;

    [Tooltip("Second attribute (Set B / Set Y)")]
    public AnimalAttribute attributeB;

    [Tooltip("The other circle in this Venn diagram (for automatic intersection detection)")]
    public DropZone pairedZone;

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
    /// Level 1: Checks whether an animal belongs in this zone by category string.
    /// </summary>
    public bool IsCorrectZone(string animalCategory)
    {
        return zoneCategory == animalCategory;
    }

    /// <summary>
    /// Level 2: Checks whether an animal belongs in this Venn diagram zone by attributes.
    /// If isInIntersection is true, the animal must have BOTH attributes.
    /// </summary>
    public bool IsCorrectZoneForAnimal(AnimalData animal, bool isInIntersection = false)
    {
        // Level 1: use category string
        if (vennRegion == VennRegion.CategoryZone)
            return zoneCategory == animal.category;

        // Level 2: check attributes
        bool hasA = GetAttribute(animal, attributeA);
        bool hasB = GetAttribute(animal, attributeB);

        // If dropped in the intersection area (overlap of both circles)
        if (isInIntersection)
            return hasA && hasB;

        // Dropped in a single circle only (not in overlap)
        switch (vennRegion)
        {
            case VennRegion.SetA:
                return hasA && !hasB;
            case VennRegion.SetB:
                return !hasA && hasB;
            case VennRegion.Outside:
                return !hasA && !hasB;
            default:
                return false;
        }
    }

    /// <summary>
    /// Checks if a screen point falls inside this zone's circle.
    /// Used to detect intersection (overlap) between two circles.
    /// </summary>
    public bool IsPointInsideCircle(Vector2 screenPoint, Camera cam)
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, screenPoint, cam, out localPoint);

        // Check distance from center against the circle radius
        float radius = Mathf.Min(rect.rect.width, rect.rect.height) * 0.5f;
        return localPoint.magnitude <= radius;
    }

    /// <summary>
    /// Returns the value of a specific attribute from the animal data.
    /// </summary>
    bool GetAttribute(AnimalData animal, AnimalAttribute attr)
    {
        switch (attr)
        {
            case AnimalAttribute.HaveFur: return animal.haveFur;
            case AnimalAttribute.CanFly: return animal.canFly;
            case AnimalAttribute.ProduceMilk: return animal.produceMilk;
            case AnimalAttribute.HaveFins: return animal.haveFins;
            default: return false;
        }
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
