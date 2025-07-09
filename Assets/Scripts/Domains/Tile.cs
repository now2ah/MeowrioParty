using Meowrio.Service;
using UnityEngine;

namespace Meowrio.Domain
{
    /// <summary>
    /// ȿ���� ������ Ÿ��
    /// </summary>
    public abstract class Tile : IEffectable
    {
        public abstract void ApplyEffect();
    }

    /// <summary>
    /// �ƹ� ȿ�� ���� �Ϲ� Ÿ��
    /// </summary>
    public class NormalTile : Tile
    {
        public override void ApplyEffect()
        {
            Debug.Log("Apply normal tile effect!");
        }
    }
}