using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StarMovement : MonoBehaviour
{
    public GameObject upstar;
    public RectTransform upperStar; // Assign the upper star's RectTransform in the Inspector
    public float moveDuration = 1f; // Duration of the movement in seconds
    public float delayDuration = 1f;
    private RectTransform starRect; // The star to move (option star)
    public Vector2 startPos;
    public bool isStreak;
    void Start()
    {
    }

    private void OnEnable()
    {
        starRect = GetComponent<RectTransform>();
       // Debug.Log("876776767676");
        Invoke("MoveStarToUpperStar", delayDuration);
    }

    [ContextMenu("MoveStarToUpperStar")]
    // Call this method to start the movement
    public void MoveStarToUpperStar()
    {
        gameObject.SetActive(true);
        StartCoroutine(MoveStar());
    }

    private System.Collections.IEnumerator MoveStar()
    {
        // Get the starting position
        startPos = starRect.anchoredPosition;

        // Convert the upper star's position to the local space of the moving star's parent
        Vector2 targetPos = RectTransformUtility.WorldToScreenPoint(null, upperStar.position);
        targetPos = (Vector2)starRect.parent.GetComponent<RectTransform>().InverseTransformPoint(targetPos);

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            starRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Set the final position
        starRect.anchoredPosition = targetPos;
        upstar.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), .1f).SetEase(Ease.Linear).OnComplete(
            delegate
            {
                if (isStreak)
                {
                    if (GamePlayHandler.instance.isMultiplayer)
                        TriviaManager.Instance.UpdateStreakText();
                    else
                        GamePlayHandler.instance.UpdateStreakText();
                }
                else
                {
                    if(GamePlayHandler.instance.isMultiplayer)
                        TriviaManager.Instance.updatePointText();
                    else
                        GamePlayHandler.instance.updatePointText();

                }
                upstar.transform.DOScale(new Vector3(1f, 1f, 1f), .1f);
                StopCoroutine(MoveStar());
            });
        // Deactivate the star once it reaches the target
        gameObject.SetActive(false);
        // Reset the star to its starting position
        starRect.anchoredPosition = startPos;
    }
}