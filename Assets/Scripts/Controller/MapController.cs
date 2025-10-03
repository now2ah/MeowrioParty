using Meowrio.Domain;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Meowrio.Controller
{
    public interface IMapDataProvider
    {

    }

    public class MapController : Singleton<MapController>, IMapDataProvider
    {
        [SerializeField] private List<GameObject> _spawnPointList;
        private Dictionary<int, TileController> _tileControllerDic;

        public override void Awake()
        {
            _tileControllerDic = new Dictionary<int, TileController>();
        }

        private void Start()
        {
            LoadMapTiles();
        }

        public TileController GetTileController(int tileIndex)
        {
            if (_tileControllerDic.TryGetValue(tileIndex, out TileController tileController)) { return tileController; }
            else
                throw new System.Exception($"there's no TileController in index : {tileIndex}");
        }

        public Transform GetSpawnPointTransform(int playerNumber)
        {
            return _spawnPointList[playerNumber].transform;
        }

        private void LoadMapTiles()
        {
            foreach (var tileObj in FindObjectsByType<TileController>(FindObjectsSortMode.None))
            {
                _tileControllerDic.Add(tileObj.tileIndex, tileObj);
            }
        }
    }
}

