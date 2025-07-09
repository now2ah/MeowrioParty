
namespace Meowrio.Domain
{
    /// <summary>
    /// 플레이어 게임 진행에 필요한 코인 재화
    /// </summary>
    public class Coin
    {
        private int _value;
        public int Value => _value;

        public Coin(int value)
        {
            _value = value;
        }

        public void Add(int addValue)
        {
            _value += addValue;
        }

        public void Lose(int loseValue)
        {
            _value -= loseValue;

            if (_value < 0)
                _value = 0;
        }
    }
}
