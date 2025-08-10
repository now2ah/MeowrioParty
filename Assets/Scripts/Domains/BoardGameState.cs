using Meowrio.Manager;
using Meowrio.Util;

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
            _boardGameManager.GenerateCharacters();
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

        public SetTurnOrderGameState(BoardGameManager boardGameManager)
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

