using Unity.Netcode;
using UnityEngine;

public enum EGamePhase
{
    GameReady,
    GamePlay,
    GameEnd,
}

public abstract class Phase
{
    protected EGamePhase _phaseType;
    public EGamePhase PhaseType { get { return _phaseType; } private set { } }
    public abstract void EnterPhase();
    public abstract void UpdatePhase();
    public abstract void ExitPhase();
}

public class GameReadyPhase : Phase
{
    BoardManager _boardManager;

    public GameReadyPhase(BoardManager boardManager, EGamePhase phase)
    {
        _boardManager = boardManager;
        _phaseType = phase;
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

    public GamePlayPhase(BoardManager boardManager, EGamePhase phase)
    {
        _boardManager = boardManager;
        _phaseType = phase;
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

    public GameEndPhase(BoardManager boardManager, EGamePhase phase)
    {
        _boardManager = boardManager;
        _phaseType = phase;
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

public class PhaseMachine : NetworkBehaviour
{
    public bool IsRunning { get; private set; }

    [SerializeField] private NetworkVariable<EGamePhase> _currentPhase = new NetworkVariable<EGamePhase>();

    private Phase _phase;

    public EGamePhase CurrentPhase { get { return _currentPhase.Value; } private set { } }

    private void Awake()
    {
        IsRunning = false;
    }

    private void Update()
    {
        if (_phase != null)
        {
            _phase.UpdatePhase();
        }
    }

    public void StartPhase(Phase startPhase)
    {
        if (IsServer)
            _currentPhase.Value = startPhase.PhaseType;
        IsRunning = true;
        _phase = startPhase;
        _phase.EnterPhase();
    }


    public void ChangePhaseRpc(Phase nextPhase)
    {
        _phase.ExitPhase();
        _phase = nextPhase;
        _phase.EnterPhase();
        if (IsServer)
            _currentPhase.Value = nextPhase.PhaseType;
    }

    public bool IsPhase(Phase checkPhase)
    {
        if (checkPhase.PhaseType == _currentPhase.Value)
            return true;
        else
            return false;
    }    
}
