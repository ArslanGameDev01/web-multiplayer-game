using Fusion;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    [Networked] public int Score { get; set; }
    [Networked] public int AvatarIndex { get; set; }
    [Networked] public NetworkString<_32> ProfileName { get; set; }
    [Networked] public bool IsReady { get; set; }
    [Networked] public bool UsedHint { get; set; }
    [Networked] public bool UsedBooster { get; set; }
}
