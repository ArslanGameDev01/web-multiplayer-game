using Fusion;

public class PlayerNetworkData : NetworkBehaviour
{
    [Networked] public int Score { get; set; }
    [Networked] public int AvatarIndex { get; set; }
    [Networked] public string ProfileName { get; set; }
    [Networked] public bool IsReady { get; set; }
    [Networked] public bool IsUseHint { get; set; }
    [Networked] public bool IsUseBooster { get; set; }

    public void SetData(int score, int avatar, string name, bool hintUsed, bool boosterUsed)
    {
        if (!HasStateAuthority) return;

        Score = score;
        AvatarIndex = avatar;
        ProfileName = name;
        IsReady = true;
        IsUseHint = hintUsed;
        IsUseBooster = boosterUsed;
    }
}
