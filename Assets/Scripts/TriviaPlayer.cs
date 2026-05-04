using Fusion;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main player asset created when a player joins the room.
/// </summary>
public class TriviaPlayer : NetworkBehaviour
{
    [Tooltip("The name of the player")]
    [Networked]
    public NetworkString<_32> PlayerName { get; set; }

    [Tooltip("Which character has the player chosen.")]
    [Networked]
    public int ChosenAvatar { get; set; } = -1;

    [Tooltip("What is the player's score.")]
    [Networked]
    public int Score { get; set; }
    
    public bool Booster3xUsed { get; set; } = false;
    public bool IsUsedSkipPower { get; set; }
    public NetworkBool IsMasterClient { get; set; }
    
    /// <summary>
    /// Unsure if this pattern is okay, but static references to the local player and a list of all players.
    /// </summary>
    public static TriviaPlayer LocalPlayer;
    
    /// <summary>
    /// A list of all players currently in the game.
    /// </summary>
    public static List<TriviaPlayer> TriviaPlayerRefs = new List<TriviaPlayer>();

    /// <summary>
    /// When a character is spawned, we have to do the checks that a user would do in case someone spawns late.
    /// </summary>
    public override void Spawned()
    {
        base.Spawned();

        // Adds this player to a list of player refs and then sorts the order by index
        TriviaPlayerRefs.Add(this);
        TriviaPlayerRefs.Sort((x, y) => x.Object.StateAuthority.AsIndex - y.Object.StateAuthority.AsIndex);

        // We assign the local test player
        if (Object.HasStateAuthority == true)
        {
            LocalPlayer = this;
        }
        transform.SetParent(FusionConnector.Instance.playerContainer, false);

        // Sets the master client value on spawn
        if (HasStateAuthority)
        {
            IsMasterClient = Runner.IsSharedModeMasterClient;
        }
    }
    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        // Removes the player from the list
        TriviaPlayerRefs.Remove(this);
        if (this == LocalPlayer)
        {
            LocalPlayer = null;

            FusionConnector.Instance.waitingPanel.SetActive(false);
            FusionConnector.Instance.roomSelectionPanel.SetActive(true);
            FusionConnector.Instance.selectedAllProfile.SetActive(true);
            FusionConnector.Instance.errorPopupPanel.SetActive(true);
            FusionConnector.Instance.joinRoomInput.text = string.Empty;
            TriviaManager tm = GameObject.FindObjectOfType<TriviaManager>();
            if (tm)
            {
                //tm.Rpc_NotifyPlayerFinished();
                Destroy(tm.gameObject);
            }
            // if (TriviaManager.Instance != null)
            // {
            //     TriviaManager.Instance.Rpc_NotifyPlayerLeft();
            //     Destroy(TriviaManager.Instance.gameObject);
            // }
        }

        if (HasStateAuthority)
            IsMasterClient = runner.IsSharedModeMasterClient;
        
        Debug.Log("Removed player");
    }
}
