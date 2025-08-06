using Meowrio.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meowrio.Services
{
    public class TurnOrderService
    {
        private const int DEFAULT_TURNORDER_VALUE = -1;

        private int[] _turnOrderArray;
        private int _currentTurnOrder;
        private List<(int playerId, int diceNumber)> _playerDicePairList;

        public IReadOnlyList<int> TurnOrderArray => _turnOrderArray;

        public TurnOrderService(int numberOfPlayers)
        {
            _turnOrderArray = new int[numberOfPlayers];

            for (int i = 0; i < numberOfPlayers; ++i)
            {
                _turnOrderArray[i] = DEFAULT_TURNORDER_VALUE;
            }

            _currentTurnOrder = 0;

            _playerDicePairList = new List<(int playerId, int diceNumber)>();
        }

        public void RegisterPlayerDiceNumber(int playerId, int diceNumber)
        {
            if (_playerDicePairList.Count >= _turnOrderArray.Length)
                throw new Exception("you can't register dice number anymore");

            _playerDicePairList.Add((playerId, diceNumber));
        }

        public void SetTurnOrder()
        {
            _playerDicePairList.OrderByDescending(x => x.diceNumber);

            for (int i = 0; i < _playerDicePairList.Count; ++i)
            {
                _turnOrderArray[i] = _playerDicePairList[i].playerId;
            }
        }

        public void GoToNextTurn()
        {
            _currentTurnOrder++;

            if (_currentTurnOrder >= _turnOrderArray.Length)
            {
                _currentTurnOrder = 0;
            }
        }

        public int GetCurrentTurnPlayerId()
        {
            if (_turnOrderArray[_currentTurnOrder] == DEFAULT_TURNORDER_VALUE)
                throw new System.Exception("Turn order array isn't initialized");

            return _turnOrderArray[_currentTurnOrder];
        }
    }
}
