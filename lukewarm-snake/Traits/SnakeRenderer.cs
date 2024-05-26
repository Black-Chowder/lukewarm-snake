using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BlackMagic;
using Kryz.Tweening;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using static BlackMagic.Globals;

namespace lukewarm_snake
{
    public class SnakeRenderer : TUpdates, TDraws
    {
        private TailHandler tail;
        private Entity parent;

        //Digestion Variables
        private readonly LinkedList<float> digestProgresses = new();

        //Death Variables
        public enum DeathStates
        {
            Alive,//When the player is alive
            Reaction,//When first hit.  Bullets get deleted and eyes grow
            Decomposition, //some time after Reaction.  Tail bits grow and fade "explode" starting from end going up to head
            Linger //Wait a bit before showing game over screen
        }
        public DeathStates DeathState { get; set; } = DeathStates.Alive;
        public TimeSpan ReactionPhaseStartTime = TimeSpan.Zero;
        public TimeSpan ReactionPhaseTime = new TimeSpan(0, 0, 0, 1);
        private const float ReactionHeadSizeModifier = 1.25f;
        private float decompositionTimer = 0f;
        private const float DecompositionTimerStep = 0.12f;
        public TimeSpan LingerPhaseStartTime = TimeSpan.Zero;
        public TimeSpan LingerPhaseTime = new TimeSpan(0, 0, 0, 1);

        //Death sfx variables
        SoundEffect decompSfx;
        SoundEffectInstance decompSfxInstance;
        private float sfxTimer = 0f,
            sfxTimerStep = 0.025f;

        SoundEffect deathSfx;
        SoundEffectInstance deathSfxInstance;


        //Describe head
        private static RenderTarget2D headTexture;
        private const int headRtBuffer = 4;
        public float HeadAngle { get; private set; } = 0f;
        public const int HeadSize = 17;

        //Describe eyes
        private static RenderTarget2D eyeTexture;
        private const int eyeRadius = 25;
        private Color eyeColor => Color.White;
        private const int pupilRadius = 2 * eyeRadius / 3;
        private Color pupilColor => Color.Black;
        private const int eyeRtBuffer = 4;
        private Vector2 eyeOffset => new Vector2(-20, 40);
        private float eyesAngle = 0f;

        //Describe body
        private RenderTarget2D bodyTexture;
        private RenderTarget2D bandBodyTexture;
        public const float BodyRadius = 50f;
        public static Color MainBodyColor => new Color(130, 74, 22);
        public static Color BandBodyColor => new Color(64, 36, 10);
        public static Color ShadowBodyColor => new Color(64, 36, 10);
        public static Color ShadowBandColor => new Color(0, 0, 0);
        private const int BandFrequency = 5;

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

        public SnakeRenderer(Entity parent, TailHandler tail, SnakeHealth health = null)
        {
            this.parent = parent;
            this.tail = tail;

            //Load sfx
            decompSfx = content.Load<SoundEffect>(@"SFX/Pressing Down Sizzling Burger");
            decompSfxInstance = decompSfx.CreateInstance();
            decompSfxInstance.IsLooped = true;

            deathSfx = content.Load<SoundEffect>(@"SFX/GS projectile splash 004");
            deathSfxInstance = deathSfx.CreateInstance();


            //Subscribe to death event
            if (health is not null) health.deathEvent += OnDeath;

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
                    MainBodyColor);
                Globals.spriteBatch.End();
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
                    MainBodyColor,
                    0f,
                    Vector2.Zero,
                    new Vector2(bodyTexture.Width, bodyTexture.Height),
                    SpriteEffects.None,
                    0f);
                Globals.spriteBatch.End();
            }

            if (bandBodyTexture is null)
            {
                bandBodyTexture = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, (int)(BodyRadius * 2f), (int)(BodyRadius * 2f));
                Globals.spriteBatch.GraphicsDevice.SetRenderTarget(bandBodyTexture);
                Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: circleShader);
                Globals.spriteBatch.Draw(DrawUtils.createTexture(Globals.spriteBatch.GraphicsDevice),
                    Vector2.Zero,
                    new Rectangle(0, 0, 1, 1),
                    BandBodyColor,
                    0f,
                    Vector2.Zero,
                    new Vector2(bodyTexture.Width, bodyTexture.Height),
                    SpriteEffects.None,
                    0f);
                Globals.spriteBatch.End();
            }


            //Create eye texture
            if (eyeTexture is null)
            {
                //Create pupil
                RenderTarget2D pupilRt = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, pupilRadius * 2, pupilRadius);
                Globals.spriteBatch.GraphicsDevice.SetRenderTarget(pupilRt);
                Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: circleShader);
                Globals.spriteBatch.Draw(DrawUtils.createTexture(Globals.spriteBatch.GraphicsDevice),
                    Vector2.Zero,
                    new Rectangle(0, 0, 1, 1),
                    pupilColor,
                    0f,
                    Vector2.Zero,
                    new Vector2(pupilRt.Width, pupilRt.Height),
                    SpriteEffects.None,
                    0f);
                Globals.spriteBatch.End();

                //Create circle
                eyeTexture = new RenderTarget2D(Globals.spriteBatch.GraphicsDevice, eyeRadius * 2, eyeRadius * 2);
                Globals.spriteBatch.GraphicsDevice.SetRenderTarget(eyeTexture);
                Globals.spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                Globals.spriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: circleShader);

                Globals.spriteBatch.Draw(DrawUtils.createTexture(Globals.spriteBatch.GraphicsDevice),
                    Vector2.Zero,
                    new Rectangle(0, 0, 1, 1),
                    eyeColor,
                    0f,
                    Vector2.Zero,
                    new Vector2(eyeTexture.Width, eyeTexture.Height),
                    SpriteEffects.None,
                    0f);

                Globals.spriteBatch.Draw(pupilRt,
                    new Vector2(eyeTexture.Width, eyeTexture.Height / 2f),
                    new Rectangle(0, 0, pupilRt.Width, pupilRt.Height),
                    Color.White,
                    0f,
                    new Vector2(pupilRt.Width, pupilRt.Height / 2f),
                    1f,
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

        public void Update()
        {
            //Update digestion
            for (LinkedListNode<float> curNode = digestProgresses.First; curNode != null; curNode = curNode.Next)
            {
                curNode.Value += 0.01f;
                if (curNode.Value > 2f)
                    digestProgresses.Remove(curNode);
            }

            //Handle Death
            switch (DeathState)
            {
                case (DeathStates.Reaction):
                    //Reaction state complete
                    if (gt.TotalGameTime - ReactionPhaseStartTime > ReactionPhaseTime)
                    {
                        decompSfxInstance.Play();
                        DeathState = DeathStates.Decomposition;
                    }
                    break;

                case (DeathStates.Decomposition):
                    decompositionTimer += DecompositionTimerStep;

                    //Handle sfx
                    sfxTimer += sfxTimerStep;
                    sfxTimer = MathHelper.Clamp(sfxTimer, 0f, 1f);
                    decompSfxInstance.Volume = EasingFunctions.InOutCubic(sfxTimer);

                    //Decomposition state is over gate
                    if (decompositionTimer < tail.Anchors.Count + 1)
                        break;

                    DeathState = DeathStates.Linger;
                    LingerPhaseStartTime = gt.TotalGameTime;
                    deathSfxInstance.Play();

                    break;

                case (DeathStates.Linger):

                    //Handle sfx
                    sfxTimer -= sfxTimerStep;
                    sfxTimer = MathHelper.Clamp(sfxTimer, 0f, 1f);
                    decompSfxInstance.Volume = EasingFunctions.InOutCubic(sfxTimer);

                    if (gt.TotalGameTime - LingerPhaseStartTime > LingerPhaseTime)
                        GameState = GameStates.GameOver; //Transition to death scene
                    break;
            }

            Prerender();
        }

        public void Prerender()
        {
            Vector2 drawPos;

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
                HeadAngle = (tail.Anchors.First.Next.Value - parent.Pos).Atan2();

            Globals.spriteBatch.Draw(headTexture,
                new Vector2(rtHead.Width, rtHead.Height) / 2f,
                new Rectangle(0, 0, headTexture.Width, headTexture.Height),
                Color.White,
                HeadAngle - MathF.PI / 2f,
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

            //Calculate size
            float headDigestModifier = 1f;
            if (digestProgresses.Count > 0)
            {
                //Get closest digest progress
                float dist = float.PositiveInfinity;
                for (LinkedListNode<float>curDigestProgress = digestProgresses.First; curDigestProgress != null; curDigestProgress = curDigestProgress.Next)
                {
                    float adjustedDigestProgress = curDigestProgress.Value;
                    dist = MathF.Min(MathF.Abs(adjustedDigestProgress), dist);
                }
                headDigestModifier = 1f - dist + 0.5f;
            }
            headDigestModifier = MathHelper.Clamp(headDigestModifier, 1f, 2f);

            float headDeathSizeModifier = 1f;
            float headDeathOpacityModifier = 1f;
            if (DeathState == DeathStates.Decomposition)
            {
                //Calculate which index is currently being decomposed
                int curDecompIndex = (int)MathF.Floor(decompositionTimer);

                if (curDecompIndex == tail.Anchors.Count)
                {
                    float headDecompProgress = decompositionTimer % 1f;

                    headDeathSizeModifier = EasingFunctions.OutCubic(headDecompProgress) + 1f;
                    headDeathOpacityModifier = 1f - EasingFunctions.InCubic(headDecompProgress);
                }
                else if (curDecompIndex > tail.Anchors.Count)
                {
                    headDeathOpacityModifier = 0f;
                }
            }
            else if (DeathState == DeathStates.Linger)
            {
                headDeathOpacityModifier = 0f;
            }

            //Handle head size on death
            if (DeathState != DeathStates.Alive)
                headDigestModifier = ReactionHeadSizeModifier;

            float curHeadSize = HeadSize * headDigestModifier * headDeathSizeModifier; //Head size on current frame
            Rectangle drawRect = new((int)drawPos.X, (int)drawPos.Y, (int)(curHeadSize), (int)(curHeadSize));

            Globals.spriteBatch.Draw(rtHead,
                drawRect,
                null,
                new Color(ShadowBodyColor, headDeathOpacityModifier),
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

                //Calculate digest modifier (size of segment based on digestion)
                float digestModifier = 1f;
                if (digestProgresses.Count > 0)
                {
                    //Get closest digest progress
                    float tailProgress = (float)tailIndex / tail.Anchors.Count;
                    float dist = float.PositiveInfinity;
                    for (LinkedListNode<float> curDigestProgress = digestProgresses.First; curDigestProgress != null; curDigestProgress = curDigestProgress.Next)
                    {
                        float adjustedDigestProgress = curDigestProgress.Value;
                        dist = MathF.Min(MathF.Abs(tailProgress - adjustedDigestProgress), dist);
                    }
                    digestModifier = 1f - dist + 0.5f;
                }
                digestModifier = MathHelper.Clamp(digestModifier, 1f, 2f);

                //Calculate size of segment based on death progress
                float deathSizeModifier = 1f;
                float deathOpacityModifier = 1f;
                if (DeathState == DeathStates.Decomposition)
                {
                    //Calculate progress of current segment's decomposition
                    float curDecompSegmentProgress = decompositionTimer % 1f;
                    
                    //Calculate which index is currently being decomposed
                    int curDecompIndex = (int)MathF.Floor(decompositionTimer);
                    curDecompIndex = tail.Anchors.Count - curDecompIndex;

                    //If is tail segment that is actively decomposing
                    if (tailIndex == curDecompIndex)
                    {
                        deathSizeModifier = EasingFunctions.OutCubic(curDecompSegmentProgress) + 1f;
                        deathOpacityModifier = 1f - EasingFunctions.InCubic(curDecompSegmentProgress);
                    }
                    
                    //Current decomposition index past point of decomposition
                    else if (tailIndex > curDecompIndex)
                        deathOpacityModifier = 0f; //Make segment transparent
                }

                //Keep segment transparent if in linger state
                else if (DeathState == DeathStates.Linger)
                    deathOpacityModifier = 0f;


                //Calculate which body texture to use
                RenderTarget2D curBodyTexture = bodyTexture;
                Color shadowColor = ShadowBodyColor;
                if (tailIndex % BandFrequency == 0)
                {
                    curBodyTexture = bandBodyTexture;
                    shadowColor = ShadowBandColor;
                }

                //Draw body ball
                Globals.spriteBatch.Draw(curBodyTexture,
                    drawPos,
                    new Rectangle(0, 0, bodyTexture.Width, bodyTexture.Height),
                    new Color(shadowColor, deathOpacityModifier),
                    0f,
                    new Vector2(bodyTexture.Width, bodyTexture.Height) / 2f,
                    drawScale * digestModifier * deathSizeModifier,
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

            //Calculate eye angles to look at food
            float targetEyesAngle = HeadAngle - MathF.PI;
            FoodManager foodManager = parent.batch.GetEntityBucket<FoodManager>()?.First() as FoodManager;
            if (foodManager is not null) 
            {
                Food food = foodManager.Food;
                targetEyesAngle = (food.Pos - parent.Pos).Atan2();
            }
            eyesAngle = MathHelper.Lerp(eyesAngle, targetEyesAngle, 0.1f);

            //Draw Eyes
            float eyeOffsetMagnitude = eyeOffset.Length();
            for (int i = 0; i < 2 && DeathState != DeathStates.Linger; i++)
            {
                //Handle decomposition
                float eyeDeathSizeMultiplier = 1f;
                float eyeDeathPosOffsetMultiplier = 1f;
                float eyeDeathOpacityMultiplier = 1f;
                int curDecompIndex = (int)MathF.Floor(decompositionTimer);
                if (curDecompIndex == tail.Anchors.Count)
                {
                    float eyeDecompProgress = decompositionTimer % 1f;

                    eyeDeathSizeMultiplier = EasingFunctions.OutCubic(eyeDecompProgress) + 1f;
                    eyeDeathPosOffsetMultiplier = eyeDeathSizeMultiplier;
                    eyeDeathOpacityMultiplier = 1f - EasingFunctions.InCubic(eyeDecompProgress);
                }

                Vector2 curEyeOffset = i == 0 ? eyeOffset : new Vector2(eyeOffset.X, -eyeOffset.Y);
                float eyeOffsetAngle = curEyeOffset.Atan2();
                Vector2 angleAdjustedOffset = (eyeOffsetAngle + HeadAngle).ToVector2() * (eyeOffsetMagnitude * eyeDeathPosOffsetMultiplier) * headDigestModifier;

                Globals.spriteBatch.Draw(eyeTexture,
                    (parent.Pos + angleAdjustedOffset) * EntityBatch.PixelateMultiplier,
                    new Rectangle(0, 0, eyeTexture.Width, eyeTexture.Height),
                    new Color(Color.White, eyeDeathOpacityMultiplier),
                    eyesAngle,
                    new Vector2(eyeTexture.Width, eyeTexture.Height) / 2f,
                    EntityBatch.PixelateMultiplier * headDigestModifier * headDigestModifier * eyeDeathSizeMultiplier,
                    SpriteEffects.None,
                    0f);
            }

            Globals.spriteBatch.End();

            Globals.spriteBatch.GraphicsDevice.SetRenderTarget(null);
        }

        public void Draw() => Globals.spriteBatch.Draw(rt, Vector2.Zero, Color.White);

        public void EatenFood() => digestProgresses.AddLast(0f);

        private void OnDeath()
        {
            DeathState = DeathStates.Reaction;
            ReactionPhaseStartTime = gt.TotalGameTime;
        }
    }
}
