
using Meowrio.Domain;

namespace Meowrio.Service
{
    /// <summary>
    /// 해당 순서 Player의 Turn을 진행
    /// </summary>
    public class TurnService
    {
        private const int MIN_DICE_NUMBER = 1;
        private const int MAX_DICE_NUMBER = 6;

        private PlayerEntity _playerEntity;
        private TileService _tileService;
        private DiceService _diceService;
        

        public TurnService(PlayerEntity playerEntity, TileService tileService)
        {
            _playerEntity = playerEntity;
            _tileService = tileService;
            _diceService = new DiceService(MIN_DICE_NUMBER, MAX_DICE_NUMBER);
        }

        public void ProgressTurn()
        {
            //roll dice
            int diceNumber = _diceService.GetRandomDiceNumber();

            //move
            int nextTileIndex = _tileService.GetNextTileIndex(_playerEntity.CurrentTileIndex, diceNumber);
            _playerEntity.MoveTo(nextTileIndex);

            //tile effect
            _tileService.ApplyTileEffect(_playerEntity, nextTileIndex);
        }
    }
}
