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

        RenderTarget2D newPrev;

        public SnakeRippleHandler()
        {
            newPrev = new RenderTarget2D(spriteBatch.GraphicsDevice, MainEntityBatch.BackgroundBuffer1.Width, MainEntityBatch.BackgroundBuffer1.Height);
        }

        public void Update()
        {
            MouseState mouseState = Mouse.GetState();
            Point mousePos = mouseState.Position;

            spriteBatch.GraphicsDevice.SetRenderTarget(newPrev);
            spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin();

            spriteBatch.Draw(MainEntityBatch.BackgroundBuffer1,
                new Rectangle(0, 0, MainEntityBatch.BackgroundBuffer1.Width, MainEntityBatch.BackgroundBuffer1.Height),
                Color.White);

            spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                new Vector2((int)(mousePos.X * EntityBatch.PixelateMultiplier), (int)(mousePos.Y * EntityBatch.PixelateMultiplier)),
                Color.White);

            spriteBatch.End();

            (MainEntityBatch.BackgroundBuffer1, newPrev) = (newPrev, MainEntityBatch.BackgroundBuffer1);
            spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }
    }
}
