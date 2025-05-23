using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BoardGameService
{
    public class Tile
    {

    }

    public class Board
    {
        Tile[] _tile;

        public Board(int tileNumber)
        {
            _tile = new Tile[tileNumber];
        }

        public Tile GetTile(int index)
        {
            return _tile[index];
        }
    }

    public class Player
    {
        private int _idNumber;
        private int _currentTileIndex;
        public int IdNumber => _idNumber;
        public int CurrentTileIndex => _currentTileIndex;

        public Player(int idNumber)
        {
            _idNumber = idNumber;
        }

        public void MoveTo(int tileIndex)
        {
            _currentTileIndex = tileIndex;
        }
    }

    private int _maxRound;
    private int _currentRound;
    private Board _board;
    private List<Player> _playerList;
    private Dictionary<Player, int> _diceNumberForSetPlayerOrderDic;
    private int _currentTurn;

    public Dictionary<Player, int> DiceNumberForSetPlayerOrderDic => _diceNumberForSetPlayerOrderDic;

    public void InitializeBoardGame(int maxRound, int tileCount)
    {
        _maxRound = maxRound;
        _currentRound = 1;
        _board = new Board(tileCount);
        _playerList = new List<Player>();
        _diceNumberForSetPlayerOrderDic = new Dictionary<Player, int>();
        _currentTurn = 0;
    }

    public void AddPlayer(int clientId)
    {
        _playerList.Add(new Player(clientId));
    }

    public Player GetPlayer(ulong clientId)
    {
        for (int i = 0; i < _playerList.Count; ++i)
        {
            if (_playerList[i].IdNumber == (int)clientId)
                return _playerList[i];
        }
        return null;
    }

    public void RollDiceForOrder(Player player)
    {
        _diceNumberForSetPlayerOrderDic[player] = RollDice(1, 6);
        Debug.Log("player : " + player.IdNumber + ", diceValue : " + _diceNumberForSetPlayerOrderDic[player]);

        if (_diceNumberForSetPlayerOrderDic.Count == _playerList.Count)
        {
            SetTurnOrder();
        }
    }

    public void BoardGameStart()
    {
        foreach (Player player in _playerList)
        {
            player.MoveTo(0);
        }
    }

    public bool IsSetOrderDone()
    {
        if (_diceNumberForSetPlayerOrderDic.Count == _playerList.Count)
            return true;
        else
            return false;
    }

    public int GetCurrentPlayerId()
    {
        return _playerList[_currentTurn].IdNumber;
    }

    private int RollDice(int minValue, int maxValue)
    {
        return UnityEngine.Random.Range(minValue, maxValue + 1);
    }

    private void SetTurnOrder()
    {
        var sortedDic = _diceNumberForSetPlayerOrderDic.OrderByDescending(p => p.Value).ToList();

        _playerList.Clear();
        foreach (var playerDiceNumPair in sortedDic)
        {
            _playerList.Add(playerDiceNumPair.Key);
        }
    }
}
