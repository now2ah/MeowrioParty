using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Meowrio.Domain
{
    public class PlayerEntity
    {
        public event Action<int, int> OnWarp;
        public event Action<int, int> OnMove;

        private const int PLAYER_START_COIN_VALUE = 10;

        private int _playerId;
        private Coin _ownedCoins;

        public int PlayerID => _playerId;
        public int CurrentTileIndex { get; private set; }

        public PlayerEntity(int playerId)
        {
            this._playerId = playerId;
            _ownedCoins = new Coin(PLAYER_START_COIN_VALUE);
        }

        public void WarpTo(int nextTileIndex)
        {
            CurrentTileIndex = nextTileIndex;
            OnWarp?.Invoke(PlayerID, CurrentTileIndex);
            Debug.Log($"Player {_playerId} warp to tile {nextTileIndex}");
        }

        public void MoveTo(int nextTileIndex)
        {
            CurrentTileIndex = nextTileIndex;
            OnMove?.Invoke(PlayerID, CurrentTileIndex);
            Debug.Log($"Player {_playerId} move to tile {nextTileIndex}");
        }

        public void GainCoin(int gainCoinValue)
        {
            _ownedCoins.Add(gainCoinValue);
            Debug.Log($"Gain coin : {gainCoinValue}");
        }

        public void LoseCoin(int loseCoinValue)
        {
            _ownedCoins.Lose(loseCoinValue);
            Debug.Log($"Lose coin : {loseCoinValue}");
        }
    }
}
