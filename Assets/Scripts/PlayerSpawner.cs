using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic player spawn based on the main shared mode sample.
/// </summary>
public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    public GameObject PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            var resultingPlayer = Runner.Spawn(PlayerPrefab, new Vector3(0, 1, 0), Quaternion.identity);

            FusionConnector connector = GameObject.FindObjectOfType<FusionConnector>();
            if (connector != null)
            {
                var testPlayer = resultingPlayer.GetComponent<TriviaPlayer>();
                
                testPlayer.PlayerName = connector.LocalPlayerName;
                testPlayer.ChosenAvatar = ProfileSelectionBD.selectedAvatarIndex;
            }
        }

        FusionConnector.Instance?.OnPlayerJoin(Runner);
    }
    public void PlayerLeft(PlayerRef player)
    {
        FusionConnector.Instance.UpdatePlayerCount(Runner);
        TriviaManager.Instance.Rpc_NotifyPlayerLeft();
        
        if (TriviaPlayer.LocalPlayer != null)
            TriviaPlayer.LocalPlayer.IsMasterClient = Runner.IsSharedModeMasterClient;
    }
}
