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
    public class TailRenderer : TUpdates, TDraws
    {
        private TailHandler tail;
        private Entity parent;

        private RenderTarget2D bodyTexture;
        public const float BodyRadius = 50f;
        private RenderTarget2D rtBody;
        private RenderTarget2D rt;

        private static Effect circleShader;
        private static Effect border;

        public int Priority => Trait.defaultPriority;

        public TailRenderer(Entity parent, TailHandler tail)
        {
            this.parent = parent;
            this.tail = tail;


            //Create body texture
            circleShader ??= Globals.content.Load<Effect>(@"Effects/CircleMask");
            if (bodyTexture is null)
            {
                bodyTexture = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, (int)(BodyRadius * 2f), (int)(BodyRadius * 2f));
                Globals.spriteBatch.GraphicsDevice.SetRenderTarget(bodyTexture);
                Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: circleShader);
                Globals.spriteBatch.Draw(DrawUtils.createTexture(Globals.spriteBatch.GraphicsDevice),
                    Vector2.Zero,
                    new Rectangle(0, 0, 1, 1),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(bodyTexture.Width, bodyTexture.Height),
                    SpriteEffects.None,
                    0f);
                Globals.spriteBatch.End();
                Globals.spriteBatch.GraphicsDevice.SetRenderTarget(null);
            }

            //Initialize render targets
            rt = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, Globals.MainEntityBatch.rt.Width, Globals.MainEntityBatch.rt.Height);
            rtBody = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, Globals.MainEntityBatch.rt.Width, Globals.MainEntityBatch.rt.Height);

            //Load border effect
            border ??= Globals.content.Load<Effect>(@"Effects/Border");
        }

        public void Update() => Prerender();

        public void Prerender()
        {
            border.Parameters["OutlineColor"].SetValue(new Vector4(0, 0, 0, 1));
            border.Parameters["texelSize"].SetValue(new Vector2(1f / (rt.Width - 1f), 1f / (rt.Height - 1f)));
            border.CurrentTechnique.Passes[0].Apply();

            //Draw plain body
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rtBody);
            Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Vector2 drawPos;

            //Draw body segments
            for (LinkedListNode<Vector2> cur = tail.Anchors.First, next = cur.Next; next != null; cur = cur.Next, next = cur.Next)
            {
                drawPos = (Vector2.Lerp(next.Value, cur.Value, tail.FormingAnchorProgress) - Vector2.One * BodyRadius) * EntityBatch.PixelateMultiplier;
                Globals.spriteBatch.Draw(bodyTexture,
                    drawPos,
                    new Rectangle(0, 0, bodyTexture.Width, bodyTexture.Height),
                    Color.Green,
                    0f,
                    Vector2.Zero,
                    EntityBatch.PixelateMultiplier,
                    SpriteEffects.None,
                    0f);
            }

            //Draw head
            drawPos = (parent.Pos - Vector2.One * BodyRadius) * EntityBatch.PixelateMultiplier;
            Globals.spriteBatch.Draw(bodyTexture,
                drawPos,
                new Rectangle(0, 0, bodyTexture.Width, bodyTexture.Height),
                Color.White,
                0f,
                Vector2.Zero,
                EntityBatch.PixelateMultiplier,
                SpriteEffects.None,
                0f);

            Globals.spriteBatch.End();
            
            //Apply border effect to body
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rt);
            Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: border);
            Globals.spriteBatch.Draw(rtBody, Vector2.Zero, Color.White);
            Globals.spriteBatch.End();

            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw() => Globals.spriteBatch.Draw(rt, Vector2.Zero, Color.White);
    }
}
