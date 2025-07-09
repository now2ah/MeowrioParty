using System.Collections.Generic;
using Meowrio.Domain;

namespace Meowrio.Service
{
    public class TileService
    {
        private List<Tile> _tileList;

        public List<Tile> TileList => _tileList;

        public TileService()
        {
            _tileList = new List<Tile>();
        }

        public TileService(List<Tile> tileList)
        {
            _tileList = tileList;
        }
    }
}
