using Meowrio.Domain;
using Meowrio.Service;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace Meowrio.Manager
{
    /// <summary>
    /// BoardGame의 진행을 관리한다
    /// </summary>
    public class BoardGameManager : NetSingleton<BoardGameManager>
    {
        private int DEFAULT_GAIN_COIN_VALUE = 3;
        private int DEFAULT_LOSE_COIN_VALUE = 3;

        TileService _tileService;
        BoardGameService _boardGameService;


        public override void Awake()
        {
            base.Awake();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _tileService = new TileService(LoadMapTiles());
            _boardGameService = new BoardGameService(_tileService, NetworkManager.Singleton.ConnectedClientsList.Count);
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                ulong clientID = client.ClientId;
                _boardGameService.AddPlayer((int)clientID, new PlayerEntity((int)clientID));
            }

            if (IsHost)
            {
                //_boardGameService.StartBoardGame(_tileService);
            }
        }

        private IReadOnlyList<Tile> LoadMapTiles()
        {
            List<Tile> tileList = new List<Tile>();
            foreach (var tileObj in FindObjectsByType<TileController>(FindObjectsSortMode.None))
            {
                if (tileObj.tileType == ETileType.None)
                {
                    tileList.Add(new NormalTile(tileObj.tileIndex));
                }
                else if (tileObj.tileType == ETileType.GainCoinTile)
                {
                    tileList.Add(new GainCoinTile(tileObj.tileIndex, DEFAULT_GAIN_COIN_VALUE));
                }
                else if (tileObj.tileType == ETileType.LoseCoinTile)
                {
                    tileList.Add(new LoseCoinTile(tileObj.tileIndex, DEFAULT_LOSE_COIN_VALUE));
                }
                else if (tileObj.tileType == ETileType.StarTile)
                {
                    tileList.Add(new StarTile(tileObj.tileIndex));
                }
                else if (tileObj.tileType == ETileType.WarpTile)
                {
                    tileList.Add(new WarpTile(tileObj.tileIndex));
                }
            }
            tileList.Sort();

            return tileList;
        }
    }
}

