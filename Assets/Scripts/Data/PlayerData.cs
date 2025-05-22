
public class PlayerData
{
    private ulong _clientId;
    public Tile currentTile;

    public int Coins;
    public int Stars;

    public PlayerData(ulong clinetID)
    {
        _clientId = clinetID;
    }
    public void MoveTo(Tile nextTile)
    {
        currentTile = nextTile;
    }

    public void AddCoins(int coinCnt)
    {
        Coins += coinCnt;
    }
}
