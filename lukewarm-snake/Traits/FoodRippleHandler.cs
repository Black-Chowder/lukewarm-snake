using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace lukewarm_snake
{
    public class FoodRippleHandler : TUpdates, TDrawsRippleInfluence
    {
        private Entity parent;

        private RenderTarget2D tailRt;
        private static Point tailRtSize => new Point(75, 140);
        private static Effect tailEffect;

        public int Priority => Trait.defaultPriority + 1;

        public FoodRippleHandler(Entity parent)
        {
            this.parent = parent;

            tailEffect ??= content.Load<Effect>(@"Effects/FoodTailRipple");
            tailRt = new RenderTarget2D(spriteBatch.GraphicsDevice, tailRtSize.X, tailRtSize.Y);
        }

        public void Update()
        {
            tailEffect.Parameters["iTime"].SetValue(parent.GetTrait<FoodRenderer>().iTime);
            tailEffect.CurrentTechnique.Passes[0].Apply();

            spriteBatch.GraphicsDevice.SetRenderTarget(tailRt);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: tailEffect);

            spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                new Rectangle(0, 0, tailRt.Width, tailRt.Height),
                Color.White);

            spriteBatch.End();
        }

        public void DrawRippleInfluence()
        {
            if (parent.DeltaPos.LengthSquared() > 0)
                spriteBatch.Draw(tailRt,
                    parent.DrawPos * EntityBatch.PixelateMultiplier,
                    null,
                    Color.White,
                    parent.GetTrait<FoodMovement>().Angle + MathF.PI / 2f,
                    new Vector2(tailRt.Width / 2f, 0f),
                    0.5f * EntityBatch.PixelateMultiplier,
                    SpriteEffects.None,
                    0f);
        }
    }
}
