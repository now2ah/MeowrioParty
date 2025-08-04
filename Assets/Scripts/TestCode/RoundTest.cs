
using Meowrio.Service;
using UnityEngine;

namespace Assets.Scripts.TestCode
{
    public class RoundTest : MonoBehaviour
    {
        private void Awake()
        {
            int maxRound = 5;
            RoundService roundService = new RoundService(maxRound);


        }
    }
}
