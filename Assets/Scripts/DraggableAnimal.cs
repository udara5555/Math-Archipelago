using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Attach to each Animal card prefab instance.
/// Allows the player to drag the animal and drop it onto a DropZone.
/// </summary>
public class DraggableAnimal : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public string animalCategory;  // Set by SetsGameplay when spawning

    Canvas parentCanvas;
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector2 originalPosition;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // Add CanvasGroup if not already present (needed to allow raycasts to pass through while dragging)
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // Find the parent canvas for proper position calculations
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Store original position so we can return if drop fails
        originalPosition = rectTransform.anchoredPosition;

        // Make semi-transparent while dragging
        canvasGroup.alpha = 0.7f;

        // Allow raycast to pass through this object so DropZone can detect the drop
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the card with the mouse/finger
        rectTransform.anchoredPosition += eventData.delta / parentCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Restore full opacity
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // Check if we dropped onto a valid DropZone
        GameObject dropTarget = eventData.pointerCurrentRaycast.gameObject;

        if (dropTarget != null)
        {
            DropZone zone = dropTarget.GetComponent<DropZone>();

            // Also check parent in case we hit the zone's label text
            if (zone == null && dropTarget.transform.parent != null)
                zone = dropTarget.transform.parent.GetComponent<DropZone>();

            if (zone != null)
            {
                if (zone.IsCorrectZone(animalCategory))
                {
                    // Correct! Snap to zone and disable further dragging
                    transform.SetParent(zone.transform);
                    rectTransform.anchoredPosition = Vector2.zero;
                    canvasGroup.blocksRaycasts = false; // Don't interfere with future drops

                    // Notify the game manager
                    SetsGameplay manager = FindFirstObjectByType<SetsGameplay>();
                    if (manager != null)
                        manager.OnAnimalPlacedCorrectly();

                    // Disable this drag component
                    enabled = false;
                    return;
                }
            }
        }

        // Wrong drop or missed — return to original position
        rectTransform.anchoredPosition = originalPosition;

        // Notify the game manager about the wrong attempt
        SetsGameplay manager2 = FindFirstObjectByType<SetsGameplay>();
        if (manager2 != null)
            manager2.OnWrongPlacement();
    }
}
