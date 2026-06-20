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

    [Tooltip("Which Venn diagram this zone belongs to (1 or 2). Used to track per-diagram placement.")]
    public int vennDiagramIndex = 1;

    [Header("Layout Settings")]
    [Tooltip("Scale for animal cards when placed (e.g. 0.1 = 10% of original size)")]
    public float placedCardScale = 0.1f;

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

        // Use rect.center to account for any pivot offset
        Vector2 fromCenter = localPoint - rect.rect.center;
        float radius = Mathf.Min(rect.rect.width, rect.rect.height) * 0.5f;
        return fromCenter.magnitude <= radius;
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
}
