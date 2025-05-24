using System.Diagnostics;

public class PlayerData
{
    private ulong _clientId;
    public TileController currentTile;    
    public int Coins;
    public int Stars;

    public PlayerData(ulong clinetID)
    {
        _clientId = clinetID;

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
