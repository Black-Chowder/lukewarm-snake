using BlackMagic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlackMagic.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Kryz.Tweening;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace lukewarm_snake
{
    public class GameOverUI : Entity
    {
        public const string STitle = "GAME OVER";
        public const string SScoreBase = "SCORE: ";
        public const string SRestart = "RESTART";
        public const string SHome = "HOME";

        private float timer = 0f;

        //Title Position Variables
        private static Vector2 TitleStartPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 TitleEndPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 10f);
        private readonly float[] titleLetterOffsets = new float[STitle.Length];
        private const float TitleLetterOffsetVariation = 0.5f;

        //Score Position Variables
        private static Vector2 ScoreStartPos => new (Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 ScoreEndPos => new(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, 30f);
        private readonly float[] scoreLetterOffsets;
        private const float ScoreLetterOffsetVariation = 0.5f;
        private string scoreAsString;
        private string SScore;
        private const float SizeChangeTime = 1f;

        //Restart Button Position Variables
        private static Vector2 RestartStartPos => new Vector2(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 RestartEndPos => new Vector2(Globals.Camera.Width, Globals.Camera.Height + 150f) * EntityBatch.PixelateMultiplier / 2f;
        private readonly float[] restartLetterOffsets = new float[SRestart.Length];
        private const float RestartLetterOffsetVariation = 0.5f;

        private bool isHoveringOverRestart = false;
        private bool wasHoveringOverRestart = false;
        private Rectangle restartHitbox;
        private static Vector2 RestartHitboxBuffer => new(20, 5);

        //Home Button Position Variables
        private static Vector2 HomeStartPos => new Vector2(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, -50f);
        private static Vector2 HomeEndPos => new Vector2(Globals.Camera.Width * EntityBatch.PixelateMultiplier / 2f, (Globals.Camera.Height + 150f) * EntityBatch.PixelateMultiplier / 2f + 20f);
        private readonly float[] homeLetterOffsets = new float[SHome.Length];
        private const float HomeLetterOffsetVariation = 0.5f;

        private bool isHoveringOverHome = false;
        private bool wasHoveringOverHome = false;
        private Rectangle homeHitbox;
        private static Vector2 HomeHitboxBuffer => new(20, 5);

        //Sound effect variables
        SoundEffect hoverSfx;
        SoundEffectInstance hoverSfxInstance;
        SoundEffect clickSfx;
        SoundEffectInstance clickSfxInstance;


        public GameOverUI() : base(Vector2.Zero)
        {
            //Initialize title
            for (int i = 0; i < titleLetterOffsets.Length; i++)
                titleLetterOffsets[i] = (float)rnd.NextDouble() * TitleLetterOffsetVariation;

            //Initialize score
            scoreAsString = PlayerScore.ToString();
            SScore = SScoreBase + scoreAsString;
            scoreLetterOffsets = new float[SScore.Length];
            for (int i = 0; i < scoreLetterOffsets.Length; i++)
                scoreLetterOffsets[i] = (float)rnd.NextDouble() * ScoreLetterOffsetVariation;

            //Initialize restart button
            for (int i = 0; i < restartLetterOffsets.Length; i++)
                restartLetterOffsets[i] = (float)rnd.NextDouble() * RestartLetterOffsetVariation;

            Vector2 restartSize = defaultFont.MeasureString(SRestart) + RestartHitboxBuffer;
            restartHitbox = new Rectangle(
                (int)((Globals.Camera.Width * EntityBatch.PixelateMultiplier - restartSize.X) / 2f),
                (int)(((Globals.Camera.Height - 6) * EntityBatch.PixelateMultiplier + restartSize.Y / 2f) / 2f),
                (int)restartSize.X, (int)restartSize.Y
            );

            //Initialize home button
            for (int i = 0; i < homeLetterOffsets.Length; i++)
                homeLetterOffsets[i] = (float)rnd.NextDouble() * HomeLetterOffsetVariation;

            Vector2 homeSize = defaultFont.MeasureString(SHome) + HomeHitboxBuffer;
            homeHitbox = new Rectangle(
                (int)((Globals.Camera.Width * EntityBatch.PixelateMultiplier - homeSize.X) / 2f),
                (int)(((Globals.Camera.Height - 6) * EntityBatch.PixelateMultiplier + homeSize.Y / 2f) / 2f + 23),
                (int)homeSize.X, (int)homeSize.Y
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

            //Play mouse over sound handling
            if (isHoveringOverHome && !wasHoveringOverHome)
                hoverSfxInstance.Play();
            wasHoveringOverHome = isHoveringOverHome;

            if (isHoveringOverRestart && !wasHoveringOverRestart)
                hoverSfxInstance.Play();
            wasHoveringOverRestart = isHoveringOverRestart;

            isHoveringOverRestart = restartHitbox.Contains(mouse.Position.ToVector2() * EntityBatch.PixelateMultiplier);
            if (mouse.LeftButton == ButtonState.Pressed && isHoveringOverRestart)
            {
                clickSfxInstance.Play();
                GameState = GameStates.StartGame;
            }

            isHoveringOverHome = homeHitbox.Contains(mouse.Position.ToVector2() * EntityBatch.PixelateMultiplier);
            if (mouse.LeftButton == ButtonState.Pressed && isHoveringOverHome)
            {
                clickSfxInstance.Play();
                GameState = GameStates.StartScreen;
            }
        }

        public override void Draw()
        {
            if (isHoveringOverRestart)
            {
                spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                    restartHitbox,
                    Color.Black * 0.25f);
            }

            if (isHoveringOverHome)
            {
                spriteBatch.Draw(DrawUtils.createTexture(spriteBatch.GraphicsDevice),
                    homeHitbox,
                    Color.Black * 0.25f);
            }
        }

        //Draw call with UI shader effect applied
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

                stringWidthProgress += defaultFont.MeasureString(STitle[i].ToString()).X;
            }

            //Draw score
            stringWidthProgress = 0f;
            fullStringWidth = defaultFont.MeasureString(SScoreBase + scoreAsString).X;
            for (int i = 0; i < SScoreBase.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(ScoreStartPos, ScoreEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer - scoreLetterOffsets[i], 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    SScore[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(SScore[i].ToString()).X;
            }

            //Draw score number
            for (int i = SScoreBase.Length; i < SScore.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(ScoreStartPos, ScoreEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer - scoreLetterOffsets[i], 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    SScore[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.One ,
                    Vector2.One * 1.001f,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(SScore[i].ToString()).X;
            }


            //Draw restart button
            stringWidthProgress = 0f;
            fullStringWidth = defaultFont.MeasureString(SRestart).X;
            for (int i = 0; i < SRestart.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(RestartStartPos, RestartEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer - restartLetterOffsets[i], 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    SRestart[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(SRestart[i].ToString()).X;
            }

            //Draw home button
            stringWidthProgress = 0f;
            fullStringWidth = defaultFont.MeasureString(SHome).X;
            for (int i = 0; i < SHome.Length; i++)
            {
                Vector2 curDrawPos = Vector2.Lerp(HomeStartPos, HomeEndPos, EasingFunctions.OutBack(MathHelper.Clamp(timer - homeLetterOffsets[i], 0, 1)));

                spriteBatch.DrawString(defaultFont,
                    SHome[i].ToString(),
                    curDrawPos + new Vector2(stringWidthProgress - fullStringWidth / 2f, 0),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    Vector2.One,
                    SpriteEffects.None,
                    0f);

                stringWidthProgress += defaultFont.MeasureString(SHome[i].ToString()).X;
            }
        }
    }
}
