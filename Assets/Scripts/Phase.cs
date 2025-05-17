using UnityEngine;

public abstract class Phase
{
    public abstract void EnterPhase();
    public abstract void UpdatePhase();
    public abstract void ExitPhase();
}

public class GameReadyPhase : Phase
{
    BoardManager _boardManager;

    public GameReadyPhase(BoardManager boardManager)
    {
        _boardManager = boardManager;
    }

    public override void EnterPhase()
    {
        
    }

    public override void UpdatePhase()
    {
        
    }

    public override void ExitPhase()
    {
        
    }
}

public class GamePlayPhase : Phase
{
    BoardManager _boardManager;

    public GamePlayPhase(BoardManager boardManager)
    {
        _boardManager = boardManager;
    }

    public override void EnterPhase()
    {

    }

    public override void UpdatePhase()
    {

    }

    public override void ExitPhase()
    {

    }
}

public class GameEndPhase : Phase
{
    BoardManager _boardManager;

    public GameEndPhase(BoardManager boardManager)
    {
        _boardManager = boardManager;
    }

    public override void EnterPhase()
    {

    }

    public override void UpdatePhase()
    {

    }

    public override void ExitPhase()
    {

    }
}

public class PhaseMachine : MonoBehaviour
{
    Phase _currentPhase;

    public Phase CurrentPhase { get { return _currentPhase; } private set { } }

    private void Update()
    {
        _currentPhase.UpdatePhase();
    }

    public void StartPhase(Phase phase)
    {
        _currentPhase = phase;
        _currentPhase.EnterPhase();
    }

    public void ChangePhase(Phase nextPhase)
    {
        _currentPhase.ExitPhase();
        _currentPhase = nextPhase;
        _currentPhase.EnterPhase();
    }

    public bool IsPhase(Phase phase)
    {
        if (_currentPhase == phase)
            return true;
        else
            return false;
    }    
}
