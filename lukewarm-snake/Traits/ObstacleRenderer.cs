using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace lukewarm_snake
{
    public class ObstacleRenderer : TUpdates, TDraws
    {
        public Entity parent;

        public const int ObstacleRadius = 35;

        private static Texture2D bulletMask;
        private static Point BulletSize => new Point(100, 100);
        private RenderTarget2D rtBuffer;
        private RenderTarget2D rt;

        //Bullet trail effects
        private static Effect tailEffect;
        private RenderTarget2D tailRt;
        private float iTimer = 0f;

        private const bool DrawHitbox = false;
        private static RenderTarget2D hitboxTexture;

        private static Effect border;

        public int Priority => Trait.defaultPriority;

        public ObstacleRenderer(Entity parent)
        {
            this.parent = parent;

            bulletMask ??= content.Load<Texture2D>(@"Graphics/Bullet");

            tailEffect ??= content.Load<Effect>(@"Effects/BulletTail");
            tailRt = new RenderTarget2D(spriteBatch.GraphicsDevice, (int)(BulletSize.X * 0.6f), BulletSize.Y * 4);

            //Create center circle texture
            if (hitboxTexture is null)
            {
                int biggestFoodRadius = ObstacleRadius;
                hitboxTexture = new RenderTarget2D(spriteBatch.GraphicsDevice, biggestFoodRadius * 2, biggestFoodRadius * 2);
                spriteBatch.GraphicsDevice.SetRenderTarget(hitboxTexture);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: content.Load<Effect>(@"Effects/CircleMask"));
                spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                    Vector2.Zero,
                    new Rectangle(0, 0, 1, 1),
                    Color.White,
                    0f,
                Vector2.Zero,
                new Vector2(hitboxTexture.Width, hitboxTexture.Height),
                    SpriteEffects.None,
                    0f);
                spriteBatch.End();
            }

            rtBuffer = new RenderTarget2D(spriteBatch.GraphicsDevice, MainEntityBatch.rt.Width, MainEntityBatch.rt.Height);
            rt = new RenderTarget2D(spriteBatch.GraphicsDevice, MainEntityBatch.rt.Width, MainEntityBatch.rt.Height);

            //Load border effect
            border ??= content.Load<Effect>(@"Effects/Border");
        }

        public void Update()
        {
            if (parent.exists) Prerender();
        }

        public void Prerender()
        {
            //Prerender tail
            iTimer += MathF.Max(TimeMod, MinTimeMod) * 0.1f;
            tailEffect.Parameters["iTimer"].SetValue(iTimer);

            spriteBatch.GraphicsDevice.SetRenderTarget(tailRt);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: tailEffect);
            spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                new Rectangle(0, 0, tailRt.Width, tailRt.Height),
                Color.White);
            spriteBatch.End();


            //Draw bullet
            border.Parameters["OutlineColor"].SetValue(new Vector4(0, 0, 0, 1));
            border.Parameters["texelSize"].SetValue(new Vector2(1f / (rt.Width - 1f), 1f / (rt.Height - 1f)));
            border.CurrentTechnique.Passes[0].Apply();

            spriteBatch.GraphicsDevice.SetRenderTarget(rtBuffer);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            if (DrawHitbox)
                spriteBatch.Draw(hitboxTexture,
                    parent.DrawPos * EntityBatch.PixelateMultiplier,
                    new Rectangle(0, 0, hitboxTexture.Width, hitboxTexture.Height),
                    Color.Blue,
                    0f,
                    new Vector2(hitboxTexture.Width, hitboxTexture.Height) / 2f,
                    EntityBatch.PixelateMultiplier,
                    SpriteEffects.None,
                    0f);


            spriteBatch.Draw(tailRt,
                (parent.DrawPos) * EntityBatch.PixelateMultiplier,
                null,
                Color.Red,
                parent.GetTrait<ObstacleMovement>().Heading.Atan2() - MathF.PI / 2f,
                new Vector2(tailRt.Width / 2f, tailRt.Height),
                EntityBatch.PixelateMultiplier,
                SpriteEffects.None,
                0f);

            spriteBatch.Draw(bulletMask,
                parent.DrawPos * EntityBatch.PixelateMultiplier,
                null,
                Color.White,
                parent.GetTrait<ObstacleMovement>().Heading.Atan2(),
                new Vector2(bulletMask.Width, bulletMask.Height) / 2f,
                BulletSize.ToVector2() / new Vector2(bulletMask.Width, bulletMask.Height) * EntityBatch.PixelateMultiplier,
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

        public void DisposeIndividual() //I'll have to actually handle individual disposing better but this'll work for now
        {
            rtBuffer.Dispose();
            rt.Dispose();
            tailRt.Dispose();
        }
    }
}
