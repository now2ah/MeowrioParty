
using System.Diagnostics;

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
