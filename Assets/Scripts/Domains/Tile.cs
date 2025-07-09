using Meowrio.Service;
using UnityEngine;

namespace Meowrio.Domain
{
    /// <summary>
    /// 효과를 가지는 타일
    /// </summary>
    public abstract class Tile : IEffectable
    {
        public abstract void ApplyEffect();
    }

    /// <summary>
    /// 아무 효과 없는 일반 타일
    /// </summary>
    public class NormalTile : Tile
    {
        public override void ApplyEffect()
        {
            Debug.Log("Apply normal tile effect!");
        }
    }
}