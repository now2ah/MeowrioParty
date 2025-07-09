using UnityEngine;

namespace Meowrio.Domain
{
    public class PlayerEntity
    {
        private const int PLAYER_START_COIN_VALUE = 10;

        private Tile _currentTile;
        private Coin _ownedCoins;

        public PlayerEntity()
        {
            _ownedCoins = new Coin(PLAYER_START_COIN_VALUE);
        }

        public void MoveTo(Tile destinationTile)
        {
            _currentTile = destinationTile;
        }

        public void GainCoin(int gainCoinValue)
        {
            _ownedCoins.Add(gainCoinValue);
            Debug.Log($"Gain coin : {gainCoinValue}");
        }
    }
}
