
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameNamePrefebHandler : MonoBehaviour
{
    public Image gameIconImage;
    public Image sponserImage;
    public TMP_Text gameNameText;
    public TMP_Text gameInfoText;
    public string gameName;
    public Sprite gameIcon;
    //public Sprite sponserSprite;
    public Button btn;
    public GameObject selectImage;
  //  public TMP_Text ageText;
 //   public TMP_Text categoryText;
    public TMP_Text sponserText;
    public List<GameObject> agerangeList;
    public List<GameObject> categoryList;
  //  public GameObject agerangeTextPrefeb;
  //  public GameObject ageRangeContent;
  //  public GameObject categoryTextPrefeb;
  //  public GameObject categoryContent;
    public List<Agerange> ageranges = new List<Agerange>();
   // public string minAge;
    //public string maxAge;
    private void OnEnable()
    {
        gameNameText.text = gameName;
        gameIconImage.sprite = gameIcon;
    }

    public void selectgameBtn(int no)
    {
        // Debug.Log("no=="+ no);
        GamePlayHandler.instance.selectedGame = no;
        GamePlayHandler.instance.questionData.gameNumber = no;
        GamePlayHandler.instance.questionData.gameName = gameName;
        GamePlayHandler.instance.apiDataSender.game_name = gameName;
        selectImage.SetActive(true);
        
        //if (GamePlayHandler.instance.isLink && !GamePlayHandler.instance.questionData.isFirstTime)
        if (GamePlayHandler.instance.urlStartPanelModeSelection.activeSelf)
        {
            // GamePlayHandler.instance.timerPanel.SetActive(true);
            // GamePlayHandler.instance.urlStartPanelModeSelection.SetActive(false);
            
            if (GamePlayHandler.instance.isMultiplayer)
            {
                GamePlayHandler.instance.OnlineMode();
            }
            else
            {
                GamePlayHandler.instance.timerPanel.SetActive(true);
                GamePlayHandler.instance.urlStartPanelModeSelection.SetActive(false);
                
                //GamePlayHandler.instance.timerPanel.SetActive(true);
                //GamePlayHandler.instance.OfflineMode();
            }
        }
        else
        {
            GamePlayHandler.instance.modeSelectionPanel.SetActive(true);
            GamePlayHandler.instance.selectedGameImgMS.gameObject.SetActive(false);
            GamePlayHandler.instance.selectedGameImgMS.gameObject.SetActive(false);
            StartCoroutine(SetGameImage(GamePlayHandler.instance.questionData.root.data.games[no].image));
            //GamePlayHandler.instance.selectedGameImgMS.sprite = GamePlayHandler.instance.questionData.root.data.games[no].image;
        }
        
    }
    public IEnumerator SetGameImage(string imageURL)
    {
        if (!string.IsNullOrEmpty(imageURL))
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success && request.downloadHandler.data != null)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                    if (texture != null)
                    {
                        Sprite sp = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                            new Vector2(0.5f, 0.5f));
                        GamePlayHandler.instance.selectedGameImgMS.sprite = sp;
                        GamePlayHandler.instance.selectedGameImgMS.gameObject.SetActive(true);
                        //GamePlayHandler.instance.selectedGameImgMS.gameObject.SetActive(true);
                        //GamePlayHandler.instance.checkPanel.SetActive(false);
                        //urlGameStartPanel.SetActive(true);
                        //GamePlayHandler.instance.isLink=true;
                    }
                    else
                    {
                        Debug.LogWarning($"Texture is null for game:");
                    }
                }
                else
                {
                    Debug.LogWarning(
                        $"Failed to download image for game: ");
                    //gameImageRefrence.gameObject.SetActive(false); // Optional fallback handling
                }
            }
        }
        else
        {
            Debug.LogWarning($"Image URL is empty for game:");
        }
    }
    
}