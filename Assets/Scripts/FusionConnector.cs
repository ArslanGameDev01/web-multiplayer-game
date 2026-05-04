using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityCoder.Samples;
using UnityEngine.SceneManagement;

public class FusionConnector : MonoBehaviour
{
    public TTS TTSData;
    [Header("UI Panels")]
    public GameObject roomSelectionPanel;
    public GameObject joinRoomPanel;
    public GameObject loadingPanel;
    public GameObject waitingPanel;
    public GameObject timerPanel;
    public GameObject selectedAllProfile;
    public GameObject errorPopupPanel;
    [Header("Others")]
    public GameObject startButton;

    [Header("Input Fields")]
    public TMP_InputField joinRoomInput;

    public TMP_Text usernameWaiting;
    public TMP_Text pinCodeWaiting;
    public TMP_Text playersCountTxt;
    public TMP_Text errorTxt;

    public string LocalPlayerName { get; set; }
    public int avatarIndexAnimal { get; set; }
    public string roomName { get; set; }
    public string LocalRoomName { get; set; }

    [SerializeField, Tooltip("The network runner prefab that will be instantiated when looking starting the game.")]
    private NetworkRunner _networkRunnerPrefab;

    [Tooltip("The canvas group that handles interactivity for the game.")]
    public CanvasGroup canvasGroup;

    [Tooltip("Prefab for the trivia game itself.")]
    public NetworkObject triviaGamePrefab;

    public Transform playerContainer;

    public static FusionConnector Instance { get; private set; }
    [Networked]
    public bool TimerMode { get; set; }

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void OnDestroy()
    {
        Instance = null;
    }
    public void TimerStatus(bool timerStatus)
    {
        TimerMode = timerStatus;
        StartTriviaGame();
    }

    public async void StartGame(bool joinRandomRoom)
    {
        loadingPanel.SetActive(true);
        canvasGroup.interactable = false;

        // If joinRandomRoom is false, it means joining room.
        if (!joinRandomRoom)
        {
            LocalRoomName = joinRoomInput.text;
            if (string.IsNullOrEmpty(LocalRoomName))
            {
                Debug.LogWarning("Room name is empty.");
                canvasGroup.interactable = true;
                loadingPanel.SetActive(false);
                return;
            }
            
        }

        string generatedUsername = "Dash" + GenerateRandomUsername(); // Always generate

        // Prepare session properties (only used when creating the room)
        Dictionary<string, SessionProperty> sessionProps = null;
        if (joinRandomRoom)
        {
            sessionProps = new Dictionary<string, SessionProperty>
            {
                { "storedUsername", generatedUsername },
                { "gameNumber", GamePlayHandler.instance.selectedGame }
            };
        }

        StartGameArgs startGameArgs = new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = joinRandomRoom ? GenerateRandomPin() : LocalRoomName,
            PlayerCount = 40,
            SessionProperties = sessionProps
        };

        NetworkRunner newRunner = Instantiate(_networkRunnerPrefab);

        StartGameResult result = await newRunner.StartGame(startGameArgs);

        if (result.Ok)
        {
            // Set room pin
            pinCodeWaiting.text = "Pin: " + newRunner.SessionInfo.Name;
            roomName = newRunner.SessionInfo.Name;
            if (newRunner.SessionInfo.Properties.TryGetValue("gameNumber", out var currentGame) &&
                GamePlayHandler.instance.selectedGame != currentGame)
            {
                errorTxt.text = "You cannot join different type of game.";
                joinRoomInput.text = "";
                errorPopupPanel.SetActive(true);
                joinRoomPanel.SetActive(false);
                LeaveRoomBtn();
                
                canvasGroup.interactable = true;
                loadingPanel.SetActive(false);
                return;
            }
            // Read storedUsername from session properties
            if (newRunner.SessionInfo.Properties.TryGetValue("storedUsername", out var storedName))
            {
                usernameWaiting.text = "Room: " + storedName;
                GoToGame();
            }
            else
            {
                // fallback: if no room exists with this pin
                errorTxt.text = "No room exists with this pin.";
                joinRoomInput.text = "";
                errorPopupPanel.SetActive(true);
                joinRoomPanel.SetActive(false);
                LeaveRoomBtn();
                
                //usernameWaiting.text = "Room: " + LocalRoomName;
            }

            
        }
        else
        {
            GoToMainMenu();
            errorPopupPanel.SetActive(true);
            errorTxt.text = "You cannot join this room!\nGame already started or internet issue";
            joinRoomInput.text = "";
            
            
            if (result.ShutdownReason == ShutdownReason.GameIsFull)
            {
                errorTxt.text = "This room is full. Please create or join another room";
            }
            else if (result.ShutdownReason == ShutdownReason.MaxCcuReached)
            {
                errorTxt.text = "We're experiencing high traffic right now. Please check back soon";
            }
            else if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                if (result.ErrorMessage.Contains("Game full", StringComparison.OrdinalIgnoreCase) 
                    || result.ErrorMessage.Contains("Room full", StringComparison.OrdinalIgnoreCase))
                {
                    errorTxt.text = "This room is full. Please create or join another room";
                }
                else if(result.ErrorMessage.Contains("CCU", StringComparison.OrdinalIgnoreCase) 
                        || result.ErrorMessage.Contains("MaxCCU", StringComparison.OrdinalIgnoreCase))
                {
                    errorTxt.text = "We're experiencing high traffic right now. Please check back soon";
                }
                //errorTxt.text = result.ErrorMessage;
            }
            //errorMessageObject.SetActive(true);
            
            
            // var gui = errorMessageObject.GetComponentInChildren<TextMeshProUGUI>();
            // if (gui) gui.text = result.ErrorMessage;
            //
            Debug.Log("Room creation failed: "+result.ErrorMessage);
        }

        canvasGroup.interactable = true;
        loadingPanel.SetActive(false);
    }

    private string GenerateRandomUsername()
    {
        return UnityEngine.Random.Range(100, 999).ToString();
    }
    private string GenerateRandomPin()
    {
        return UnityEngine.Random.Range(1000, 9999).ToString();
    }
    public void GoToMainMenu()
    {
        waitingPanel.SetActive(false);
        roomSelectionPanel.SetActive(true);
        joinRoomPanel.SetActive(false);
    }

    public void GoToGame()
    {
        timerPanel.SetActive(false);
        waitingPanel.SetActive(true);
        roomSelectionPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
    }

    internal void OnPlayerJoin(NetworkRunner runner)
    {
        // Only set pregame messages if the game hasn't started.
        if (TriviaManager.TriviaManagerPresent)
            return;
        
        UpdatePlayerCount(runner);

        if (runner.IsSharedModeMasterClient)
            startButton.SetActive(runner.ActivePlayers.Count() > 1);
        else
            startButton.SetActive(false);
    }

    public void UpdatePlayerCount(NetworkRunner runner)
    {
        int count = runner.ActivePlayers.Count();
        playersCountTxt.text = $"{count}/40";
        
        if (runner.IsSharedModeMasterClient)
            startButton.SetActive(runner.ActivePlayers.Count() > 1);
    }

    public void StartTriviaGame()
    {
        //mainGameObject.SetActive(true);
        selectedAllProfile.SetActive(false);
        timerPanel.SetActive(false);
        waitingPanel.SetActive(false);
        roomSelectionPanel.SetActive(false);
        joinRoomPanel.SetActive(false);

        NetworkRunner runner = null;
        // If no runner has been assigned, we cannot start the game
        if (NetworkRunner.Instances.Count > 0)
        {
            runner = NetworkRunner.Instances[0];
        }

        if (runner == null)
        {
            Debug.Log("No runner found.");
            SceneManager.LoadScene(0);
            return;
        }
        
        // If no trivia manager has been made and we are the master mode client.
        // Redundant but being safe.
        if (runner.IsSharedModeMasterClient && !TriviaManager.TriviaManagerPresent)
        {
            runner.Spawn(triviaGamePrefab);
        }

    }

    public void LeaveRoomBtn()
    {
        NetworkRunner runner = null;
        // If no runner has been assigned, we cannot start the game
        if (NetworkRunner.Instances.Count > 0)
        {
            runner = NetworkRunner.Instances[0];
        }
        if(runner!=null)
            runner.Shutdown(true, ShutdownReason.Ok);
        
        //SceneManager.LoadScene(1);
        roomSelectionPanel.SetActive(true);
        waitingPanel.SetActive(false);
    }
    
}
