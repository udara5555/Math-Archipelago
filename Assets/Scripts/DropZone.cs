using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Attach to each drop zone circle (MammalZone, BirdZone, FishZone).
/// Stores the category this zone accepts and provides a method to check matches.
/// </summary>
public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("Zone Settings")]
    [Tooltip("The category this zone accepts: Mammal, Bird, or Fish")]
    public string zoneCategory;  // Set in Inspector: "Mammal", "Bird", or "Fish"

    /// <summary>
    /// Called when a draggable is dropped onto this zone.
    /// Checks if the animal's category matches this zone.
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        // No action here — the DraggableAnimal handles the result.
        // This interface is needed so the drop zone is recognized as a valid target.
    }

    /// <summary>
    /// Checks whether an animal belongs in this zone.
    /// </summary>
    public bool IsCorrectZone(string animalCategory)
    {
        return zoneCategory == animalCategory;
    }
}
