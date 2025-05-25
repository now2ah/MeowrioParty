using UnityEngine;
public enum ETileType
{
    None,
    CoinPlusTile,
    CoinMinusTile,
    StarTile,
    WarpTile
}

public class TileController : MonoBehaviour
{
    public ETileType tileType;
    public int tileIndex;

    [Header("Effect Parameters")]
    public int eventParam;
    public TileController MoveTo;

    public void TileEvent(PlayerData playerData, PlayerController playerController)
    {
        switch (tileType)
        {
            case ETileType.CoinPlusTile:
                playerData.UpdateCoinCnt(eventParam);
                LeaderBoardManager.Instance.UpdateCoin(playerData.ClientId, eventParam);
                Debug.Log("Coinplus");
                break;
            case ETileType.CoinMinusTile:
                playerData.UpdateCoinCnt(-eventParam);
                LeaderBoardManager.Instance.UpdateCoin(playerData.ClientId, -eventParam);
                Debug.Log("CoinMinusTile");
                break;
            case ETileType.StarTile:
                playerData.UpdateStarCnt(eventParam);
                Debug.Log("StarTile");
                break;
            case ETileType.WarpTile:
                WarpTo(playerData,playerController, MoveTo);
                Debug.Log("WarpTile");
                break;
            default:
                 playerData.UpdateCoinCnt(eventParam);
                 Debug.Log("Coinplus");
                break;
        }
    }

    private void WarpTo(PlayerData playerData, PlayerController playerController, TileController targetTile)
    {
        playerData.MoveTo(targetTile);
        playerController.TransportPlayer(targetTile);
    }
}
