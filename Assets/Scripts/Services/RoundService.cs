
using Meowrio.Domain;
using Meowrio.Service;
using System.Collections.Generic;

namespace Meowrio.Service
{
    /// <summary>
    /// BoardGame의 정해진 Round를 반복 진행
    /// </summary>
    public class RoundService
    {
        private TileService _tileService;
        private TurnService _turnService;
        private int _maxRound;
        private int _roundCount;


        public RoundService(int maxRound, TileService tileService)
        {
            _tileService = tileService;
            _maxRound = maxRound;
            _roundCount = 0;
        }

        public void StartRound(IReadOnlyDictionary<int, PlayerEntity> playerDic, IReadOnlyList<int> turnOrderList)
        {
            _roundCount++;



            for (int i = 1; i <= _maxRound; ++i)
            {
                for (int j = 0; j < turnOrderList.Count; ++j)
                {
                    _turnService = new TurnService(playerDic[turnOrderList[j]], _tileService);
                    _turnService.ProgressTurn();
                }
                _roundCount++;
            }
        }
    }
}
