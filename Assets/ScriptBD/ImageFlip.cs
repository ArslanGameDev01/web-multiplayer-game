using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class ImageFlip : MonoBehaviour
{
    [SerializeField] private GameObject frontPanel; // Front panel with multiple components
    [SerializeField] private GameObject backPanel; // Back panel with multiple components
    [SerializeField] private Button flipButton; // Button to flip to back
    [SerializeField] private Button closeButton; // Button to flip to front
    [SerializeField] private float flipDuration = 0.5f; // Duration of flip animation
    [SerializeField] private Ease flipEase = Ease.InOutQuad; // Animation ease type

    private CanvasGroup frontCanvasGroup;
    private CanvasGroup backCanvasGroup;
    private bool isFrontShowing = true; // Tracks which panel is visible
    private bool isFlipping = false; // Prevents multiple flips

    private void Awake()
    {
        // Get CanvasGroup components
        frontCanvasGroup = frontPanel.GetComponent<CanvasGroup>();
        backCanvasGroup = backPanel.GetComponent<CanvasGroup>();

        // Validate components
        if (frontPanel == null || backPanel == null) Debug.LogError("Front or Back Panel not assigned!");
        if (frontCanvasGroup == null) Debug.LogError("FrontPanel missing CanvasGroup!");
        if (backCanvasGroup == null) Debug.LogError("BackPanel missing CanvasGroup!");
        if (flipButton == null) Debug.LogError("FlipButton not assigned!");
        if (closeButton == null) Debug.LogError("CloseButton not assigned!");

        // Initialize panel states
        frontCanvasGroup.alpha = 1f;
        frontCanvasGroup.interactable = true;
        frontCanvasGroup.blocksRaycasts = true;

        backCanvasGroup.alpha = 0f;
        backCanvasGroup.interactable = false;
        backCanvasGroup.blocksRaycasts = false;

        // Set up button listeners
        flipButton.onClick.AddListener(OnFlipButtonClicked);
        closeButton.onClick.AddListener(OnCloseButtonClicked);

        // If closeButton is separate (not on BackPanel), deactivate it initially
        if (closeButton.transform.parent != backPanel.transform)
            closeButton.gameObject.SetActive(false);
    }
    

    private void OnFlipButtonClicked()
    {
        if (isFlipping) return; // Prevent multiple flips
        FlipPanel(toBack: true);
    }

    private void OnCloseButtonClicked()
    {
        if (isFlipping) return; // Prevent multiple flips
        FlipPanel(toBack: false);
    }

    private void FlipPanel(bool toBack)
    {
        isFlipping = true;

        // Create a DOTween sequence for the flip animation
        Sequence flipSequence = DOTween.Sequence();

        // First half: Rotate to 90 degrees and fade out current panel
        float targetAngle = toBack ? 90f : -90f;
        flipSequence.Append(transform.DORotate(new Vector3(0, targetAngle, 0), flipDuration / 2)
            .SetEase(flipEase));
        flipSequence.Join(toBack ? frontCanvasGroup.DOFade(0, flipDuration / 2)
                                : backCanvasGroup.DOFade(0, flipDuration / 2));

        // Halfway: Switch panel visibility
        flipSequence.AppendCallback(() =>
        {
            // Update panel states
            frontCanvasGroup.alpha = toBack ? 0f : 1f;
            frontCanvasGroup.interactable = !toBack;
            frontCanvasGroup.blocksRaycasts = !toBack;

            backCanvasGroup.alpha = toBack ? 1f : 0f;
            backCanvasGroup.interactable = toBack;
            backCanvasGroup.blocksRaycasts = toBack;

            // Reset rotation for second half
            transform.rotation = Quaternion.Euler(0, toBack ? -90f : 90f, 0);
            isFrontShowing = !toBack;
        });

        // Second half: Rotate to final position and fade in new panel
        flipSequence.Append(transform.DORotate(new Vector3(0, toBack ? 0 : 0, 0), flipDuration / 2)
            .SetEase(flipEase));
        flipSequence.Join(toBack ? backCanvasGroup.DOFade(1, flipDuration / 2)
                                : frontCanvasGroup.DOFade(1, flipDuration / 2));

        // On complete, update button states and reset flipping flag
        flipSequence.OnComplete(() =>
        {
            isFlipping = false;
            // If closeButton is separate, toggle buttons
            if (closeButton.transform.parent != backPanel.transform)
            {
                flipButton.gameObject.SetActive(!toBack);
                closeButton.gameObject.SetActive(toBack);
            }
        });
    }

    private void OnDestroy()
    {
        // Clean up button listeners
        flipButton.onClick.RemoveListener(OnFlipButtonClicked);
        closeButton.onClick.RemoveListener(OnCloseButtonClicked);
    }
}