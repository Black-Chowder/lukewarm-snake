using BlackMagic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlackMagic.Globals;

namespace lukewarm_snake
{
    public class ObstacleRippleHandler : TDrawsRippleInfluence
    {
        private Entity parent;

        public ObstacleRippleHandler(Entity parent)
        {
            this.parent = parent;
        }

        public void DrawRippleInfluence()
        {
            spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                parent.DrawPos * EntityBatch.PixelateMultiplier,
                null,
                Color.White,
                parent.GetTrait<ObstacleMovement>().Heading.Atan2(),
                Vector2.Zero,
                Vector2.One,
                SpriteEffects.None,
                0f);
        }
    }
}
