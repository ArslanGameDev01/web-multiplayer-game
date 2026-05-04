using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityCoder.Samples;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GamePlayHandler : MonoBehaviour
{
    public static GamePlayHandler instance;

    public TTS TTSData;
    public APIDataSender apiDataSender;
    public APIDataFetching apiDataFetching;
    public QuestionData questionData; // Reference to ScriptableObject
    public SponsorHandler sponsorHandler;
    public Scrollbar scrollbar;
    public GameObject checkPanel;
    public int selectedGame; // Index of the selected game in questionData.root.data.games
    public int total3XBoost = 1;
    public int totalQuestion;
    public int currentQuestion;
    public int score;
    public int streak;
    public int rightQuestion;
    public int wrongQuestion;
    public int maxQuestion;
    public int continiousRightQuestion;
    public int threeRightQuestion;
    public int sevenRightQuestion;
    public int totalSevenInRow;
    public bool isLink;
    public bool isMultiplayer;
    public Sprite questionSprite;
    public Image questionPanelImg;
    public GameObject loadingImage;
    public Image zoomPanelImg;
    public GameObject questionImgObj;
    public GameObject needleImgObj;
    public Image timefillimg;
    [Header("Question Objects")] public TMP_Text timerText;
    public TMP_Text streakText;
    public TMP_Text scoreText;
    public TMP_Text questionNOText;
    public TMP_Text questionText;
    public TMP_Text questionText2;
    public TMP_Text opt1Text;
    public TMP_Text opt2Text;
    public TMP_Text opt3Text;
    public TMP_Text opt4Text;
    public Sprite correctState;
    public Sprite incorrectState;
    public Sprite defaultSprite;
    public Sprite boosterActiveSprite;
    public Sprite boosterDefaltSprite;
    public GameNamePrefebHandler gameNamePrefebHandler;
    public GameObject contentPanel;
    public OptionHandler opt1Handler;
    public OptionHandler opt2Handler;
    public OptionHandler opt3Handler;
    public OptionHandler opt4Handler;
    public GameObject skipBtn;
    public GameObject boosterBtn;
    public float scrollStep = 0.15f;
    public float scrollSpeed = 5f;
    public bool isSkip = true;
    public bool Check;
    public bool is3XBoosterActive = false;
    public bool isGameOver = false;
    public List<GameObject> mainMenuScrollObjects;
    [Header("Loading Objects")] public Slider loadingSlider;

    [Header("GameOver Objects")] public int streakPoints;
    public int accuracyPoints;
    public int tripleDownPoints;
    public int sevanPoints;
    public int totalPoints;
    public TMP_Text streakPointText;
    public TMP_Text accuracyPointText;
    public TMP_Text tripleDownPointText;
    public TMP_Text sevanPointText;
    public TMP_Text totalPointText;
    public TMP_Text resultHeaderText;
    public TMP_Text resultdisscriptionText;
    public List<FinalResult> finalResults;
    [Header("Game Panel")] public GameObject mainMenuPanel;
    public GameObject gamePlayPanel;
    public GameObject modeSelectionPanel;
    public GameObject timerPanel;
    public GameObject gameOverPanel;
    public GameObject loadingPanel;
    public List<OptionHandler> optionHandlers;
    public List<GameObject> questionObjects;
    public List<Button> optButton;
    public List<Question> withOutTimerQuestions;
    public List<Question> withTimerQuestions;
    public List<int> use_Question_No = new List<int>();
    private bool usingTimerQuestions = false;
    private float timerDuration = 60f;
    private float currentTime;
    private Coroutine timerCoroutine;
    public int totalgames;
    
    //URL data
    public GameObject urlStartPanelModeSelection;

    public Sprite modeSelectedSprite;
    public Sprite modeNonSelectedSprite;

    public Image offlineModeImg;
    public Image OnlineModeImg;
    
    //Mode Selection Panel
    
    public Image selectedGameImgMS;

    public Image inGameOfflineModeImg;
    public Image inGameOnlineModeImg;

    //TTS
    
    private bool isTTS=false;
    public Image TTSIconImg;
    public Sprite TTSOnSprite;
    public Sprite TTSOffSprite;
    
    private void Awake()
    {
        instance = this;
    }
   
    public void onDownArrowClick()
    {
        float currentValue = scrollbar.value;
        scrollStep = scrollbar.numberOfSteps;
        if (currentValue > 0)
        {
            float targetValue = Mathf.Clamp01(currentValue - 0.25f);
            StopAllCoroutines(); // Stop any ongoing scroll animations
            StartCoroutine(SmoothScrollTo(targetValue));
        }
    }

    private IEnumerator SmoothScrollTo(float target)
    {
        while (Mathf.Abs(scrollbar.value - target) > 0.001f)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, target, Time.deltaTime * scrollSpeed);
            yield return null;
        }

        scrollbar.value = target; // Ensure exact final value
    }

    void Start()
    {
        if (withOutTimerQuestions == null)
            withOutTimerQuestions = new List<Question>();
        if (withTimerQuestions == null)
            withTimerQuestions = new List<Question>();
    }

    IEnumerator StartLoading()
    {
        yield return new WaitForSeconds(0.001f);
        if (loadingSlider.value < 1f)
        {
            loadingSlider.value += 0.0025f;
            StartCoroutine(StartLoading());
        }
        else
        {
            loadingSlider.value = 0f;
            loadingPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }
    }

    public void ForScrollerObj(bool check)
    {
        for (int i = 0; i < mainMenuScrollObjects.Count; i++)
        {
            mainMenuScrollObjects[i].SetActive(check);
        }
    }

    public UseQuestion use_questions;

    public void LoadQuestion()
    {
        score = 0;
        isGameOver = false;
        maxQuestion = 0;
        rightQuestion = 0;
        wrongQuestion = 0;
        continiousRightQuestion = 0;
        threeRightQuestion = 0;
        streak = 0;
        totalSevenInRow = 0;
        currentQuestion = 0;
        scoreText.text = score.ToString();
        streakText.text = streak.ToString();
        isSkip = true;
        skipBtn.SetActive(true);
        total3XBoost = 1;
        streakPoints = 0;
        accuracyPoints = 0;
        tripleDownPoints = 0;
        sevanPoints = 0;
        totalPoints = 0;
        boosterBtn.SetActive(true);
        // boosterBtn.GetComponent<Image>().sprite = boosterDefaltSprite;
        // boosterBtn.GetComponent<Image>().SetNativeSize();
        if (boosterBtn.TryGetComponent<Image>(out Image image))
        {
            image.sprite = boosterDefaltSprite;
            image.SetNativeSize();
        }
        is3XBoosterActive = false;

        if (questionData == null || questionData.root == null || questionData.root.data == null ||
            questionData.root.data.games == null || selectedGame < 0 ||
            selectedGame >= questionData.root.data.games.Count)
        {
            Debug.LogError("QuestionData is not assigned, invalid selectedGame index, or no game data available!");
            return;
        }

        withTimerQuestions.Clear();
        withOutTimerQuestions.Clear();

// Get questions for the selected game
        Game selectedGameData = questionData.root.data.games[selectedGame];
        string gameName = selectedGameData.game_name;
        if (selectedGameData.questions == null || selectedGameData.questions.Count == 0)
        {
            Debug.LogError($"No questions available for game: {gameName}");
            return;
        }

// Find or create the QuestionNo entry for the gameName in use_questions
        QuestionNo gameQuestionNo = use_questions.use_questions.Find(q => q.GameName == gameName);
        if (gameQuestionNo == null)
        {
            gameQuestionNo = new QuestionNo
            {
                GameName = gameName,
                q_no = new List<int>()
            };
            use_questions.use_questions.Add(gameQuestionNo);
           // Debug.Log($"Created new QuestionNo entry for game {gameName}.");
        }

        usingTimerQuestions = Check;
        totalQuestion = 0;

        List<Question> availableQuestions = new List<Question>(selectedGameData.questions);

// Calculate remaining questions
        int remainingQuestions = selectedGameData.questions.Count - gameQuestionNo.q_no.Count;

        if (usingTimerQuestions)
        {
            int questionsToPick = Mathf.Min(35, availableQuestions.Count); // Target 20 questions for withTimerQuestions

            // Check if remaining questions are fewer than required
            if (remainingQuestions < questionsToPick)
            {
             //   Debug.Log(
                //    $"Not enough remaining questions ({remainingQuestions}) for game {gameName}. Resetting used questions list.");
                gameQuestionNo.q_no.Clear();
                availableQuestions =
                    new List<Question>(selectedGameData.questions); // Refresh available questions
            }

            // Select unique questions not in gameQuestionNo.q_no
            while (withTimerQuestions.Count < questionsToPick && availableQuestions.Count > 0)
            {
                int randomIndex = Random.Range(0, availableQuestions.Count);
                int questionIndex = selectedGameData.questions.IndexOf(availableQuestions[randomIndex]);

                if (!gameQuestionNo.q_no.Contains(questionIndex))
                {
                    withTimerQuestions.Add(availableQuestions[randomIndex]);
                    gameQuestionNo.q_no.Add(questionIndex); // Add to game's q_no list
                    //Debug.Log($"Added question index {questionIndex} to withTimerQuestions for game {gameName}.");
                }

                availableQuestions.RemoveAt(randomIndex); // Remove to avoid session duplicates
            }

            totalQuestion = withTimerQuestions.Count;
            ShuffleList(withTimerQuestions); // Shuffle after selection
        }
        else
        {
            //int questionsToPick = Random.Range(12, 16);
            int questionsToPick = 10;
            questionsToPick = Mathf.Min(questionsToPick, availableQuestions.Count);

            // Check if remaining questions are fewer than required
            if (remainingQuestions < questionsToPick)
            {
              //  Debug.Log(
                 //   $"Not enough remaining questions ({remainingQuestions}) for game {gameName}. Resetting used questions list.");
                gameQuestionNo.q_no.Clear();
                availableQuestions =
                    new List<Question>(selectedGameData.questions); // Refresh available questions
            }

            // Select unique questions not in gameQuestionNo.q_no
            while (withOutTimerQuestions.Count < questionsToPick && availableQuestions.Count > 0)
            {
                int randomIndex = Random.Range(0, availableQuestions.Count);
                int questionIndex = selectedGameData.questions.IndexOf(availableQuestions[randomIndex]);

                if (!gameQuestionNo.q_no.Contains(questionIndex))
                {
                    withOutTimerQuestions.Add(availableQuestions[randomIndex]);
                    gameQuestionNo.q_no.Add(questionIndex); // Add to game's q_no list
                  //  Debug.Log($"Added question index {questionIndex} to withOutTimerQuestions for game {gameName}.");
                }

                availableQuestions.RemoveAt(randomIndex); // Remove to avoid session duplicates
            }

            totalQuestion = withOutTimerQuestions.Count;
        }

        currentQuestion = 0;
        questionNOText.text = "Question " + (currentQuestion + 1).ToString();
        maxQuestion = currentQuestion + 1;
        DisplayCurrentQuestion();
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public List<Question> GetCurrentQuestionList()
    {
        return usingTimerQuestions ? withTimerQuestions : withOutTimerQuestions;
    }

    public void TTSBtnHover()
    {
        if (Input.touchSupported && Input.touchCount > 0)
            return;
        #if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            if (TTSData != null)
            {
                TTSData.Speak(
                    isTTS ? "Turn Off" : "Read Aloud",
                    1, 1, "en-US", "Microsoft Zira - English (United States)");
            }
        }
        catch (System.EntryPointNotFoundException ex)
        {
            Debug.LogWarning("TTS function not found. Maybe JS plugin not ready yet: " + ex.Message);
        }
#else
        Debug.Log("TTS is only supported in WebGL builds.");
#endif
    }
    public void TTSMode()
    {
        if (isTTS)
        {
            isTTS = false;
            TTSIconImg.sprite = TTSOffSprite;
        }
        else
        {
            isTTS=true;
            TTSIconImg.sprite = TTSOnSprite;
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                if (TTSData != null)
                {
                    TTSData.Speak(questionText.text, 1, 1, "en-US", "Microsoft Zira - English (United States)");
                }
            }
            catch (System.EntryPointNotFoundException ex)
            {
                Debug.LogWarning("TTS function not found. Maybe JS plugin not ready yet: " + ex.Message);
            }
#else
            Debug.Log("TTS is only supported in WebGL builds.");
#endif
        }
    }
    public void DisplayCurrentQuestion()
    {
        if (currentQuestion >= totalQuestion) return;
        for (int i = 0; i < questionObjects.Count; i++)
        {
            questionObjects[i].SetActive(true);
        }

        for (int i = 0; i < optButton.Count; i++)
        {
            optButton[i].interactable = true;
        }

        Question q = GetCurrentQuestionList()[currentQuestion];

        if (!string.IsNullOrEmpty(q.image))
        {
            StartCoroutine(QuestionImage(q));
            questionText2.gameObject.SetActive(true);
            questionText.gameObject.SetActive(false);
        }
        else
        {
            questionText2.gameObject.SetActive(false);
            questionText.gameObject.SetActive(true);
            questionImgObj.SetActive(false);
        }

        questionText.text = q.question;
        questionText2.text = q.question;
        if (isTTS)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                if (TTSData != null)
                {
                    TTSData.Speak(questionText.text, 1, 1, "en-US", "Microsoft Zira - English (United States)");
                }
            }
            catch (System.EntryPointNotFoundException ex)
            {
                Debug.LogWarning("TTS function not found. Maybe JS plugin not ready yet: " + ex.Message);
            }
#else
            Debug.Log("TTS is only supported in WebGL builds.");
#endif
        }

        string correctAnswer = q.option_a;
        List<string> options = new List<string> { q.option_a, q.option_b, q.option_c, q.option_d };
        ShuffleList(options);

        opt1Text.text = options[0];
        opt2Text.text = options[1];
        opt3Text.text = options[2];
        opt4Text.text = options[3];

        opt1Handler.SetCorrect(options[0] == correctAnswer, this);
        opt2Handler.SetCorrect(options[1] == correctAnswer, this);
        opt3Handler.SetCorrect(options[2] == correctAnswer, this);
        opt4Handler.SetCorrect(options[3] == correctAnswer, this);
    }

    public IEnumerator QuestionImage(Question q)
    {
        questionPanelImg.gameObject.SetActive(false);
        loadingImage.SetActive(true);

        string imageURL = q.image;
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                if (texture != null)
                {
                    //texture.filterMode = FilterMode.Trilinear; // test to improve quality
                    
                    questionSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        Vector2.zero);
                    questionPanelImg.sprite = questionSprite;
                    loadingImage.SetActive(false);
                    questionPanelImg.gameObject.SetActive(true);
                    zoomPanelImg.sprite = questionSprite;
                }
                else
                {
                    questionText2.gameObject.SetActive(false);
                    questionText.gameObject.SetActive(true);
                    questionImgObj.SetActive(false);
                }
            }
            else
            {
                questionText2.gameObject.SetActive(false);
                questionText.gameObject.SetActive(true);
                questionImgObj.SetActive(false);
              //  Debug.LogWarning($"Failed to load image from {imageURL}: {request.error}");
            }
        }
    }

    public void LoadNextQuestionWithDelay()
    {
        if (!isGameOver)
            StartCoroutine(WaitAndLoad());
    }

    private IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(1f);

        currentQuestion++;
        
        maxQuestion = currentQuestion + 1;
        if (currentQuestion < totalQuestion)
        {
            questionNOText.text = "Question " + (currentQuestion + 1).ToString();
            for (int i = 0; i < questionObjects.Count; i++)
            {
                questionObjects[i].SetActive(false);
            }

            DisplayCurrentQuestion();
        }
        else
        {
            sponsorHandler.ShowPostMessage(selectedGame);
            //EndGameDueToTimeout();
        }
    }

    public void ResetTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            currentTime = 0;
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
        }
    }

    public void HomeBtnClick()
    {
        SceneManager.LoadScene(0);
    }

    public void TimerBntClick(bool dothis)
    {
        //SetForStart();
        Check = dothis;
        //LoadQuestion();
        //mainMenuPanel.SetActive(false);
        //timerPanel.SetActive(false);
        //gamePlayPanel.SetActive(true);

        //if (Check)
        //{
        //    timefillimg.enabled = true;
        //    needleImgObj.GetComponent<Animator>().enabled = true;
        //    StartTimer();
        //}
        //else
        //{
        //    timefillimg.enabled = false;
        //    needleImgObj.GetComponent<Animator>().enabled = false;
        //    timerText.text = "Off";
        //}

        if (isLink)
        {
            if (isMultiplayer)
            {
                OnlineMode();
            }
            else
            {
                OfflineMode();
            }
        }
        else
        {
            //GamePlayHandler.instance.modeSelectionPanel.SetActive(true);
            OfflineMode();
        }

        //Invoke("SetInGamePlay", 1f);
    }
    #region Functions Added By Arslan

    
    public void OfflineMode()
    {
        SetForStart();
        LoadQuestion();
        mainMenuPanel.SetActive(false);
        timerPanel.SetActive(false);
        gamePlayPanel.SetActive(true);

        if (Check)
        {
            timefillimg.enabled = true;
            // needleImgObj.GetComponent<Animator>().enabled = true;
            if (needleImgObj.TryGetComponent<Animator>(out Animator animator))
            {
                animator.enabled = true;
            }
            StartTimer();
        }
        else
        {
            timefillimg.enabled = false;
            // needleImgObj.GetComponent<Animator>().enabled = false;
            if (needleImgObj.TryGetComponent<Animator>(out Animator animator))
            {
                animator.enabled = false;
            }
            timerText.text = "Off";
            StartOffTimer();
        }


        modeSelectionPanel.SetActive(false);
        Invoke("SetInGamePlay", 1f);
    }
    public void OnlineMode()
    {
        isMultiplayer = true;
        SceneManager.LoadScene(1);
    }
    public void SetOffline()
    {
        offlineModeImg.sprite = modeSelectedSprite;
        OnlineModeImg.sprite = modeNonSelectedSprite;
        
        offlineModeImg.SetNativeSize();
        OnlineModeImg.SetNativeSize();
        
        offlineModeImg.gameObject.GetComponent<UIShiny>().enabled = true;
        OnlineModeImg.gameObject.GetComponent<UIShiny>().enabled = false;
        // selecterImgOffline.SetActive(true);
        // selecterImgOnline.SetActive(false);

        isMultiplayer = false;
    }
    public void SetOnline()
    {
        offlineModeImg.sprite = modeNonSelectedSprite;
        OnlineModeImg.sprite = modeSelectedSprite;
        
        offlineModeImg.SetNativeSize();
        OnlineModeImg.SetNativeSize();
        
        offlineModeImg.gameObject.GetComponent<UIShiny>().enabled = false;
        OnlineModeImg.gameObject.GetComponent<UIShiny>().enabled = true;
        // selecterImgOffline.SetActive(false);
        // selecterImgOnline.SetActive(true);

        isMultiplayer = true;
    }
    //In Game Mode Selection Data
    public void ModeSelectionStartBtn()
    {
        if (isMultiplayer)
        {
            OnlineMode();
        }
        else
        {
            modeSelectionPanel.SetActive(false);
            timerPanel.SetActive(true);
            //OfflineMode();
        }
    }
    public void SetOfflineInGame()
    {
        inGameOfflineModeImg.sprite = modeSelectedSprite;
        inGameOnlineModeImg.sprite = modeNonSelectedSprite;
        
        inGameOfflineModeImg.SetNativeSize();
        inGameOnlineModeImg.SetNativeSize();
        
        inGameOfflineModeImg.gameObject.GetComponent<UIShiny>().enabled = true;
        inGameOnlineModeImg.gameObject.GetComponent<UIShiny>().enabled = false;

        isMultiplayer = false;
    }
    public void SetOnlineInGame()
    {
        inGameOfflineModeImg.sprite = modeNonSelectedSprite;
        inGameOnlineModeImg.sprite = modeSelectedSprite;
        
        inGameOfflineModeImg.SetNativeSize();
        inGameOnlineModeImg.SetNativeSize();
        
        inGameOfflineModeImg.gameObject.GetComponent<UIShiny>().enabled = false;
        inGameOnlineModeImg.gameObject.GetComponent<UIShiny>().enabled = true;

        isMultiplayer = true;
    }
    #endregion
    private void StartTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        currentTime = timerDuration;
        timefillimg.color = Color.green;
        timefillimg.fillAmount = 1f;
        timerCoroutine = StartCoroutine(CountdownTimer());
    }
    private void StartOffTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        currentTime = timerDuration;
        timerCoroutine = StartCoroutine(CountdownOffTimer());
    }

    public void updatePointText()
    {
        scoreText.text = score.ToString();
        scoreText.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), .1f).SetEase(Ease.Linear).OnComplete(
            delegate { scoreText.transform.DOScale(new Vector3(1f, 1f, 1f), .1f); });
    }

    public void UpdateStreakText()
    {
        streak = continiousRightQuestion;
        streakText.text = streak.ToString();
        streakText.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), .1f).SetEase(Ease.Linear).OnComplete(
            delegate { streakText.transform.DOScale(new Vector3(1f, 1f, 1f), .1f); });
    }

    private IEnumerator CountdownTimer()
    {
        float initialDuration = timerDuration;

        while (currentTime > 0)
        {
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
            timefillimg.fillAmount = currentTime / initialDuration;

            float timePercentage = currentTime / initialDuration;
            if (timePercentage <= 0.25f)
            {
                timefillimg.color = Color.red;
            }
            else if (timePercentage <= 0.5f)
            {
                timefillimg.color = Color.yellow;
            }
            else
            {
                timefillimg.color = Color.green;
            }

            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
        }
        

        timerText.text = "0";
        timefillimg.fillAmount = 0f;
        sponsorHandler.ShowPostMessage(selectedGame);
    }
    private IEnumerator CountdownOffTimer()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1f);
            currentTime -= 1f;
        }
    }

    public void EndGameDueToTimeout()
    {
        apiDataSender.status = 0;
        apiDataSender.streak = streak;
        apiDataSender.max_question = maxQuestion;
        apiDataSender.right_question = rightQuestion;
        apiDataSender.wrong_question = wrongQuestion;
        streakPointText.text = streakPoints.ToString() + " Points";
        if (wrongQuestion == 0 && maxQuestion>1)
        {
            accuracyPoints = 10;
            score += 10;
            
            updatePointText();
        }

        accuracyPointText.text = accuracyPoints.ToString() + " Points";
        tripleDownPointText.text = tripleDownPoints.ToString() + " Points";
        sevanPointText.text = sevanPoints.ToString() + " Points";
        totalPoints = score;
        if (totalPoints == 0)
        {
            resultHeaderText.text = "";
            resultdisscriptionText.text = "";
        }
        else if (totalPoints > 0 && totalPoints <= 10)
        {
            resultHeaderText.text = finalResults[0].header;
            resultdisscriptionText.text = finalResults[0].discription;
        }
        else if (totalPoints > 10 && totalPoints <= 20)
        {
            resultHeaderText.text = finalResults[1].header;
            resultdisscriptionText.text = finalResults[1].discription;
        }
        else if (totalPoints > 20 && totalPoints <= 30)
        {
            resultHeaderText.text = finalResults[2].header;
            resultdisscriptionText.text = finalResults[2].discription;
        }
        else
        {
            resultHeaderText.text = finalResults[3].header;
            resultdisscriptionText.text = finalResults[3].discription;
        }

        totalPointText.text = totalPoints.ToString();
        apiDataSender.points = totalPoints;
        apiDataSender.seven_in_row = totalSevenInRow;
        apiDataSender.time_taken = 60 - (int)currentTime;
        apiDataSender.SendData();
        isGameOver = true;
        gameOverPanel.SetActive(true);
    }

    public void SkipBtnClick()
    {
        if (isSkip)
        {
            isSkip = false;
            skipBtn.SetActive(false);
            currentQuestion++;
            questionNOText.text = "Question " + (currentQuestion + 1).ToString();
            maxQuestion = currentQuestion + 1;
            if (currentQuestion < totalQuestion)
            {
                DisplayCurrentQuestion();
            }
            else
            {
                sponsorHandler.ShowPostMessage(selectedGame);
                //  EndGameDueToTimeout();
            }
        }
    }

    public void BoosterBtnClick()
    {
        is3XBoosterActive = true;
        boosterBtn.GetComponent<Button>().enabled = false;
        total3XBoost = 0;
        // boosterBtn.GetComponent<Image>().sprite = boosterActiveSprite;
        // boosterBtn.GetComponent<Image>().SetNativeSize();
        if (boosterBtn.TryGetComponent<Image>(out Image image))
        {
            image.sprite = boosterActiveSprite;
            image.SetNativeSize();
        }
    }

    public void SetForStart()
    {
        for (int i = 0; i < questionObjects.Count; i++)
        {
            questionObjects[i].GetComponent<DOTweenAnimation>().delay = 0.5f + (0.1f * i);
        }
    }

    public void SetInGamePlay()
    {
        for (int i = 0; i < questionObjects.Count; i++)
        {
            questionObjects[i].GetComponent<DOTweenAnimation>().delay = 0.1f * i;
        }
    }
}

[System.Serializable]
public class FinalResult
{
    public string header;
    public string discription;
}