using Fusion;
using UnityEngine;

public class FusionPlayerBD : NetworkBehaviour
{
    [Networked]
    public PlayerStatsBD playerStats { get; set; }

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Debug.Log("This is the local player");
        }
        else
        {
            Debug.Log("This is a remote player");
        }
    }

    public void SetInitialProfile(string name, int avatar)
    {
        playerStats = new PlayerStatsBD
        {
            profileName = name,
            avatar = avatar,
            score = 0,
            isReady = false,
            isUseHint = false,
            isUseBooster = false
        };
    }
}
