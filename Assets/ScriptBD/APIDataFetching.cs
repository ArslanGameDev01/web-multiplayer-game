using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class APIDataFetching : MonoBehaviour
{
    public bool isTetsing;

    // UI elements assigned in Inspector
    public GamePlayHandler gamePlayHandler;
    public GameFilterHandler gameFilterHandler;
    public URLParameterReader urlParameterReader;
    [SerializeField] private QuestionData gameScriptable; // Stores game data
    public UseQuestion useQuestion;
    [SerializeField] private GameObject loadingPanel; // Loading screen
    [SerializeField] private GameObject mainMenuPanel; // Main menu screen
    [SerializeField] private GameObject errorPanel; // Error screen
    [SerializeField] private Button retryButton; // Retry button
    [SerializeField] private Slider loadingSlider; // Loading bar
    [SerializeField] private Text errorText; // Error message text
    public List<GameNamePrefebHandler> contentObj = new List<GameNamePrefebHandler>(); // List of game objects

    private const int MAX_RETRY_ATTEMPTS = 3; // Max retry attempts
    private const float RETRY_DELAY = 2f; // Delay between retries
    private int currentAttempts = 0; // Current retry count
    private Texture2D texture;

    void Start()
    {
        // Check if all required components are assigned
        if (gameScriptable == null || loadingPanel == null || mainMenuPanel == null ||
            loadingSlider == null || errorPanel == null || retryButton == null)
        {
            // Debug.LogError("Missing components in Inspector!");
            ShowError("Setup Error: Missing components");
            return;
        }

        // Debug.Log("5141414");
        // Initialize UI
        retryButton.onClick.AddListener(OnRetryButtonClicked);
        loadingPanel.SetActive(true);
        errorPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        loadingSlider.value = 0f;
        //   Debug.Log("4212432");
        // Start fetching data
        gameScriptable.root.data.games.Clear();
        gameScriptable.root.data.categories.Clear();
        gameScriptable.root.data.ageranges.Clear();
        StartCoroutine(FetchData());
    }

    // Called when retry button is clicked
    public void OnRetryButtonClicked()
    {
        errorPanel.SetActive(false);
        loadingPanel.SetActive(true);
        loadingSlider.value = 0f; // Reset loading bar
        currentAttempts = 0; // Reset attempts
        StartCoroutine(FetchData());
    }

    // Show error message and stop loading
    void ShowError(string message)
    {
        //  Debug.LogError(message);
        if (errorText != null)
        {
            errorText.text = message;
            errorText.gameObject.SetActive(true);
        }

        errorPanel.SetActive(true);
        // loadingPanel.SetActive(false);
    }

    // Fetch data from API
    IEnumerator FetchData()
    {
        currentAttempts++;
        // Debug.Log($"Fetching data, attempt {currentAttempts}");
        string URl = null;
        if (isTetsing)
        {
            URl = "http://192.168.100.176:8000/api/fetch/all/game";
        }
        else
        {
            URl = "https://braindashadmin.braingamestudio.com/api/fetch/all/game";
        }

        //  Debug.Log(URl);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(URl))
        {
            // Debug.Log("000000");
            // Start loading bar animation
            StartCoroutine(UpdateLoadingBar());
            //  Debug.Log("1111111");
            yield return webRequest.SendWebRequest();
            //  Debug.Log("222222");
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // Debug.Log("3333333");
                errorPanel.SetActive(false);
                StartCoroutine(ProcessData(webRequest.downloadHandler.text));
            }
            else
            {
                //  Debug.Log("4444444");
                ShowError($"Data fetch failed (Attempt {currentAttempts} of {MAX_RETRY_ATTEMPTS})");

                if (currentAttempts < MAX_RETRY_ATTEMPTS)
                {
                    // Debug.Log("5555555");
                    yield return new WaitForSeconds(RETRY_DELAY);
                    StartCoroutine(FetchData()); // Auto-retry
                }
                else
                {
                    // Debug.Log("6666666");
                    ShowError("Failed to fetch data after 3 attempts");
                }
            }
        }
    }

    public bool isdataload;

    // Update loading bar while loading
    IEnumerator UpdateLoadingBar()
    {
        while (loadingPanel.activeSelf && !errorPanel.activeSelf && !isdataload)
        {
            if (loadingSlider.value < 1f)
            {
                loadingSlider.value += 0.0025f; // Slowly fill bar
            }
            else
            {
                loadingSlider.value = 0f; // Loop back to 0
            }

            yield return null;
        }
    }

    // Process fetched data
    IEnumerator ProcessData(string jsonData)
    {
        //      Debug.Log(jsonData);
        gameScriptable.root = JsonUtility.FromJson<Root>(jsonData);
        if (gameScriptable.root?.data?.games == null || gameScriptable.root.data.games.Count == 0)
        {
            ShowError("No game data received");
            yield break;
        }

        gamePlayHandler.totalgames = gameScriptable.root.data.games.Count;

        for (int i = 0; i < gamePlayHandler.totalgames; i++)
        {
            useQuestion.ResizeGameObjectList(i, gameScriptable.root.data.games[i].game_name);
        }

        foreach (var obj in contentObj)
        {
            if (obj != null) Destroy(obj.gameObject);
        }

        contentObj.Clear();

        gamePlayHandler.ForScrollerObj(gamePlayHandler.totalgames > 2);

        for (int i = 0; i < gamePlayHandler.totalgames; i++)
        {
            var gameData = gameScriptable.root.data.games[i];

            GameNamePrefebHandler obj = Instantiate(gamePlayHandler.gameNamePrefebHandler,
                gamePlayHandler.contentPanel.transform);
            contentObj.Add(obj);

            int index = i;

            var dotween = obj.GetComponent<DOTweenAnimation>();
            if (dotween != null)
                dotween.delay = 0.1f + (0.1f * i);

            var shiny = obj.GetComponent<UIShiny>();
            if (shiny != null)
            {
                shiny.rotation = -45 + (90 * i);
                shiny.duration = Random.Range(1.25f, 2f);
            }

            obj.btn.onClick.AddListener(() => GamePlayHandler.instance.sponsorHandler.ShowPreMessage(index, obj));
            obj.selectImage.SetActive(false);

            obj.gameName = gameData.game_name;
            obj.gameInfoText.text = gameData.short_description;
            obj.sponserText.text = gameData.powered_by;
            obj.ageranges = gameData.ageranges;
            for (int k = 0; k < obj.agerangeList.Count; k++)
            {
                obj.agerangeList[k].SetActive(false);
            }

            for (int l = 0; l < obj.agerangeList.Count; l++)
            {
                obj.categoryList[l].SetActive(false);
            } // obj.maxAge = gameData.max_age;

            for (int j = 0; j < obj.ageranges.Count; j++)
            {
                obj.agerangeList[j].SetActive(true);
                obj.agerangeList[j].transform.GetChild(0).GetComponent<TMP_Text>().text = obj.ageranges[j].min_age + "-" + obj.ageranges[j].max_age;
               // GameObject newageObj = Instantiate(obj.agerangeTextPrefeb, obj.ageRangeContent.transform);
             //   newageObj.GetComponent<TMP_Text>().text = obj.ageranges[j].min_age + "-" + obj.ageranges[j].max_age;
             ///   newageObj.transform.localScale = Vector3.one;
            }

            for (int j = 0; j < gameData.categories.Count; j++)
            {
                obj.categoryList[j].SetActive(true);
                obj.categoryList[j].transform.GetChild(0).GetComponent<TMP_Text>().text =gameData.categories[j].title;
                // GameObject newcategryObj = Instantiate(obj.categoryTextPrefeb, obj.categoryContent.transform);
                // newcategryObj.GetComponent<TMP_Text>().text = gameData.categories[j].title;
                // newcategryObj.transform.localScale = Vector3.one;
            }

            //  obj.categoryText.text = gameData.categories.ToString();
            obj.sponserImage.gameObject.SetActive(gameData.sponser != 0);
            //Debug.Log("isFirstTime== " + gameScriptable.isFirstTime);
            if (!gameScriptable.isFirstTime)
            {
                urlParameterReader.StartGameNew();
            }

            string imageURL = gameData.image;
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
                            obj.gameIcon = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f));
                            obj.gameIconImage.sprite = obj.gameIcon;
                            obj.gameIconImage.gameObject.SetActive(true);
                        }
                        else
                        {
                            Debug.LogWarning($"Texture is null for game: {gameData.game_name}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning(
                            $"Failed to download image for game: {gameData.game_name}, URL: {imageURL}, Error: {request.error}");
                        obj.gameIconImage.gameObject.SetActive(false); // Optional fallback handling
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Image URL is empty for game: {gameData.game_name}");
            }

            obj.gameObject.SetActive(true);
            isdataload = true;

            StartCoroutine(FinishLoading()); // Optional: move this outside the loop if it should only run once
        }
    }


    // Ensure loading bar fills before showing main menu
    IEnumerator FinishLoading()
    {
        // Debug.Log("9090909");// Fill loading bar to 1
        while (loadingSlider.value < 1f)
        {
            loadingSlider.value += 0.0025f;
            yield return null;
        }

        gameFilterHandler.UpdatedFilater();
        // Hide loading panel, show main menu
        loadingPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}