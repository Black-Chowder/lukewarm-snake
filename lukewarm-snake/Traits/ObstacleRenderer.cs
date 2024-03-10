using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace lukewarm_snake
{
    public class ObstacleRenderer : TUpdates, TDraws
    {
        public Entity parent;

        public const int ObstacleRadius = 50;

        private static RenderTarget2D texture;
        private RenderTarget2D rtBuffer;
        private RenderTarget2D rt;

        private static Effect border;

        public int Priority => Trait.defaultPriority;

        public ObstacleRenderer(Entity parent)
        {
            this.parent = parent;

            //Create center circle texture
            if (texture is null)
            {
                int biggestFoodRadius = ObstacleRadius;
                texture = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, biggestFoodRadius * 2, biggestFoodRadius * 2);
                Globals.spriteBatch.GraphicsDevice.SetRenderTarget(texture);
                Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: Globals.content.Load<Effect>(@"Effects/CircleMask"));
                Globals.spriteBatch.Draw(DrawUtils.createTexture(Globals.spriteBatch.GraphicsDevice),
                    Vector2.Zero,
                    new Rectangle(0, 0, 1, 1),
                    Color.White,
                    0f,
                Vector2.Zero,
                new Vector2(texture.Width, texture.Height),
                    SpriteEffects.None,
                    0f);
                Globals.spriteBatch.End();
            }

            rtBuffer = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, Globals.MainEntityBatch.rt.Width, Globals.MainEntityBatch.rt.Height);
            rt = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, Globals.MainEntityBatch.rt.Width, Globals.MainEntityBatch.rt.Height);

            //Load border effect
            border ??= Globals.content.Load<Effect>(@"Effects/Border");
        }

        public void Update()
        {
            if (parent.exists) Prerender();
        }

        public void Prerender()
        {
            border.Parameters["OutlineColor"].SetValue(new Vector4(0, 0, 0, 1));
            border.Parameters["texelSize"].SetValue(new Vector2(1f / (rt.Width - 1f), 1f / (rt.Height - 1f)));
            border.CurrentTechnique.Passes[0].Apply();

            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rtBuffer);
            Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Globals.spriteBatch.Draw(texture,
                parent.DrawPos * EntityBatch.PixelateMultiplier,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.Red,
                0f,
                new Vector2(texture.Width, texture.Height) / 2f,
                EntityBatch.PixelateMultiplier,
                SpriteEffects.None,
                0f);

            Globals.spriteBatch.End();
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rt);
            Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: border);
            Globals.spriteBatch.Draw(rtBuffer, Vector2.Zero, Color.White);
            Globals.spriteBatch.End();
        }

        public void Draw() => Globals.spriteBatch.Draw(rt, Vector2.Zero, Color.White);

        public void DisposeIndividual() //I'll have to actually handle individual disposing better but this'll work for now
        {
            rtBuffer.Dispose();
            rt.Dispose();
        }
    }
}
