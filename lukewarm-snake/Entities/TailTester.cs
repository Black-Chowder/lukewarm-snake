using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlackMagic.Globals;
using BlackMagic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace lukewarm_snake
{
    public class TailTester : Entity
    {
        LinkedList<Vector2> anchors = new();
        Vector2 startPos = new Vector2(300, Globals.Camera.Height) / 2f;
        const int anchorsCount = 5;
        const float anchorDist = 200f;

        float digestProgress = 0f;
        const float DigestProgressStep = 0.002f;

        public TailTester() : base(Vector2.Zero)
        {
            //initialize anchors
            for (int i = 0; i < anchorsCount; i++)
            {
                anchors.AddLast(new Vector2(i * anchorDist, 0) + startPos);
            }
        }

        public override void Update()
        {
            digestProgress += DigestProgressStep;
            digestProgress = MathHelper.Clamp(digestProgress, 0f, 1f);
        }

        public override void Draw()
        {
            int tailIndex = 0;
            for (LinkedListNode<Vector2> cur = anchors.First; cur != null; cur = cur.Next, tailIndex++)
            {
                float sizeScaler = 1f;
                float tailProgress = (float)tailIndex / anchors.Count;
                float dist = MathF.Abs(tailProgress - digestProgress);
                sizeScaler = 1f - dist;

                spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                    cur.Value * EntityBatch.PixelateMultiplier,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One * (sizeScaler * 5f + 1f),
                    SpriteEffects.None,
                    0f);
            }

            Vector2 digestProgressDrawPos = startPos + new Vector2(digestProgress * anchorsCount * anchorDist, -100);
            digestProgressDrawPos *= EntityBatch.PixelateMultiplier;
            spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                digestProgressDrawPos,
                null,
                Color.Red,
                0f,
                Vector2.Zero,
                Vector2.One * 5f,
                SpriteEffects.None,
                0f);
        }
    }
}
