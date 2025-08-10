using System.Collections.Generic;
using UnityEngine;

namespace Meowrio.Controller
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _spawnPointList;

        public void PlaceToSpawnPoint(GameObject gameObject, int playerNumber)
        {
            gameObject.transform.position = _spawnPointList[playerNumber].transform.position;
        }
    }
}

