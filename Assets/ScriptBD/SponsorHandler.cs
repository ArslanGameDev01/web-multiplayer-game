using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

public class SponsorHandler : MonoBehaviour
{
    [SerializeField] private QuestionData gameScriptable;
    [SerializeField] private GameObject sponsorPanel;
    [SerializeField] private GameObject loading;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private Image sponsorImage;
    public GameNamePrefebHandler newObj;
    public string sponserLink;
    private Sprite currentSponsorSprite;
    private int currentGameIndex = -1;
    public bool isPre;
    public GameObject closeBtn1;
    public GameObject closeBtn2;
    public GameObject visetSponserBtn;

    void Start()
    {
        if (gameScriptable == null || sponsorPanel == null || messageText == null || sponsorImage == null)
        {
            Debug.LogError("Missing components in SponsorHandler Inspector!");
            return;
        }

        sponsorPanel.SetActive(false);
    }

    public void ShowPreMessage(int gameIndex, GameNamePrefebHandler obj)
    {
        var game = gameScriptable.root.data.games[gameIndex];
        closeBtn2.SetActive(false);
        visetSponserBtn.SetActive(false);
        closeBtn1.SetActive(true);
        headerText.text = "Thanks to our sponsor!";
        loading.SetActive(true);
        /// Debug.Log("ShowPreMessage");
        currentGameIndex = gameIndex;
        isPre = true;
        newObj = obj;
        /// Debug.Log(IsValidGameIndex(gameIndex));
        if (!IsValidGameIndex(gameIndex))
        {
            //  Debug.Log("Invalid game index");
            SponserCloseBtnClick();
        }
        else
        {
            // Debug.Log("0000");
            if (game.sponser <= 0)
            {
                //  Debug.Log("01111");
                sponsorPanel.SetActive(false);
                return;
            }

            //  Debug.Log("0222");
            messageText.text = game.pre_msg ?? "No Pre-Message Available";
            LoadSponsorImageIfNeeded(game);
            sponsorPanel.SetActive(true);
        }
    }

    public void ShowPostMessage(int gameIndex)
    {
        var game = gameScriptable.root.data.games[gameIndex];
        sponserLink = game.sponsor_link;
        loading.SetActive(true);
        headerText.text = "Thanks for playing!";
        currentGameIndex = gameIndex;
        isPre = false;
        if (!IsValidGameIndex(gameIndex))
        {
            SponserCloseBtnClick();
            // Debug.Log("Invalid game index");
        }
        else
        {
            if (game.sponser <= 0)
            {
                sponsorPanel.SetActive(false);
                return;
            }

            messageText.text = game.post_msg ?? "No Post-Message Available";
            LoadSponsorImageIfNeeded(game);
            sponsorPanel.SetActive(true);
        }

        if (!string.IsNullOrEmpty(sponserLink))
        {
            closeBtn1.SetActive(false);
            closeBtn2.SetActive(true);
            visetSponserBtn.SetActive(true);
        }
        else
        {
            closeBtn2.SetActive(false);
            visetSponserBtn.SetActive(false);
            closeBtn1.SetActive(true);
        }
    }

    public void VisitSponsorBtnClick()
    {
        Application.OpenURL(sponserLink);
    }

    private bool IsValidGameIndex(int gameIndex)
    {
        //  Debug.Log("0000");
        if (gameScriptable.root.data.games[gameIndex].sponser <= 0)
        {
            return false;
        }

        // Debug.Log("2222");
        return true;
    }

    private void LoadSponsorImageIfNeeded(Game game)
    {
        //  Debug.Log("0333");
        if (currentSponsorSprite != null && sponsorImage.sprite == currentSponsorSprite)
        {
            //   Debug.Log("image loded");
            sponsorImage.gameObject.SetActive(true);
            loading.SetActive(false);
            return;
        }

        //  Debug.Log("0444");
        //  Debug.Log(game.pre_post_panel_image);
        //  Debug.Log(game.game_name);
        if (!string.IsNullOrEmpty(game.pre_post_panel_image))
        {
            //  Debug.Log("0555");
            StartCoroutine(LoadSponsorImage(game.pre_post_panel_image));
        }
        else
        {
            //   Debug.Log("0666");
            sponsorImage.gameObject.SetActive(false);
            // Debug.LogWarning($"Sponsor image URL missing for game: {game.game_name}");
        }
    }

    private IEnumerator LoadSponsorImage(string imageUrl)
    {
        //  Debug.Log(imageUrl);
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl))
        {
            //  Debug.Log("0777");
            yield return request.SendWebRequest();
            //  Debug.Log("0888");
            if (request.result == UnityWebRequest.Result.Success)
            {
                //   Debug.Log("09999");
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                if (texture != null)
                {
                    //   Debug.Log("1000");
                    currentSponsorSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f));
                    sponsorImage.sprite = currentSponsorSprite;
                    loading.SetActive(false);
                    sponsorImage.gameObject.SetActive(true);
                }
                else
                {
                    //  Debug.Log("11111");
                    sponsorImage.gameObject.SetActive(false);
                    // Debug.LogWarning($"Failed to create texture for sponsor image: {imageUrl}");
                }
            }
            else
            {
                //  Debug.Log("12222");
                sponsorImage.gameObject.SetActive(false);
                //Debug.LogWarning($"Failed to load sponsor image from {imageUrl}: {request.error}");
            }
        }
    }

    public void HideSponsorPanel()
    {
        sponsorPanel.SetActive(false);
    }

    public void SponserCloseBtnClick()
    {
        HideSponsorPanel();
        if (isPre)
        {
            newObj.selectgameBtn(currentGameIndex);
        }
        else
        {
            if(GamePlayHandler.instance.isMultiplayer)
            {
                TriviaManager.Instance.EndGameDueToTimeout();
            }
            else
            {
                GamePlayHandler.instance.EndGameDueToTimeout();
            }
            
        }
    }
}