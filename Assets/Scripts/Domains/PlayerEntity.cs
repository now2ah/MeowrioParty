using UnityEngine;

namespace Meowrio.Domain
{
    public class PlayerEntity
    {
        private const int PLAYER_START_COIN_VALUE = 10;

        private int playerId;
        private Coin _ownedCoins;

        public int CurrentTileIndex { get; private set; }

        public PlayerEntity(int playerId)
        {
            this.playerId = playerId;
            _ownedCoins = new Coin(PLAYER_START_COIN_VALUE);
        }

        public void MoveTo(int nextTileIndex)
        {
            CurrentTileIndex = nextTileIndex;
            Debug.Log($"Player {playerId} move to tile {nextTileIndex}");
        }

        public void GainCoin(int gainCoinValue)
        {
            _ownedCoins.Add(gainCoinValue);
            Debug.Log($"Gain coin : {gainCoinValue}");
        }
    }
}
