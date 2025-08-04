
namespace Meowrio.Service
{
    /// <summary>
    /// BoardGame의 정해진 Round를 반복 진행
    /// </summary>
    public class RoundService
    {
        private int _maxRound;
        private int _roundCount;

        public RoundService(int maxRound)
        {
            _maxRound = maxRound;
            _roundCount = 0;
        }

        
    }
}
