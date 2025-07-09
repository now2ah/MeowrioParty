using Meowrio.Service;
using UnityEngine;

namespace Meowrio.Domain
{
    /// <summary>
    /// 효과를 가지는 타일
    /// </summary>
    public abstract class Tile : IEffectable
    {
        public abstract void ApplyEffect(PlayerEntity affectedPlayer);
    }

    /// <summary>
    /// 아무 효과 없는 일반 타일
    /// </summary>
    public class NormalTile : Tile
    {
        public override void ApplyEffect(PlayerEntity affectedPlayer)
        {
            Debug.Log($"Apply normal tile effect to { affectedPlayer }!");
        }
    }

    public class GainCoinTile : Tile
    {
        private int _gainCoinValue;

        public GainCoinTile(int gainCoinValue)
        {
            _gainCoinValue = gainCoinValue;
        }

        public override void ApplyEffect(PlayerEntity affectedPlayer)
        {
            affectedPlayer.GainCoin(_gainCoinValue);
            Debug.Log($"Apply gain coin tile effect to {affectedPlayer} by add {_gainCoinValue} coins!");
        }
    }
}