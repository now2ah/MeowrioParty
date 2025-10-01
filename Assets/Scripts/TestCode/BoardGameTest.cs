using Meowrio.Domain;
using Meowrio.Service;
using UnityEngine;
using System.Collections.Generic;


namespace Assets.Scripts.TestCode
{
    public class BoardGameTest : MonoBehaviour
    {
        BoardGameService _boardGameService;
        TileService _tileService;

        private void Awake()
        {
            List<Tile> testTileList = new List<Tile>();
            for (int i=0; i<8; ++i)
            {
                testTileList.Add(new NormalTile(i));
            }
            
            _tileService = new TileService(testTileList);
            _boardGameService = new BoardGameService(_tileService, 2);
            _boardGameService.AddPlayer(0);
            _boardGameService.AddPlayer(1);

            _boardGameService.RollDiceForSetTurnOrder(0);
            _boardGameService.RollDiceForSetTurnOrder(1);
            _boardGameService.SetTurnOrder();

            _boardGameService.StartBoardGame(_tileService);
        }
    }
}
