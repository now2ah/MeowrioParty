using Meowrio.Domain;
using Meowrio.Service;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Meowrio.Util;
using Meowrio.Controller;
using System;
using System.Threading.Tasks;

namespace Meowrio.Manager
{
    /// <summary>
    /// BoardGame의 진행을 관리한다
    /// </summary>
    public class BoardGameManager : NetSingleton<BoardGameManager>
    {
        [SerializeField] private CameraController _cameraController;

        private int DEFAULT_GAIN_COIN_VALUE = 3;
        private int DEFAULT_LOSE_COIN_VALUE = 3;

        TileService _tileService;
        BoardGameService _boardGameService;
        CharacterFactory _characterFactory;
        MapController _mapController;

        BoardGameStateMachine _boardGameStateMachine;
        IntroBoardGameState _introBoardGameState;
        SetTurnOrderGameState _setTurnOrderGameState;
        BoardGameState _boardGameState;

        public event Action OnIntroBoardGame;

        public override void Awake()
        {
            base.Awake();
            _characterFactory = gameObject.GetComponent<CharacterFactory>();
            _mapController = gameObject.GetComponent<MapController>();
            _boardGameStateMachine = new BoardGameStateMachine();
            _introBoardGameState = new IntroBoardGameState(this);
            _setTurnOrderGameState = new SetTurnOrderGameState(this);
            _boardGameState = new BoardGameState(this);
        }

        public void Update()
        {
            _boardGameStateMachine.Update();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsHost == false)
                return;

            _tileService = new TileService(LoadMapTiles());
            _boardGameService = new BoardGameService(_tileService, NetworkManager.Singleton.ConnectedClientsList.Count);
            _boardGameStateMachine.StartState(_introBoardGameState);
        }

        public async void IntroSequenceAsync()
        {
            GenerateCharacters();
            _cameraController.ChangeCamera(CameraType.Board);

            await Task.Delay(2000);

            _boardGameStateMachine.ChangeState(_setTurnOrderGameState);
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

            tileList.Sort((tile1, tile2) => tile1.CompareTo(tile2.IndexNumber));

            return tileList;
        }

        private void GenerateCharacters()
        {
            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                ulong clientID = client.ClientId;
                _boardGameService.AddPlayer((int)clientID, new PlayerEntity((int)clientID));
                Transform character = _characterFactory.GenerateCharacter((ECharacterType)clientID);
                character.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);
                _mapController.PlaceToSpawnPoint(character.gameObject, (int)clientID);
            }
        }

        
    }
}

