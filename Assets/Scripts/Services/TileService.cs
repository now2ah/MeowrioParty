using System.Collections.Generic;
using Meowrio.Domain;

namespace Meowrio.Service
{
    public class TileService
    {
        private IReadOnlyList<Tile> _tileList;

        public IReadOnlyList<Tile> TileList => _tileList;

        public TileService()
        {
            _tileList = new List<Tile>();
        }

        public TileService(IReadOnlyList<Tile> tileList)
        {
            _tileList = tileList;
        }

        public int GetNextTileIndex(int playerTileIndex, int diceNumber)
        {
            int nextTileIndex = (playerTileIndex + diceNumber) % _tileList.Count;
            return nextTileIndex;
        }

        public void ApplyTileEffect(PlayerEntity playerEntity, int tileIndex)
        {
            _tileList[tileIndex].ApplyEffect(playerEntity);
        }
    }
}
