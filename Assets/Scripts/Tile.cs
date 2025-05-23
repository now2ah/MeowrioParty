
//TileData
using System.Diagnostics;

public abstract class Tile
{
    public int tileIndex = 0;
    public abstract void TileEvent(PlayerData playerData, int coin);
}

public class CoinTile : Tile
{
    public CoinTile(int index)
    {
        tileIndex = index;
    }
    public override void TileEvent(PlayerData playerData, int coin)
    {
        playerData.UpdateCoinCnt(coin);
    }
}
public class StarTile : Tile
{
    public override void TileEvent(PlayerData playerData, int star)
    {
        playerData.UpdateStarCnt(star);
    }
}
public class WrapTile : Tile
{
    public override void TileEvent(PlayerData playerData, int coin)
    {
        playerData.UpdateCoinCnt(coin);
    }
}
