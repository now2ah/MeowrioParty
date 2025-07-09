using UnityEngine;
using Random = System.Random;

namespace Meowrio.Service
{
    public class DiceService
    {
        private int _minimumNumber;
        private int _maximumNumber;
        private Random _random;

        public DiceService(int minimumNumber, int maximumNumber)
        {
            _minimumNumber = minimumNumber;
            _maximumNumber = maximumNumber;
            _random = new Random();
        }

        public int GetRandomDiceNumber()
        {
            int randomNumber = _random.Next(_minimumNumber, _maximumNumber + 1);
            Debug.Log($"Random dice number is {randomNumber}");
            return randomNumber;
        }
    }
}
