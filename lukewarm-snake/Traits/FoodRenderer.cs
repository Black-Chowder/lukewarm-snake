using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lukewarm_snake
{
    public class FoodRenderer : TUpdates, TDraws
    {
        private Entity parent;

        public const int FoodRadius = 50;

        private static RenderTarget2D texture;
        private RenderTarget2D rtBuffer;
        private RenderTarget2D rt;

        private static Effect border;

        private RenderTarget2D tailRt;
        private float iTime = 0f;
        private static Point tailRtSize => new Point(75, 140);
        private static Effect tailEffect;

        public int Priority => Trait.defaultPriority;

        public FoodRenderer(Entity parent)
        {
            this.parent = parent;

            //Create center circle texture
            if (texture is null)
            {
                int biggestFoodRadius = FoodRadius;
                texture = new RenderTarget2D(spriteBatch.GraphicsDevice, biggestFoodRadius * 2, biggestFoodRadius * 2);
                spriteBatch.GraphicsDevice.SetRenderTarget(texture);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: content.Load<Effect>(@"Effects/CircleMask"));
                spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                    Vector2.Zero,
                    new Rectangle(0, 0, 1, 1),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(texture.Width, texture.Height),
                    SpriteEffects.None,
                    0f);
                spriteBatch.End();
            }

            rtBuffer = new RenderTarget2D(spriteBatch.GraphicsDevice, MainEntityBatch.rt.Width, MainEntityBatch.rt.Height);
            rt = new RenderTarget2D(spriteBatch.GraphicsDevice, MainEntityBatch.rt.Width, MainEntityBatch.rt.Height);

            //Load border effect
            border ??= content.Load<Effect>(@"Effects/Border");

            //Load tail effect
            tailEffect ??= content.Load<Effect>(@"Effects/FoodTail");
            tailRt = new RenderTarget2D(spriteBatch.GraphicsDevice, tailRtSize.X, tailRtSize.Y);
        }

        public void Update() => Prerender();

        public void Prerender()
        {
            //Prerender tail
            iTime += MathF.Min(MathF.Max(MinTimeMod, TimeMod), 2f);
            tailEffect.Parameters["iTime"].SetValue(iTime);
            tailEffect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.GraphicsDevice.SetRenderTarget(tailRt);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: tailEffect);

            spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                new Rectangle(0, 0, tailRt.Width, tailRt.Height),
                Color.White);

            spriteBatch.End();


            border.Parameters["OutlineColor"].SetValue(new Vector4(0, 0, 0, 1));
            border.Parameters["texelSize"].SetValue(new Vector2(1f / (rt.Width - 1f), 1f / (rt.Height - 1f)));
            border.CurrentTechnique.Passes[0].Apply();

            spriteBatch.GraphicsDevice.SetRenderTarget(rtBuffer);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(texture,
                parent.DrawPos * EntityBatch.PixelateMultiplier,
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.DarkOliveGreen,
                0f,
                new Vector2(texture.Width, texture.Height) / 2f,
                EntityBatch.PixelateMultiplier,
                SpriteEffects.None,
                0f);

            spriteBatch.Draw(tailRt,
                parent.DrawPos * EntityBatch.PixelateMultiplier,
                null,
                Color.White,
                parent.GetTrait<FoodMovement>().Angle + MathF.PI / 2f,
                new Vector2(tailRt.Width / 2f, 0f),
                EntityBatch.PixelateMultiplier,
                SpriteEffects.None,
                0f);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.SetRenderTarget(rt);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: border);
            spriteBatch.Draw(rtBuffer, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public void Draw() => spriteBatch.Draw(rt, Vector2.Zero, Color.White);
    }
}
