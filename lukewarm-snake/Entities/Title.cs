using BlackMagic;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Kryz.Tweening;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using static System.Formats.Asn1.AsnWriter;

namespace lukewarm_snake
{
    public class Title : Entity
    {
        public const string STitle = "SUPERHOT SNAKE";
        public const string Start = "START";
        public const string SScoreboard = "SCORES";

        private float timer = 0f;

        //Title Position Variables
        private static Vector2 TitleStartPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 TitleEndPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 10f);
        private readonly float[] titleLetterOffsets = new float[STitle.Length];
        private const float TitleLetterOffsetVariation = 0.5f;

        //Start button position variables
        private static Vector2 StartStartPos => new Vector2(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 StartEndPos => new Vector2(Globals.Camera.Width, Globals.Camera.Height) * EntityBatch.PixelateMultiplier / 2f;
        private readonly float[] startLetterOffsets = new float[Start.Length];
        private const float StartLetterOffsetVariation = 0.5f;
        private const float StartTimerWait = 0.25f;
        private const float BobTime = 1f;
        private const float BobDist = 8f;

        private bool isHoveringOverStart = false;
        private bool wasHovering = false;
        private Rectangle startHitbox;
        private static Vector2 StartHitboxBuffer => new(20, 5);

        //Scoreboard button variables
        public const bool ScoreboardButtonEnabled = false;
        private static Vector2 ScoreboardStartPos => new Vector2(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 ScoreboardEndPos => new Vector2(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, (Globals.Camera.Height + 150f) * EntityBatch.PixelateMultiplier / 2f + 20f);

        private bool isHoveringOverScore = false;
        private bool wasHoveringOverScore = false;
        private Rectangle scoreboardHitbox;
        private static Vector2 SoreboardHitboxBuffer => new(20, 5);

        //Sound effect variables
        SoundEffect hoverSfx;
        SoundEffectInstance hoverSfxInstance;
        SoundEffect clickSfx;
        SoundEffectInstance clickSfxInstance;

        public Title() : base(Vector2.Zero)
        {
            for (int i = 0; i < titleLetterOffsets.Length; i++)
                titleLetterOffsets[i] = (float)rnd.NextDouble() * TitleLetterOffsetVariation;
            
            for (int i = 0; i < startLetterOffsets.Length; i++)
                startLetterOffsets[i] = ((float)i / startLetterOffsets.Length) * StartLetterOffsetVariation;


            Vector2 startSize = defaultFont.MeasureString(Start) + StartHitboxBuffer;
            startHitbox = new Rectangle(
                (int)((Globals.Camera.Width * EntityBatch.PixelateMultiplier - startSize.X) / 2f),
                (int)(((Globals.Camera.Height - 6) * EntityBatch.PixelateMultiplier + startSize.Y / 2f) / 2f),
                (int)startSize.X, (int)startSize.Y
            );

            Vector2 scoreboardSize = defaultFont.MeasureString(SScoreboard) + SoreboardHitboxBuffer;
            scoreboardHitbox = new Rectangle(
                (int)((Globals.Camera.Width * EntityBatch.PixelateMultiplier - scoreboardSize.X) / 2f),
                (int)(((Globals.Camera.Height - 6) * EntityBatch.PixelateMultiplier + scoreboardSize.Y / 2f) / 2f + 23),
                (int)scoreboardSize.X, (int)scoreboardSize.Y
            );

            hoverSfx = content.Load<SoundEffect>(@"SFX/UIMisc_User Interface Vocalisation, Robotic, Futuristic,_344 Audio_Organic User Interface_12 (1)");
            hoverSfxInstance = hoverSfx.CreateInstance();

            clickSfx = content.Load<SoundEffect>(@"SFX/UIMisc_User Interface Vocalisation, Robotic, Futuristic,_344 Audio_Organic User Interface_33");
            clickSfxInstance = clickSfx.CreateInstance();
        }

        public override void Update()
        {
            timer += 0.01f;

            MouseState mouse = Mouse.GetState();

            isHoveringOverStart = startHitbox.Contains(mouse.Position.ToVector2() * EntityBatch.PixelateMultiplier) && timer - StartTimerWait > 1;

            //Play mouse over sound handling
            if (isHoveringOverStart && !wasHovering)
                hoverSfxInstance.Play();
            wasHovering = isHoveringOverStart;

            if (mouse.LeftButton == ButtonState.Pressed && isHoveringOverStart)
            {
                clickSfxInstance.Play();
                GameState = GameStates.StartGame;
            }

            //Scoreboard button handling
            if (!ScoreboardButtonEnabled)
                return;

            if (isHoveringOverScore && !wasHoveringOverScore)
                hoverSfxInstance.Play();
            wasHoveringOverScore = isHoveringOverScore;

            isHoveringOverScore = scoreboardHitbox.Contains(mouse.Position.ToVector2() * EntityBatch.PixelateMultiplier);
            if (mouse.LeftButton == ButtonState.Pressed && isHoveringOverScore)
            {
                clickSfxInstance.Play();
                GameState = GameStates.StartScreen;
            }

        }

        public override void Draw()
        {
            if (isHoveringOverStart)
            {
                spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                    startHitbox,
                    Color.Black * 0.25f);
            }

            if (ScoreboardButtonEnabled && isHoveringOverScore)
            {
                spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                    scoreboardHitbox,
                    Color.Black * 0.25f);
            }
        }

        //Will draw with UI shader effect
        public override void DrawRaw()
        {
            //Draw title
            float stringWidthProgress = 0f;
            float fullStringWidth = defaultFont.MeasureString(STitle).X;
            for (int i = 0; i < STitle.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(TitleStartPos, TitleEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer - titleLetterOffsets[i], 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    STitle[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                //Could make this more efficient by caching the width of the characters
                stringWidthProgress += defaultFont.MeasureString(STitle[i].ToString()).X;
            }

            //Draw start button
            stringWidthProgress = 0f;
            fullStringWidth = defaultFont.MeasureString(Start).X;
            for (int i = 0; i < Start.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(StartStartPos, StartEndPos, EasingFunctions.OutBounce(MathHelper.Clamp(timer - StartTimerWait - startLetterOffsets[i], 0, 1)));

                if (timer - StartLetterOffsetVariation > 0)
                {
                    float yOffsetProgress = (timer - startLetterOffsets[i]) % (BobTime);
                    yOffsetProgress = 1 - MathF.Abs(yOffsetProgress - 0.5f) * 2f;

                    yOffsetProgress = EasingFunctions.InOutCubic(yOffsetProgress);
                    curDrawPos += new Vector2(0, yOffsetProgress * BobDist);
                }

                spriteBatch.DrawString(defaultFont,
                    Start[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0f),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(STitle[i].ToString()).X;
            }

            //Draw score button
            if (!ScoreboardButtonEnabled)
                return;

            stringWidthProgress = 0f;
            fullStringWidth = defaultFont.MeasureString(SScoreboard).X;
            for (int i = 0; i < SScoreboard.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(ScoreboardStartPos, ScoreboardEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer, 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    SScoreboard[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(SScoreboard[i].ToString()).X;
            }
        }
    }
}
