using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackMagic
{
    public class Wall : Entity, ITileDrawer
    {
        public Rigidbody rb;
        TileDrawer tileDrawer;
        private List<Rectangle> rects;

        private static Texture2D tilemap = null;

        public Wall(int chunkSize, int tileSize, float tileScale) : base(Vector2.Zero)
        {
            //Width = tileSize * tileScale * map.GetLength(0);
            //Height = tileSize * tileScale * map.GetLength(1);

            //TODO: Make this a public static method to set
            //tilemap ??= Globals.TempEnvSpriteSheet;
            Rectangle tileRectSpecs = new Rectangle(16, 16, 12, 11);
            tileDrawer = new TileDrawer(this, tilemap, tileRectSpecs, chunkSize, tileSize, tileScale);
            tileDrawer.Layer = 0.89f;
            AddTrait(tileDrawer);

            rb = new Rigidbody(this, true);
            rb.camera = Globals.Camera;
            AddTrait(rb);
        }

        public void AddChunk(int[,] map, Vector2 pos) =>
            tileDrawer.AddChunk(map, pos);

        public void Prerender()
        {
            tileDrawer.Prerender();

            this.rects = new List<Rectangle>();

            int[,] map = tileDrawer.Map.Clone<int>(); 
            List<Rectangle> rects = map.ToRectangles();
            for (int i = 0; i < rects.Count; i++)
            {
                rects[i] = new Rectangle(
                    (int)(rects[i].X * tileDrawer.TileSize * tileDrawer.TileScale + tileDrawer.OffsetX * tileDrawer.ChunkSize * tileDrawer.TileScale),
                    (int)(rects[i].Y * tileDrawer.TileSize * tileDrawer.TileScale + tileDrawer.OffsetY * tileDrawer.ChunkSize * tileDrawer.TileScale),
                    (int)(rects[i].Width * tileDrawer.TileSize * tileDrawer.TileScale),
                    (int)(rects[i].Height * tileDrawer.TileSize * tileDrawer.TileScale));
                HitRect hitRect = new HitRect(this, rects[i]);
                rb.hitboxes.Add(hitRect);
                this.rects.Add(rects[i]);
            }
        }

        public override void Draw()
        {
            base.Draw();
            //rb.DrawHitboxBorders();
        }
    }
}
