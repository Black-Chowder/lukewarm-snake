using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlackMagic
{
    public interface ITileDrawer
    {
        public void AddChunk(int[,] map, Vector2 pos);
        public void Prerender();
    }

    public class TileDrawer : Trait, TDraws, ITileDrawer
    {
        public int[,] Map { get; private set; }

        //Stores the size of each tile on the tilemap
        public int TileSize { get; private set; }

        //Multiplier for tileSize when drawing
        public float TileScale { get; private set; }

        private Texture2D tilemap = null;
        private Rectangle[] tileRects;

        public Vector2 DrawPos { get; protected set; } = Vector2.Zero;

        private RenderTarget2D rt = null;
        private List<TileChunkData> chunks = new();
        public float OffsetX { get; private set; } = float.PositiveInfinity;
        public float OffsetY { get; private set; } = float.PositiveInfinity;
        private float RightBound = float.NegativeInfinity;
        private float BottomBound = float.NegativeInfinity;

        public int ChunkSize { get; protected set; }

        public Camera camera { get; set; }

        public float Layer { get; set; } = 0f;

        private class TileChunkData
        {
            public int[,] Map { get; set; }
            public Vector2 Pos { get; set; }
            public TileChunkData(int[,] map, Vector2 pos)
            {
                Map = map;
                Pos = pos;
            }
        }

        public TileDrawer(Entity parent, Texture2D texture, Rectangle tileRectSpecs, int chunkSize, int tileSize, float tileScale = 2f) : base(parent)
        {
            this.TileSize = tileSize;
            this.TileScale = tileScale;
            this.camera = Globals.Camera;

            ChunkSize = chunkSize;

            this.tilemap = texture;
            this.tileRects = DrawUtils.spriteSheetLoader(tileRectSpecs.X, tileRectSpecs.Y, tileRectSpecs.Width, tileRectSpecs.Height);
        }


        public void AddChunk(int[,] map, Vector2 pos)
        {
            TileChunkData tcd = new(map, pos);
            AddChunk(tcd);
        }

        private void AddChunk(TileChunkData chunk)
        {
            chunks.Add(chunk);
            
            OffsetX = MathF.Min(OffsetX, chunk.Pos.X);
            OffsetY = MathF.Min(OffsetY, chunk.Pos.Y);

            RightBound = MathF.Max(RightBound, (chunk.Pos.X + ChunkSize));
            BottomBound = MathF.Max(BottomBound, (chunk.Pos.Y + ChunkSize));
        }

        public void Prerender()
        {
            Map = new int[(int)(RightBound - OffsetX), (int)(BottomBound - OffsetY)];

            //Fill full map with chunk data
            foreach (var chunk in chunks)
                for (int r = 0; r < chunk.Map.GetLength(0); r++)
                    for (int c = 0; c < chunk.Map.GetLength(1); c++)
                        Map[(int)(chunk.Pos.X - OffsetX + r), (int)(chunk.Pos.Y - OffsetY + c)] = chunk.Map[r, c];


            //Render the map to the render target
            parent.batch.disposeEvent += Dispose;
            rt = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, (int)(RightBound - OffsetX) * ChunkSize, (int)(BottomBound - OffsetY) * ChunkSize);
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rt);
            Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.spriteBatch.Begin();
            for (int r = 0; r < Map.GetLength(0); r++)
            {
                for (int c = 0; c < Map.GetLength(1); c++)
                {
                    if (Map[r, c] == 0)
                        continue;

                    int tileID = Map[r, c];

                    //Handle tile flipping / rotation
                    SpriteEffects spriteEffect = SpriteEffects.None;
                    float rotation = 0f;
                    
                    //Separate flags from tile id
                    int flags = (tileID >> 29) & 0b111;
                    tileID &= ~(0b111 << 29);
                    tileID--; //this is arbitrairy right now, but can be changed to tilesets firstgrid property

                    //Apply effects according to flags [horizontal flip, verical, flip, anti-diagonal flip]
                    switch (flags)
                    {
                        case 0b000:
                            break;
                        case 0b001:
                            rotation = MathF.PI * -0.5f;
                            break;
                        case 0b010:
                            rotation = MathF.PI * -0.5f;
                            break;
                        case 0b011:
                            spriteEffect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                            rotation = MathF.PI * 0.5f;
                            break;
                        case 0b100:
                            spriteEffect = SpriteEffects.FlipHorizontally;
                            break;
                        case 0b101:
                            rotation = MathF.PI / 2f;
                            break;
                        case 0b110:
                            spriteEffect = SpriteEffects.FlipHorizontally | SpriteEffects.FlipVertically;
                            break;
                        case 0b111:
                            rotation = MathF.PI;
                            break;
                    }

                    //Draw tile
                    Globals.spriteBatch.Draw(tilemap,
                        new Vector2(r * TileSize + TileSize / 2f, c * TileSize + TileSize / 2f),
                        tileRects[tileID],
                        Color.White,
                        rotation, 
                        new Vector2(TileSize / 2f, TileSize / 2f),
                        1f,
                        spriteEffect,
                        0f);
                }
            }
            Globals.spriteBatch.End();
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        public override void Update(GameTime gameTime) { }

        public void Draw()
        {
            Globals.spriteBatch.Draw(rt,
                new Vector2(
                    (OffsetX * ChunkSize * TileScale - Globals.Camera.X) * Globals.Camera.Zoom, 
                    (OffsetY * ChunkSize * TileScale - Globals.Camera.Y) * Globals.Camera.Zoom),
                new Rectangle(0, 0, (int)(RightBound - OffsetX) * ChunkSize, (int)(BottomBound - OffsetY) * ChunkSize),
                Color.White,
                0f,
                Vector2.Zero,
                TileScale * Globals.Camera.Zoom,
                SpriteEffects.None,
                Layer);
        }

        public void Dispose()
        {
            if (rt is not null)
                rt.Dispose();
        }
    }
}
