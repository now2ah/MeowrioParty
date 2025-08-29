using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace Meowrio.Controller
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _spawnPointList;

        public Transform GetSpawnPointTransform(int playerNumber)
        {
            return _spawnPointList[playerNumber].transform;
        }

        public void PlaceToSpawnPoint(GameObject gameObject, int playerNumber)
        {
            gameObject.transform.position = _spawnPointList[playerNumber].transform.position;
            gameObject.transform.rotation = _spawnPointList[playerNumber].transform.rotation;
        }
    }
}

