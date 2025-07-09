using Meowrio.Domain;
using Meowrio.Service;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TestCode
{
    public class MovingPlayerToTileTest : MonoBehaviour
    {
        DiceService _diceService;
        TileService _tileService;
        PlayerEntity _player;

        private void Awake()
        {
            _diceService = new DiceService(1, 6);

            List<Tile> testTileList = new List<Tile>();
            for (int i = 0; i < 8; ++i)
            {
                testTileList.Add(new GainCoinTile(3));
            }
            _tileService = new TileService(testTileList);

            _player = new PlayerEntity();
        }

        private void Start()
        {
            Test();
        }

        public void Test()
        {
            int randomNum = _diceService.GetRandomDiceNumber();
            Tile destinationTile = _tileService.TileList[randomNum];

            _player.MoveTo(destinationTile);
            destinationTile.ApplyEffect(_player);
        }
    }
}
