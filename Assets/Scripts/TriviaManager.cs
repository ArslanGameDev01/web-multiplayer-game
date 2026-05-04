using DG.Tweening;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Singleton network behavoir that manages the game and is managed by the shared mode master client.
/// </summary>
public class TriviaManager : NetworkBehaviour, IStateAuthorityChanged
{
    [Header("Brain Dash Multiplayer")]

    #region Brain Dash Multiplayer Data
    
    private int playersFinished = 0; // Only used on MasterClient
    private int totalPlayers;        // Used to compare how many finished

    public GameObject leaderboardEntryPrefab;
    public Transform leaderboardContent;

    public UseQuestion use_questions;
    public APIDataSender apiDataSender;
    public QuestionData questionData; // Reference to ScriptableObject
    public SponsorHandler sponsorHandler;
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
    public Sprite questionSprite;
    public Image questionPanelImg;
    public GameObject loadingImage;
    public GameObject questionImgObj;
    public Image timefillimg;
    [Header("Question Objects")] public TMP_Text timerText;
    public GameObject timerText2;
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
    public OptionsHandlerMultiplayer opt1Handler;
    public OptionsHandlerMultiplayer opt2Handler;
    public OptionsHandlerMultiplayer opt3Handler;
    public OptionsHandlerMultiplayer opt4Handler;
    public GameObject skipBtn;
    public GameObject boosterBtn;
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

    public GameObject gamePlayPanel;
    public GameObject waitingPanel;
    public GameObject leaderboardPanel;
    public GameObject gameOverPanel;
    public List<OptionsHandlerMultiplayer> optionHandlers;
    public List<GameObject> questionObjects;
    public List<Button> optButton;
    public List<Question> withOutTimerQuestions;
    public List<Question> withTimerQuestions;
    public List<int> use_Question_No = new List<int>();
    private bool usingTimerQuestions = false;
    private float timerDuration = 60f;
    private float currentTime;
    private Coroutine timerCoroutine;

    bool isUseHint;
    bool isUseBooster;

    //TTS
    
    private bool isTTS=false;
    public Image TTSIconImg;
    public Sprite TTSOnSprite;
    public Sprite TTSOffSprite;

    #endregion

    [Header("Brain Dash Data Ended")]

    [Tooltip("The current state of the game.")]
    [Networked, OnChangedRender(nameof(OnTriviaGameStateChanged))]
    public TriviaGameState GameState { get; set; } = TriviaGameState.Intro;
    
    [Tooltip("Button displayed to leave the game after a round ends.")]
    public GameObject leaveGameBtn;

    [Tooltip("Button displayed, only to the master client, to start a new game.")]
    public GameObject startNewGameBtn;
    
    /// <summary>
    /// Has a trivia manager been made; set to true on spawn and false on despawn
    /// </summary>
    public static bool TriviaManagerPresent { get; private set; } = false;

    /// <summary>
    /// The different states of the trivia game.  Made as a byte since there are not that many.
    /// </summary>
    public enum TriviaGameState : byte
    {
        Intro = 0,
        ShowQuestion = 1,
        ShowAnswer = 2,
        GameOver = 3,
        NewRound = 4,
    }

    public static TriviaManager Instance;
    private void Awake()
    {
        Instance = this;
        selectedGame = questionData.gameNumber;
    }
    

    public override void Spawned()
    {
        // Disallows players from joining once the game is started.
        if (Runner.IsSharedModeMasterClient)
        {
            Runner.SessionInfo.IsOpen = false;
            Runner.SessionInfo.IsVisible = false;
        }

        // If we have state authority, we set an intro time and randomized the question list.
        if (HasStateAuthority)
        {
            SelectAndDistributeQuestions();
        }

        TriviaManagerPresent = true;

        //FusionConnector.Instance?.SetPregameMessage(string.Empty);

        OnTriviaGameStateChanged();
        //UpdateCurrentQuestion();
        //UpdateQuestionsAskedText();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        TriviaManagerPresent = false;
    }
    
    public void PickAnswer(int index){}
    private void OnTriviaGameStateChanged()
    {
        // If showing an answer, we show which players got the question correct and increase their score.
        if (GameState == TriviaGameState.Intro || GameState == TriviaGameState.NewRound)
        {
            // Resets the score for the player
            TriviaPlayer.LocalPlayer.Score = 0;

            SelectAndDistributeQuestions();
        }
        else if (GameState == TriviaGameState.GameOver)
        {
            OnGameStateGameOver();
        }
        leaveGameBtn.SetActive(GameState == TriviaGameState.GameOver);
        startNewGameBtn.SetActive(GameState == TriviaGameState.GameOver && Runner.IsSharedModeMasterClient == true);
    }

    private void OnGameStateGameOver()
    {
        ShowLeaderboard();
    }

    public void ShowLeaderboard()
    {
        List<TriviaPlayer> winners = new List<TriviaPlayer>(TriviaPlayer.TriviaPlayerRefs);
        winners.Sort((x, y) => y.Score - x.Score);
        
        waitingPanel.SetActive(false);
        gamePlayPanel.SetActive(false);
        leaderboardPanel.SetActive(true);
        // Clear previous leaderboard entries in case of game replay
        foreach (Transform child in leaderboardContent)
        {
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < winners.Count; i++)
        {
            var entry = winners[i];
            GameObject go = Instantiate(leaderboardEntryPrefab, leaderboardContent);
            var ui = go.GetComponent<LeaderboardEntryUI>();
            
            bool isLocal = entry == TriviaPlayer.LocalPlayer;
            // bool isTop = i == 0;
            // bool isSecond = i == 1;
            // bool isThird = i == 2;

            ui.Setup(entry.PlayerName.Value, entry.Score, i + 1, entry.ChosenAvatar,isLocal , i==0, i==1, i==2,entry.IsUsedSkipPower, entry.Booster3xUsed);
        }
    }

    public async void LeaveGame()
    {
        await Runner.Shutdown(true, ShutdownReason.Ok);

        FusionConnector fc = GameObject.FindObjectOfType<FusionConnector>();
        if (fc)
        {
            Destroy(fc.gameObject);
            // fc.mainMenuObject.SetActive(true);
            // fc.mainGameObject.SetActive(false);
        }

        SceneManager.LoadScene(0);
    }

    public void StartNewGame()
    {
        if (HasStateAuthority == false)
            return;

        GameState = TriviaGameState.NewRound;

    }
    public void StateAuthorityChanged()
    {
        if (GameState == TriviaGameState.GameOver)
        {
            startNewGameBtn.SetActive(Runner.IsSharedModeMasterClient);
        }
    }

    // Brain Dash Multiplayer Data started
    [Networked] public int playersFinishedCount { get; set; }

    public void EndGameDueToTimeout()
    {
        apiDataSender.status = 1;
        apiDataSender.room_name = FusionConnector.Instance.roomName;
        apiDataSender.game_name= questionData.gameName;
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
        TriviaPlayer.LocalPlayer.Score = score;

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
        if (GamePlayHandler.instance.Check)
            apiDataSender.time_taken = 60;
        else
            apiDataSender.time_taken = 300 - (int)currentTime;
        
        apiDataSender.SendData();
        isGameOver = true;
        //gameOverPanel.SetActive(true);
        
        waitingPanel.SetActive(true);
        //GameState = TriviaGameState.GameOver;

        Rpc_NotifyPlayerFinished();
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_NotifyPlayerFinished()
    {
        playersFinishedCount++; // Keep a count of how many players have finished

        if (playersFinishedCount >= Runner.ActivePlayers.Count())
        {
            playersFinishedCount = 0;
            GameState = TriviaGameState.GameOver;
        }
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_NotifyPlayerLeft()
    {
        if (playersFinishedCount >= Runner.ActivePlayers.Count())
        {
            playersFinishedCount = 0;
            GameState = TriviaGameState.GameOver;
        }
    }


    private void StartTimer()
    {
        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        currentTime = timerDuration;
        timefillimg.color = Color.green;
        timefillimg.fillAmount = 1f;
        timerCoroutine = StartCoroutine(CountdownTimer());
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
            if (!GamePlayHandler.instance.Check && Mathf.CeilToInt(currentTime) < 60)
            {
                timerText2.SetActive(false);
                timerText.gameObject.SetActive(true);
            }
        }
        

        timerText.text = "0";
        timefillimg.fillAmount = 0f;
        sponsorHandler.ShowPostMessage(selectedGame);
    }
    public void BoosterBtnClick()
    {
        isUseBooster = true;
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
        TriviaPlayer.LocalPlayer.Booster3xUsed = true;
    }
    public void SkipBtnClick()
    {
        if (isSkip)
        {
            isUseHint = true;
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
    ////////Multiplayer gameplay working Started, working fine, not need to change now/////////////////////

    private void SelectAndDistributeQuestions()
    {
        List<int> selectedIndices = new List<int>();
        Game selectedGameData = questionData.root.data.games[selectedGame];

       // int questionsToPick = GamePlayHandler.instance.Check ? Mathf.Min(35, selectedGameData.questions.Count) : 10;
        int questionsToPick = FusionConnector.Instance.TimerMode ? Mathf.Min(35, selectedGameData.questions.Count) : 10;
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < selectedGameData.questions.Count; i++)
        {
            availableIndices.Add(i);
        }

        // Shuffle the availableIndices list
        for (int i = 0; i < availableIndices.Count; i++)
        {
            int rand = Random.Range(i, availableIndices.Count);
            (availableIndices[i], availableIndices[rand]) = (availableIndices[rand], availableIndices[i]);
        }

        for (int i = 0; i < questionsToPick; i++)
        {
            selectedIndices.Add(availableIndices[i]);
        }

        string indicesStr = string.Join(",", selectedIndices);

        if (HasStateAuthority)
        {
            RPC_SetTimerMode(FusionConnector.Instance.TimerMode);
            RPC_LoadQuestionsFromIndices(indicesStr);
        }
            
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_SetTimerMode(bool timerMode)
    {
        GamePlayHandler.instance.Check = timerMode;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_LoadQuestionsFromIndices(string indicesStr)
    {
        gamePlayPanel.SetActive(true);
        // if (GamePlayHandler.instance.isLink)
        // {
        // }

        if (!GamePlayHandler.instance.Check)
        {
            timerText2.SetActive(true);
            timerText.gameObject.SetActive(false);
            timerDuration = 300f; // 5 minutes
        }
        StartTimer();

        leaderboardPanel.SetActive(false);

        List<int> indices = new List<int>();
        foreach (string s in indicesStr.Split(','))
        {
            if (int.TryParse(s, out int index))
                indices.Add(index);
        }

        // Reset variables
        score = 0;
        accuracyPoints = 0;
        tripleDownPoints = 0;
        sevanPoints = 0;
        streakPoints = 0;
        
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
        boosterBtn.SetActive(true);
        // boosterBtn.GetComponent<Button>().enabled = true;
        // boosterBtn.GetComponent<Image>().sprite = boosterDefaltSprite;
        // boosterBtn.GetComponent<Image>().SetNativeSize();
        if (boosterBtn.TryGetComponent<Button>(out Button button))
        {
            button.enabled = true;
        }

        if (boosterBtn.TryGetComponent<Image>(out Image image))
        {
            image.sprite = boosterDefaltSprite;
            image.SetNativeSize();
        }
        is3XBoosterActive = false;

        usingTimerQuestions = Check;
        withTimerQuestions.Clear();
        withOutTimerQuestions.Clear();

        Game selectedGameData = questionData.root.data.games[selectedGame];
        List<Question> source = selectedGameData.questions;

        foreach (int i in indices)
        {
            if (i >= 0 && i < source.Count)
            {
                if (usingTimerQuestions)
                    withTimerQuestions.Add(source[i]);
                else
                    withOutTimerQuestions.Add(source[i]);
            }
        }

        totalQuestion = indices.Count;
        currentQuestion = 0;
        questionNOText.text = "Question " + (currentQuestion + 1).ToString();
        maxQuestion = currentQuestion + 1;
        DisplayCurrentQuestion();
    }
    public void UpdateStreakText()
    {
        streak = continiousRightQuestion;
        streakText.text = streak.ToString();
        streakText.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), .1f).SetEase(Ease.Linear).OnComplete(
            delegate { streakText.transform.DOScale(new Vector3(1f, 1f, 1f), .1f); });
    }
    public void updatePointText()
    {
        scoreText.text = score.ToString();
        scoreText.transform.DOScale(new Vector3(1.15f, 1.15f, 1.15f), .1f).SetEase(Ease.Linear).OnComplete(
            delegate { scoreText.transform.DOScale(new Vector3(1f, 1f, 1f), .1f); });
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
            if (FusionConnector.Instance.TTSData != null)
            {
                FusionConnector.Instance.TTSData.Speak(
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
                if (FusionConnector.Instance.TTSData != null)
                {
                    FusionConnector.Instance.TTSData.Speak(questionText.text, 1, 1, "en-US", "Microsoft Zira - English (United States)");
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
                if (FusionConnector.Instance.TTSData != null)
                {
                    FusionConnector.Instance.TTSData.Speak(questionText.text, 1, 1, "en-US", "Microsoft Zira - English (United States)");
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
                    questionSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                        Vector2.zero);
                    questionPanelImg.sprite = questionSprite;

                    loadingImage.SetActive(false);
                    questionPanelImg.gameObject.SetActive(true);
                    //zoomPanelImg.sprite = questionSprite;
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
        StartCoroutine(isGameOver ? GameOverWaitAndLoad() : WaitAndLoad());
    }
    private IEnumerator GameOverWaitAndLoad()
    {
        yield return new WaitForSeconds(1f);
        waitingPanel.SetActive(true);
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
            yield return new WaitForSeconds(0.5f);
            //Debug.Log("Selected Game: " + selectedGame);
            sponsorHandler.ShowPostMessage(selectedGame);
            //EndGameDueToTimeout();
        }
    }
}
