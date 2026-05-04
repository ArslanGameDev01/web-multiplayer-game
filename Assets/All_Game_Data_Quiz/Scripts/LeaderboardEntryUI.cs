using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardEntryUI : MonoBehaviour
{
    public Image currentEntry;
    public Image RankBG;
    public Sprite firstRankSprite;
    public Sprite secondRankSprite;
    public Sprite thirdRankSprite;
    public Sprite currentRankSprite;
    public TMP_Text playerNameText;
    public TMP_Text scoreText;
    public TMP_Text rankText;
    public Image avatarImage;
    
    public GameObject currentPlayerBG;
    public TMP_Text currentPlayerScoreText;

    public GameObject powerHint;
    public GameObject powerBooster;

    public Color localPlayerColor = new Color(1f, 0.8f, 0.6f); // Light Orange
    public Color topPlayerColor = new Color(0.7f, 1f, 0.7f);    // Light Green

    public Sprite[] avatarSprites;

    public void Setup(string playerName, int score, int rank, int avatarIndex, bool isLocalPlayer, bool isTopPlayer,bool isSecondPlayer,bool isThirdPlayer, bool isHint, bool isBooster)
    {
        playerNameText.text = playerName;
        scoreText.text = score.ToString();
        rankText.text = $"{rank}";
        if (avatarIndex >= 0 && avatarIndex < avatarSprites.Length)
        {
            avatarImage.sprite = avatarSprites[avatarIndex];
        }
        if (isHint)
            powerHint.SetActive(true);
        if (isBooster)
            powerBooster.SetActive(true);

        // Set color based on player type
        if (isLocalPlayer)
        {
            currentEntry.color = localPlayerColor;
            RankBG.sprite = currentRankSprite;
            
            currentPlayerScoreText.text = score.ToString();
            currentPlayerBG.SetActive(true);
            scoreText.gameObject.SetActive(false);
            //playerNameText.text = "You";
        }
        if (isTopPlayer)
        {
            currentEntry.color = topPlayerColor;
            RankBG.transform.GetChild(0).gameObject.SetActive(false);
            RankBG.sprite = firstRankSprite;
            //if (isLocalPlayer)
            //{
            //    playerNameText.text = "You";
            //}
            //else
            //{
            //    playerNameText.text = playerName;
            //}
        }

        if (isSecondPlayer)
        {
            RankBG.transform.GetChild(0).gameObject.SetActive(false);
            RankBG.sprite = secondRankSprite;
        }

        if (isThirdPlayer)
        {
            RankBG.transform.GetChild(0).gameObject.SetActive(false);
            RankBG.sprite = thirdRankSprite;
        }
            

    }

    public void ShowFullScore()
    {
        TriviaManager.Instance.gameOverPanel.SetActive(true);
    }
}
