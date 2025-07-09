
namespace Meowrio.Domain
{
    public class PlayerEntity
    {
        private const int PLAYER_START_COIN_VALUE = 10;

        private Coin _ownedCoins;

        public PlayerEntity()
        {
            _ownedCoins = new Coin(PLAYER_START_COIN_VALUE);
        }
    }
}
