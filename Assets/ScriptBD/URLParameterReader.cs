using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class URLParameterReader : MonoBehaviour
{
    public static URLParameterReader Instance;
    public bool isTest;
    public string URL;
    public string quizValue;
    public QuestionData questionData;
    public TMP_Text gameNameText;
    public Image gameImageRefrence;
    public GameObject urlGameStartPanel;
    public GameObject urlGameInfoPanel;
    private string url;
    public TMP_Text gameInfoText;
    public List<GameObject> agerangeList;
    public List<GameObject> categoryList;
    int gameN0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
       Reset();
    }

    public void Reset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void StartGameNew()
    {
        if (isTest)
        {
            quizValue = GetQueryParameter("quiz");
            //Debug.Log("Quiz parameter value: " + quizValue);
            StartCoroutine(CompairString());
        }
        else
        {
            ///#if UNITY_WEBGL && !UNITY_EDITOR
            //Debug.Log("1211212121212121221");
            quizValue = GetQueryParameter("quiz");
            //Debug.Log("Quiz parameter value: " + quizValue);
            StartCoroutine(CompairString());
//#endif
        }
    }

    string GetQueryParameter(string key)
    {
        url = isTest ? URL : Application.absoluteURL;
        //Debug.Log("url== " + url);
        if (string.IsNullOrEmpty(url))
            return null;

        int index = url.IndexOf('?');
        if (index == -1)
            return null;

        string queryString = url.Substring(index + 1);
        string[] parameters = queryString.Split('&');

        foreach (string param in parameters)
        {
            string[] pair = param.Split('=');
            if (pair.Length == 2 && pair[0] == key)
            {
                string rawValue = WWW.UnEscapeURL(pair[1]);
                return rawValue.Replace("-", " ").ToLower();
            }
        }

        return null;
    }

    IEnumerator CompairString()
    {
        // Debug.Log("00000000000");
        GamePlayHandler.instance.checkPanel.SetActive(true);
        var games = questionData.root.data.games;
        yield return new WaitForSeconds(1f);
        if (games.Count != GamePlayHandler.instance.apiDataFetching.contentObj.Count)
        {
            //     Debug.Log("111111111111");
            StartCoroutine(CompairString());
        }
        else
        {
            for (int i = 0; i < games.Count; i++)
            {
                var game = games[i];

                if (!string.IsNullOrEmpty(game.game_name) && game.game_name.ToLower() == quizValue)
                {
                    gameN0 = i;
                    questionData.gameNumber = gameN0;
                    gameNameText.text = game.game_name;
                    Debug.Log("Matching game found: " + game.game_name);
                    Debug.Log("i== " + i);
                    // Call the desired function here
                    // GamePlayHandler.instance.sponsorHandler.ShowPreMessage(i,
                    //     GamePlayHandler.instance.apiDataFetching.contentObj[i]);
                    StopCoroutine(CompairString());
                    StartCoroutine(GetGameImage(game.image));
                    //  Debug.Log("222222222");
                    break;
                }
                else
                {
                    //   Debug.Log("3333333333");
                    GamePlayHandler.instance.checkPanel.SetActive(false);
                }

                //  Debug.Log("444444444444");
            }

            //  Debug.Log("55555555555");
            questionData.isFirstTime = true;
        }


        // Debug.LogWarning("No matching game found for: " + quizValue);
        // Debug.LogWarning("No matching game found for: " + quizValue);
    }

    public IEnumerator GetGameImage(string imageURL)
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
                        gameImageRefrence.sprite = sp;
                        gameImageRefrence.gameObject.SetActive(true);
                        GamePlayHandler.instance.checkPanel.SetActive(false);
                        urlGameStartPanel.SetActive(true);
                        GamePlayHandler.instance.isLink=true;
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
                    gameImageRefrence.gameObject.SetActive(false); // Optional fallback handling
                }
            }
        }
        else
        {
            Debug.LogWarning($"Image URL is empty for game:");
        }
    }

    public void LinkPanelStartBtnClick()
    {
        //urlGameStartPanel.SetActive(false);
        GamePlayHandler.instance.sponsorHandler.ShowPreMessage(gameN0,
            GamePlayHandler.instance.apiDataFetching.contentObj[gameN0]);
    }

    public void LinkPanelInfoBtnClick()
    {
        for (int k = 0; k < agerangeList.Count; k++)
        {
            agerangeList[k].SetActive(false);
        }

        for (int l = 0; l < agerangeList.Count; l++)
        {
            categoryList[l].SetActive(false);
        }

        var gamedata = questionData.root.data.games[gameN0];
        gameInfoText.text = gamedata.short_description;

        for (int j = 0; j < gamedata.ageranges.Count; j++)
        {
            agerangeList[j].SetActive(true);
            agerangeList[j].transform.GetChild(0).GetComponent<TMP_Text>().text =
                gamedata.ageranges[j].min_age + "-" + gamedata.ageranges[j].max_age;
        }

        for (int j = 0; j < gamedata.categories.Count; j++)
        {
            categoryList[j].SetActive(true);
            categoryList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = gamedata.categories[j].title;
        }

        urlGameStartPanel.SetActive(false);
        urlGameInfoPanel.SetActive(true);
    }
}