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
    public class SnakeRippleHandler : TUpdates
    {
        public int Priority { get => Trait.defaultPriority; }

        public SnakeRippleHandler() { }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState();
            Point mousePos = mouseState.Position;

            spriteBatch.GraphicsDevice.SetRenderTarget(MainEntityBatch.RippleInfluenceBuffer);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();

            spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                new Vector2((int)(mousePos.X * EntityBatch.PixelateMultiplier), (int)(mousePos.Y * EntityBatch.PixelateMultiplier)),
                Color.White);

            spriteBatch.End();
            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
