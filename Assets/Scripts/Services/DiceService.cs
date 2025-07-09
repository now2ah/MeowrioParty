
using System;

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
            return _random.Next(_minimumNumber, _maximumNumber);
        }
    }
}
