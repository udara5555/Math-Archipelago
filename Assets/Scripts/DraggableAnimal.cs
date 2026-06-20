using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach to each Animal card prefab instance.
/// Allows the player to drag the animal and drop it onto a DropZone.
/// In Level 2, the animal must be placed in BOTH Venn diagrams before it's removed from the cards panel.
/// </summary>
public class DraggableAnimal : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public string animalCategory;  // Set by SetsGameplay when spawning
    [HideInInspector] public AnimalData animalData;    // Full data for Level 2 attribute checking

    Canvas parentCanvas;
    RectTransform rectTransform;
    CanvasGroup canvasGroup;
    Vector2 originalPosition;
    Vector3 originalScale;

    // Track which Venn diagrams this animal has been placed in (Level 2)
    HashSet<int> placedInDiagrams = new HashSet<int>();

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

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
                // Check if the drop is in the intersection (overlap) of two Venn circles
                bool isInIntersection = false;
                if ((zone.vennRegion == VennRegion.SetA || zone.vennRegion == VennRegion.SetB)
                    && zone.pairedZone != null)
                {
                    isInIntersection = zone.pairedZone.IsPointInsideCircle(
                        eventData.position, eventData.pressEventCamera);
                }

                if (zone.IsCorrectZoneForAnimal(animalData, isInIntersection))
                {
                    // --- LEVEL 1 (CategoryZone): place and remove immediately ---
                    if (zone.vennRegion == VennRegion.CategoryZone)
                    {
                        rectTransform.localScale = new Vector3(zone.placedCardScale, zone.placedCardScale, 1f);
                        canvasGroup.blocksRaycasts = false;
                        enabled = false;

                        SetsGameplay manager = FindFirstObjectByType<SetsGameplay>();
                        if (manager != null)
                            manager.OnAnimalPlacedCorrectly();
                        return;
                    }

                    // --- LEVEL 2 (Venn zones): track per-diagram placement ---

                    // Check if already placed in THIS diagram
                    if (placedInDiagrams.Contains(zone.vennDiagramIndex))
                    {
                        // Already placed in this diagram — return to original position silently
                        rectTransform.anchoredPosition = originalPosition;
                        return;
                    }

                    // Create a scaled copy at the drop position inside the diagram
                    CreatePlacedCopy(zone.placedCardScale);

                    // Track this diagram as completed
                    placedInDiagrams.Add(zone.vennDiagramIndex);

                    // Notify the game manager
                    SetsGameplay mgr = FindFirstObjectByType<SetsGameplay>();
                    if (mgr != null)
                        mgr.OnAnimalPlacedCorrectly();

                    // Return original card to the cards panel
                    rectTransform.anchoredPosition = originalPosition;

                    // If placed in BOTH Venn diagrams, remove from cards panel
                    if (placedInDiagrams.Count >= 2)
                    {
                        Destroy(gameObject);
                    }

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

    /// <summary>
    /// Creates a small non-draggable copy of this animal card at the current drop position.
    /// The copy stays in the Venn diagram as a visual marker.
    /// </summary>
    void CreatePlacedCopy(float scale)
    {
        // Create a new GameObject with Image for the placed animal
        GameObject copy = new GameObject(animalData.animalName + "_placed");
        copy.transform.SetParent(transform.parent, false);

        // Copy the RectTransform position
        RectTransform copyRect = copy.AddComponent<RectTransform>();
        copyRect.anchoredPosition = rectTransform.anchoredPosition;
        copyRect.sizeDelta = rectTransform.sizeDelta;
        copyRect.localScale = new Vector3(scale, scale, 1f);

        // Copy the animal sprite
        Image copyImage = copy.AddComponent<Image>();
        Image originalImage = GetComponent<Image>();
        if (originalImage != null)
        {
            copyImage.sprite = originalImage.sprite;
            copyImage.preserveAspect = true;
        }

        // Copy the name text if it exists
        TMP_Text originalText = GetComponentInChildren<TMP_Text>();
        if (originalText != null)
        {
            GameObject textObj = new GameObject("AnimalName");
            textObj.transform.SetParent(copy.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            RectTransform origTextRect = originalText.GetComponent<RectTransform>();
            textRect.anchorMin = origTextRect.anchorMin;
            textRect.anchorMax = origTextRect.anchorMax;
            textRect.anchoredPosition = origTextRect.anchoredPosition;
            textRect.sizeDelta = origTextRect.sizeDelta;

            TMP_Text copyText = textObj.AddComponent<TextMeshProUGUI>();
            copyText.text = originalText.text;
            copyText.fontSize = originalText.fontSize;
            copyText.alignment = originalText.alignment;
            copyText.color = originalText.color;
        }

        // Make the copy non-interactive
        CanvasGroup copyGroup = copy.AddComponent<CanvasGroup>();
        copyGroup.blocksRaycasts = false;
    }
}
