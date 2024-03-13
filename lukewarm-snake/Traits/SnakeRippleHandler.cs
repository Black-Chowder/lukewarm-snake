using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lukewarm_snake
{
    public class SnakeRippleHandler : TDrawsRippleInfluence
    {
        private Entity parent;

        private static RenderTarget2D headTexture;

        public int Priority { get => Trait.defaultPriority; }
        public SnakeRippleHandler(Entity parent) 
        {
            this.parent = parent;

            //Prerender head texture
            if (headTexture is null)
            {
                Texture2D rawHeadTexture = content.Load<Texture2D>(@"Graphics/SnakeHead");
                headTexture = new RenderTarget2D(spriteBatch.GraphicsDevice, rawHeadTexture.Width, rawHeadTexture.Height);
                spriteBatch.GraphicsDevice.SetRenderTarget(headTexture);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                spriteBatch.Draw(rawHeadTexture, Vector2.Zero, Color.White);
                spriteBatch.End();
                rawHeadTexture.Dispose();
            }
        }

        public void DrawRippleInfluence()
        {
            Vector2 drawPos = parent.Pos * EntityBatch.PixelateMultiplier;
            Rectangle drawRect = new((int)drawPos.X, (int)drawPos.Y, SnakeRenderer.HeadSize, SnakeRenderer.HeadSize);

            spriteBatch.Draw(headTexture,
                drawRect,
                null,
                Color.White,
                parent.GetTrait<SnakeRenderer>().HeadAngle - MathF.PI / 2f,
                new Vector2(headTexture.Width, headTexture.Height) / 2f,
                SpriteEffects.None,
                0f);
        }
    }
}
