using UnityEngine;
using System.Runtime.InteropServices;

public class WebGLShareButton : MonoBehaviour
{
    public GameObject ErrorPanel;

    // Import JavaScript functions from our .jslib file
    [DllImport("__Internal")]
    private static extern void ShareText(string title, string text, string url);

    [DllImport("__Internal")]
    private static extern bool IsMobileBrowser();

    // Share content parameters
    public string shareTitle = "Check out this awesome game!";
    public string shareText = "I just played this amazing game made with Unity!";
    public string shareUrl = "https://your-game-url.com";

    // Call this method from a button click
    public void OnShareButtonClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                // Check if running on mobile browser with Web Share API support
                if (IsMobileBrowser())
                {
                    ShareText(shareTitle, shareText, shareUrl);
                }
                else
                {
                    // Fallback for desktop - open a Twitter share window as an example
                    ErrorPanel.SetActive(true);
                    
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Share failed: " + e.Message);
            }
#else
        Debug.Log("Sharing only works in WebGL builds!");
#endif
    }

    public void DisactiveErrorPanel()
    {
        ErrorPanel.SetActive(false);
    }

    // Optional: Add this to a UI Button in the Inspector
    void Start()
    {
        // You can add button listener here if not using Inspector
    }
}