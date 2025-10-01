
using Meowrio.Domain;
using Meowrio.Service;
using System.Collections.Generic;
using UnityEngine;

namespace Meowrio.Service
{
    /// <summary>
    /// 참여하는 플레이어의 순서에 따라 라운드를 보드 게임을 진행한다
    /// </summary>
    public class BoardGameService
    {
        private const int MIN_TURNORDER_DICENUMBER = 1;
        private const int MAX_TURNORDER_DICENUMBER = 6;
        private const int DEFAULT_ROUND = 3;

        private TileService _tileService;
        private DiceService _diceService;
        private TurnOrderService _turnOrderService;
        private RoundService _roundService;
        private Dictionary<int, PlayerEntity> _playerDic;

        public IReadOnlyDictionary<int, PlayerEntity> PlayerDic => _playerDic;

        public BoardGameService(TileService tileService, int numberOfPlayers)
        {
            _tileService = tileService;
            _turnOrderService = new TurnOrderService(numberOfPlayers);
            _diceService = new DiceService(MIN_TURNORDER_DICENUMBER, MAX_TURNORDER_DICENUMBER);
            _playerDic = new Dictionary<int, PlayerEntity>();
        }

        public int PlayerCount => _playerDic.Count;

        public void AddPlayer(int playerID)
        {
            PlayerEntity playerEntity = new PlayerEntity(playerID);
            _playerDic.Add(playerID, playerEntity);
        }

        public int RollDiceForSetTurnOrder(int playerID)
        {
            int diceNumber = _diceService.GetRandomDiceNumber();
            _turnOrderService.RegisterPlayerDiceNumber(playerID, diceNumber);
            Debug.Log($"Player {playerID} roll the dice : {diceNumber}");
            return diceNumber;
        }

        public void SetTurnOrder()
        {
            _turnOrderService.SetTurnOrder();
        }

        public void StartBoardGame(TileService tileService)
        {
            foreach (var player in _playerDic)
            {
                player.Value.MoveTo(tileService.StartTileIndex);
            }

            _roundService = new RoundService(DEFAULT_ROUND, tileService);
            _roundService.StartRound(_playerDic, _turnOrderService.TurnOrderArray);
        }
    }
}

