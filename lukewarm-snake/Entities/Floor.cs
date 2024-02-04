using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class Floor : Entity, ITileDrawer
    {
        public TileDrawer tileDrawer { get; set; }

        public Floor(int chunkSize, int tileSize, float tileScale) : base(Vector2.Zero)
        {
            /*
            Texture2D tilemap = Globals.TempEnvSpriteSheet;
            Rectangle tileRectSpecs = new Rectangle(16, 16, 12, 11);
            tileDrawer = new TileDrawer(this, tilemap, tileRectSpecs, chunkSize, tileSize, tileScale);
            tileDrawer.Layer = 0.9f;
            AddTrait(tileDrawer);
            */
        }

        public void AddChunk(int[,] map, Vector2 pos) =>
            tileDrawer.AddChunk(map, pos);

        public void Prerender() => 
            tileDrawer.Prerender();
    }
}
