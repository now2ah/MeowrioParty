using UnityEngine;
public enum ETileType
{
    None,
    CoinTile,
    StarTile,
    WarpTile
}

public class TileController : MonoBehaviour
{
    public ETileType tileType;
}
