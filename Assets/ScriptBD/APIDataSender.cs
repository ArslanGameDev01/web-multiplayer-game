using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class APIDataSender : MonoBehaviour
{
    private string apiUrl = "https://braindashadmin.braingamestudio.com/api/store/analytic";

    private string testAPIKey = "http://13.48.1.190/api/store/analytic";

    public bool isTest;
   // public GamePlayHandler board;

    // Example data to send (you can replace these with your game variables)
    [HideInInspector] public int  streak = 5;
    [HideInInspector]public int max_question = 2;
    [HideInInspector] public int right_question = 5;
    [HideInInspector] public int wrong_question = 1;
    [HideInInspector] public int time_taken = 1;
    [HideInInspector]  public int points = 1;
    [HideInInspector] public int seven_in_row = 1;
    [HideInInspector]  public string game_name = "";
    [HideInInspector] public int  status = 0;
    [HideInInspector] public string  room_name = "";

    private string URL = "";
   // public Root reciveroot;

    private void Start()
    {
    }
    [ContextMenu("SendData")]
    public void SendData()
    {
        
        // Call the method to send data (e.g., when a game session ends)
        StartCoroutine(SendUserAttemptData());
    }

    private IEnumerator SendUserAttemptData()
    {
        if (isTest)
        {
            URL = testAPIKey;
        }
        else
        {
            URL = apiUrl;
        }
        // Create a new UnityWebRequest for POST
        using (UnityWebRequest webRequest = UnityWebRequest.Post(URL, new WWWForm()))
        {
            // Create a form section for the data
            WWWForm form = new WWWForm();
            form.AddField("streak", streak);
            form.AddField("max_question", max_question);
            form.AddField("right_question", right_question);
            form.AddField("wrong_question", wrong_question);
            form.AddField("time_taken", time_taken);
            form.AddField("points", points);
            form.AddField("seven_in_row", seven_in_row);
            form.AddField("game_name", game_name);
            form.AddField("status", status);
            form.AddField("room_name", room_name);

            // Attach the form data to the request
            webRequest.uploadHandler = new UploadHandlerRaw(form.data);
            webRequest.SetRequestHeader("Content-Type",
                form.headers["Content-Type"]); // Use the correct content type from WWWForm

            // Set the download handler to receive the response
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            // Log the data being sent for debugging
           Debug.Log("Build 13 Feb 3pm: Attempting to send data:\n" +
                      $"status: {status}\n" +
                      $"room_name: {room_name}\n" +
                      $"streak: {streak}\n" +
                      $"max_question: {max_question}\n" +
                      $"right_question: {right_question}\n" +
                      $"time_taken: {time_taken}\n" +
                      $"points: {points}\n" +
                      $"seven_in_row: {seven_in_row}\n" +
                      $"game_name: {game_name}\n" +
                      $"wrong_question: {wrong_question}");

            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();
            //Check Result
            // if (webRequest.result == UnityWebRequest.Result.Success)
            // {
            //     Debug.Log(" Web Request Successful!");
            //     Debug.Log("Response Code: " + webRequest.responseCode);
            //     Debug.Log("Server Response: " + webRequest.downloadHandler.text);
            // }
            // else
            // {
            //     Debug.LogError(" Web Request Failed!");
            //     Debug.LogError("Error: " + webRequest.error);
            //     Debug.LogError("Response Code: " + webRequest.responseCode);
            //
            //     if (!string.IsNullOrEmpty(webRequest.downloadHandler.text))
            //     {
            //         Debug.LogError("Server Response: " + webRequest.downloadHandler.text);
            //     }
            //     
            // }
        }
    }
}
