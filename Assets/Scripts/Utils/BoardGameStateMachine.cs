using UnityEngine;

namespace Meowrio.Util
{
    public class BoardGameStateMachine
    {
        IBoardGameState _currentState;

        public void StartState(IBoardGameState startState)
        {
            _currentState = startState;
            _currentState.EnterState();
        }

        public void ChangeState(IBoardGameState nextState)
        {
            _currentState.ExitState();
            _currentState = nextState;
            _currentState.EnterState();
        }

        public void Update()
        {
            _currentState.UpdateState();
        }
    }

    public interface IBoardGameState
    {
        public void EnterState();
        public void UpdateState();
        public void ExitState();
    }
}

