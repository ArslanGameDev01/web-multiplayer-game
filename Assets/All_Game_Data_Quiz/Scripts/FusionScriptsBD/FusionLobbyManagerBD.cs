using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FusionLobbyManagerBD : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject roomSelectionPanel;
    public GameObject createRoomPanel;
    public GameObject joinRoomPanel;
    public GameObject loadingPanel;
    public GameObject waitingPanel;
    public GameObject roomPanel;
    public GameObject timerPanel;
    public GameObject selectedAllProfile;
    public GameObject errorPopupPanel;
    [Header("Others")]
    public GameObject startButton;

    [Header("Input Fields")]
    public TMP_InputField createRoomInput;
    public TMP_InputField joinRoomInput;

    public TMP_Text usernameWaiting;
    public TMP_Text pinCodeWaiting;
    public TMP_Text playersCountTxt;

    //private int PlayersCount;
    //private NetworkRunner _runner;

    // public static string GenerateRandomUsername() => Random.Range(100, 1000).ToString();
    // public static string GenerateRandomPIN() => Random.Range(1000, 10000).ToString();

    // public void OnClickCreateRoom()
    // {
    //     string roomName = GenerateRandomPIN();
    //     if (!string.IsNullOrEmpty(roomName))
    //         StartHostGame(roomName);
    // }
    //
    // public void OnClickJoinRoom()
    // {
    //     string roomName = joinRoomInput.text;
    //     if (!string.IsNullOrEmpty(roomName))
    //         JoinClientGame(roomName);
    // }
    //
    // private async void StartHostGame(string roomName)
    // {
    //     loadingPanel.SetActive(true);
    //     roomSelectionPanel.SetActive(false);
    //     joinRoomPanel.SetActive(false);
    //
    //     if (_runner == null)
    //         _runner = gameObject.AddComponent<NetworkRunner>();
    //
    //     var sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>() ??
    //                        _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
    //
    //     var result = await _runner.StartGame(new StartGameArgs()
    //     {
    //         GameMode = GameMode.Host,
    //         SessionName = roomName,
    //         SceneManager = sceneManager,
    //         Scene = SceneRef.FromIndex(2)
    //     });
    //
    //     if (result.Ok)
    //     {
    //         roomPanel.SetActive(true);
    //         pinCodeWaiting.text = "Pin: " + roomName;
    //         usernameWaiting.text = "Room: Dash" + GenerateRandomUsername();
    //         loadingPanel.SetActive(false);
    //         waitingPanel.SetActive(true);
    //     }
    //     else
    //     {
    //         Debug.LogError("Failed to start host session: " + result.ShutdownReason);
    //         loadingPanel.SetActive(false);
    //         roomSelectionPanel.SetActive(true);
    //     }
    // }
    //
    // private async void JoinClientGame(string roomName)
    // {
    //     loadingPanel.SetActive(true);
    //     createRoomPanel.SetActive(false);
    //     joinRoomPanel.SetActive(false);
    //
    //     if (_runner == null)
    //         _runner = gameObject.AddComponent<NetworkRunner>();
    //
    //     var sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>() ??
    //                        _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
    //
    //     var result = await _runner.StartGame(new StartGameArgs()
    //     {
    //         GameMode = GameMode.Client,
    //         SessionName = roomName,
    //         SceneManager = sceneManager,
    //         Scene = SceneRef.FromIndex(2)
    //     });
    //
    //     if (result.Ok)
    //     {
    //         waitingPanel.SetActive(true);
    //         loadingPanel.SetActive(false);
    //     }
    //     else
    //     {
    //         Debug.LogError("Failed to join room: " + result.ShutdownReason);
    //         loadingPanel.SetActive(false);
    //         joinRoomPanel.SetActive(true);
    //     }
    // }
    //
    // public async void OnStartButtonClicked()
    // {
    //     if (_runner != null && _runner.IsServer)
    //     {
    //         var sceneManager = _runner.GetComponent<NetworkSceneManagerDefault>();
    //         if (sceneManager != null)
    //         {
    //             await sceneManager.LoadScene(SceneRef.FromIndex(2), new NetworkLoadSceneParameters());
    //         }
    //         else
    //         {
    //             Debug.LogError("Missing NetworkSceneManagerDefault on runner.");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Only the host can start the game.");
    //     }
    // }
}
