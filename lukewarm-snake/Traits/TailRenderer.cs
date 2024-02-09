using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace lukewarm_snake
{
    public class TailRenderer : TDraws
    {
        private TailHandler tail;
        private Entity parent;

        private Texture2D bodyTexture;
        private float bodyRadius = 50;

        public TailRenderer(Entity parent, TailHandler tail)
        {
            this.parent = parent;
            this.tail = tail;

            bodyTexture ??= DrawUtils.createCircleTexture(Globals.spriteBatch.GraphicsDevice, (int)(bodyRadius * 2f));
        }

        public void Draw()
        {
            //Draw body segments
            for (LinkedListNode<Vector2> cur = tail.Anchors.First, next = cur.Next; next != null; cur = cur.Next, next = cur.Next)
            {
                Globals.spriteBatch.Draw(bodyTexture,
                    (Vector2.Lerp(next.Value, cur.Value, tail.FormingAnchorProgress) - Vector2.One * bodyRadius) * EntityBatch.PixelateMultiplier,
                    new Rectangle(0, 0, bodyTexture.Width, bodyTexture.Height),
                    Color.Green,
                    0f,
                    Vector2.Zero,
                    EntityBatch.PixelateMultiplier,
                    SpriteEffects.None,
                    0f);
            }

            //Draw head
            Globals.spriteBatch.Draw(bodyTexture,
                (parent.Pos - Vector2.One * bodyRadius) * EntityBatch.PixelateMultiplier,
                new Rectangle(0, 0, bodyTexture.Width, bodyTexture.Height),
                Color.White,
                0f,
                Vector2.Zero,
                EntityBatch.PixelateMultiplier,
                SpriteEffects.None,
                0f);
        }
    }
}
