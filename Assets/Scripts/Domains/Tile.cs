using Meowrio.Service;
using UnityEngine;

namespace Meowrio.Domain
{
    /// <summary>
    /// ȿ���� ������ Ÿ��
    /// </summary>
    public abstract class Tile : IEffectable
    {
        public abstract void ApplyEffect(PlayerEntity affectedPlayer);
    }

    /// <summary>
    /// �ƹ� ȿ�� ���� �Ϲ� Ÿ��
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