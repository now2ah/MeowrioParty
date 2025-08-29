using Meowrio.Controller;
using Meowrio.Domain;
using Meowrio.Service;
using Meowrio.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Meowrio.Manager
{
    /// <summary>
    /// BoardGame의 진행을 관리한다
    /// </summary>
    public class BoardGameManager : NetSingleton<BoardGameManager>
    {
        private int DEFAULT_GAIN_COIN_VALUE = 3;
        private int DEFAULT_LOSE_COIN_VALUE = 3;

        private TileService _tileService;
        private BoardGameService _boardGameService;
        private CharacterFactory _characterFactory;
        private MapController _mapController;
        private CameraController _cameraController;

        private BoardGameStateMachine _boardGameStateMachine;
        private IntroBoardGameState _introBoardGameState;
        private SetTurnOrderGameState _setTurnOrderGameState;
        private BoardGameState _boardGameState;

        public event Action<int> OnPlayerInput;
        public event Action OnSetTurnOrderStateStarted;

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

        public void Start()
        {
            _cameraController = FindAnyObjectByType<CameraController>();
            _boardGameStateMachine.StartState(_introBoardGameState);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsHost == false)
                return;

            _tileService = new TileService(LoadMapTiles());
            _boardGameService = new BoardGameService(_tileService, NetworkManager.Singleton.ConnectedClientsList.Count);
        }

        public void Update()
        {
            _boardGameStateMachine.Update();
        }

        public int PlayerCount => _boardGameService.PlayerCount;

        public void ProcessInput(int clientID)
        {
            OnPlayerInput.Invoke(clientID);
        }

        public async void IntroSequenceAsync()
        {
            GenerateCharacters();
            _cameraController.ChangeCameraRpc(CameraType.Board);

            await Task.Delay(2000);

            _boardGameStateMachine.ChangeState(_setTurnOrderGameState);
            OnSetTurnOrderStateStarted.Invoke();
        }

        public void SetTurnOrderSequence()
        {
            _cameraController.ChangeCameraRpc(CameraType.Stage);
        }

        public void RollDiceForSetTurnOrder(int playerID)
        {
            _boardGameService.RollDiceForSetTurnOrder(playerID);
        }

        public void SetTurnOrder()
        {
            _boardGameService.SetTurnOrder();
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
            if (IsServer == false)
                return;

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                ulong clientID = client.ClientId;
                _boardGameService.AddPlayer((int)clientID, new PlayerEntity((int)clientID));
                Transform character = _characterFactory.GenerateCharacter((ECharacterType)clientID);
                character.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID);

                Transform spawnPoint = _mapController.GetSpawnPointTransform((int)clientID);
                character.GetComponent<PlayerController>().MoveToRpc(spawnPoint.position, spawnPoint.rotation);
            }
        }
    }
}

