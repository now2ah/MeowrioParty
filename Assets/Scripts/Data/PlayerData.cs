using System.Diagnostics;

public class PlayerData
{
    public ulong ClientId;
    public string PlayerName;
    public TileController currentTile;    
    public int Coins;
    public int Stars;

    public PlayerData(){}
    public PlayerData(ulong clinetID)
    {
        ClientId = clinetID;

    }
    public void MoveTo(TileController nextTile)
    {
        currentTile = nextTile;
    }

    public void UpdateCoinCnt(int coinCnt)
    {
        Debug.Print("myCoin");
        Coins += coinCnt;
    }
    public void UpdateStarCnt(int starCnt)
    {
        Stars += starCnt;
    }

}
