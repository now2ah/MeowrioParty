using Meowrio.Manager;
using Meowrio.Util;
using System;
using Unity.Netcode;

namespace Meowrio.Domain
{
    public class IntroBoardGameState : IBoardGameState
    {
        private BoardGameManager _boardGameManager;

        public IntroBoardGameState(BoardGameManager boardGameManager)
        {
            _boardGameManager = boardGameManager;
        }

        public void EnterState()
        {
            _boardGameManager.IntroSequenceAsync();
        }

        public void ExitState()
        {
        }

        public void UpdateState()
        {
            
        }
    }

    public class SetTurnOrderGameState : IBoardGameState
    {
        private BoardGameManager _boardGameManager;
        private int _completeRollPlayerCount = 0;

        public SetTurnOrderGameState(BoardGameManager boardGameManager)
        {
            _boardGameManager = boardGameManager;
        }

        [Rpc(SendTo.Server)]
        private void BoardGameManager_OnPlayerInputRpc(int playerID)
        {
            _boardGameManager.RollDiceForSetTurnOrder(playerID);
            _completeRollPlayerCount++;

            if (_completeRollPlayerCount == _boardGameManager.PlayerCount)
                _boardGameManager.SetTurnOrder();
        }

        public void EnterState()
        {
            _boardGameManager.OnPlayerInput += BoardGameManager_OnPlayerInputRpc;
            _boardGameManager.SetTurnOrderSequence();
        }

        public void ExitState()
        {
            _boardGameManager.OnPlayerInput -= BoardGameManager_OnPlayerInputRpc;
        }

        public void UpdateState()
        {
            
        }
    }

    public class BoardGameState : IBoardGameState
    {
        private BoardGameManager _boardGameManager;

        public BoardGameState(BoardGameManager boardGameManager)
        {
            _boardGameManager = boardGameManager;
        }

        public void EnterState()
        {
            
        }

        public void ExitState()
        {
            
        }

        public void UpdateState()
        {
            
        }
    }
}

