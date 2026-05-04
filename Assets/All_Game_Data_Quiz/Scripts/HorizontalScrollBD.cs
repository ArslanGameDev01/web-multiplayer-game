using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HorizontalScrollBD : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [Header("Scroll View")]
    public ScrollRect scrollRect;

    [Header("Scroll Buttons")]
    public Button leftButton;
    public Button rightButton;

    [Header("Scroll Settings")]
    [Range(0.1f, 0.5f)] public float scrollStep = 0.2f;
    public float scrollSpeed = 5f;

    private float targetPosition;
    private bool isDragging = false;

    void Start()
    {
        targetPosition = scrollRect.horizontalNormalizedPosition;
        UpdateButtonStates();
    }

    void Update()
    {
        if (!isDragging)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                targetPosition,
                Time.deltaTime * scrollSpeed
            );

            // Update buttons while scrolling
            UpdateButtonStates();
        }
        else
        {
            // Update target while dragging
            targetPosition = scrollRect.horizontalNormalizedPosition;
        }
    }

    public void ScrollLeft()
    {
        targetPosition = Mathf.Clamp01(targetPosition - scrollStep);
        UpdateButtonStates();
    }

    public void ScrollRight()
    {
        targetPosition = Mathf.Clamp01(targetPosition + scrollStep);
        UpdateButtonStates();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        targetPosition = scrollRect.horizontalNormalizedPosition;
        UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        leftButton.interactable = targetPosition > 0.01f;
        rightButton.interactable = targetPosition < 0.99f;
    }
}
