using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private float headAngle = 0f;

        private static RenderTarget2D headTexture;

        private RenderTarget2D bodyTexture;
        public const float BodyRadius = 50f;
        private RenderTarget2D rtBody;
        private RenderTarget2D rtHead;
        private const int headRtBuffer = 4;
        private RenderTarget2D rt;

        private static Effect circleShader;
        private static Effect border;
        private static Effect shadowShader;

        public int Priority => Trait.defaultPriority;

        public TailRenderer(Entity parent, TailHandler tail)
        {
            this.parent = parent;
            this.tail = tail;

            //Load head texture
            if (headTexture is null)
            {
                Texture2D rawHeadTexture = Globals.content.Load<Texture2D>(@"Graphics/SnakeHead");
                headTexture = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, rawHeadTexture.Width, rawHeadTexture.Height);
                Globals.spriteBatch.GraphicsDevice.SetRenderTarget(headTexture);
                Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                Globals.spriteBatch.Draw(rawHeadTexture,
                    Vector2.Zero,
                    Color.Green);
                Globals.spriteBatch.End();
                Globals.spriteBatch.GraphicsDevice.SetRenderTarget(null);
            }
            

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
                    Color.Green,
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
            rtHead = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, headTexture.Width + headRtBuffer, headTexture.Height + headRtBuffer);

            //Load border effect
            border ??= Globals.content.Load<Effect>(@"Effects/Border");

            //Load shadow effect
            shadowShader ??= Globals.content.Load<Effect>(@"Effects/BodyShadow");
        }

        public void Update() => Prerender();

        public void Prerender()
        {
            Vector2 drawPos;
            Color shadowBodyColor = new Color(0f, 0.2f, 0f, 1f);

            border.Parameters["OutlineColor"].SetValue(new Vector4(0, 0, 0, 1));
            border.Parameters["texelSize"].SetValue(new Vector2(1f / (rt.Width - 1f), 1f / (rt.Height - 1f)));
            border.CurrentTechnique.Passes[0].Apply();

            shadowShader.Parameters["LightAngle"].SetValue(Globals.ShadowAngle);
            shadowShader.CurrentTechnique.Passes[0].Apply();

            //Prerender head rotated
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rtHead);
            Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);

            //Calculate head angle
            if (tail.Anchors.First() != parent.Pos)
                headAngle = (tail.Anchors.First.Next.Value - parent.Pos).Atan2();

            Globals.spriteBatch.Draw(headTexture,
                new Vector2(rtHead.Width, rtHead.Height) / 2f,
                new Rectangle(0, 0, headTexture.Width, headTexture.Height),
                Color.White,
                headAngle - MathF.PI / 2f,
                new Vector2(headTexture.Width, headTexture.Height) / 2f,
                1f,
                SpriteEffects.None,
                0f);

            Globals.spriteBatch.End();


            //Draw plain body
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rtBody);
            Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp, effect: shadowShader);

            //Draw head
            drawPos = (parent.Pos) * EntityBatch.PixelateMultiplier;
            Rectangle drawRect = new((int)drawPos.X, (int)drawPos.Y, 17, 17);

            Globals.spriteBatch.Draw(rtHead,
                drawRect,
                null,
                shadowBodyColor,
                0f,
                new Vector2(rtHead.Width, rtHead.Height) / 2f,
                SpriteEffects.None,
                0f);

            //Draw body segments
            float tailIndex = 1f;
            if (tail.Anchors.Count > 0) for (LinkedListNode<Vector2> cur = tail.Anchors.First, next = cur.Next; next != null; cur = cur.Next, next = cur.Next, tailIndex++)
            {
                drawPos = (Vector2.Lerp(next.Value, cur.Value, tail.FormingAnchorProgress)) * EntityBatch.PixelateMultiplier;
                Globals.spriteBatch.Draw(bodyTexture,
                    drawPos,
                    new Rectangle(0, 0, bodyTexture.Width, bodyTexture.Height),
                    shadowBodyColor,
                    0f,
                    new Vector2(bodyTexture.Width, bodyTexture.Height) / 2f,
                    EntityBatch.PixelateMultiplier,
                    SpriteEffects.None,
                    (tailIndex + 1f) / (tail.Anchors.Count + 1f));
            }

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
