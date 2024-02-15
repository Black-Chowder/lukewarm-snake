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
    public class SnakeRenderer : TUpdates, TDraws
    {
        private TailHandler tail;
        private Entity parent;

        //Describe head
        private static RenderTarget2D headTexture;
        private const int headRtBuffer = 4;
        private float headAngle = 0f;

        //Describe eyes
        private static RenderTarget2D eyeTexture;
        private const int eyeRadius = 25;
        private const int eyeRtBuffer = 4;
        private Vector2 eyeOffset => new Vector2(-20, 40);

        //Describe body
        private RenderTarget2D bodyTexture;
        public const float BodyRadius = 50f;

        //Describe end of tail
        public const int ShrinkBeginIndex = 15;
        public const float TailEndRadius = 5f;

        //Render targets (in order of use)
        private RenderTarget2D rtHead; //Adjust head angle
        private RenderTarget2D rtBody; //Apply shadow shader to head + body parts
        private RenderTarget2D rtBorder; //Add border to body
        private RenderTarget2D rt; //Border effect
        private Point rtSize => new Point(Globals.MainEntityBatch.rt.Width, Globals.MainEntityBatch.rt.Height);

        private static Effect circleShader;
        private static Effect border;
        private static Effect shadowShader;

        public int Priority => Trait.defaultPriority;

        public SnakeRenderer(Entity parent, TailHandler tail)
        {
            this.parent = parent;
            this.tail = tail;

            
            //Load border effect
            border ??= Globals.content.Load<Effect>(@"Effects/Border");

            //Load shadow effect
            shadowShader ??= Globals.content.Load<Effect>(@"Effects/BodyShadow");


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


            //Create eye texture
            if (eyeTexture is null)
            {
                //Create circle
                eyeTexture = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, eyeRadius * 2, eyeRadius * 2);
                Globals.spriteBatch.GraphicsDevice.SetRenderTarget(eyeTexture);
                Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: circleShader);
                Globals.spriteBatch.Draw(DrawUtils.createTexture(Globals.spriteBatch.GraphicsDevice),
                    Vector2.Zero,
                    new Rectangle(0, 0, 1, 1),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    new Vector2(eyeTexture.Width, eyeTexture.Height),
                    SpriteEffects.None,
                    0f);
                Globals.spriteBatch.End();
            }


            //Initialize render targets
            rt = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, rtSize.X, rtSize.Y);
            rtHead = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, headTexture.Width + headRtBuffer, headTexture.Height + headRtBuffer); 
            rtBody = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, rtSize.X, rtSize.Y);
            rtBorder = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, rtSize.X, rtSize.Y);
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
            int tailIndex = 1;
            for (LinkedListNode<Vector2> cur = tail.Anchors.First, next = cur.Next; next != null; cur = cur.Next, next = cur.Next, tailIndex++)
            {
                drawPos = Vector2.Lerp(next.Value, cur.Value, tail.FormingAnchorProgress) * EntityBatch.PixelateMultiplier;
                
                //Calculate draw scale
                Vector2 drawScale = Vector2.One * EntityBatch.PixelateMultiplier;
                int tailEndProgressIndex = tail.Anchors.Count - tailIndex;
                if (tailEndProgressIndex <= ShrinkBeginIndex)
                    drawScale = Vector2.One * EntityBatch.PixelateMultiplier * MathHelper.Lerp(TailEndRadius, BodyRadius, (float)tailEndProgressIndex / ShrinkBeginIndex) / BodyRadius;

                //Draw body ball
                Globals.spriteBatch.Draw(bodyTexture,
                    drawPos,
                    new Rectangle(0, 0, bodyTexture.Width, bodyTexture.Height),
                    shadowBodyColor,
                    0f,
                    new Vector2(bodyTexture.Width, bodyTexture.Height) / 2f,
                    drawScale,
                    SpriteEffects.None,
                    (tailIndex + 1f) / (tail.Anchors.Count + 1f));
            }

            Globals.spriteBatch.End();


            //Add border to body
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rtBorder);
            Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: border);
            Globals.spriteBatch.Draw(rtBody, Vector2.Zero, Color.White);
            Globals.spriteBatch.End();


            //Add eyes
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(rt);
            Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
            Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            //Draw bordered body
            Globals.spriteBatch.Draw(rtBorder, Vector2.Zero, Color.White);

            //Draw Eyes
            float eyeOffsetMagnitude = eyeOffset.Length();
            for (int i = 0; i < 2; i++)
            {
                Vector2 curEyeOffset = i == 0 ? eyeOffset : new Vector2(eyeOffset.X, -eyeOffset.Y);
                float eyeOffsetAngle = curEyeOffset.Atan2();
                Vector2 angleAdjustedOffset = (eyeOffsetAngle + headAngle).ToVector2() * eyeOffsetMagnitude;

                Globals.spriteBatch.Draw(eyeTexture,
                    (parent.Pos + angleAdjustedOffset) * EntityBatch.PixelateMultiplier,
                    new Rectangle(0, 0, eyeTexture.Width, eyeTexture.Height),
                    Color.White,
                    0f,
                    new Vector2(eyeTexture.Width, eyeTexture.Height) / 2f,
                    EntityBatch.PixelateMultiplier,
                    SpriteEffects.None,
                    0f);
            }

            Globals.spriteBatch.End();
            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw() => Globals.spriteBatch.Draw(rt, Vector2.Zero, Color.White);
    }
}
