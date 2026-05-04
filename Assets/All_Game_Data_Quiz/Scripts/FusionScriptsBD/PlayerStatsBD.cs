using Fusion;

public struct PlayerStatsBD : INetworkStruct
{
    public int score;
    public int avatar;
    public NetworkString<_32> profileName;
    public bool isReady;
    public bool isUseHint;
    public bool isUseBooster;
}
