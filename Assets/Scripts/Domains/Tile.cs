using Meowrio.Service;
using System;
using UnityEngine;

namespace Meowrio.Domain
{
    /// <summary>
    /// 효과를 가지는 타일
    /// </summary>
    public abstract class Tile : IEffectable, IComparable<int>
    {
        public int IndexNumber { get; protected set; }

        public Tile(int indexNumber)
        {
            IndexNumber = indexNumber;
        }

        public abstract void ApplyEffect(PlayerEntity affectedPlayer);

        public int CompareTo(int otherIndexNumber)
        {
            return IndexNumber.CompareTo(otherIndexNumber);
        }
    }

    /// <summary>
    /// 아무 효과 없는 일반 타일
    /// </summary>
    public class NormalTile : Tile
    {
        public NormalTile(int indexNumber) : base(indexNumber)
        {
            IndexNumber = indexNumber;
        }

        public override void ApplyEffect(PlayerEntity affectedPlayer)
        {
            Debug.Log($"Apply normal tile effect to { affectedPlayer }!");
        }
    }

    /// <summary>
    /// 코인을 얻는 타일
    /// </summary>
    public class GainCoinTile : Tile
    {
        private int _gainCoinValue;

        public GainCoinTile(int indexNumber, int gainCoinValue) : base(indexNumber)
        {
            IndexNumber = indexNumber;
            _gainCoinValue = gainCoinValue;
        }

        public override void ApplyEffect(PlayerEntity affectedPlayer)
        {
            affectedPlayer.GainCoin(_gainCoinValue);
            Debug.Log($"Apply gain coin tile effect to {affectedPlayer} by add {_gainCoinValue} coins!");
        }
    }

    /// <summary>
    /// 코인을 얻는 타일
    /// </summary>
    public class LoseCoinTile : Tile
    {
        private int _loseCoinValue;

        public LoseCoinTile(int indexNumber, int loseCoinValue) : base(indexNumber)
        {
            IndexNumber = indexNumber;
            _loseCoinValue = loseCoinValue;
        }

        public override void ApplyEffect(PlayerEntity affectedPlayer)
        {
            affectedPlayer.LoseCoin(_loseCoinValue);
            Debug.Log($"Apply lose coin tile effect to {affectedPlayer} by add {_loseCoinValue} coins!");
        }
    }

    public class StarTile : Tile
    {
        public StarTile(int indexNumber) : base(indexNumber)
        {
            IndexNumber = indexNumber;
        }

        public override void ApplyEffect(PlayerEntity affectedPlayer)
        {
            Debug.Log($"Apply star tile effect to {affectedPlayer}");
        }
    }

    public class WarpTile : Tile
    {
        public WarpTile(int indexNumber) : base(indexNumber)
        {
            IndexNumber = indexNumber;
        }

        public override void ApplyEffect(PlayerEntity affectedPlayer)
        {
            Debug.Log($"Apply warp tile effect to {affectedPlayer}");
        }
    }
}